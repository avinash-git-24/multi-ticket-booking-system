-- ============================================================================
-- ShowSphere Seed Script: Expand All Rajkot Theatres, Screens, Seats & Showtimes
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE ShowSphereDb;
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Ensure All 7 Prominent Theatres in Rajkot Exist (CityId = 1)
IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'Cosmoplex Multiplex' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('Cosmoplex Multiplex', 1, 'Kalawad Road, Near University Road, Rajkot', 22.2858, 70.7684, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'INOX Multiplex R-World' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('INOX Multiplex R-World', 1, 'R-World, 150 Feet Ring Road, Rajkot', 22.2742, 70.7781, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'PVR Crystal Mall' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('PVR Crystal Mall', 1, 'Crystal Mall, Kalawad Road, Rajkot', 22.2815, 70.7651, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'Cinemax: The Imperial Palace' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('Cinemax: The Imperial Palace', 1, 'Dr. Yagnik Road, Jagnath Plot, Rajkot', 22.2982, 70.7964, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'Time Cinema' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('Time Cinema', 1, 'Anand Bungalow Chowk, Mavdi Main Road, Rajkot', 22.2685, 70.7850, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'Rajshree Cinema' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('Rajshree Cinema', 1, 'Bhupendra Road, Gundavadi, Rajkot', 22.2970, 70.8035, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Theatres WHERE TheatreName = 'Galaxy Cinema' AND CityId = 1)
BEGIN
    INSERT INTO dbo.Theatres (TheatreName, CityId, Address, Latitude, Longitude, IsActive)
    VALUES ('Galaxy Cinema', 1, 'Dhebar Road, Opp. ST Bus Station, Rajkot', 22.2925, 70.8052, 1);
END
GO

-- 2. Ensure Screens Exist for Each Rajkot Theatre
DECLARE @T_Cosmoplex INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'Cosmoplex Multiplex' AND CityId = 1);
DECLARE @T_Inox INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'INOX Multiplex R-World' AND CityId = 1);
DECLARE @T_Pvr INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'PVR Crystal Mall' AND CityId = 1);
DECLARE @T_Cinemax INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'Cinemax: The Imperial Palace' AND CityId = 1);
DECLARE @T_Time INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'Time Cinema' AND CityId = 1);
DECLARE @T_Rajshree INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'Rajshree Cinema' AND CityId = 1);
DECLARE @T_Galaxy INT = (SELECT TOP 1 TheatreId FROM dbo.Theatres WHERE TheatreName = 'Galaxy Cinema' AND CityId = 1);

-- Cinemax Screens
IF @T_Cinemax IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Cinemax AND ScreenName = 'Screen 1 (Gold Class)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Cinemax, 'Screen 1 (Gold Class)', '2D', 60, 1);
END

IF @T_Cinemax IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Cinemax AND ScreenName = 'Screen 2 (Dolby 7.1)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Cinemax, 'Screen 2 (Dolby 7.1)', '2D', 60, 1);
END

-- Time Cinema Screens
IF @T_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Time AND ScreenName = 'Audi 1 (Dolby Atmos)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Time, 'Audi 1 (Dolby Atmos)', '2D', 60, 1);
END

IF @T_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Time AND ScreenName = 'Audi 2 (Digital 2K)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Time, 'Audi 2 (Digital 2K)', '2D', 60, 1);
END

-- Rajshree Cinema Screens
IF @T_Rajshree IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Rajshree AND ScreenName = 'Main Audi (4K Laser)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Rajshree, 'Main Audi (4K Laser)', '2D', 60, 1);
END

-- Galaxy Cinema Screens
IF @T_Galaxy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Galaxy AND ScreenName = 'Audi 1 (Digital Sound)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Galaxy, 'Audi 1 (Digital Sound)', '2D', 60, 1);
END

-- Inox Screen 2
IF @T_Inox IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Inox AND ScreenName = 'Screen 2 (Dolby Atmos)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Inox, 'Screen 2 (Dolby Atmos)', '2D', 60, 1);
END

