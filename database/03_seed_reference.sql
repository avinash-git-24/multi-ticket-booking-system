-- ============================================================================
-- ShowSphere Reference Seed Data: Cities, Roles, Theatres, Screens, Seats
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE ShowSphereDb;
GO

-- 1. Seed Roles
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = 'Admin')
BEGIN
    INSERT INTO dbo.Roles (RoleName, Description)
    VALUES ('Admin', 'Full administrative authority over catalogue, showtimes, inventory, and bookings');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = 'User')
BEGIN
    INSERT INTO dbo.Roles (RoleName, Description)
    VALUES ('User', 'Registered customer eligible to browse, hold seats, and book tickets');
END
GO

-- 2. Seed Cities (Rajkot as primary college demonstration city)
SET IDENTITY_INSERT dbo.Cities ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE CityId = 1)
BEGIN
    INSERT INTO dbo.Cities (CityId, CityName, StateName, IsActive)
    VALUES (1, 'Rajkot', 'Gujarat', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE CityId = 2)
BEGIN
    INSERT INTO dbo.Cities (CityId, CityName, StateName, IsActive)
    VALUES (2, 'Ahmedabad', 'Gujarat', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE CityId = 3)
BEGIN
    INSERT INTO dbo.Cities (CityId, CityName, StateName, IsActive)
    VALUES (3, 'Surat', 'Gujarat', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE CityId = 4)
BEGIN
    INSERT INTO dbo.Cities (CityId, CityName, StateName, IsActive)
    VALUES (4, 'Vadodara', 'Gujarat', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Cities WHERE CityId = 5)
BEGIN
    INSERT INTO dbo.Cities (CityId, CityName, StateName, IsActive)
    VALUES (5, 'Mumbai', 'Maharashtra', 1);
END

SET IDENTITY_INSERT dbo.Cities OFF;
GO

-- 3. Seed Physical Theatres in Rajkot
SET IDENTITY_INSERT dbo.Theatres ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreId, TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES (1, 'Cosmoplex Multiplex', 1, 'Kalawad Road, Near University Road, Rajkot', 22.2858, 70.7684, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreId = 2)
BEGIN
    INSERT INTO dbo.Theatres (TheatreId, TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES (2, 'INOX Multiplex R-World', 1, 'R-World, 150 Feet Ring Road, Rajkot', 22.2742, 70.7781, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreId = 3)
BEGIN
    INSERT INTO dbo.Theatres (TheatreId, TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES (3, 'PVR Crystal Mall', 1, 'Crystal Mall, Kalawad Road, Rajkot', 22.2815, 70.7651, 1);
END

SET IDENTITY_INSERT dbo.Theatres OFF;
GO

-- 4. Seed Physical Screens
SET IDENTITY_INSERT dbo.Screens ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE ScreenId = 1)
BEGIN
    INSERT INTO dbo.Screens (ScreenId, TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (1, 1, 'Audi 1 (Dolby Atmos)', '2D', 60, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE ScreenId = 2)
BEGIN
    INSERT INTO dbo.Screens (ScreenId, TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (2, 1, 'Audi 2 (3D Projection)', '3D', 60, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE ScreenId = 3)
BEGIN
    INSERT INTO dbo.Screens (ScreenId, TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (3, 2, 'Screen 1 (Laser 4K)', '2D', 60, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE ScreenId = 4)
BEGIN
    INSERT INTO dbo.Screens (ScreenId, TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (4, 3, 'Audi 1 (IMAX Experience)', 'IMAX', 60, 1);
END

SET IDENTITY_INSERT dbo.Screens OFF;
GO

-- 5. Seed Physical Seats for All Screens (60 seats each: Rows A, B = Silver; C, D = Gold; E, F = Platinum)
DECLARE @ScreenId INT;
DECLARE @RowIdx INT;
DECLARE @SeatNum INT;
DECLARE @RowLabel VARCHAR(2);
DECLARE @Tier NVARCHAR(50);

DECLARE screen_cursor CURSOR FOR
SELECT ScreenId FROM dbo.Screens;

OPEN screen_cursor;
FETCH NEXT FROM screen_cursor INTO @ScreenId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if seats already populated
    IF NOT EXISTS (SELECT 1 FROM dbo.Seats WHERE ScreenId = @ScreenId)
    BEGIN
        SET @RowIdx = 1;
        WHILE @RowIdx <= 6
        BEGIN
            SET @RowLabel = CASE @RowIdx
                WHEN 1 THEN 'A'
                WHEN 2 THEN 'B'
                WHEN 3 THEN 'C'
                WHEN 4 THEN 'D'
                WHEN 5 THEN 'E'
                WHEN 6 THEN 'F'
            END;

            SET @Tier = CASE 
                WHEN @RowIdx IN (1, 2) THEN 'Silver'
                WHEN @RowIdx IN (3, 4) THEN 'Gold'
                ELSE 'Platinum'
            END;

            SET @SeatNum = 1;
            WHILE @SeatNum <= 10
            BEGIN
                INSERT INTO dbo.Seats (ScreenId, RowLabel, SeatNumber, SeatTier, IsActive)
                VALUES (@ScreenId, @RowLabel, @SeatNum, @Tier, 1);

                SET @SeatNum = @SeatNum + 1;
            END;

            SET @RowIdx = @RowIdx + 1;
        END;
    END;

    FETCH NEXT FROM screen_cursor INTO @ScreenId;
END;

CLOSE screen_cursor;
DEALLOCATE screen_cursor;
GO

-- 6. Seed Event Venues in Rajkot
SET IDENTITY_INSERT dbo.Venues ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 1)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive)
    VALUES (1, 'Hemu Gadhvi Natyagruh', 1, 'Tagore Road, Near Race Course, Rajkot', 1050, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 2)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive)
    VALUES (2, 'Race Course Open Air Theatre', 1, 'Race Course Ring Road, Rajkot', 5000, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 3)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive)
    VALUES (3, 'SCA Stadium Khandheri', 1, 'Jamnagar Highway, Khandheri, Rajkot', 28000, 1);
END

SET IDENTITY_INSERT dbo.Venues OFF;
GO
