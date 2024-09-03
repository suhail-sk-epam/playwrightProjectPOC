using Microsoft.Playwright;
using BsiPlaywrightPoc.Extensions;

namespace BsiPlaywrightPoc.Pages
{
    public class BasketPage
    {
        private readonly IPage _page;

        public BasketPage(IPage page)
        {
            _page = page;
        }

        private ILocator AgreeToTermsCheckboxLocator => _page.Locator("[id='agreeToTerms']");
        private ILocator ProceedToCheckoutLocator => _page.Locator("text=Proceed to Checkout");
        private ILocator ContinueToPaymentLocator => _page.Locator("text=Continue to payment");

        public async Task ClickAgreeToTermsCheckbox()
        {
            await AgreeToTermsCheckboxLocator.WaitUntilAvailableAndClickAsync();
        }

        public async Task<PaymentInformationPage> ClickProceedToCheckoutButton()
        {
            await ProceedToCheckoutLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentInformationPage(_page);
        }

        public async Task<PaymentInformationPage> ProceedToCheckout()
        {
            await ClickAgreeToTermsCheckbox();
            await ClickProceedToCheckoutButton();
            return new PaymentInformationPage(_page);
        }
    }
}
