using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Application.Services;
using Vitalia.API.Services;
using Vitalia.Infrastructure.Data;
using Vitalia.Infrastructure.Repositories;

namespace Vitalia.API.Extensions;

public static class VitaliaServiceCollectionExtensions
{
    public static IServiceCollection AddVitaliaServices(
        this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<ICartService, CartService>();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderService, OrderService>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}