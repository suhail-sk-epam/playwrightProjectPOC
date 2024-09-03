using BsiPlaywrightPoc.Extensions;
using BsiPlaywrightPoc.Model.Enums;
using Microsoft.Playwright;

namespace BsiPlaywrightPoc.Pages
{
    public class ProductPage
    {
        private readonly IPage _page;

        public ProductPage(IPage page)
        {
            _page = page;
        }

        private ILocator PersonalPurchaseButtonLocator => _page.Locator("text=Personal Purchase");
        private ILocator PurchaseAgainButtonLocator => _page.Locator("text=Purchase again");
        private ILocator AddToBasketButtonLocator => _page.Locator("text=Add to Basket");
        private ILocator ProceedToBasketLocator => _page.Locator("text=Proceed to Basket");
        private ILocator DigitalOptionStandardLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Digital" });
        private ILocator HardCopyOptionStandardLocator => _page.GetByRole(AriaRole.Button, new() { Name = "Hard Copy" });
        private ILocator QuantityInputLocator => _page.Locator("[data-testid='quantity-input']");

        public async Task ClickDigitalStandardAsync() => await DigitalOptionStandardLocator.WaitUntilAvailableAndClickAsync();

        public async Task ClickHardCopyStandardAsync() => await HardCopyOptionStandardLocator.WaitUntilAvailableAndClickAsync();

        public async Task AddQuantityAsync(int desiredQuantity)
        {
            var currentQuantity = int.Parse((await QuantityInputLocator.WaitUntilAvailableAndReturnValueAsync("value"))!);

            if (currentQuantity != desiredQuantity)
            {
                await QuantityInputLocator.WaitUntilAvailableAndSendTextAsync($"{desiredQuantity}");
            }
        }

        public async Task ClickAddToBasket()
        {
            var isAddToBasketButtonVisible = await AddToBasketButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
            if (!isAddToBasketButtonVisible)
            {
                var isPersonalPurchaseButtonVisible = await PersonalPurchaseButtonLocator.WaitUntilAvailableAndReturnIsVisibleAsync();
                switch (isPersonalPurchaseButtonVisible)
                {
                    case true:
                        await PersonalPurchaseButtonLocator.WaitUntilAvailableAndClickAsync();
                        break;
                    default:
                        await PurchaseAgainButtonLocator.WaitUntilAvailableAndClickAsync();
                        break;
                }
            }
            await AddToBasketButtonLocator.WaitUntilAvailableAndClickAsync();
        }

        public async Task<BasketPage> ClickProceedToBasket()
        {
            await ProceedToBasketLocator.WaitUntilAvailableAndClickAsync();
            return new BasketPage(_page);
        }

        public async Task<BasketPage> AddDisplayedStandardAndProceedToBasket(PurchaseType purchaseType, int purchaseQuantity)
        {
            switch (purchaseType)
            {
                case PurchaseType.DigitalCopy:
                    await ClickDigitalStandardAsync();
                    break;
                case PurchaseType.HardCopy:
                    await ClickHardCopyStandardAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(purchaseType), purchaseType, null);
            };

            await AddQuantityAsync(purchaseQuantity);
            await ClickAddToBasket();
            await ClickProceedToBasket();
            return new BasketPage(_page);
        }
    }
}
