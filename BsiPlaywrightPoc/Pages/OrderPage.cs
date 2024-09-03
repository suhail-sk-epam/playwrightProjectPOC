using BsiPlaywrightPoc.Extensions;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class OrderPage
    {
        private readonly IPage _page;

        public OrderPage(IPage page)
        {
            _page = page;
        }

        private ILocator OrderNumberLocator => _page.Locator("text=Order Number").First.Locator("..").Last;

        public async Task<string> GetFirstOrderNumberDisplayed() => await OrderNumberLocator.WaitUntilAvailableAndReturnTextAsync();

        public async Task<string> GetOrderNumberByTitle(string standardTitle)
        {
            var productLinkLocator = _page.Locator($"button[data-testid='order-line-product-link']:has-text('{standardTitle}')");
            var orderContainer = productLinkLocator.Locator("ancestor::div[role='group']");
            var orderNumberLocator = orderContainer.Locator("text=Order Number").First.Locator("..").Last;

            return await orderNumberLocator.WaitUntilAvailableAndReturnTextAsync();
        }
    }
}
