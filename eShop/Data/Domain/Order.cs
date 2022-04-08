using Ardalis.GuardClauses;

namespace eShop.Data.Domain
{
    public class Order : BaseEntity, IAggregateRoot
    {
        private Order()
        {
            // required by EF
        }

        public Order(string buyerId, Address shipToAddress, List<OrderItem> items)
        {
            Guard.Against.NullOrEmpty(buyerId, nameof(buyerId));
            Guard.Against.Null(shipToAddress, nameof(shipToAddress));
            Guard.Against.Null(items, nameof(items));

            BuyerId = buyerId;
            ShipToAddress = shipToAddress;
            _orderItems = items;
        }

        public string BuyerId { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public Address ShipToAddress { get; private set; }

        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public decimal Total()
        {
            var total = 0m;
            foreach (var item in _orderItems)
            {
                total += item.UnitPrice * item.Units;
            }
            return total;
        }
    }
}
