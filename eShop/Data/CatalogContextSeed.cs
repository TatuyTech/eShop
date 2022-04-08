using eShop.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace eShop.Data
{

    public class CatalogContextSeed
    {
        public static async Task SeedAsync(AppDbContext catalogContext,
            ILogger logger,
            int retry = 0)
        {
            var retryForAvailability = retry;
            try
            {
                if (catalogContext.Database.IsSqlServer())
                {
                    catalogContext.Database.Migrate();
                }

                if (!await catalogContext.Categories.AnyAsync())
                {
                    await catalogContext.Categories.AddRangeAsync(
                        GetPreconfiguredCategories());

                    await catalogContext.SaveChangesAsync();
                }

                if (!await catalogContext.Products.AnyAsync())
                {
                    await catalogContext.Products.AddRangeAsync(
                        GetPreconfiguredItems());

                    await catalogContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability >= 10) throw;

                retryForAvailability++;

                logger.LogError(ex.Message);
                await SeedAsync(catalogContext, logger, retryForAvailability);
                throw;
            }
        }

        static IEnumerable<Category> GetPreconfiguredCategories()
        {
            return new List<Category>
            {
                new("Azure"),
                new(".NET"),
                new("Visual Studio"),
                new("SQL Server"),
                new("Other")
            };
        }

        static IEnumerable<Product> GetPreconfiguredItems()
        {
            return new List<Product>
            {
                new(2, ".NET Bot Black Sweatshirt", ".NET Bot Black Sweatshirt", 19.5M,  "/images/products/1.png"),
                new(1, ".NET Black & White Mug", ".NET Black & White Mug", 8.50M, "/images/products/2.png"),
                new(2, "Prism White T-Shirt", "Prism White T-Shirt", 12,  "/images/products/3.png"),
                new(2, ".NET Foundation Sweatshirt", ".NET Foundation Sweatshirt", 12, "/images/products/4.png"),
                new(3, "Roslyn Red Sheet", "Roslyn Red Sheet", 8.5M, "/images/products/5.png"),
                new(2, ".NET Blue Sweatshirt", ".NET Blue Sweatshirt", 12, "/images/products/6.png"),
                new(2, "Roslyn Red T-Shirt", "Roslyn Red T-Shirt",  12, "/images/products/7.png"),
                new(2, "Kudu Purple Sweatshirt", "Kudu Purple Sweatshirt", 8.5M, "/images/products/8.png"),
                new(1, "Cup<T> White Mug", "Cup<T> White Mug", 12, "/images/products/9.png"),
                new(3, ".NET Foundation Sheet", ".NET Foundation Sheet", 12, "/images/products/10.png"),
                new(3, "Cup<T> Sheet", "Cup<T> Sheet", 8.5M, "/images/products/11.png"),
                new(2, "Prism White TShirt", "Prism White TShirt", 12, "/images/products/12.png")
            };
        }
    }
}
