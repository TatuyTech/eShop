using Ardalis.GuardClauses;

namespace eShop.Data.Domain
{
    public class BasketItem : BaseEntity
    {

        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public int ProductId { get; private set; }
        public int BasketId { get; private set; }

        public BasketItem(int productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            UnitPrice = unitPrice;
            SetQuantity(quantity);
        }

        public void AddQuantity(int quantity)
        {
            Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);

            Quantity += quantity;
        }

        public void SetQuantity(int quantity)
        {
            Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);

            Quantity = quantity;
        }
    }
}
