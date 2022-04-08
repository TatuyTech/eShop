using eShop.Data;
using eShop.Services;
using eShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eShop.ViewComponents
{
    public class BasketViewComponent : ViewComponent
    {
        private readonly IBasketViewModelService _basketService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public BasketViewComponent(IBasketViewModelService basketService,
                        SignInManager<ApplicationUser> signInManager)
        {
            _basketService = basketService;
            _signInManager = signInManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new BasketViewModel
            {
                ItemsCount = await CountTotalBasketItems()
            };
            return View(vm);
        }

        private async Task<int> CountTotalBasketItems()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
                return await _basketService.CountTotalBasketItems(HttpContext.User.Identity.Name);
            
            string anonymousId = GetAnnonymousIdFromCookie();
            if (anonymousId == null) return 0;

            return await _basketService.CountTotalBasketItems(anonymousId);
        }

        private string GetAnnonymousIdFromCookie()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                var id = Request.Cookies[Constants.BASKET_COOKIENAME];

                if (Guid.TryParse(id, out var _))
                {
                    return id;
                }
            }
            return null;
        }
    }
}
