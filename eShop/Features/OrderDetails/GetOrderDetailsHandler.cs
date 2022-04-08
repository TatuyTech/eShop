using MediatR;
using eShop.ViewModels;
using eShop.Data;
using Microsoft.EntityFrameworkCore;

namespace eShop.Features.OrderDetails;

public class GetOrderDetailsHandler : IRequestHandler<GetOrderDetails, OrderViewModel>
{
    private readonly AppDbContext _orderRepository;

    public GetOrderDetailsHandler(AppDbContext orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderViewModel> Handle(GetOrderDetails request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.Orders
            .Where(order => order.Id == request.OrderId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered)
            .SingleOrDefaultAsync();

        if (order == null)
        {
            return null;
        }

        return new OrderViewModel
        {
            OrderDate = order.OrderDate,
            OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                PictureUrl = oi.ItemOrdered.PictureUri,
                ProductId = oi.ItemOrdered.ProductId,
                ProductName = oi.ItemOrdered.ProductName,
                UnitPrice = oi.UnitPrice,
                Units = oi.Units
            }).ToList(),
            OrderNumber = order.Id,
            ShippingAddress = order.ShipToAddress,
            Total = order.Total()
        };
    }
}
