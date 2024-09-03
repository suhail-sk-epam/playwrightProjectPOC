using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class PaymentConfirmationPage
    {
        private readonly IPage _page;

        public PaymentConfirmationPage(IPage page)
        {
            _page = page;
        }

        private ILocator OrderConfirmationNumberLocator => _page.Locator("p:has-text('Confirmation')");
        private ILocator ContinueShoppingButtonLocator => _page.Locator("text=Continue shopping");

        public async Task<string> GetOrderNumber()
        {
            return await OrderConfirmationNumberLocator.WaitUntilAvailableAndReturnTextAsync();
        }

        public async Task<HomePage> ClickContinueShopping()
        {
            await ContinueShoppingButtonLocator.WaitUntilAvailableAndClickAsync();
            return new HomePage(_page);
        }
    }
}
