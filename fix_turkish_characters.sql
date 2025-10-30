-- Türkçe karakter sorununu düzelt
USE FlightInfoDb;

-- Mevcut verileri güncelle
UPDATE Flights SET Origin = 'İstanbul' WHERE Origin = 'Ä°stanbul';
UPDATE Flights SET Origin = 'İzmir' WHERE Origin = 'Ä°zmir';
UPDATE Flights SET Destination = 'Diyarbakır' WHERE Destination = 'DiyarbakÄ±r';
UPDATE Flights SET Destination = 'İstanbul' WHERE Destination = 'Ä°stanbul';
UPDATE Flights SET Destination = 'İzmir' WHERE Destination = 'Ä°zmir';

-- Kontrol et
SELECT FlightNumber, Origin, Destination FROM Flights WHERE Origin LIKE '%Ä%' OR Destination LIKE '%Ä%';
