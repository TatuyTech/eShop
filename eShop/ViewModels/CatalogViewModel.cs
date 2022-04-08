using Microsoft.AspNetCore.Mvc.Rendering;

namespace eShop.ViewModels
{
    public class CatalogViewModel
    {
        public List<ProductViewModel> Products { get; set; }
        public int? CategoryFilterApplied { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public PaginationInfoViewModel PaginationInfo { get; set; }
    }
}
