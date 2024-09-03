using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class ShippingPage
    {
        private readonly IPage _page;

        public ShippingPage(IPage page)
        {
            _page = page;
        }
        private ILocator ContinueToPaymentButtonLocator => _page.Locator("text=Continue to payment");

        public async Task<PaymentPage> ClickContinueToPayment()
        {
            await ContinueToPaymentButtonLocator.WaitUntilAvailableAndClickAsync();
            return new PaymentPage(_page);
        }
    }
}
