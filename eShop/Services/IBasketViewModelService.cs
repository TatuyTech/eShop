using eShop.Data;
using eShop.Data.Domain;
using eShop.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
        Task<int> CountTotalBasketItems(string username);
    }
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly AppDbContext _db;
        public BasketViewModelService(AppDbContext DB)
        {
            _db = DB;
        }

        public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
        {
            var basket = await _db.Baskets
                .Include(b => b.Items)
                .Where(b => b.BuyerId == userName)
                .SingleOrDefaultAsync();
            
            if (basket == null)
            {
                return await CreateBasketForUser(userName);
            }
            var viewModel = await Map(basket);
            return viewModel;
        }

        private async Task<BasketViewModel> CreateBasketForUser(string userId)
        {
            var basket = new Basket(userId);
            await _db.Baskets.AddAsync(basket);

            return new BasketViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
            };
        }

        private async Task<List<BasketItemViewModel>> GetBasketItems(IReadOnlyCollection<BasketItem> basketItems)
        {
            int[] ids = basketItems.Select(b => b.ProductId).ToArray();
            var products = await _db.Products.Where(c => ids.Contains(c.Id)).ToListAsync();

            var items = basketItems.Select(basketItem =>
            {
                var catalogItem = products.First(c => c.Id == basketItem.ProductId);

                var basketItemViewModel = new BasketItemViewModel
                {
                    Id = basketItem.Id,
                    UnitPrice = basketItem.UnitPrice,
                    Quantity = basketItem.Quantity,
                    ProductId = basketItem.ProductId,
                    PictureUrl = catalogItem.PictureUri,
                    ProductName = catalogItem.Name
                };
                return basketItemViewModel;
            }).ToList();

            return items;
        }

        private async Task<BasketViewModel> Map(Basket basket)
        {
            return new BasketViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = await GetBasketItems(basket.Items)
            };
        }

        public async Task<int> CountTotalBasketItems(string username)
        {
            if (username == null || username == "") return 0;
            return await _db.Baskets
                .Where(basket => basket.BuyerId == username)
                .SelectMany(item => item.Items)
                .SumAsync(sum => sum.Quantity); 
        }
    }
}
