namespace eShop.Data.Domain
{
    public class Basket : BaseEntity, IAggregateRoot
    {
        public string BuyerId { get; private set; }
        private readonly List<BasketItem> _items = new List<BasketItem>();
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

        public int TotalItems => _items.Sum(i => i.Quantity);


        public Basket(string buyerId)
        {
            BuyerId = buyerId;
        }

        public void AddItem(int productId, decimal unitPrice, int quantity = 1)
        {
            if (!Items.Any(i => i.ProductId == productId))
            {
                _items.Add(new BasketItem(productId, quantity, unitPrice));
                return;
            }
            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            existingItem.AddQuantity(quantity);
        }

        public void RemoveEmptyItems()
        {
            _items.RemoveAll(i => i.Quantity == 0);
        }

        public void SetNewBuyerId(string buyerId)
        {
            BuyerId = buyerId;
        }
    }
}
