using eShop.Services;
using eShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Pages.Admin
{
    [Authorize(Roles = Constants.ADMINISTRATORS)]
    public class IndexModel : PageModel
    {
        private readonly ICatalogViewModelService _catalogViewModelService;

        public IndexModel(ICatalogViewModelService catalogViewModelService)
        {
            _catalogViewModelService = catalogViewModelService;
        }

        public CatalogViewModel CatalogModel { get; set; } = new CatalogViewModel();

        public async Task OnGet(CatalogViewModel catalogModel, int? pageId)
        {
            CatalogModel = await _catalogViewModelService.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.CategoryFilterApplied);
        }
    }
}
