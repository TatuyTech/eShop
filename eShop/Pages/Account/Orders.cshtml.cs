using eShop.Features.MyOrders;
using eShop.Services;
using eShop.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Pages.Account
{
    [Authorize]
    public class OrdersModel : PageModel
    {
        private readonly IMediator _mediator;

        public OrdersModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IEnumerable<OrderViewModel> OrderVM { get; set; } = new List<OrderViewModel>();

        public async Task OnGet()
        {
            OrderVM = await _mediator.Send(new GetMyOrders(User.Identity.Name));
        }
    }
}
