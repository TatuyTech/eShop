using Ardalis.GuardClauses;

namespace eShop.Data.Domain
{
    public class Product : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public string PictureUri { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; }

        public Product(int categoryId,
            string description,
            string name,
            decimal price,
            string pictureUri)
        {
            CategoryId = categoryId;
            Description = description;
            Name = name;
            Price = price;
            PictureUri = pictureUri;
        }

        public void UpdateDetails(string name, string description, decimal price)
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(description, nameof(description));
            Guard.Against.NegativeOrZero(price, nameof(price));

            Name = name;
            Description = description;
            Price = price;
        }

        public void UpdateCategory(int categoryId)
        {
            Guard.Against.Zero(categoryId, nameof(categoryId));
            CategoryId = categoryId;
        }

        public void UpdatePictureUri(string pictureName)
        {
            if (string.IsNullOrEmpty(pictureName))
            {
                PictureUri = string.Empty;
                return;
            }
            PictureUri = $"images\\products\\{pictureName}?{new DateTime().Ticks}";
        }
    }
}
