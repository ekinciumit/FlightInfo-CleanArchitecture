# 🚀 FlightInfo - Clean Architecture Monorepo

Modern, scalable flight information system built with Clean Architecture principles.

## 📁 Project Structure

```
FlightInfo-CleanArchitecture/
├── 🎯 FlightInfo.Domain/          # Core Business Entities
├── 🎯 FlightInfo.Application/     # Business Logic & Services
├── 🎯 FlightInfo.Infrastructure/  # Data Access & External Services
├── 🎯 FlightInfo.Api/            # Web API Controllers
├── 🎯 FlightInfo.Shared/          # DTOs & Common Types
├── 🎯 FlightInfo.Frontend/        # React Frontend
├── 🎯 scripts/                    # Development Scripts
└── 🎯 docker-compose.yml         # Docker Configuration
```

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+
- Docker Desktop
- SQL Server (or Docker)

### Development Setup

FlightInfo – Clean Architecture
================================

Modern, çok katmanlı bir uçuş arama ve rezervasyon uygulaması.

İçindekiler
------------
- Mimari Genel Bakış
- Özellikler
- Teknolojiler
- Hızlı Başlangıç (Backend + Frontend)
- Gizli Bilgilerin Yönetimi (User Secrets / .env.local)
- Komutlar ve Script’ler
- Docker ile Çalıştırma (opsiyonel)
- Testler
- Sorun Giderme

Mimari Genel Bakış
------------------
- Clean Architecture katmanları:
  - FlightInfo.Domain: Varlıklar, değer nesneleri, domain kuralları
  - FlightInfo.Application: Use-case servisleri, kontratlar, validasyon
  - FlightInfo.Infrastructure: EF Core, repository’ler, harici servisler
  - FlightInfo.Api: ASP.NET Core Web API (JWT auth, middleware)
  - FlightInfo.Frontend: React + Vite (TypeScript)

Özellikler
----------
- Uçuş arama ve listeleme
- Fiyat seçenekleri ve rezervasyon akışı
- JWT tabanlı kimlik doğrulama, 2FA (email/SMS)
- Admin arayüzü (uçuş/fiyat yönetimi, loglar)

Teknolojiler
------------
- Backend: .NET 9, EF Core, FluentValidation, AutoMapper
- Frontend: React 19, Vite 7, TypeScript 5
- Diğer: Twilio (SMS), SMTP (email), Docker (opsiyonel)

Hızlı Başlangıç
---------------
Önkoşullar: .NET 9 SDK, Node 20+, npm, SQL Server (LocalDB ya da Docker)

1) Depoyu klonla
```
git clone https://github.com/ekinciumit/FlightInfo-CleanArchitecture.git
cd FlightInfo-CleanArchitecture
```

2) Backend gizli değerleri ayarla (User Secrets)
```
cd FlightInfo.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=FlightInfoDb;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
dotnet user-secrets set "Jwt:Key" "YOUR_DEV_JWT_SECRET"
dotnet user-secrets set "Jwt:Issuer" "FlightInfoAPI"
dotnet user-secrets set "Jwt:Audience" "FlightInfoUsers"
dotnet user-secrets set "Email:SmtpHost" "smtp.gmail.com"
dotnet user-secrets set "Email:SmtpPort" "587"
dotnet user-secrets set "Email:SmtpUsername" "your@email"
dotnet user-secrets set "Email:SmtpPassword" "your_app_password"
dotnet user-secrets set "Email:FromEmail" "your@email"
dotnet user-secrets set "Email:FromName" "FlightInfo"
dotnet user-secrets set "Twilio:AccountSid" "xxx"
dotnet user-secrets set "Twilio:AuthToken" "xxx"
dotnet user-secrets set "Twilio:FromNumber" "+90xxxxxxxxxx"
```

3) Frontend ortam değişkeni
```
cd ../FlightInfo.Frontend
echo VITE_API_BASE_URL=http://localhost:7104/api > .env.local
npm ci
```

4) Çalıştır
```
# Backend
cd ../FlightInfo.Api
dotnet run --launch-profile https

# Frontend (ayrı terminal)
cd ../FlightInfo.Frontend
npm run dev
```
- API: http://localhost:7104 (https: 7032)
- Frontend: terminalde yazan Vite adresi (genelde http://localhost:5173)

Gizli Bilgilerin Yönetimi
-------------------------
- appsettings.Development.json dosyasında yalnızca placeholder’lar bulunur.
- Gerçek değerler dotnet user-secrets ve Frontend için .env.local dosyasında tutulur (repo’ya girmez).

Komutlar ve Script’ler
----------------------
- Backend build+test: `dotnet build`, `dotnet test`
- Frontend prod build: `npm run build`
- Kolaylaştırıcı script’ler: `scripts/build.ps1`, `scripts/dev.ps1`, `scripts/fix_project.ps1`

Docker (opsiyonel)
------------------
- SQL Server veya diğer servisleri kolay başlatmak için `docker-compose.yml` kullanabilirsiniz (ihtiyaca göre düzenleyin).

Testler
------
- Backend testleri: `dotnet test`
- Test sonuçları ve coverage çıktıları `.gitignore` kapsamındadır.

Sorun Giderme
-------------
- Windows’ta Vite/esbuild kilidi: Tüm Node süreçlerini kapatın (`taskkill /IM node.exe /F`) ve `npm ci` tekrar deneyin. Gerekirse .env.local’i yeniden oluşturun.
- SQL bağlantısı: `ConnectionStrings:DefaultConnection` değerini user-secrets üzerinden kontrol edin (`dotnet user-secrets list`).

   ```bash
   git clone <repository>
   cd FlightInfo-CleanArchitecture
   ```

2. **Start Development Environment**
   ```powershell
   .\scripts\dev.ps1
   ```

3. **Access Applications**
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:7104
   - Swagger: http://localhost:7104/swagger
   - Database: localhost:1433

### Manual Setup

1. **Backend**
   ```bash
   cd FlightInfo.Api
   dotnet run
   ```

2. **Frontend**
   ```bash
   cd FlightInfo.Frontend
   npm install
   npm run dev
   ```

## 🏗️ Architecture

### Clean Architecture Layers

- **Domain**: Core business entities and rules
- **Application**: Business logic and use cases
- **Infrastructure**: Data access and external services
- **API**: Web controllers and endpoints
- **Shared**: Common DTOs and types

### Technology Stack

**Backend:**
- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- FluentValidation
- Swagger/OpenAPI

**Frontend:**
- React 19
- TypeScript
- Vite
- Axios
- React Router

## 🐳 Docker

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# View logs
docker-compose logs -f
```

## 📝 Scripts

- `.\scripts\dev.ps1` - Start development environment
- `.\scripts\build.ps1` - Build all projects
- `.\scripts\test.ps1` - Run all tests

## 🔧 Development

### Backend Development
```bash
cd FlightInfo.Api
dotnet run
```

### Frontend Development
```bash
cd FlightInfo.Frontend
npm run dev
```

### Database Migrations
```bash
cd FlightInfo.Api
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## 📊 API Documentation

Swagger documentation available at: http://localhost:7104/swagger

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test FlightInfo.Tests
```

## 🚀 Deployment

### Production Build
```bash
.\scripts\build.ps1
```

### Docker Production
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## 📋 Features

- ✅ User Authentication & Authorization
- ✅ Flight Search & Booking
- ✅ Reservation Management
- ✅ Clean Architecture
- ✅ Docker Support
- ✅ API Documentation
- ✅ Responsive UI

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

