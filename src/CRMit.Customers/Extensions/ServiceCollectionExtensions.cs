using CRMit.Customers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CRMit.Customers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStartupTask<T>(this IServiceCollection services)
            where T : class, IStartupTask
            => services.AddTransient<IStartupTask, T>();
    }
}
