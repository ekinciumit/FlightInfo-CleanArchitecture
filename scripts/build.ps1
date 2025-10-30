# 🎯 Build Script
Write-Host "🔨 Building FlightInfo Solution..." -ForegroundColor Green

# Build Backend
Write-Host "📦 Building Backend..." -ForegroundColor Blue
dotnet build FlightInfo.sln -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Backend build failed!" -ForegroundColor Red
    exit 1
}

# Build Frontend
Write-Host "🎨 Building Frontend..." -ForegroundColor Blue
Set-Location FlightInfo.Frontend
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Frontend build failed!" -ForegroundColor Red
    exit 1
}

Set-Location ..

Write-Host "✅ Build completed successfully!" -ForegroundColor Green

