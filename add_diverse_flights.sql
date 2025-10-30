-- Çeşitli test uçuşları ekle
USE FlightInfoDb;

-- Önce mevcut test verilerini temizle (isteğe bağlı)
-- DELETE FROM FlightPrices WHERE FlightId IN (SELECT Id FROM Flights WHERE FlightNumber LIKE 'TK%');
-- DELETE FROM Flights WHERE FlightNumber LIKE 'TK%';

-- Çeşitli uçuşlar ekle
INSERT INTO Flights (FlightNumber, Origin, Destination, DepartureTime, ArrivalTime, Status, AircraftType, Capacity, AvailableSeats, CreatedAt, IsDeleted, UpdatedAt) VALUES
-- Sabah erken uçuşları
('TK001', 'İstanbul', 'Ankara', '2024-12-20 06:00:00', '2024-12-20 07:30:00', 'Scheduled', 'Boeing 737', 180, 150, GETUTCDATE(), 0, GETUTCDATE()),
('TK002', 'İstanbul', 'İzmir', '2024-12-20 06:30:00', '2024-12-20 08:00:00', 'OnTime', 'Airbus A320', 160, 140, GETUTCDATE(), 0, GETUTCDATE()),
('TK003', 'Ankara', 'Antalya', '2024-12-20 07:00:00', '2024-12-20 08:45:00', 'Scheduled', 'Boeing 737', 180, 120, GETUTCDATE(), 0, GETUTCDATE()),

-- Sabah geç uçuşları
('TK004', 'İstanbul', 'Bodrum', '2024-12-20 09:00:00', '2024-12-20 10:30:00', 'Scheduled', 'Airbus A320', 160, 100, GETUTCDATE(), 0, GETUTCDATE()),
('TK005', 'İzmir', 'İstanbul', '2024-12-20 09:30:00', '2024-12-20 11:00:00', 'OnTime', 'Boeing 737', 180, 80, GETUTCDATE(), 0, GETUTCDATE()),

-- Öğlen uçuşları
('TK006', 'Antalya', 'İstanbul', '2024-12-20 12:00:00', '2024-12-20 13:30:00', 'Boarding', 'Airbus A320', 160, 90, GETUTCDATE(), 0, GETUTCDATE()),
('TK007', 'İstanbul', 'Trabzon', '2024-12-20 13:00:00', '2024-12-20 14:30:00', 'Scheduled', 'Boeing 737', 180, 110, GETUTCDATE(), 0, GETUTCDATE()),

-- Öğleden sonra uçuşları
('TK008', 'Bodrum', 'İstanbul', '2024-12-20 15:00:00', '2024-12-20 16:30:00', 'Delayed', 'Airbus A320', 160, 70, GETUTCDATE(), 0, GETUTCDATE()),
('TK009', 'İstanbul', 'Gaziantep', '2024-12-20 16:00:00', '2024-12-20 17:45:00', 'Scheduled', 'Boeing 737', 180, 130, GETUTCDATE(), 0, GETUTCDATE()),

-- Akşam uçuşları
('TK010', 'Trabzon', 'İstanbul', '2024-12-20 18:00:00', '2024-12-20 19:30:00', 'Scheduled', 'Airbus A320', 160, 60, GETUTCDATE(), 0, GETUTCDATE()),
('TK011', 'İstanbul', 'Diyarbakır', '2024-12-20 19:00:00', '2024-12-20 21:00:00', 'OnTime', 'Boeing 737', 180, 85, GETUTCDATE(), 0, GETUTCDATE()),

-- Gece uçuşları
('TK012', 'Gaziantep', 'İstanbul', '2024-12-20 21:00:00', '2024-12-20 22:30:00', 'Scheduled', 'Airbus A320', 160, 45, GETUTCDATE(), 0, GETUTCDATE()),
('TK013', 'İstanbul', 'Van', '2024-12-20 22:00:00', '2024-12-20 23:45:00', 'Cancelled', 'Boeing 737', 180, 0, GETUTCDATE(), 0, GETUTCDATE()),

-- Farklı durumlarda uçuşlar
('TK014', 'Diyarbakır', 'İstanbul', '2024-12-20 23:00:00', '2024-12-21 00:30:00', 'Completed', 'Airbus A320', 160, 0, GETUTCDATE(), 0, GETUTCDATE()),
('TK015', 'İstanbul', 'Erzurum', '2024-12-21 01:00:00', '2024-12-21 02:30:00', 'InProgress', 'Boeing 737', 180, 25, GETUTCDATE(), 0, GETUTCDATE()),

