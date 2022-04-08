using Ardalis.GuardClauses;

namespace eShop.Data.Domain
{
    public class ProductOrdered
    {
        public ProductOrdered(int productId, string productName, string pictureUri)
        {
            Guard.Against.OutOfRange(productId, nameof(productId), 1, int.MaxValue);
            Guard.Against.NullOrEmpty(productName, nameof(productName));
            Guard.Against.NullOrEmpty(pictureUri, nameof(pictureUri));

            ProductId = productId;
            ProductName = productName;
            PictureUri = pictureUri;
        }

        private ProductOrdered()
        {
            // required by EF
        }

        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUri { get; private set; }
    }
}
