using eShop.Data;
using eShop.Data.Domain;
using eShop.Exceptions;
using eShop.Services;
using eShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Pages
{

    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrderService _orderService;
        private string _username = null;
        private readonly IBasketViewModelService _basketViewModelService;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService)
        {
            _basketService = basketService;
            _signInManager = signInManager;
            _orderService = orderService;
            _basketViewModelService = basketViewModelService;
        }

        public BasketViewModel BasketVM { get; set; } = new BasketViewModel();

        public async Task OnGet()
        {
            await SetBasketModelAsync();
        }

        public async Task<IActionResult> OnPost(IEnumerable<BasketItemViewModel> items)
        {
            try
            {
                await SetBasketModelAsync();

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var updateModel = items.ToDictionary(b => b.Id.ToString(), b => b.Quantity);
                await _basketService.SetQuantities(BasketVM.Id, updateModel);
                await _orderService.CreateOrderAsync(BasketVM.Id, new Address("123 Main St.", "Kent", "OH", "United States", "44240"));
                await _basketService.DeleteBasketAsync(BasketVM.Id);
            }
            catch (EmptyBasketOnCheckoutException emptyBasketOnCheckoutException)
            {
                //Redirect to Empty Basket page
                return RedirectToPage("/Basket");
            }

            return RedirectToPage("/Index");
        }

        private async Task SetBasketModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                BasketVM = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetBasketCookieAndUserName();
                BasketVM = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }
        }

        private void GetOrSetBasketCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                _username = Request.Cookies[Constants.BASKET_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
        }
    }
}
