-- 2026 Uçuş Test Verileri
-- Önce ülkeleri ekle
INSERT INTO Countries (Code, Name, CreatedAt, IsDeleted) VALUES
('TR', 'Türkiye', GETUTCDATE(), 0),
('DE', 'Almanya', GETUTCDATE(), 0),
('FR', 'Fransa', GETUTCDATE(), 0),
('IT', 'İtalya', GETUTCDATE(), 0),
('ES', 'İspanya', GETUTCDATE(), 0),
('GB', 'İngiltere', GETUTCDATE(), 0),
('US', 'Amerika', GETUTCDATE(), 0),
('JP', 'Japonya', GETUTCDATE(), 0);

-- Şehirleri ekle
INSERT INTO Cities (Name, Code, CountryId, CreatedAt, IsDeleted) VALUES
('İstanbul', 'IST', 1, GETUTCDATE(), 0),
('Ankara', 'ANK', 1, GETUTCDATE(), 0),
('İzmir', 'IZM', 1, GETUTCDATE(), 0),
('Berlin', 'BER', 2, GETUTCDATE(), 0),
('Münih', 'MUC', 2, GETUTCDATE(), 0),
('Paris', 'PAR', 3, GETUTCDATE(), 0),
('Roma', 'ROM', 4, GETUTCDATE(), 0),
('Madrid', 'MAD', 5, GETUTCDATE(), 0),
('Londra', 'LON', 6, GETUTCDATE(), 0),
('New York', 'NYC', 7, GETUTCDATE(), 0),
('Tokyo', 'TOK', 8, GETUTCDATE(), 0);

-- Havalimanlarını ekle
INSERT INTO Airports (Code, Name, FullName, CityId, CreatedAt, IsDeleted) VALUES
('IST', 'İstanbul Havalimanı', 'İstanbul Havalimanı', 1, GETUTCDATE(), 0),
('ESB', 'Esenboğa', 'Ankara Esenboğa Havalimanı', 2, GETUTCDATE(), 0),
('ADB', 'Adnan Menderes', 'İzmir Adnan Menderes Havalimanı', 3, GETUTCDATE(), 0),
('BER', 'Berlin Brandenburg', 'Berlin Brandenburg Havalimanı', 4, GETUTCDATE(), 0),
('MUC', 'Münih', 'Münih Havalimanı', 5, GETUTCDATE(), 0),
('CDG', 'Charles de Gaulle', 'Paris Charles de Gaulle Havalimanı', 6, GETUTCDATE(), 0),
('FCO', 'Fiumicino', 'Roma Fiumicino Havalimanı', 7, GETUTCDATE(), 0),
('MAD', 'Barajas', 'Madrid Barajas Havalimanı', 8, GETUTCDATE(), 0),
('LHR', 'Heathrow', 'Londra Heathrow Havalimanı', 9, GETUTCDATE(), 0),
('JFK', 'John F. Kennedy', 'New York JFK Havalimanı', 10, GETUTCDATE(), 0),
('NRT', 'Narita', 'Tokyo Narita Havalimanı', 11, GETUTCDATE(), 0);

