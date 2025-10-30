using FlightInfo.Application.Interfaces;
using FlightInfo.Application.Interfaces.Services;
using FlightInfo.Application.Interfaces.Repositories;
using FlightInfo.Infrastructure.Data;
using FlightInfo.Infrastructure.Interfaces;
using FlightInfo.Infrastructure.Persistence;
using FlightInfo.Infrastructure.Repositories;
using FlightInfo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightInfo.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Infrastructure layer dependency injection registration
    /// </summary>
    public static class InfrastructureRegistration
    {
        /// <summary>
        /// Registers infrastructure services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repository Pattern
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IFlightRepository, OptimizedFlightRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IAirportRepository, AirportRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ITwoFactorCodeRepository, TwoFactorCodeRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Cache Service
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();

            // Email Service
            services.AddScoped<IEmailSender, EmailSender>();

            // Notification Services
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<ISmsService, TwilioSmsService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}

