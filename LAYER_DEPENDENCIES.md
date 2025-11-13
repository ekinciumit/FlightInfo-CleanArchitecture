# FlightInfo Clean Architecture - Katmanlar Arası Bağımlılıklar

## 📊 Bağımlılık Diyagramı

```
┌─────────────────────────────────────────────────────────────┐
│                 FlightInfo.Frontend                          │
│  (React UI - Presentation Layer)                             │
│  - React 19 + TypeScript + Vite                              │
│  - HTTP/REST API ile iletişim                                │
│  - API'ye bağımlı (HTTP istekleri)                           │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ HTTP/REST
                            │ (API çağrıları)
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    FlightInfo.Tests                          │
│  (Test katmanı - tüm katmanları test eder)                  │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ bağımlı
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    FlightInfo.Api                            │
│  (Web API katmanı - Presentation Layer)                     │
│  - Controllers                                               │
│  - Middleware                                                │
│  - DependencyInjection                                       │
└─────────────────────────────────────────────────────────────┘
                            │
            ┌───────────────┼───────────────┐
            │               │               │
            ▼               ▼               ▼
┌──────────────────┐  ┌──────────────┐  ┌──────────────┐
│ FlightInfo.      │  │ FlightInfo.  │  │ FlightInfo.  │
│ Application      │  │ Infrastructure│  │ Shared      │
│                  │  │              │  │              │
│ (Business Logic) │  │ (Data Access)│  │ (DTOs)       │
└──────────────────┘  └──────────────┘  └──────────────┘
         │                    │                   │
         │                    │                   │
         └────────────────────┼───────────────────┘
                              │
                              ▼
                   ┌──────────────────────┐
                   │  FlightInfo.Domain   │
                   │                      │
                   │  (Core Business)     │
                   │  - Entities          │
                   │  - Value Objects     │
                   │  - Domain Events     │
                   │  - Exceptions        │
                   │  - Enums             │
                   └──────────────────────┘
```

## 🔗 Detaylı Bağımlılık Analizi

### 1. **FlightInfo.Domain** (En Alt Katman)
**Bağımlılıkları:** Yok ❌
- Hiçbir projeye bağımlı değil
- Sadece .NET SDK kullanır
- Saf domain mantığı içerir

**İçeriği:**
- Entities (User, Flight, Reservation, vb.)
- Value Objects (GeoCoordinates)
- Domain Events (CountryCreatedEvent, FlightBookedEvent)
- Exceptions
- Enums

---

### 2. **FlightInfo.Shared**
**Bağımlılıkları:**
- ✅ `FlightInfo.Domain` → Domain entity'lerine referans verir

**İçeriği:**
- DTOs (Data Transfer Objects)
- Shared Enums
- Constants

**Not:** Domain katmanına bağımlı, ama Application/Infrastructure'a bağımlı değil.

---

### 3. **FlightInfo.Application** (Business Logic Layer)
**Bağımlılıkları:**
- ✅ `FlightInfo.Domain` → Domain entity'lerini kullanır
- ✅ `FlightInfo.Shared` → DTOs ve shared türleri kullanır

**İçeriği:**
- Services (AuthService, FlightService, UserService, vb.)
- Interfaces (IUserService, IFlightService, IRepository, vb.)
- Contracts (Request/Response DTOs)
- Validators (FluentValidation)
- Mapping (AutoMapper Profiles)

**NuGet Paketleri:**
- AutoMapper
- FluentValidation
- Microsoft.EntityFrameworkCore (sadece abstractions)
- Microsoft.Extensions.Caching.Memory
- Microsoft.IdentityModel.Tokens
- System.IdentityModel.Tokens.Jwt

**Önemli:** Application katmanı Infrastructure'a bağımlı DEĞİL. Sadece interface'leri tanımlar.

---

### 4. **FlightInfo.Infrastructure** (Data Access Layer)
**Bağımlılıkları:**
- ✅ `FlightInfo.Application` → Application interface'lerini implement eder
- ✅ `FlightInfo.Domain` → Domain entity'lerini kullanır

