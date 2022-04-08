using eShop.Services;
using eShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Pages.Admin
{
    [Authorize(Roles = Constants.ADMINISTRATORS)]
    public class EditProductModel : PageModel
    {
        private readonly ICatalogViewModelService _catalog;

        public EditProductModel(ICatalogViewModelService catalogItemViewModelService)
        {
            _catalog = catalogItemViewModelService;
        }

        [BindProperty]
        public ProductViewModel ProductVM { get; set; } = new ProductViewModel();

        public async Task OnGet(int Id)
        {
            ProductVM = await _catalog.GetProduct(Id);
        }

        public async Task<IActionResult> OnPost(ProductViewModel productDetails)
        {
            if (productDetails?.Id == null)
            {
                return RedirectToPage("/Admin/Index");
            }
            await _catalog.UpdateProduct(productDetails);
            return RedirectToPage("/Admin/Index");
        }
    }
}
