using System.Text.Json;
using BsiPlaywrightPoc.Helpers;
using BsiPlaywrightPoc.Model.AppSettings;
using BsiPlaywrightPoc.Model.Enums;
using BsiPlaywrightPoc.Model.ResponseObjects.SapDwh;
using BsiPlaywrightPoc.Model.User;
using BsiPlaywrightPoc.Pages;
using BsiPlaywrightPoc.TestData;
using FluentAssertions;
using TechTalk.SpecFlow;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace BsiPlaywrightPoc.StepDefinitions.Checkout
{
    [Binding]
    public sealed class CheckoutSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HomePage _homePage;
        private readonly StandardPage _standardPage;
        private readonly ProductPage _productPage;
        private readonly BasketPage _basketPage;
        private readonly PaymentInformationPage _paymentInformationPage;
        private readonly PaymentPage _paymentPage;
        private readonly PaymentConfirmationPage _paymentConfirmationPage;
        private readonly ShippingPage _shippingPage;
        private readonly OrderPage _orderPage;
        private readonly ExecuteDbQueriesHelper _executeDbQueriesHelper;
        private PurchaseType _purchaseType;
        private string _standardSapIdToBePurchased;
        private int _standardQuantityPurchased;
        private string _modeOfPayment;
        private string _orderNumber;
        private AppSettings _appSettings;

        public CheckoutSteps(ScenarioContext scenarioContext, HomePage homePage, StandardPage standardPage, ProductPage productPage, BasketPage basketPage, PaymentInformationPage paymentInformationPage, PaymentPage paymentPage, PaymentConfirmationPage paymentConfirmationPage, ShippingPage shippingPage, ExecuteDbQueriesHelper executeDbQueriesHelper, OrderPage orderPage)
        {
            _scenarioContext = scenarioContext;
            _homePage = homePage;
            _standardPage = standardPage;
            _productPage = productPage;
            _basketPage = basketPage;
            _paymentInformationPage = paymentInformationPage;
            _paymentPage = paymentPage;
            _paymentConfirmationPage = paymentConfirmationPage;
            _shippingPage = shippingPage;
            _executeDbQueriesHelper = executeDbQueriesHelper;
            _orderPage = orderPage;
        }

        [Given(@"I have '([^']*)' quantity '([^']*)', '([^']*)', standard, in my basket")]
        public async Task GivenIaDigitalStandardInMyBasket(int purchaseQuantity, string sapId, string standardTypeToBePurchased)
        {
            _purchaseType = standardTypeToBePurchased.ToLower() switch
            {
                "digital copy" => PurchaseType.DigitalCopy,
                "hard copy" => PurchaseType.HardCopy,
                _ => throw new ArgumentOutOfRangeException(nameof(standardTypeToBePurchased), standardTypeToBePurchased, null)
            };

            _standardSapIdToBePurchased = sapId.Equals("random", StringComparison.OrdinalIgnoreCase) ? _purchaseType.GetRandomStandardSapId() : sapId;

            await _homePage.NavigateToStandardPageAsync();
            await _standardPage.SearchAndOpenStandard(_standardSapIdToBePurchased);
            await _productPage.AddDisplayedStandardAndProceedToBasket(_purchaseType, purchaseQuantity);

            _standardQuantityPurchased = purchaseQuantity;
        }

        [Given(@"I complete the payment form using '([^']*)' from '([^']*)'")]
        public async Task GivenICompletePaymentUsing(string payBy, string country)
        {
            var user = _scenarioContext.Get<UserCredentials>();

            var addressDetails = RandomData.GetAddressDetails(country);

            var cardDetails = payBy.ToLower() switch
            {
                "credit card" => RandomData.GenerateCreditCardDetails(),
                "invoice" => RandomData.GetInvoiceCreditDetails(),
                _ => null
            };

            await _basketPage.ProceedToCheckout();
            await _paymentInformationPage.FillOutPaymentInformationForm(addressDetails, user.Firstname, user.Lastname);

            switch (_purchaseType)
            {
                case PurchaseType.DigitalCopy:
                    await _paymentInformationPage.ClickContinueToPayment();
                    break;
                case PurchaseType.HardCopy:
                default:
                    await _paymentInformationPage.ClickContinueToShipping();
                    await _shippingPage.ClickContinueToPayment();
                    break;
            }

            switch (payBy.ToLower())
            {
                case "invoice":
                    await _paymentPage.ClickPayByInvoice();
                    break;
                default:
                    await _paymentPage.FillOutPaymentInformationForm(cardDetails!.Number, cardDetails.Expiry, cardDetails.CVV, $"{user.Firstname} {user.Lastname}");
                    break;
            }

            _modeOfPayment = payBy.Equals("credit card", StringComparison.OrdinalIgnoreCase) ? "card" : "invoice";
        }

        [When(@"I click pay now")]
        public async Task WhenICompleteCheckout()
        {
            switch (_modeOfPayment)
            {
                case "invoice":
                    await _paymentPage.ClickCompleteOrder();
                    break;
                case "card":
                    await _paymentPage.ClickPayNow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_modeOfPayment), _modeOfPayment, null);
            }
        }

        [Then(@"the standard purchase should be successful")]
        public async Task ThenTheDigitalStandardPurchaseShouldBeSuccessful()
        {
            _appSettings = _scenarioContext.Get<AppSettings>();

            _orderNumber = await _paymentConfirmationPage.GetOrderNumber();
            _orderNumber.Should().Contain("Confirmation #");

            // let get order number from order page
            await _paymentConfirmationPage.ClickContinueShopping();
            await _homePage.NavigateToYourOrders();

            _orderNumber = await _orderPage.GetFirstOrderNumberDisplayed();
            _orderNumber.Should().NotBeNullOrEmpty();
            _orderNumber.ToLower().Should().Contain(_appSettings.RuntimeSettings.Environment);

            // extract only the order number
            _orderNumber = _orderNumber[(_orderNumber.IndexOf('-') + 1)..];
        }

        [Then(@"purchase details should appear in the dwh database")]
        public void ThenPurchaseDetailsShouldAppearInTheDwhDatabase()
        {
            // sap integration db
            var sapDbResponse = _executeDbQueriesHelper.QueryOrderStateFromSapIntegrationDb(_orderNumber);

            // it takes a tick for the data to land in the db, a little annoying
            var count = 0;
            var cancellationToken = new CancellationTokenSource();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            while (count < 5 && sapDbResponse == null)
            {
                Task.Delay(500, cancellationToken.Token);
                sapDbResponse = _executeDbQueriesHelper.QueryOrderStateFromSapIntegrationDb(_orderNumber);
                count++;
            }

            sapDbResponse.Rows.Count.Should().Be(1);
            var sapIntegrationDbResponse = JsonSerializer.Deserialize<SapDwhDbResponseObject>(sapDbResponse.Rows[0]["SapResponse"].ToString()!);

            sapIntegrationDbResponse!.OrderStatus.Should().Be("OrderCreated", $"Purchased standard: {_standardSapIdToBePurchased}, with order number: {_orderNumber} did not make it to the Sap db.");
            sapIntegrationDbResponse.SapItemsData.FirstOrDefault()!.ProductId.Should().Be(_standardSapIdToBePurchased, $"Order did not make it to the Sap db, but order was successful on the front end with order no: {_orderNumber}");
            sapIntegrationDbResponse.SapItemsData.FirstOrDefault()!.Quantity.Should().Be(_standardQuantityPurchased, $"Order quantity did not match what was purchased that is in the Sap db, but order was successful on the front end with order no: {_orderNumber}");

            // dwh db
            var dwhDbResponse = _executeDbQueriesHelper.QueryOrdersFromDwhDb(_orderNumber);
            dwhDbResponse.Rows.Count.Should().Be(1);
            dwhDbResponse.Rows[0]["Number"].ToString().Should().Be(_orderNumber, $"Order did not make it to the dwh db, but order was successful on the front end with order no: {_orderNumber}");
            dwhDbResponse.Rows[0]["PaymentType"].ToString().Should().Be(_modeOfPayment, $"Order no: {_orderNumber}, was purchased with {_modeOfPayment}, but found {sapDbResponse.Rows[0]["PaymentType"]} in the dwh DB");
        }
    }
}
