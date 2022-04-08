using eShop.Services;
using eShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Pages
{
    public class BasketModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly IBasketViewModelService _basketViewModelService;

        public BasketModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService)
        {
            _basketService = basketService;
            _basketViewModelService = basketViewModelService;
        }

        public BasketViewModel BasketVM { get; set; } = new BasketViewModel();

        public async Task OnGet()
        {
            BasketVM = await _basketViewModelService.GetOrCreateBasketForUser(GetOrSetBasketCookieAndUserName());
        }

        public async Task<IActionResult> OnPost(ProductViewModel productDetails)
        {
            if (productDetails?.Id == null)
            {
                return RedirectToPage("/Index");
            }

            var username = GetOrSetBasketCookieAndUserName();
            await _basketService.AddItemToBasket(username, productDetails.Id, productDetails.Price);
            return RedirectToPage("/Index");
        }

        public async Task OnPostUpdate(IEnumerable<BasketItemViewModel> items)
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            var basketView = await _basketViewModelService.GetOrCreateBasketForUser(GetOrSetBasketCookieAndUserName());
            var updateModel = items.ToDictionary(b => b.Id.ToString(), b => b.Quantity);
            await _basketService.SetQuantities(basketView.Id, updateModel);
            BasketVM = await _basketViewModelService.GetOrCreateBasketForUser(GetOrSetBasketCookieAndUserName());
        }

        private string GetOrSetBasketCookieAndUserName()
        {
            string userName = null;

            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                return Request.HttpContext.User.Identity.Name;
            }

            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                userName = Request.Cookies[Constants.BASKET_COOKIENAME];

                if (!Request.HttpContext.User.Identity.IsAuthenticated)
                {
                    if (!Guid.TryParse(userName, out var _))
                    {
                        userName = null;
                    }
                }
            }
            if (userName != null) return userName;

            userName = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true };
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, userName, cookieOptions);

            return userName;
        }
    }
}
