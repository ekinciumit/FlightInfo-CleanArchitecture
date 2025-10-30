using FlightInfo.Api.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FlightInfo.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// JWT ayarlarını oku
var jwtSettings = builder.Configuration.GetSection("Jwt");

// ✅ CORS policy ekle - Güvenli CORS konfigürasyonu
builder.Services.AddCors(options =>
{
    // Development için localhost'lara izin ver
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5173", "http://localhost:5174")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Production için sadece belirli origin'lere izin ver
    options.AddPolicy("AllowProduction", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Value?.Split(',')
            ?? new[] { "https://yourdomain.com" };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Development için geniş CORS (sadece Development ortamında)
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    // Production'da AllowAll policy yok - güvenlik için
});

// ✅ Authentication servislerini ekle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

// ✅ Servisleri ekle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger + JWT ayarları
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FlightInfo.Api",
        Version = "v1"
    });

    // 🔑 JWT Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer {token}' formatında giriniz",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ✅ Clean Architecture DI Registration
builder.Services.AddApiServices(builder.Configuration);

// ✅ FluentValidation Registration
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// ✅ Development ortamında Swagger aktif
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ HTTPS zorlaması aktif (Production'da gerekli)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// ✅ CORS'u ortam bazlı aktif et
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowFrontend");
}
else
{
    app.UseCors("AllowProduction");
}

// ✅ Middleware'leri ekle
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

// 🔑 Authentication → Authorization sırası doğru
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
