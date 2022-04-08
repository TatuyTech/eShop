using eShop.Services;
using MediatR;
using System.Reflection;

namespace eShop.Configuration;

public static class ConfigureWebServices
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBasketViewModelService, BasketViewModelService>();
        services.AddScoped<ICatalogViewModelService, CatalogViewModelService>();
        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
