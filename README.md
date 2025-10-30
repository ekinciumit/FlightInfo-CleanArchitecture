🚀 FlightInfo – Clean Architecture Monorepo
==========================================

Modern, production‑ready flight search and reservation system built with Clean Architecture. Clean separation of concerns, testable core, and a smooth DX across API and React frontend.

![Architecture](https://img.shields.io/badge/Architecture-Clean-blueviolet?style=for-the-badge) ![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge) ![React](https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react&logoColor=black) ![Vite](https://img.shields.io/badge/Vite-7-646CFF?style=for-the-badge&logo=vite&logoColor=white) ![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)


Table of Contents
-----------------
- Overview
- Features
- Tech Stack
- Project Structure
- Quick Start
- Configuration & Secrets
- Development
- Docker (optional)
- Database & Migrations
- API Docs
- Testing
- Scripts
- Troubleshooting
- Contributing
- License


Overview
--------
FlightInfo demonstrates a real‑world, layered architecture for flight discovery, pricing and booking. The domain core stays pure; application logic orchestrates use cases; infrastructure integrates with EF Core, email/SMS; the API exposes endpoints secured via JWT + 2FA; the React app provides a responsive UI.


Features
--------
- Flight search, sorting/filtering, price options
- Reservation create/cancel/restore
- JWT authentication with optional 2FA (email/SMS)
- Admin panel for flights, prices and logs
- Robust validation, mapping and logging


Tech Stack
----------
- Backend: .NET 9, ASP.NET Core, Entity Framework Core, FluentValidation, AutoMapper, JWT
- Frontend: React 19, Vite 7, TypeScript 5, Axios, React Router
- Tooling: Docker, PowerShell scripts, xUnit


Project Structure
-----------------

```
FlightInfo-CleanArchitecture/
├── FlightInfo.Domain/         # Core business entities, value objects, domain events
├── FlightInfo.Application/    # Use cases, DTO contracts, validators, service interfaces
├── FlightInfo.Infrastructure/ # EF Core, repositories, email/SMS/cache, persistence
├── FlightInfo.Api/            # ASP.NET Core Web API, JWT, middleware, DI
├── FlightInfo.Shared/         # Shared DTOs, enums, constants
├── FlightInfo.Frontend/       # React + Vite (TypeScript) app
├── scripts/                   # Dev and build helpers
└── docker-compose.yml         # Optional infra stack
```

High‑Level Architecture
-----------------------

```
┌──────────────┐     ┌───────────────────────┐     ┌────────────────────┐
│   Frontend   │ →→  │        API            │ →→  │   Infrastructure   │
│ (React/Vite) │     │ (ASP.NET Core, JWT)   │     │ (EF, Email, SMS)   │
└──────┬───────┘     └──────────┬────────────┘     └──────────┬─────────┘
       │                         │                             │
       │                  ┌──────▼──────┐                      │
       │                  │ Application │  orchestrates use     │
       │                  │  (UseCases) │  cases + validation   │
       │                  └──────┬──────┘                      │
       │                         │                             │
       │                  ┌──────▼──────┐                      │
       └─────────────────▶│   Domain    │◀─────────────────────┘
                          │ (Entities)  │  pure business core
                          └─────────────┘
```


Quick Start
-----------

Prerequisites: .NET 9 SDK, Node.js 20+, npm, SQL Server (LocalDB or Docker)

1) Clone
```
git clone https://github.com/ekinciumit/FlightInfo-CleanArchitecture.git
cd FlightInfo-CleanArchitecture
```

2) Backend secrets (User Secrets)
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

3) Frontend environment
```
cd ../FlightInfo.Frontend
echo VITE_API_BASE_URL=http://localhost:7104/api > .env.local
npm ci
```

4) Run
```
# Backend (HTTPS profile recommended)
cd ../FlightInfo.Api
dotnet run --launch-profile https

# Frontend (separate terminal)
cd ../FlightInfo.Frontend
npm run dev
```

- API: [`http://localhost:7104`](http://localhost:7104) (HTTPS: `https://localhost:7032`)
- Frontend (Vite): typically [`http://localhost:5173`](http://localhost:5173)


Configuration & Secrets
-----------------------
- `appsettings.Development.json` contains placeholders only.
- Real values are stored via .NET User Secrets (backend) and `.env.local` (frontend). These are not committed.
- For production, prefer environment variables or a secret manager (e.g., Azure Key Vault).


Development
-----------
- Build and test backend:
```
dotnet build
dotnet test
```

- Frontend production build:
```
cd FlightInfo.Frontend
npm run build
```

- Helpful scripts (PowerShell):
  - `scripts/dev.ps1` start dev environment
  - `scripts/build.ps1` build all projects
  - `scripts/fix_project.ps1` fix common local issues


Docker (optional)
-----------------
Use Docker to spin up infra quickly (e.g., SQL Server):

```
docker-compose up -d
docker-compose logs -f
docker-compose down
```


Database & Migrations
---------------------
Run EF Core migrations from `FlightInfo.Api` if/when needed:

```
cd FlightInfo.Api
dotnet ef migrations add <MigrationName>
dotnet ef database update
```


API Docs
--------
Swagger UI is available at: [`http://localhost:7104/swagger`](http://localhost:7104/swagger)


Testing
-------
Run all tests:
```
dotnet test
```

Run a specific test project:
```
dotnet test FlightInfo.Tests
```


Scripts
-------
- `scripts/dev.ps1` – Start development environment
- `scripts/build.ps1` – Build all projects
- `scripts/test.ps1` – Run all tests


Troubleshooting
---------------
- Vite/esbuild lock on Windows:
  - Kill all Node processes: `taskkill /IM node.exe /F`
  - Reinstall deps: `npm ci` then `npm run dev` or `npm run build`
- SQL connection: verify with `dotnet user-secrets list` in `FlightInfo.Api`.
- Ports already in use: stop previous instances or change ports in launch settings.


Contributing
------------
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests
5. Open a pull request


License
-------
This project is licensed under the MIT License.


