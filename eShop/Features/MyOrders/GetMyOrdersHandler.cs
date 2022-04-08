using MediatR;
using eShop.ViewModels;
using eShop.Data;
using Microsoft.EntityFrameworkCore;

namespace eShop.Features.MyOrders;

public class GetMyOrdersHandler : IRequestHandler<GetMyOrders, IEnumerable<OrderViewModel>>
{
    private readonly AppDbContext _orderRepository;

    public GetMyOrdersHandler(AppDbContext orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderViewModel>> Handle(GetMyOrders request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.Orders
            .Where(o => o.BuyerId == request.UserName)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered)
            .ToListAsync();

        return orders.Select(o => new OrderViewModel
        {
            OrderDate = o.OrderDate,
            OrderItems = o.OrderItems?.Select(oi => new OrderItemViewModel()
            {
                PictureUrl = oi.ItemOrdered.PictureUri,
                ProductId = oi.ItemOrdered.ProductId,
                ProductName = oi.ItemOrdered.ProductName,
                UnitPrice = oi.UnitPrice,
                Units = oi.Units
            }).ToList(),
            OrderNumber = o.Id,
            ShippingAddress = o.ShipToAddress,
            Total = o.Total()
        });
    }
}
