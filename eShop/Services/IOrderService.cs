using Ardalis.GuardClauses;
using eShop.Data;
using eShop.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(int basketId, Address shippingAddress);
    }
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext DB)
        {
            _db = DB;
        }

        public async Task CreateOrderAsync(int basketId, Address shippingAddress)
        {
            var basket = await _db.Baskets
                   .Include(b => b.Items)
                   .Where(b => b.Id == basketId)
                   .SingleOrDefaultAsync();

            var items = await GetBasketItems(basket.Items);

            var order = new Order(basket.BuyerId, shippingAddress, items);

            Guard.Against.NullBasket(basketId, basket);
            Guard.Against.EmptyBasketOnCheckout(basket.Items);

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        private async Task<List<OrderItem>> GetBasketItems(IReadOnlyCollection<BasketItem> basketItems)
        {
            int[] ids = basketItems.Select(b => b.ProductId).ToArray();
            var products = await _db.Products.Where(c => ids.Contains(c.Id)).ToListAsync();

            var items = basketItems.Select(basketItem =>
            {
                var product = products.First(c => c.Id == basketItem.ProductId);
                var OrderItemViewModel = new OrderItem(new ProductOrdered(product.Id, product.Name, product.PictureUri)
                    , basketItem.UnitPrice, basketItem.Quantity);
                return OrderItemViewModel;
            }).ToList();

            return items;
        }
    }
}
