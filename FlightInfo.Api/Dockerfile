# 🎯 Backend Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7104

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["FlightInfo.Api/FlightInfo.Api.csproj", "FlightInfo.Api/"]
COPY ["FlightInfo.Application/FlightInfo.Application.csproj", "FlightInfo.Application/"]
COPY ["FlightInfo.Domain/FlightInfo.Domain.csproj", "FlightInfo.Domain/"]
COPY ["FlightInfo.Infrastructure/FlightInfo.Infrastructure.csproj", "FlightInfo.Infrastructure/"]
COPY ["FlightInfo.Shared/FlightInfo.Shared.csproj", "FlightInfo.Shared/"]

# Restore dependencies
RUN dotnet restore "FlightInfo.Api/FlightInfo.Api.csproj"

# Copy source code
COPY . .
WORKDIR "/src/FlightInfo.Api"

# Build and publish
RUN dotnet build "FlightInfo.Api.csproj" -c Release -o /app/build
RUN dotnet publish "FlightInfo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlightInfo.Api.dll"]
