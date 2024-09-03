using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.User;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class PaymentInformationPage
    {
        private readonly IPage _page;

        public PaymentInformationPage(IPage page) => _page = page;

        private ILocator CountryDropdownLocator => _page.Locator("[name='countryCode']");
        private ILocator StateDropdownLocator => _page.Locator("[id='Select1']");
        private ILocator FirstnameInputFieldLocator => _page.Locator("[id='TextField0']");
        private ILocator LastnameInputFieldLocator => _page.Locator("[id='TextField1']");
        private ILocator AddressInputFieldLocator => _page.Locator("[placeholder='Address']");
        private ILocator CityInputFieldLocator => _page.Locator("[placeholder='City']");
        private ILocator PostcodeInputFieldLocator => _page.Locator("[placeholder='Postcode'], [placeholder='Postal code']");
        private ILocator ContinueToPaymentButtonLocator => _page.Locator("text=Continue to payment");
        private ILocator ContinueToShippingButtonLocator => _page.Locator("text=Continue to shipping");
        private ILocator AddressSuggestionCloseIconLocator => _page.Locator("[aria-label='Close suggestions']");
        //private ILocator OneTrustCookieAcceptButtonLocator => _page.Locator("#onetrust-accept-btn-handler");

        public async Task FillOutPaymentInformationForm(UserAddressDetails userAddressDetails, string firstname, string lastname)
        {
            // Wait for the cookie consent button to appear and click it
            //var isCookieAcceptButton = await OneTrustCookieAcceptButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
            //if (isCookieAcceptButton)
            //{
            //    await OneTrustCookieAcceptButtonLocator.ClickAsync();
            //}

            await CountryDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.Country);
            await FirstnameInputFieldLocator.WaitUntilAvailableAndSendTextAsync(firstname);
            await LastnameInputFieldLocator.WaitUntilAvailableAndSendTextAsync(lastname);
            await AddressInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.Address);

            var isAddressSuggestionVisible = await AddressSuggestionCloseIconLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
            if (isAddressSuggestionVisible)
            {
                await AddressSuggestionCloseIconLocator.WaitUntilAvailableAndClickAsync();
            }

            await CityInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.City);

            if (userAddressDetails.Country.ToLower() != "zimbabwe")
            {
                await PostcodeInputFieldLocator.WaitUntilAvailableAndSendTextAsync(userAddressDetails.Postcode);
            }

            switch (userAddressDetails.Country.ToLower())
            {
                case "australia":
                    await StateDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.State);
                    break;

                case "new zealand":
                    await StateDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.Region);
                    break;

                case "canada":
                    await StateDropdownLocator.WaitUntilAvailableAndSelectFromDropdownByTextAsync(userAddressDetails.Province);
                    break;
            }
        }

        public async Task<PaymentPage> ClickContinueToPayment()
        {
            await ContinueToPaymentButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentPage(_page);
        }

        public async Task<ShippingPage> ClickContinueToShipping()
        {
            await ContinueToShippingButtonLocator.WaitUntilAvailableAndClickAsync();
            return new ShippingPage(_page);
        }
    }
}
