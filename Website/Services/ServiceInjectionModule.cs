using Microsoft.Extensions.DependencyInjection;
using Services;

namespace App.Services
{
    public static class ServiceInjectionModule
    {
        public static IServiceCollection InjectService(this IServiceCollection services)
        {
            services.AddTransient<IComCustomerService, ComCustomerService>();
            services.AddTransient<IOrderService, OrderService>();
            return services;
        }
    }
}
