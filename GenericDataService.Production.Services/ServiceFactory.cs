using GenericDataService.GenericService;
using GenericDataService.GenericService.Attributes;
using GenericDataService.GenericService.Interfaces;
using GenericDataService.Production.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GenericDataService.Production.Services
{
    public class ProductionServiceMap : Dictionary<string, Type> { }

    public class ProductionServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ProductionServiceMap _serviceMap;

        public ProductionServiceFactory(
            IServiceProvider provider,
            ProductionServiceMap serviceMap)
        {
            _provider = provider;
            _serviceMap = serviceMap;
        }

        public IGenericService GetService(string signature)
        {
            if (!_serviceMap.TryGetValue(signature, out var type))
                throw new KeyNotFoundException($"Service '{signature}' not found.");

            return (IGenericService)_provider.GetRequiredService(type);
        }
    }

    public static class RegisterGenericService
    {
        public static IServiceCollection AddProductModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterProductionServices();

            services.AddDbContext<ProductionDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static IServiceCollection RegisterProductionServices(this IServiceCollection services)
        {
            var serviceMap = new ProductionServiceMap();

            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(IGenericService).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ServiceSignatureAttribute>();
                if (attr == null) continue;

                services.AddScoped(type);
                serviceMap.Add(attr.Signature, type);
            }

            services.AddSingleton(serviceMap);
            services.AddScoped<ProductionServiceFactory>();

            return services;
        }
    }
}