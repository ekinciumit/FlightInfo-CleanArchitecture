using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Application.Interfaces;
using FlightInfo.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace FlightInfo.Application.DependencyInjection
{
    /// <summary>
    /// Application layer dependency injection registration
    /// </summary>
    public static class ApplicationRegistration
    {
        /// <summary>
        /// Registers application services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper configuration
            services.AddAutoMapper(typeof(ApplicationRegistration));

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            // NotificationService is registered in Infrastructure layer
            // Infrastructure services will be registered in Infrastructure layer
            // services.AddScoped<IEmailSender, Infrastructure.Services.EmailSender>();
            // services.AddScoped<ICacheService, Infrastructure.Services.MemoryCacheService>();
            // services.AddScoped<IUnitOfWork, Infrastructure.Persistence.UnitOfWork>();

            return services;
        }
    }
}


