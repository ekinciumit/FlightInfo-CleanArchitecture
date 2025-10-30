-- Gelecek tarihli uçuşlar ekle
INSERT INTO Flights (FlightNumber, Origin, Destination, DepartureTime, ArrivalTime, AircraftType, Capacity, AvailableSeats, Status, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
('TK001', 'İstanbul', 'New York', DATEADD(day, 15, GETDATE()), DATEADD(day, 15, DATEADD(hour, 10, GETDATE())), 'Boeing 777', 300, 300, 'Planned', GETDATE(), GETDATE(), 0),
('TK102', 'Ankara', 'İstanbul', DATEADD(day, 20, GETDATE()), DATEADD(day, 20, DATEADD(hour, 4, GETDATE())), 'Airbus A320', 180, 180, 'Planned', GETDATE(), GETDATE(), 0),
('TK202', 'İstanbul', 'Londra', DATEADD(day, 25, GETDATE()), DATEADD(day, 25, DATEADD(hour, 8, GETDATE())), 'Boeing 737', 200, 200, 'Planned', GETDATE(), GETDATE(), 0),
('TK302', 'İzmir', 'Antalya', DATEADD(day, 30, GETDATE()), DATEADD(day, 30, DATEADD(hour, 2, GETDATE())), 'ATR 72', 70, 70, 'Planned', GETDATE(), GETDATE(), 0);

-- Fiyat bilgileri ekle
INSERT INTO FlightPrices (FlightId, Class, Price, Currency, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
(1, 'Economy', 500.00, 'TRY', GETDATE(), GETDATE(), 0),
(1, 'Business', 1500.00, 'TRY', GETDATE(), GETDATE(), 0),
(2, 'Economy', 200.00, 'TRY', GETDATE(), GETDATE(), 0),
(2, 'Business', 600.00, 'TRY', GETDATE(), GETDATE(), 0),
(3, 'Economy', 800.00, 'TRY', GETDATE(), GETDATE(), 0),
(3, 'Business', 2000.00, 'TRY', GETDATE(), GETDATE(), 0),
(4, 'Economy', 150.00, 'TRY', GETDATE(), GETDATE(), 0),
(4, 'Business', 400.00, 'TRY', GETDATE(), GETDATE(), 0);