-- PVR Screen 2
IF @T_Pvr IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Screens WHERE TheatreId = @T_Pvr AND ScreenName = 'Audi 2 (4DX Motion)')
BEGIN
    INSERT INTO dbo.Screens (TheatreId, ScreenName, ScreenType, TotalSeats, IsActive)
    VALUES (@T_Pvr, 'Audi 2 (4DX Motion)', '3D', 60, 1);
END
GO

-- 3. Populate Physical Seats (60 seats: Rows A-F, Seats 1-10) for Any Screens Missing Seats
DECLARE @ScreenId INT;
DECLARE @RowIdx INT;
DECLARE @SeatNum INT;
DECLARE @RowLabel VARCHAR(2);
DECLARE @Tier NVARCHAR(50);

DECLARE missing_screen_cursor CURSOR FOR
SELECT ScreenId 
FROM dbo.Screens s
WHERE NOT EXISTS (SELECT 1 FROM dbo.Seats st WHERE st.ScreenId = s.ScreenId);

OPEN missing_screen_cursor;
FETCH NEXT FROM missing_screen_cursor INTO @ScreenId;

WHILE @@FETCH_STATUS = 0
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

    FETCH NEXT FROM missing_screen_cursor INTO @ScreenId;
END;

CLOSE missing_screen_cursor;
DEALLOCATE missing_screen_cursor;
GO

-- 4. Seed Dynamic Multi-Theatre Showtimes for Today and Next 7 Days in Rajkot
DECLARE @Sc_Cosmoplex1 INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'Cosmoplex Multiplex' AND s.ScreenName LIKE '%Audi 1%');
DECLARE @Sc_Cosmoplex2 INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'Cosmoplex Multiplex' AND s.ScreenName LIKE '%Audi 2%');
DECLARE @Sc_Inox1 INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'INOX Multiplex R-World' AND s.ScreenName LIKE '%Screen 1%');
DECLARE @Sc_Inox2 INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'INOX Multiplex R-World' AND s.ScreenName LIKE '%Screen 2%');
DECLARE @Sc_Pvr1 INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'PVR Crystal Mall' AND s.ScreenName LIKE '%Audi 1%');
DECLARE @Sc_Pvr2 INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'PVR Crystal Mall' AND s.ScreenName LIKE '%Audi 2%');
DECLARE @Sc_Cinemax INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'Cinemax: The Imperial Palace');
DECLARE @Sc_Time INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'Time Cinema');
DECLARE @Sc_Rajshree INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'Rajshree Cinema');
DECLARE @Sc_Galaxy INT = (SELECT TOP 1 ScreenId FROM dbo.Screens s JOIN dbo.Theatres t ON s.TheatreId = t.TheatreId WHERE t.TheatreName = 'Galaxy Cinema');

DECLARE @Today DATE = CAST(GETDATE() AS DATE);
DECLARE @DayOffset INT = 0;
DECLARE @ShowtimeDate DATE;

