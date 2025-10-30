# Test verisi ekleme scripti
$baseUrl = "http://localhost:7104/api"

# Önce ülkeleri ekle
Write-Host "Ülkeler ekleniyor..." -ForegroundColor Green

$countries = @(
    @{ Code = "TR"; Name = "Türkiye" },
    @{ Code = "DE"; Name = "Almanya" },
    @{ Code = "FR"; Name = "Fransa" },
    @{ Code = "IT"; Name = "İtalya" },
    @{ Code = "ES"; Name = "İspanya" },
    @{ Code = "GB"; Name = "İngiltere" },
    @{ Code = "US"; Name = "Amerika" },
    @{ Code = "JP"; Name = "Japonya" }
)

foreach ($country in $countries) {
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/Country" -Method POST -Body ($country | ConvertTo-Json) -ContentType "application/json"
        Write-Host "✓ $($country.Name) eklendi" -ForegroundColor Green
    } catch {
        Write-Host "✗ $($country.Name) eklenemedi: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Şehirleri ekle
Write-Host "`nŞehirler ekleniyor..." -ForegroundColor Green

$cities = @(
    @{ Name = "İstanbul"; Code = "IST"; CountryId = 1 },
    @{ Name = "Ankara"; Code = "ANK"; CountryId = 1 },
    @{ Name = "İzmir"; Code = "IZM"; CountryId = 1 },
    @{ Name = "Berlin"; Code = "BER"; CountryId = 2 },
    @{ Name = "Münih"; Code = "MUC"; CountryId = 2 },
    @{ Name = "Paris"; Code = "PAR"; CountryId = 3 },
    @{ Name = "Roma"; Code = "ROM"; CountryId = 4 },
    @{ Name = "Madrid"; Code = "MAD"; CountryId = 5 },
    @{ Name = "Londra"; Code = "LON"; CountryId = 6 },
    @{ Name = "New York"; Code = "NYC"; CountryId = 7 },
    @{ Name = "Tokyo"; Code = "TOK"; CountryId = 8 }
)

foreach ($city in $cities) {
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/City" -Method POST -Body ($city | ConvertTo-Json) -ContentType "application/json"
        Write-Host "✓ $($city.Name) eklendi" -ForegroundColor Green
    } catch {
        Write-Host "✗ $($city.Name) eklenemedi: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nTest verisi ekleme tamamlandı!" -ForegroundColor Yellow
Write-Host "Şimdi uçuş verilerini kontrol edin: $baseUrl/Flight" -ForegroundColor Cyan
