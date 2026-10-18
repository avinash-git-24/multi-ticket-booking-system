-- ============================================================================
-- ShowSphere Demo Seed Data: Movies, Showtimes, Events, and DataSources
-- For College Live Demonstration in Rajkot, Gujarat
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Seed Movies
SET IDENTITY_INSERT dbo.Movies ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 1)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (1, 'Singham Again', 'Bajirao Singham returns to battle a formidable new enemy across India, assembling a high-stakes squad to rescue his family and restore justice.', 144, 'Hindi', 'Action, Drama', '2024-11-01', 'UA', 'https://images.unsplash.com/photo-1536440136628-849c177e76a1?w=600&auto=format&fit=crop&q=80', 'https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=1200&auto=format&fit=crop&q=80', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 2)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (2, 'Kasoombo', 'Based on Vimal Kumar Dhami novel Amar Balidan, depicting Dadu Barot and 51 brave warriors who stood against the ruthless Sultanate army to defend the sacred temples of Palitana, Gujarat.', 152, 'Gujarati', 'Historical, Action, Drama', '2024-02-16', 'UA', 'https://images.unsplash.com/photo-1578328819058-b69f3a3b0f6b?w=600&auto=format&fit=crop&q=80', 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23?w=1200&auto=format&fit=crop&q=80', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 3)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (3, 'Stree 2', 'The town of Chanderi is plagued by a headless terror named Sarkata. Vicky and his friends must summon the power of the enigmatic Stree to safeguard their people.', 149, 'Hindi', 'Comedy, Horror', '2024-08-15', 'UA', 'https://images.unsplash.com/photo-1509281373149-e957c6296406?w=600&auto=format&fit=crop&q=80', 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23?w=1200&auto=format&fit=crop&q=80', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 4)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (4, 'Kalki 2898 AD', 'Set in a post-apocalyptic world in the year 2898 AD, an avatar of Lord Vishnu descends to protect humanity from dark authoritarian forces.', 181, 'Hindi', 'Sci-Fi, Action', '2024-06-27', 'UA', 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=600&auto=format&fit=crop&q=80', 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=1200&auto=format&fit=crop&q=80', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 5)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (5, 'Bhool Bhulaiyaa 3', 'Ruhaan visits the royal estate of Rakht Ghat in West Bengal where he is confronted by two vengeful spirits claiming to be the real Manjulika.', 158, 'Hindi', 'Comedy, Horror', '2024-11-01', 'UA', 'https://images.unsplash.com/photo-1514306191717-452ec28c7814?w=600&auto=format&fit=crop&q=80', 'https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=1200&auto=format&fit=crop&q=80', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 6)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (6, 'Pushpa 2: The Rule', 'Pushpa Raj continues his dominance in the red sandalwood syndicate while evading the ruthless police officer Bhanwar Singh Shekhawat in a fiery showdown.', 190, 'Hindi', 'Action, Drama', '2024-12-05', 'UA', 'https://images.unsplash.com/photo-1533928298208-27ff66555d8d?w=600&auto=format&fit=crop&q=80', 'https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=1200&auto=format&fit=crop&q=80', 'Upcoming', SYSUTCDATETIME());
END

SET IDENTITY_INSERT dbo.Movies OFF;
GO

-- 2. Seed Dynamic Showtimes for Today and Next 7 Days in Rajkot
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
DECLARE @DayOffset INT = 0;
DECLARE @ShowtimeDate DATE;

WHILE @DayOffset < 7
BEGIN
    SET @ShowtimeDate = DATEADD(DAY, @DayOffset, @Today);

    -- Cosmoplex Multiplex Audi 1 (Screen 1) - Singham Again
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 1 AND MovieId = 1 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (1, 1, DATEADD(MINUTE, 30, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 54, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 220.00, 300.00, 1, SYSUTCDATETIME());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 1 AND MovieId = 1 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (1, 1, DATEADD(MINUTE, 45, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 9, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 240.00, 320.00, 1, SYSUTCDATETIME());
    END

    -- Cosmoplex Multiplex Audi 2 (Screen 2) - Kasoombo
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 2 AND MovieId = 2 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 14)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (2, 2, DATEADD(MINUTE, 15, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 47, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 140.00, 200.00, 280.00, 1, SYSUTCDATETIME());
    END

    -- INOX Multiplex Screen 1 (Screen 3) - Stree 2
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 3 AND MovieId = 3 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 15)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (3, 3, DATEADD(MINUTE, 0, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 29, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 220.00, 300.00, 1, SYSUTCDATETIME());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 3 AND MovieId = 3 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 20)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (3, 3, DATEADD(MINUTE, 15, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 44, DATEADD(HOUR, 22, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 240.00, 320.00, 1, SYSUTCDATETIME());
    END

    -- PVR Crystal Mall Audi 1 (Screen 4) - Kalki 2898 AD
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 4 AND MovieId = 4 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 17)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (4, 4, DATEADD(MINUTE, 30, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 31, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 260.00, 350.00, 1, SYSUTCDATETIME());
    END

    SET @DayOffset = @DayOffset + 1;
END;
GO

-- 3. Populate ShowtimeSeats for All Created Showtimes from Screen Physical Seats
DECLARE @StId INT;
DECLARE @ScId INT;

DECLARE st_cursor CURSOR FOR
SELECT s.ShowtimeId, s.ScreenId
FROM dbo.Showtimes s
WHERE NOT EXISTS (SELECT 1 FROM dbo.ShowtimeSeats ss WHERE ss.ShowtimeId = s.ShowtimeId);

OPEN st_cursor;
FETCH NEXT FROM st_cursor INTO @StId, @ScId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO dbo.ShowtimeSeats (ShowtimeId, SeatId, Status)
    SELECT @StId, s.SeatId, 'Available'
    FROM dbo.Seats s
    WHERE s.ScreenId = @ScId AND s.IsActive = 1;

    FETCH NEXT FROM st_cursor INTO @StId, @ScId;
END;

CLOSE st_cursor;
DEALLOCATE st_cursor;
GO

-- 4. Record Data Provenance
IF NOT EXISTS (SELECT 1 FROM dbo.DataSources WHERE EntityType = 'Movie' AND EntityId = 1)
BEGIN
    INSERT INTO dbo.DataSources (EntityType, EntityId, SourceName, SourceUrl, VerifiedAtUtc, VerificationStatus, Notes)
    VALUES ('Movie', 1, 'Central Board of Film Certification / Rohit Shetty Picturez', 'https://www.cbfcindia.gov.in', SYSUTCDATETIME(), 'Verified', 'Official theatrical release metadata and censor certification.');
END

IF NOT EXISTS (SELECT 1 FROM dbo.DataSources WHERE EntityType = 'Movie' AND EntityId = 2)
BEGIN
    INSERT INTO dbo.DataSources (EntityType, EntityId, SourceName, SourceUrl, VerifiedAtUtc, VerificationStatus, Notes)
    VALUES ('Movie', 2, 'Big Box Series / Gujarat State Censor Registry', 'https://www.cbfcindia.gov.in', SYSUTCDATETIME(), 'Verified', 'Official Gujarati historical feature film certification and runtime.');
END
GO
