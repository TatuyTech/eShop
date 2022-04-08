using eShop.Features.OrderDetails;
using eShop.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eShop.Pages.Account
{
    [Authorize]
    public class OrderDetailModel : PageModel
    {
        private readonly IMediator _mediator;

        public OrderDetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public OrderViewModel OrderVM { get; set; } = new OrderViewModel();

        public async Task OnGet(int orderId)
        {
            OrderVM = await _mediator.Send(new GetOrderDetails(User.Identity.Name, orderId));
        }
    }
}