**İçeriği:**
- Repositories (OptimizedFlightRepository, UserRepository, vb.)
- Data Context (AppDbContext)
- Services (EmailSender, MemoryCacheService, TwilioSmsService, vb.)
- Persistence (UnitOfWork)

**NuGet Paketleri:**
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.SqlServer
- Twilio (SMS servisi için)

**Önemli:** 
- Application katmanındaki interface'leri implement eder
- Domain entity'lerini doğrudan kullanır
- Shared katmanına bağımlı değil

---

### 5. **FlightInfo.Api** (Presentation Layer)
**Bağımlılıkları:**
- ✅ `FlightInfo.Application` → Application servislerini kullanır
- ✅ `FlightInfo.Infrastructure` → Infrastructure servislerini kullanır
- ✅ `FlightInfo.Shared` → Shared DTOs kullanır

**İçeriği:**
- Controllers (AuthController, FlightController, vb.)
- Middleware (ExceptionMiddleware, LoggingMiddleware)
- DependencyInjection (ApiRegistration)

**NuGet Paketleri:**
- AutoMapper
- FluentValidation.AspNetCore
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore (Swagger)

**Önemli:** 
- Application ve Infrastructure'ı koordine eder
- Domain'e doğrudan erişmez (Application üzerinden)

---

### 6. **FlightInfo.Tests** (Test Layer)
**Bağımlılıkları:**
- ✅ `FlightInfo.Api` → API'yi test eder
- ✅ `FlightInfo.Application` → Application servislerini test eder
- ✅ `FlightInfo.Infrastructure` → Infrastructure'ı test eder

**NuGet Paketleri:**
- xUnit
- Moq
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.AspNetCore.Mvc.Testing

---

### 7. **FlightInfo.Frontend** (React UI - Client Layer)
**Bağımlılıkları:**
- ✅ `FlightInfo.Api` → HTTP/REST API çağrıları yapar (external dependency)
- ❌ .NET projelerine doğrudan bağımlı değil

**Teknoloji Stack:**
- React 19.1.1
- TypeScript 5.8.3
- Vite 7.1.7
- React Router DOM 7.9.1
- Axios 1.12.2

**İçeriği:**
- **Pages:** HomePage, LoginPage, RegisterPage, SearchPage, SearchResults, BookingsPage, ProfilePage, AdminDashboard, AdminFlightsPage, AdminUsersPage, AdminLogsPage, TwoFactorVerify
- **Components:** Navbar, Toast, ToastContainer, ConfirmationModal, ConfirmDialog
- **Services:** api.ts (Axios instance), authService.ts, flightService.ts, locationService.ts
- **Contexts:** ToastContext
- **Config:** environment.ts (API URL configuration)
- **Types:** TypeScript type definitions
- **Utils:** airlineUtils, errorHandler

**API Bağlantısı:**
- Base URL: `http://localhost:7104/api` (development)
- Axios interceptor ile JWT token otomatik eklenir
- REST API endpoints kullanılır:
  - `/api/Auth/login`
  - `/api/Auth/register`
  - `/api/Flight/with-prices`
  - `/api/User`
  - `/api/Log`
  - `/api/Reservation`
  - vb.

**Önemli:** 
- Frontend, .NET backend projelerinden bağımsızdır
- Sadece HTTP/REST API üzerinden iletişim kurar
- API'ye bağımlı ama .NET projelerine build-time bağımlılığı yoktur
- Ayrı bir Node.js/React projesidir

---

## 📋 Bağımlılık Matrisi

| Katman | Domain | Shared | Application | Infrastructure | Api | Tests | Frontend |
|--------|--------|--------|-------------|----------------|-----|-------|----------|
| **Domain** | - | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Shared** | ✅ | - | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Application** | ✅ | ✅ | - | ❌ | ❌ | ❌ | ❌ |
| **Infrastructure** | ✅ | ❌ | ✅ | - | ❌ | ❌ | ❌ |
| **Api** | ❌ | ✅ | ✅ | ✅ | - | ❌ | ❌ |
| **Tests** | ❌ | ❌ | ✅ | ✅ | ✅ | - | ❌ |
| **Frontend** | ❌ | ❌ | ❌ | ❌ | 🌐* | ❌ | - |