-- 20 Adet 2026 Uçuşu
INSERT INTO Flights (FlightNumber, Origin, Destination, DepartureTime, ArrivalTime, Status, AircraftType, Capacity, AvailableSeats, CreatedAt, IsDeleted) VALUES
('TK001', 'İstanbul', 'Ankara', '2026-01-15 08:00:00', '2026-01-15 09:30:00', 'Scheduled', 'Boeing 737-800', 180, 180, GETUTCDATE(), 0),
('TK002', 'Ankara', 'İstanbul', '2026-01-15 14:30:00', '2026-01-15 16:00:00', 'Scheduled', 'Airbus A320', 150, 150, GETUTCDATE(), 0),
('TK003', 'İstanbul', 'Berlin', '2026-01-20 10:00:00', '2026-01-20 12:30:00', 'Scheduled', 'Boeing 777-300ER', 350, 350, GETUTCDATE(), 0),
('TK004', 'Berlin', 'İstanbul', '2026-01-20 18:00:00', '2026-01-20 22:30:00', 'Scheduled', 'Boeing 777-300ER', 350, 350, GETUTCDATE(), 0),
('TK005', 'İstanbul', 'Paris', '2026-02-05 09:30:00', '2026-02-05 12:00:00', 'Scheduled', 'Airbus A330-300', 300, 300, GETUTCDATE(), 0),
('TK006', 'Paris', 'İstanbul', '2026-02-05 16:00:00', '2026-02-05 20:30:00', 'Scheduled', 'Airbus A330-300', 300, 300, GETUTCDATE(), 0),
('TK007', 'İstanbul', 'Roma', '2026-02-10 11:00:00', '2026-02-10 13:30:00', 'Scheduled', 'Boeing 737-900', 200, 200, GETUTCDATE(), 0),
('TK008', 'Roma', 'İstanbul', '2026-02-10 17:30:00', '2026-02-10 21:00:00', 'Scheduled', 'Boeing 737-900', 200, 200, GETUTCDATE(), 0),
('TK009', 'İstanbul', 'Madrid', '2026-02-15 08:30:00', '2026-02-15 11:30:00', 'Scheduled', 'Airbus A321', 220, 220, GETUTCDATE(), 0),
('TK010', 'Madrid', 'İstanbul', '2026-02-15 15:00:00', '2026-02-15 19:30:00', 'Scheduled', 'Airbus A321', 220, 220, GETUTCDATE(), 0),
('TK011', 'İstanbul', 'Londra', '2026-03-01 07:00:00', '2026-03-01 10:30:00', 'Scheduled', 'Boeing 787-9', 280, 280, GETUTCDATE(), 0),
('TK012', 'Londra', 'İstanbul', '2026-03-01 14:00:00', '2026-03-01 19:30:00', 'Scheduled', 'Boeing 787-9', 280, 280, GETUTCDATE(), 0),
('TK013', 'İstanbul', 'New York', '2026-03-10 13:00:00', '2026-03-10 18:30:00', 'Scheduled', 'Boeing 777-300ER', 350, 350, GETUTCDATE(), 0),
('TK014', 'New York', 'İstanbul', '2026-03-10 22:00:00', '2026-03-11 15:30:00', 'Scheduled', 'Boeing 777-300ER', 350, 350, GETUTCDATE(), 0),
('TK015', 'İstanbul', 'Tokyo', '2026-03-20 10:30:00', '2026-03-21 06:00:00', 'Scheduled', 'Boeing 777-300ER', 350, 350, GETUTCDATE(), 0),
('TK016', 'Tokyo', 'İstanbul', '2026-03-20 14:00:00', '2026-03-21 01:30:00', 'Scheduled', 'Boeing 777-300ER', 350, 350, GETUTCDATE(), 0),
('TK017', 'Ankara', 'Münih', '2026-04-05 12:00:00', '2026-04-05 14:30:00', 'Scheduled', 'Airbus A320', 150, 150, GETUTCDATE(), 0),
('TK018', 'Münih', 'Ankara', '2026-04-05 19:00:00', '2026-04-05 23:30:00', 'Scheduled', 'Airbus A320', 150, 150, GETUTCDATE(), 0),
('TK019', 'İzmir', 'Berlin', '2026-04-15 16:30:00', '2026-04-15 19:00:00', 'Scheduled', 'Boeing 737-800', 180, 180, GETUTCDATE(), 0),
('TK020', 'Berlin', 'İzmir', '2026-04-15 21:30:00', '2026-04-16 02:00:00', 'Scheduled', 'Boeing 737-800', 180, 180, GETUTCDATE(), 0);

