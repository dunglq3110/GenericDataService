using GenericDataService.GenericService.Interfaces;
using GenericDataService.GenericService.Notification;
using GenericDataService.GenericService.Pipeline.PostSteps;
using GenericDataService.GenericService.Pipeline.PreSteps;
using GenericDataService.Infrastructure.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataService.GenericService
{
    public static class GenericServiceExtensions
    {
        public static IServiceCollection AddGenericService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<GenericServiceDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //services.AddScoped<IPostStep, AuditStep>();
            //services.AddScoped<IPostStep, NotificationStep>();

            //services.AddScoped<IPreStep, AuthorizationStep>();
            //services.AddScoped<IPreStep, ValidationStep>();

            services.AddScoped<ServicePipeline>();

            services.AddScoped<EventRegistryService>();


            return services;
        }
    }
}
