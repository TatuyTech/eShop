namespace eShop.Data.Domain
{
    public class Category : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public Category(string name)
        {
            Name = name;
        }
    }
}
