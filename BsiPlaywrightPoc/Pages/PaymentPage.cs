using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class PaymentPage
    {
        private readonly IPage _page;

        public PaymentPage(IPage page)
        {
            _page = page;
        }

        // Locators targeting the iframes and the input fields within them
        // Define locators for the iframes
        private IFrameLocator CardNumberIframeLocator => _page.FrameLocator("iframe[id^='card-fields-number']");
        private IFrameLocator ExpiryDateIframeLocator => _page.FrameLocator("iframe[id^='card-fields-expiry']");
        private IFrameLocator SecurityCodeIframeLocator => _page.FrameLocator("iframe[id^='card-fields-verification_value']");
        private IFrameLocator NameOnCardIframeLocator => _page.FrameLocator("iframe[id^='card-fields-name']");

        // Define locators for the input fields within the iframes
        private ILocator CardNumberInputFieldLocator => CardNumberIframeLocator.Locator("input");
        private ILocator ExpiryDateInputFieldLocator => ExpiryDateIframeLocator.Locator("text=Expiration date (MM / YY)");
        private ILocator SecurityCodeInputFieldLocator => SecurityCodeIframeLocator.Locator("text=Security code");
        private ILocator NameOnCardInputFieldLocator => NameOnCardIframeLocator.Locator("text=Name on card");
        private ILocator PayNowButtonLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Pay now" });

        // pay by invoice
        private ILocator PayByInvoiceRadioButtonLocator => _page.Locator("text=Pay by Invoice");
        private ILocator CompleteOrderButtonLocator => _page.Locator("text=Complete order");


        public async Task FillOutPaymentInformationForm(string cardNumber, string cardExpiryDate, string cardSecurityCode, string nameOnPaymentCard)
        {
            // Fill out the Name on Card
            await NameOnCardInputFieldLocator.WaitUntilAvailableAndSendTextAsync(nameOnPaymentCard);
            await SecurityCodeInputFieldLocator.WaitUntilAvailableAndSendTextAsync(cardSecurityCode);
            await ExpiryDateInputFieldLocator.WaitUntilAvailableAndSendTextAsync(cardExpiryDate);
            await CardNumberInputFieldLocator.First.WaitUntilAvailableAndSendTextAsync(cardNumber);
        }

        public async Task ClickPayByInvoice()
        {
            await PayByInvoiceRadioButtonLocator.WaitUntilAvailableAndClickAsync();
        }

        public async Task<PaymentConfirmationPage> ClickPayNow()
        {
            await PayNowButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentConfirmationPage(_page);
        }

        public async Task<PaymentConfirmationPage> ClickCompleteOrder()
        {
            await CompleteOrderButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentConfirmationPage(_page);
        }
    }
}
