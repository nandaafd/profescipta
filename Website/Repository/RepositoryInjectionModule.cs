using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace App.Repository
{
    public static class RepositoryInjectionModule
    {
        public static IServiceCollection InjectPersistence(this IServiceCollection services)
        {
            services.AddTransient<IRepository<Domain.ComCustomer>, Repository<Domain.ComCustomer>>();
            services.AddTransient<IRepository<Domain.SoOrder>, Repository<Domain.SoOrder>>();
            services.AddTransient<IRepository<Domain.SoItem>, Repository<Domain.SoItem>>();
            services.AddScoped<IDbContextFactory, DbContextFactory>();
            return services;
        }
    }
}