-- Her uçuş için farklı fiyatlar (Economy, Business, First Class)
INSERT INTO FlightPrices (FlightId, Class, Price, Currency, AvailableSeats, IsActive, ValidFrom, ValidTo, CreatedAt) VALUES
-- TK001 Fiyatları
(1, 'Economy', 450.00, 'TRY', 120, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(1, 'Business', 850.00, 'TRY', 40, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(1, 'First', 1200.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK002 Fiyatları
(2, 'Economy', 420.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(2, 'Business', 780.00, 'TRY', 35, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(2, 'First', 1100.00, 'TRY', 15, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK003 Fiyatları (İstanbul-Berlin)
(3, 'Economy', 1200.00, 'TRY', 200, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(3, 'Business', 2200.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(3, 'First', 3500.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK004 Fiyatları (Berlin-İstanbul)
(4, 'Economy', 1150.00, 'TRY', 200, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(4, 'Business', 2100.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(4, 'First', 3300.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK005 Fiyatları (İstanbul-Paris)
(5, 'Economy', 1100.00, 'TRY', 180, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(5, 'Business', 2000.00, 'TRY', 80, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(5, 'First', 3200.00, 'TRY', 40, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK006 Fiyatları (Paris-İstanbul)
(6, 'Economy', 1050.00, 'TRY', 180, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(6, 'Business', 1900.00, 'TRY', 80, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(6, 'First', 3000.00, 'TRY', 40, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK007 Fiyatları (İstanbul-Roma)
(7, 'Economy', 950.00, 'TRY', 120, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(7, 'Business', 1800.00, 'TRY', 60, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(7, 'First', 2800.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK008 Fiyatları (Roma-İstanbul)
(8, 'Economy', 900.00, 'TRY', 120, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(8, 'Business', 1700.00, 'TRY', 60, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(8, 'First', 2600.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK009 Fiyatları (İstanbul-Madrid)
(9, 'Economy', 1000.00, 'TRY', 140, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(9, 'Business', 1850.00, 'TRY', 60, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(9, 'First', 2900.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK010 Fiyatları (Madrid-İstanbul)
(10, 'Economy', 950.00, 'TRY', 140, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(10, 'Business', 1750.00, 'TRY', 60, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(10, 'First', 2700.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK011 Fiyatları (İstanbul-Londra)
(11, 'Economy', 1300.00, 'TRY', 180, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(11, 'Business', 2400.00, 'TRY', 80, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(11, 'First', 3800.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK012 Fiyatları (Londra-İstanbul)
(12, 'Economy', 1250.00, 'TRY', 180, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(12, 'Business', 2300.00, 'TRY', 80, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(12, 'First', 3600.00, 'TRY', 20, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK013 Fiyatları (İstanbul-New York)
(13, 'Economy', 2500.00, 'TRY', 200, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(13, 'Business', 4500.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(13, 'First', 7500.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK014 Fiyatları (New York-İstanbul)
(14, 'Economy', 2400.00, 'TRY', 200, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(14, 'Business', 4300.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(14, 'First', 7200.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK015 Fiyatları (İstanbul-Tokyo)
(15, 'Economy', 3200.00, 'TRY', 200, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(15, 'Business', 5800.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(15, 'First', 9500.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK016 Fiyatları (Tokyo-İstanbul)
(16, 'Economy', 3100.00, 'TRY', 200, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(16, 'Business', 5600.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(16, 'First', 9200.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK017 Fiyatları (Ankara-Münih)
(17, 'Economy', 1100.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(17, 'Business', 2000.00, 'TRY', 40, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(17, 'First', 3200.00, 'TRY', 10, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK018 Fiyatları (Münih-Ankara)
(18, 'Economy', 1050.00, 'TRY', 100, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(18, 'Business', 1900.00, 'TRY', 40, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(18, 'First', 3000.00, 'TRY', 10, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK019 Fiyatları (İzmir-Berlin)
(19, 'Economy', 1150.00, 'TRY', 120, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(19, 'Business', 2100.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(19, 'First', 3400.00, 'TRY', 10, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),

-- TK020 Fiyatları (Berlin-İzmir)
(20, 'Economy', 1100.00, 'TRY', 120, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(20, 'Business', 2000.00, 'TRY', 50, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE()),
(20, 'First', 3200.00, 'TRY', 10, 1, '2025-10-22 00:00:00', '2026-12-31 23:59:59', GETUTCDATE());