✅ = Bağımlı (Proje referansı)  
🌐 = HTTP/REST API bağımlılığı (external, runtime)  
❌ = Bağımsız

**Not:** Frontend, API'ye HTTP üzerinden bağlanır, .NET projelerine build-time bağımlılığı yoktur.

---

## 🎯 Clean Architecture Prensipleri

### ✅ Doğru Bağımlılık Yönleri:

1. **Domain → Hiçbir yere bağımlı değil** ✓
2. **Shared → Sadece Domain'e bağımlı** ✓
3. **Application → Domain ve Shared'a bağımlı** ✓
4. **Infrastructure → Application ve Domain'e bağımlı** ✓
5. **Api → Application, Infrastructure ve Shared'a bağımlı** ✓

### ⚠️ Dikkat Edilmesi Gerekenler:

1. **Application katmanı Infrastructure'a bağımlı değil** - Bu doğru! Application sadece interface'leri tanımlar, Infrastructure bunları implement eder (Dependency Inversion Principle).

2. **Infrastructure, Application interface'lerini implement eder** - Bu Clean Architecture'ın temel prensibidir.

3. **Api katmanı Domain'e doğrudan erişmez** - Domain entity'lerine Application üzerinden erişir.

---

## 🔄 Dependency Injection Akışı

### Backend (API) Dependency Injection:
```
Program.cs (Api)
    ↓
ApiRegistration.AddApiServices()
    ↓
    ├──→ InfrastructureRegistration.AddInfrastructureServices()
    │       ├──→ DbContext (SQL Server)
    │       ├──→ Repositories (Application interface'lerini implement eder)
    │       ├──→ UnitOfWork
    │       └──→ External Services (Email, SMS, Cache)
    │
    └──→ ApplicationRegistration.AddApplicationServices()
            ├──→ Services (Application servisleri)
            ├──→ AutoMapper
            └──→ Validators
```

### Frontend → Backend İletişim Akışı:
```
React Component (Frontend)
    ↓
Service Layer (authService.ts, flightService.ts, vb.)
    ↓
Axios Instance (api.ts)
    ↓
HTTP/REST Request
    ↓
FlightInfo.Api (Controllers)
    ↓
Application Services
    ↓
Infrastructure Repositories
    ↓
Database
```

---

## 📝 Özet

Bu proje **Clean Architecture** prensiplerine uygun olarak tasarlanmıştır:

### Backend (.NET):
- ✅ **Domain** katmanı tamamen bağımsızdır
- ✅ **Application** katmanı sadece interface'leri tanımlar, Infrastructure bunları implement eder
- ✅ **Bağımlılık yönü** her zaman iç katmanlara doğrudur (Domain'e doğru)
- ✅ **Dependency Inversion Principle** uygulanmıştır
- ✅ Her katman kendi sorumluluğuna odaklanmıştır

### Frontend (React):
- ✅ **Frontend** ayrı bir React/TypeScript projesidir
- ✅ Backend'e sadece HTTP/REST API üzerinden bağlanır
- ✅ .NET projelerine build-time bağımlılığı yoktur
- ✅ Tamamen bağımsız geliştirilebilir ve deploy edilebilir

### Proje Yapısı:
```
FlightInfo-CleanArchitecture/
├── FlightInfo.Domain/          # Core business entities
├── FlightInfo.Shared/          # Shared DTOs
├── FlightInfo.Application/    # Business logic
├── FlightInfo.Infrastructure/ # Data access & external services
├── FlightInfo.Api/            # REST API endpoints
├── FlightInfo.Tests/          # Unit & integration tests
└── FlightInfo.Frontend/        # React UI (separate project)
```

**Son Güncelleme:** 2024

