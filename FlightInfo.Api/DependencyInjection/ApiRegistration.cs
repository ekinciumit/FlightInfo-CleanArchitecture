using FlightInfo.Application.DependencyInjection;
using FlightInfo.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FlightInfo.Api.DependencyInjection
{
    /// <summary>
    /// API layer dependency injection registration
    /// </summary>
    public static class ApiRegistration
    {
        /// <summary>
        /// Registers all application services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Infrastructure services (Database, Repositories, etc.)
            services.AddInfrastructureServices(configuration);

            // Application services (Business logic)
            services.AddApplicationServices();

            return services;
        }
    }
}