WHILE @DayOffset <= 7
BEGIN
    SET @ShowtimeDate = DATEADD(DAY, @DayOffset, @Today);

    -- ========================================================================
    -- MovieId 13: Rajini: The Jailer 2 across ALL 7 Rajkot Theatres
    -- ========================================================================

    -- Cosmoplex Multiplex (Screen 1)
    IF @Sc_Cosmoplex1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cosmoplex1, DATEADD(MINUTE, 15, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 250.00, 350.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Cosmoplex1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 13)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cosmoplex1, DATEADD(MINUTE, 45, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 190.00, 260.00, 360.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Cosmoplex1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cosmoplex1, DATEADD(MINUTE, 15, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), 220.00, 290.00, 390.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Cosmoplex1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 21)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cosmoplex1, DATEADD(MINUTE, 45, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 24, CAST(@ShowtimeDate AS DATETIME2))), 200.00, 270.00, 370.00, 1, SYSUTCDATETIME());
    END

    -- INOX Multiplex R-World (Screen 3)
    IF @Sc_Inox1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Inox1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 9)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Inox1, DATEADD(MINUTE, 30, DATEADD(HOUR, 9, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), 170.00, 240.00, 340.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Inox1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Inox1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 12)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Inox1, DATEADD(MINUTE, 45, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), 190.00, 260.00, 360.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Inox1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Inox1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 16)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Inox1, DATEADD(MINUTE, 30, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 19, CAST(@ShowtimeDate AS DATETIME2))), 210.00, 280.00, 380.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Inox1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Inox1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 20)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Inox1, DATEADD(MINUTE, 0, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 22, CAST(@ShowtimeDate AS DATETIME2))), 230.00, 300.00, 400.00, 1, SYSUTCDATETIME());
    END

    -- PVR Crystal Mall (Screen 4 - IMAX Experience)
    IF @Sc_Pvr1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Pvr1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Pvr1, DATEADD(MINUTE, 0, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), 250.00, 350.00, 480.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Pvr1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Pvr1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 13)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Pvr1, DATEADD(MINUTE, 15, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 270.00, 380.00, 520.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Pvr1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Pvr1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 17)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Pvr1, DATEADD(MINUTE, 0, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 19, CAST(@ShowtimeDate AS DATETIME2))), 300.00, 420.00, 580.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Pvr1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Pvr1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 20)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Pvr1, DATEADD(MINUTE, 30, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 23, CAST(@ShowtimeDate AS DATETIME2))), 320.00, 450.00, 600.00, 1, SYSUTCDATETIME());
    END

    -- Cinemax: The Imperial Palace
    IF @Sc_Cinemax IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cinemax AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 11)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cinemax, DATEADD(MINUTE, 0, DATEADD(HOUR, 11, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 260.00, 360.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Cinemax IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cinemax AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 14)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cinemax, DATEADD(MINUTE, 30, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), 200.00, 280.00, 380.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Cinemax IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cinemax AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cinemax, DATEADD(MINUTE, 0, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), 220.00, 300.00, 400.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Cinemax IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cinemax AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 21)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Cinemax, DATEADD(MINUTE, 30, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 24, CAST(@ShowtimeDate AS DATETIME2))), 210.00, 290.00, 390.00, 1, SYSUTCDATETIME());
    END

    -- Time Cinema
    IF @Sc_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Time AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Time, DATEADD(MINUTE, 30, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 220.00, 300.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Time AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 13)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Time, DATEADD(MINUTE, 45, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 170.00, 240.00, 320.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Time AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 17)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Time, DATEADD(MINUTE, 15, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), 190.00, 260.00, 350.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Time AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 20)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Time, DATEADD(MINUTE, 45, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 23, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 250.00, 330.00, 1, SYSUTCDATETIME());
    END

    -- Rajshree Cinema
    IF @Sc_Rajshree IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Rajshree AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 12)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Rajshree, DATEADD(MINUTE, 15, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), 130.00, 180.00, 250.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Rajshree IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Rajshree AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 15)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Rajshree, DATEADD(MINUTE, 45, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 200.00, 270.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Rajshree IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Rajshree AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 19)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Rajshree, DATEADD(MINUTE, 0, DATEADD(HOUR, 19, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), 170.00, 220.00, 290.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Rajshree IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Rajshree AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 22)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Rajshree, DATEADD(MINUTE, 15, DATEADD(HOUR, 22, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 25, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 210.00, 280.00, 1, SYSUTCDATETIME());
    END

    -- Galaxy Cinema
    IF @Sc_Galaxy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Galaxy AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Galaxy, DATEADD(MINUTE, 45, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 30, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 120.00, 170.00, 240.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Galaxy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Galaxy AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 14)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Galaxy, DATEADD(MINUTE, 0, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 140.00, 190.00, 260.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Galaxy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Galaxy AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Galaxy, DATEADD(MINUTE, 15, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 210.00, 280.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Galaxy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Galaxy AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 21)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, @Sc_Galaxy, DATEADD(MINUTE, 30, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 24, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 200.00, 270.00, 1, SYSUTCDATETIME());
    END

    -- ========================================================================
    -- MovieId 14: Drishyam The Conclusion (Cosmoplex, INOX, PVR, Cinemax)
    -- ========================================================================
    IF @Sc_Cosmoplex2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex2 AND MovieId = 14 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 11)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (14, @Sc_Cosmoplex2, DATEADD(MINUTE, 0, DATEADD(HOUR, 11, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 170.00, 240.00, 330.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Inox2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Inox2 AND MovieId = 14 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 14)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (14, @Sc_Inox2, DATEADD(MINUTE, 30, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 250.00, 340.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Pvr2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Pvr2 AND MovieId = 14 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (14, @Sc_Pvr2, DATEADD(MINUTE, 15, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 0, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), 220.00, 300.00, 420.00, 1, SYSUTCDATETIME());
    END

    -- ========================================================================
    -- MovieId 1: Singham Again (Cosmoplex, INOX, PVR, Time Cinema, Galaxy)
    -- ========================================================================
    IF @Sc_Cosmoplex2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex2 AND MovieId = 1 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 15)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (1, @Sc_Cosmoplex2, DATEADD(MINUTE, 30, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 20, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 250.00, 350.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Inox2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Inox2 AND MovieId = 1 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (1, @Sc_Inox2, DATEADD(MINUTE, 0, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 50, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), 190.00, 260.00, 360.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Time IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Time AND MovieId = 1 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 14)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (1, @Sc_Time, DATEADD(MINUTE, 0, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 50, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 230.00, 320.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Galaxy IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Galaxy AND MovieId = 1 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 12)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (1, @Sc_Galaxy, DATEADD(MINUTE, 30, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 20, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), 130.00, 180.00, 250.00, 1, SYSUTCDATETIME());
    END

    -- ========================================================================
    -- MovieId 2: Kasoombo (Gujarati Blockbuster - Cosmoplex, Rajshree, Galaxy, Time)
    -- ========================================================================
    IF @Sc_Cosmoplex2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Cosmoplex2 AND MovieId = 2 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 19)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (2, @Sc_Cosmoplex2, DATEADD(MINUTE, 15, DATEADD(HOUR, 19, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 5, DATEADD(HOUR, 22, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 220.00, 300.00, 1, SYSUTCDATETIME());
    END

    IF @Sc_Rajshree IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = @Sc_Rajshree AND MovieId = 2 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (2, @Sc_Rajshree, DATEADD(MINUTE, 0, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 50, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), 120.00, 170.00, 240.00, 1, SYSUTCDATETIME());
    END

    SET @DayOffset = @DayOffset + 1;
END;
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 5. Populate Physical ShowtimeSeats for all newly added showtimes
DECLARE @StId INT;
DECLARE @ScId INT;

DECLARE new_st_cursor CURSOR FOR
SELECT s.ShowtimeId, s.ScreenId
FROM dbo.Showtimes s
WHERE NOT EXISTS (SELECT 1 FROM dbo.ShowtimeSeats ss WHERE ss.ShowtimeId = s.ShowtimeId);

OPEN new_st_cursor;
FETCH NEXT FROM new_st_cursor INTO @StId, @ScId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO dbo.ShowtimeSeats (ShowtimeId, SeatId, Status)
    SELECT @StId, s.SeatId, 'Available'
    FROM dbo.Seats s
    WHERE s.ScreenId = @ScId AND s.IsActive = 1;

    FETCH NEXT FROM new_st_cursor INTO @StId, @ScId;
END;

CLOSE new_st_cursor;
DEALLOCATE new_st_cursor;
GO

PRINT 'Successfully seeded all Rajkot theatres, screens, seats, and multi-theatre showtimes.';
GO
