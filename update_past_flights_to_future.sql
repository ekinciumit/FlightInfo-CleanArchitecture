-- Geçmiş tarihli uçuşları gelecek tarihlere taşıma script'i
-- Oluşturulma Tarihi: 2025-11-03

USE FlightInfoDb;
GO

-- Bugünün tarihi
DECLARE @Today DATETIME = GETDATE();
DECLARE @StartDate DATE = DATEADD(DAY, 7, CAST(@Today AS DATE)); -- Bugünden 7 gün sonra başla

-- Önce uçuş sürelerini hesaplayıp geçici tabloda sakla ve rastgele günleri hesapla
WITH FlightDurations AS (
    SELECT 
        Id,
        DepartureTime,
        ArrivalTime,
        DATEDIFF(MINUTE, DepartureTime, ArrivalTime) AS DurationMinutes,
        -- Her uçuş için sabit bir rastgele gün değeri (7-60 arası)
        7 + (ABS(CHECKSUM(CONVERT(VARBINARY(16), Id))) % 54) AS RandomDays
    FROM Flights
    WHERE DepartureTime < @Today
    AND IsDeleted = 0
)
-- Ana güncelleme: Geçmiş tarihli uçuşları gelecek tarihlere taşı
UPDATE f
SET 
    -- Yeni kalkış zamanı: başlangıç tarihi + rastgele gün + orijinal saat/dakika/saniye
    DepartureTime = DATEADD(SECOND, 
        DATEPART(SECOND, f.DepartureTime),
        DATEADD(MINUTE, 
            DATEPART(MINUTE, f.DepartureTime),
            DATEADD(HOUR, 
                DATEPART(HOUR, f.DepartureTime),
                DATEADD(DAY, fd.RandomDays, CAST(@StartDate AS DATETIME))
            )
        )
    ),
    -- Varış zamanı: yeni kalkış zamanı + orijinal uçuş süresi
    ArrivalTime = DATEADD(MINUTE, 
        fd.DurationMinutes,
        DATEADD(SECOND, 
            DATEPART(SECOND, f.DepartureTime),
            DATEADD(MINUTE, 
                DATEPART(MINUTE, f.DepartureTime),
                DATEADD(HOUR, 
                    DATEPART(HOUR, f.DepartureTime),
                    DATEADD(DAY, fd.RandomDays, CAST(@StartDate AS DATETIME))
                )
            )
        )
    )
FROM Flights f
INNER JOIN FlightDurations fd ON f.Id = fd.Id;

-- Sonuçları göster
SELECT 
    'Güncelleme tamamlandı!' AS Result,
    COUNT(*) AS UpdatedFlights,
    MIN(DepartureTime) AS EarliestNewDate,
    MAX(DepartureTime) AS LatestNewDate
FROM Flights
WHERE DepartureTime >= CAST(@StartDate AS DATETIME)
AND IsDeleted = 0;

-- Güncellenen uçuşları göster
SELECT TOP 30
    Id,
    FlightNumber,
    Origin,
    Destination,
    DepartureTime AS NewDepartureTime,
    ArrivalTime AS NewArrivalTime,
    Status,
    DATEDIFF(MINUTE, DepartureTime, ArrivalTime) AS FlightDurationMinutes
FROM Flights
WHERE DepartureTime >= CAST(@StartDate AS DATETIME)
AND IsDeleted = 0
ORDER BY DepartureTime;

GO
