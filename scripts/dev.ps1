# 🎯 Development Script
Write-Host "🚀 Starting FlightInfo Development Environment..." -ForegroundColor Green

# Check if Docker is running
if (-not (Get-Process "Docker Desktop" -ErrorAction SilentlyContinue)) {
    Write-Host "⚠️  Docker Desktop is not running. Please start Docker Desktop first." -ForegroundColor Yellow
    exit 1
}

# Start all services
Write-Host "📦 Starting all services with Docker Compose..." -ForegroundColor Blue
docker-compose up -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check service status
Write-Host "🔍 Checking service status..." -ForegroundColor Blue
docker-compose ps

Write-Host "✅ Development environment is ready!" -ForegroundColor Green
Write-Host "🌐 Frontend: http://localhost:5173" -ForegroundColor Cyan
Write-Host "🔧 Backend API: http://localhost:7104" -ForegroundColor Cyan
Write-Host "📊 Swagger: http://localhost:7104/swagger" -ForegroundColor Cyan
Write-Host "🗄️  Database: localhost:1433" -ForegroundColor Cyan

