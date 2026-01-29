using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;


namespace Presentation
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Регистрация Application-сервисов
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IPropertyService, PropertyService>();

            // Регистрация репозиториев
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();

            return services;
        }
    }
}