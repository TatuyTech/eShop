using eShop.Data;
using eShop.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services
{
    public interface ICatalogViewModelService
    {
        Task<CatalogViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? categoryId);
        Task<IEnumerable<SelectListItem>> GetCategories();
        Task UpdateProduct(ProductViewModel viewModel);
        Task<ProductViewModel> GetProduct(int id);
    }

    public class CatalogViewModelService : ICatalogViewModelService
    {
        private readonly ILogger<CatalogViewModelService> _logger;
        private readonly AppDbContext _db;

        public CatalogViewModelService(
            ILoggerFactory loggerFactory,
            AppDbContext itemRepository)
        {
            _logger = loggerFactory.CreateLogger<CatalogViewModelService>();
            _db = itemRepository;
        }
        public async Task<ProductViewModel> GetProduct(int id)
        {
            var i = await _db.Products
                .Where(i => i.Id == id).SingleOrDefaultAsync();

            return new ProductViewModel()
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                PictureUri = i.PictureUri
            };
        }

        public async Task<CatalogViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? categoryId)
        {
            _logger.LogInformation("GetCatalogItems called.");

            var totalItems = await _db.Products
                .Where(i => (!categoryId.HasValue || i.CategoryId == categoryId))
                .CountAsync();

            int skip = itemsPage * pageIndex;
            
            // the implementation below using ForEach and Count. We need a List.
            var itemsOnPage = await _db.Products
                .Where(i => (!categoryId.HasValue || i.CategoryId == categoryId))
                .Skip(skip).Take(itemsPage).ToListAsync();

            var vm = new CatalogViewModel()
            {
                Products = itemsOnPage.Select(i => new ProductViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    PictureUri = i.PictureUri,
                    Price = i.Price
                }).ToList(),
                Categories = (await GetCategories()).ToList(),
                CategoryFilterApplied = categoryId ?? 0,
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / itemsPage)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return vm;
        }

        public async Task<IEnumerable<SelectListItem>> GetCategories()
        {
            _logger.LogInformation("GetBrands called.");
            var brands = await _db.Categories.ToListAsync();

            var items = brands
                .Select(brand => new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Name })
                .OrderBy(b => b.Text)
                .ToList();

            var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
            items.Insert(0, allItem);

            return items;
        }

        public async Task UpdateProduct(ProductViewModel viewModel)
        {
            var existingCatalogItem = await _db.Products.FindAsync(viewModel.Id);
            existingCatalogItem.UpdateDetails(viewModel.Name, viewModel.Description, viewModel.Price);
            _db.Products.Update(existingCatalogItem);
            await _db.SaveChangesAsync();
        }
    }

}