-- Uluslararası uçuşlar (uzun süreli)
('TK016', 'İstanbul', 'Londra', '2024-12-20 10:00:00', '2024-12-20 13:30:00', 'Scheduled', 'Boeing 777', 350, 200, GETUTCDATE(), 0, GETUTCDATE()),
('TK017', 'İstanbul', 'Paris', '2024-12-20 14:00:00', '2024-12-20 17:30:00', 'OnTime', 'Airbus A330', 300, 180, GETUTCDATE(), 0, GETUTCDATE()),
('TK018', 'İstanbul', 'Berlin', '2024-12-20 18:00:00', '2024-12-20 21:30:00', 'Scheduled', 'Boeing 777', 350, 150, GETUTCDATE(), 0, GETUTCDATE());

-- Fiyat seçenekleri ekle
INSERT INTO FlightPrices (FlightId, Class, Price, Currency, AvailableSeats, CreatedAt, IsActive, ValidFrom, ValidTo) VALUES
-- TK001 - İstanbul-Ankara (Ekonomi, Business)
(1, 'Economy', 450.00, 'TRY', 120, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(1, 'Business', 850.00, 'TRY', 30, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK002 - İstanbul-İzmir (Ekonomi, Business, First)
(2, 'Economy', 380.00, 'TRY', 100, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(2, 'Business', 720.00, 'TRY', 25, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(2, 'First', 1200.00, 'TRY', 15, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK003 - Ankara-Antalya (Sadece Ekonomi)
(3, 'Economy', 520.00, 'TRY', 120, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK004 - İstanbul-Bodrum (Ekonomi, Business)
(4, 'Economy', 420.00, 'TRY', 80, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(4, 'Business', 780.00, 'TRY', 20, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK005 - İzmir-İstanbul (Ekonomi)
(5, 'Economy', 360.00, 'TRY', 80, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK006 - Antalya-İstanbul (Ekonomi, Business)
(6, 'Economy', 480.00, 'TRY', 70, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(6, 'Business', 900.00, 'TRY', 20, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK007 - İstanbul-Trabzon (Ekonomi)
(7, 'Economy', 580.00, 'TRY', 110, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK008 - Bodrum-İstanbul (Ekonomi, Business)
(8, 'Economy', 400.00, 'TRY', 60, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(8, 'Business', 750.00, 'TRY', 10, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK009 - İstanbul-Gaziantep (Ekonomi)
(9, 'Economy', 650.00, 'TRY', 130, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK010 - Trabzon-İstanbul (Ekonomi, Business)
(10, 'Economy', 550.00, 'TRY', 50, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(10, 'Business', 950.00, 'TRY', 10, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK011 - İstanbul-Diyarbakır (Ekonomi)
(11, 'Economy', 680.00, 'TRY', 85, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK012 - Gaziantep-İstanbul (Ekonomi, Business)
(12, 'Economy', 620.00, 'TRY', 40, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(12, 'Business', 1100.00, 'TRY', 5, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK013 - İstanbul-Van (İptal edilmiş - fiyat yok)
-- (13, 'Economy', 750.00, 'TRY', 0, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK014 - Diyarbakır-İstanbul (Tamamlanmış - fiyat yok)
-- (14, 'Economy', 650.00, 'TRY', 0, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK015 - İstanbul-Erzurum (Devam ediyor - fiyat yok)
-- (15, 'Economy', 720.00, 'TRY', 25, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK016 - İstanbul-Londra (Uluslararası - USD)
(16, 'Economy', 450.00, 'USD', 150, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(16, 'Business', 850.00, 'USD', 30, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(16, 'First', 1500.00, 'USD', 20, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK017 - İstanbul-Paris (Uluslararası - EUR)
(17, 'Economy', 380.00, 'EUR', 120, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(17, 'Business', 720.00, 'EUR', 35, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(17, 'First', 1300.00, 'EUR', 25, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),

-- TK018 - İstanbul-Berlin (Uluslararası - EUR)
(18, 'Economy', 420.00, 'EUR', 100, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(18, 'Business', 780.00, 'EUR', 30, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE())),
(18, 'First', 1400.00, 'EUR', 20, GETUTCDATE(), 1, GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE()));

PRINT 'Test uçuşları ve fiyatları başarıyla eklendi!';
