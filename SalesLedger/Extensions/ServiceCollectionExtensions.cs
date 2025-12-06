using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesLedger.Application.Interfaces;
using SalesLedger.Application.Services;
using SalesLedger.Infrastructure.Data;
using SalesLedger.Infrastructure.Repositories;

namespace SalesLedger.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSalesLedgerServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Database
            services.AddDbContext<SalesLedgerDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("SalesLedgerDb")));
            // Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            // Application Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            return services;
        }
    }
}
