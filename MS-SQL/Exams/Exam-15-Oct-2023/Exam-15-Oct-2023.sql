CREATE DATABASE TouristAgency 

GO

USE TouristAgency

GO

-- Problem 01 DDL
CREATE TABLE Countries
(
	Id INT PRIMARY KEY IDENTITY
	,[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE Destinations
(
	Id INT PRIMARY KEY IDENTITY
	,[Name] VARCHAR(50) NOT NULL
	,CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)

CREATE TABLE Rooms
(
	Id INT PRIMARY KEY IDENTITY
	,Type VARCHAR(40) NOT NULL
	,Price DECIMAL(18, 2) NOT NULL
	,BedCount INT NOT NULL
	,CHECK (BedCount > 0 AND BedCount <= 10) 
)

CREATE TABLE Hotels
(
	Id INT PRIMARY KEY IDENTITY
	,[Name] VARCHAR(50) NOT NULL
	,DestinationId INT FOREIGN KEY REFERENCES Destinations(Id) NOT NULL
)

CREATE TABLE Tourists
(
	Id INT PRIMARY KEY IDENTITY
	,[Name] NVARCHAR(80) NOT NULL
	,PhoneNumber VARCHAR(20) NOT NULL
	,Email VARCHAR(80) 
	,CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)

CREATE TABLE Bookings
(
	Id INT PRIMARY KEY IDENTITY
	,ArrivalDate DateTime2 NOT NULL
	,DepartureDate DateTime2 NOT NULL
	,AdultsCount INT NOT NULL
	,CHECK (AdultsCount >= 1 AND AdultsCount <= 10)
	,ChildrenCount INT NOT NULL
	,CHECK (AdultsCount >= 0 AND AdultsCount <= 9)
	,TouristId INT FOREIGN KEY REFERENCES Tourists(Id) NOT NULL
	,HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
	,RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL
)

CREATE TABLE HotelsRooms
(
	HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
	,RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL
	,PRIMARY KEY(HotelId, RoomId)
)

GO

-- Problem 02
INSERT INTO Tourists([Name], PhoneNumber, Email, CountryId)
VALUES
	('John Rivers', '653-551-1555', 'john.rivers@example.com', 6)
	,('Adeline Agla�', '122-654-8726', 'adeline.aglae@example.com', 2)
	,('Sergio Ramirez', '233-465-2876', 's.ramirez@example.com', 3)
	,('Johan M�ller', '322-876-9826', 'j.muller@example.com', 7)
	,('Eden Smith', '551-874-2234', 'eden.smith@example.com', 6)

INSERT INTO Bookings(ArrivalDate, DepartureDate, AdultsCount, ChildrenCount, TouristId, HotelId, RoomId)
VALUES
	('2024-03-01', '2024-03-11', 1, 0, 21, 3, 5)
	,('2023-12-28', '2024-01-06', 2, 1, 22, 13, 3)	
	,('2023-11-15', '2023-11-20', 1, 2, 23, 19, 7)	
	,('2023-12-05', '2023-12-09', 4, 0, 24, 6, 4)	
	,('2024-05-01', '2024-05-07', 6, 0, 25, 14, 6)	

GO

-- Problem 03
UPDATE Bookings
SET DepartureDate = DATEADD(DAY, 1, DepartureDate)
WHERE ArrivalDate LIKE '2023-12-%'

UPDATE Tourists
SET Email = NULL
WHERE [Name] LIKE '%MA%'


SELECT *
FROM Bookings
WHERE ArrivalDate LIKE '2023-12-%'

SELECT COUNT(*) AS UpdatedBookingsCount
FROM Bookings
WHERE DepartureDate = '2023-12-16'

GO

UPDATE Bookings
SET DepartureDate = DATEADD(DAY, 1, DepartureDate)
WHERE ArrivalDate >= '2023-12-01' AND ArrivalDate <= '2023-12-31';

UPDATE Tourists
SET Email = NULL
WHERE Name LIKE '%MA%';

GO

-- Problem 04
DELETE
FROM Bookings
WHERE TouristId IN (6, 16, 25)

DELETE
FROM Tourists
WHERE [Name] LIKE '%Smith'

GO

-- Problem 05
SELECT 
	CONVERT(varchar, b.ArrivalDate, 23) AS ArrivalDate
	,b.AdultsCount
	,b.ChildrenCount
FROM Bookings AS b
JOIN Rooms AS r ON b.RoomId = r.Id
ORDER BY r.Price DESC, b.ArrivalDate

GO

-- Problem 06
SELECT 
	h.Id
	,h.[Name]
FROM Hotels AS h
JOIN HotelsRooms AS hr ON h.Id = hr.HotelId
JOIN Rooms AS r ON r.Id = hr.RoomId
JOIN Bookings AS b ON h.Id = b.HotelId
WHERE r.[Type] =  'VIP Apartment'
GROUP BY h.Id, h.[Name]
ORDER BY COUNT(*) DESC

GO

-- Problem 07
SELECT 
	t.Id 
	,t.Name 
	,t.PhoneNumber
FROM Tourists AS t
LEFT JOIN Bookings AS b ON t.Id = b.TouristId
WHERE b.TouristId IS NULL
ORDER BY t.Name ASC

GO

-- Problem 08
SELECT TOP (10)
	h.[Name] AS HotelName
	,d.[Name] AS DestinationName
	,c.[Name] AS CountryName
FROM Bookings AS b
JOIN Tourists AS t ON t.Id = b.TouristId
JOIN Hotels AS h ON b.HotelId = h.Id
JOIN Destinations AS d ON d.Id = h.DestinationId
JOIN Countries AS c ON d.CountryId = c.Id
WHERE b.ArrivalDate < '2023-12-31'
AND b.HotelId % 2 <> 0 
ORDER BY c.[Name] ASC, b.ArrivalDate ASC

GO

-- Problem 09
SELECT 
	h.[Name] AS HotelName
	,r.Price AS RoomPrice
FROM Tourists AS t
JOIN Bookings AS b ON t.Id = b.TouristId
JOIN Hotels AS h ON b.HotelId = h.Id
JOIN Rooms AS r ON r.Id = b.RoomId
WHERE t.[Name] NOT LIKE '%EZ'
ORDER BY r.Price DESC

GO

-- Problem 10
SELECT 
	h.[Name] AS HotelName
	,SUM(r.Price * DATEDIFF(day, B.ArrivalDate, B.DepartureDate)) AS HotelRevenue
FROM Bookings AS b
JOIN  Hotels AS h ON b.HotelId = h.Id
JOIN Rooms AS r ON b.RoomId = r.Id
GROUP BY h.[Name]
ORDER BY HotelRevenue DESC

GO

-- Problem 11
CREATE FUNCTION udf_RoomsWithTourists(@name VARCHAR(50)) 
RETURNS INT AS
BEGIN
	DECLARE @totalNumebrOfTourists INT;

	SET @totalNumebrOfTourists = 
	(
		SELECT 
			SUM(b.ChildrenCount + b.AdultsCount)
		FROM Rooms AS r 
		JOIN Bookings AS b ON r.Id = b.RoomId
		WHERE r.[Type] =  @name
	)

	RETURN @totalNumebrOfTourists;
END

GO

SELECT dbo.udf_RoomsWithTourists('Double Room')

GO

-- Problem 12
CREATE PROC usp_SearchByCountry(@country VARCHAR(50))
AS
BEGIN
	SELECT 
		t.[Name]
		,t.PhoneNumber
		,t.Email
		,COUNT(*) AS CountOfBookings
	FROM Bookings AS b 
	JOIN Rooms AS r ON b.RoomId = r.Id
	JOIN Tourists AS t ON b.TouristId = t.Id
	JOIN Countries AS c ON t.CountryId = c.Id
	WHERE c.[Name] = @country
	GROUP BY t.[Name], t.PhoneNumber, t.Email
END

EXEC usp_SearchByCountry 'Greece'


































