using Ardalis.GuardClauses;
using eShop.Data;
using eShop.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services
{

    public interface IBasketService
    {
        Task TransferBasketAsync(string anonymousId, string userName);
        Task<Basket> AddItemToBasket(string username, int catalogItemId, decimal price, int quantity = 1);
        Task<Basket> SetQuantities(int basketId, Dictionary<string, int> quantities);
        Task DeleteBasketAsync(int basketId);
    }

    public class BasketService : IBasketService
    {
        private readonly AppDbContext _db;

        public BasketService(AppDbContext DB)
        {
            _db = DB;
        }

        public async Task<Basket> AddItemToBasket(string username, int catalogItemId, decimal price, int quantity = 1)
        {
            var basket = await _db.Baskets.Where(b => b.BuyerId == username)
                .Include(b => b.Items).SingleOrDefaultAsync();

            if (basket == null)
            {
                basket = new Basket(username);
                _db.Baskets.Add(basket);
                await _db.SaveChangesAsync();
            }

            basket.AddItem(catalogItemId, price, quantity);
            await _db.SaveChangesAsync();

            return basket;
        }

        public async Task DeleteBasketAsync(int basketId)
        {
            var basket = await _db.Baskets.FindAsync(basketId);
            if (basket != null)
            {
                _db.Baskets.Remove(basket);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Basket> SetQuantities(int basketId, Dictionary<string, int> quantities)
        {
            Guard.Against.Null(quantities, nameof(quantities));
            var basket = await _db.Baskets
                .Where(b => b.Id == basketId)
                .Include(b => b.Items)
                .SingleOrDefaultAsync(); 
            Guard.Against.NullBasket(basketId, basket);

            foreach (var item in basket.Items)
            {
                if (quantities.TryGetValue(item.Id.ToString(), out var quantity))
                {
                    item.SetQuantity(quantity);
                }
            }
            basket.RemoveEmptyItems();
            _db.Baskets.Update(basket);
            await _db.SaveChangesAsync();
            return basket;
        }

        public async Task TransferBasketAsync(string anonymousId, string userName)
        {
            Guard.Against.NullOrEmpty(anonymousId, nameof(anonymousId));
            Guard.Against.NullOrEmpty(userName, nameof(userName));
            var anonymousBasket = await _db.Baskets.Where(b => b.BuyerId == anonymousId)
            .Include(b => b.Items).SingleOrDefaultAsync();
            if (anonymousBasket == null) return;
            var userBasket = await _db.Baskets.Where(b => b.BuyerId == userName)
            .Include(b => b.Items).SingleOrDefaultAsync();
            if (userBasket == null)
            {
                userBasket = new Basket(userName);
                _db.Baskets.Add(userBasket);
                await _db.SaveChangesAsync();
                userBasket = await _db.Baskets.Where(b => b.BuyerId == userName)
                    .Include(b => b.Items).SingleOrDefaultAsync();
            }
            foreach (var item in anonymousBasket.Items)
            {
                userBasket.AddItem(item.ProductId, item.UnitPrice, item.Quantity);
            }
            _db.Baskets.Remove(anonymousBasket);
            _db.Baskets.Update(userBasket);
            await _db.SaveChangesAsync();
        }
    }
}
