-- ============================================================================
-- ShowSphere Seed: 10 Live BookMyShow Rajkot Currently Showing Movies
-- Seed MovieId 13 to 22 with Local Theatrical Posters and Active Showtimes
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Insert 10 BookMyShow Rajkot Movies (MovieId 13 to 22)
SET IDENTITY_INSERT dbo.Movies ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 13)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (13, 'Rajini: The Jailer 2', 'Superstar Rajinikanth returns as Tiger Muthuvel Pandian in Jailer 2, dealing with formidable adversaries to protect justice and his family.', 165, 'Tamil, Hindi, Telugu', 'Action/Thriller', '2026-10-18', 'UA', '/images/movies/Rajini_The_Jailer_2.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Rajini: The Jailer 2',
        Description = 'Superstar Rajinikanth returns as Tiger Muthuvel Pandian in Jailer 2, dealing with formidable adversaries to protect justice and his family.',
        DurationMinutes = 165,
        Language = 'Tamil, Hindi, Telugu',
        Genre = 'Action/Thriller',
        ReleaseDate = '2026-10-18',
        Certificate = 'UA',
        PosterUrl = '/images/movies/Rajini_The_Jailer_2.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 13;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 14)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (14, 'Drishyam The Conclusion', 'Vijay Salgaonkar faces the ultimate test of wit and resilience to protect his family in this thrilling conclusion to the iconic suspense saga.', 152, 'Hindi', 'Drama/Mystery/Thriller', '2026-10-17', 'UA', '/images/movies/Drishyam_The_Conclusion.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Drishyam The Conclusion',
        Description = 'Vijay Salgaonkar faces the ultimate test of wit and resilience to protect his family in this thrilling conclusion to the iconic suspense saga.',
        DurationMinutes = 152,
        Language = 'Hindi',
        Genre = 'Drama/Mystery/Thriller',
        ReleaseDate = '2026-10-17',
        Certificate = 'UA',
        PosterUrl = '/images/movies/Drishyam_The_Conclusion.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 14;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 15)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (15, 'Nayyi Navelli', 'A hilarious and spooky rollercoaster as a newlywed couple encounters eccentric spirits with unfinished business in their ancestral home.', 135, 'Hindi', 'Comedy/Family/Horror', '2026-10-16', 'UA', '/images/movies/Nayyi_Navelli.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Nayyi Navelli',
        Description = 'A hilarious and spooky rollercoaster as a newlywed couple encounters eccentric spirits with unfinished business in their ancestral home.',
        DurationMinutes = 135,
        Language = 'Hindi',
        Genre = 'Comedy/Family/Horror',
        ReleaseDate = '2026-10-16',
        Certificate = 'UA',
        PosterUrl = '/images/movies/Nayyi_Navelli.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 15;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 16)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (16, 'Prahaar', 'An intense, hard-hitting biographical drama chronicling valor, uncompromising principles, sacrifice, and duty in the face of insurmountable odds.', 140, 'Hindi', 'Biography/Drama', '2026-10-15', 'UA', '/images/movies/Prahaar.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Prahaar',
        Description = 'An intense, hard-hitting biographical drama chronicling valor, uncompromising principles, sacrifice, and duty in the face of insurmountable odds.',
        DurationMinutes = 140,
        Language = 'Hindi',
        Genre = 'Biography/Drama',
        ReleaseDate = '2026-10-15',
        Certificate = 'UA',
        PosterUrl = '/images/movies/Prahaar.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 16;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 17)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (17, 'Street Fighter', 'Elite martial artists and fighters from around the globe assemble to clash in a high-stakes, underground tournament for honor and survival.', 125, 'English, Hindi', 'Action/Adventure/Comedy/Thriller', '2026-10-14', 'UA', '/images/movies/Street_Fighter.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Street Fighter',
        Description = 'Elite martial artists and fighters from around the globe assemble to clash in a high-stakes, underground tournament for honor and survival.',
        DurationMinutes = 125,
        Language = 'English, Hindi',
        Genre = 'Action/Adventure/Comedy/Thriller',
        ReleaseDate = '2026-10-14',
        Certificate = 'UA',
        PosterUrl = '/images/movies/Street_Fighter.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 17;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 18)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (18, 'Hanuman Ansh', 'A grand, inspirational devotional drama capturing the timeless devotion, boundless courage, and divine virtues of Lord Hanuman.', 158, 'Hindi, Telugu', 'Biography/Devotional/Drama', '2026-10-13', 'U', '/images/movies/Hanuman_Ansh.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Hanuman Ansh',
        Description = 'A grand, inspirational devotional drama capturing the timeless devotion, boundless courage, and divine virtues of Lord Hanuman.',
        DurationMinutes = 158,
        Language = 'Hindi, Telugu',
        Genre = 'Biography/Devotional/Drama',
        ReleaseDate = '2026-10-13',
        Certificate = 'U',
        PosterUrl = '/images/movies/Hanuman_Ansh.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 18;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 19)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (19, 'Doraemon: Castle of the Undersea Devil', 'Nobita, Doraemon, and friends dive into the deepest mysteries of the ocean floor to prevent an ancient mechanical evil from rising.', 98, 'Hindi, Japanese', 'Adventure/Animation/Comedy/Family/Fantasy', '2026-10-12', 'U', '/images/movies/Doraemon_Castle.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Doraemon: Castle of the Undersea Devil',
        Description = 'Nobita, Doraemon, and friends dive into the deepest mysteries of the ocean floor to prevent an ancient mechanical evil from rising.',
        DurationMinutes = 98,
        Language = 'Hindi, Japanese',
        Genre = 'Adventure/Animation/Comedy/Family/Fantasy',
        ReleaseDate = '2026-10-12',
        Certificate = 'U',
        PosterUrl = '/images/movies/Doraemon_Castle.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 19;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 20)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (20, 'The Vvaan - Force of the Forrest', 'An ancient forest entity awakens to confront modern exploitation, leading to a clash between primal supernatural forces and humankind.', 142, 'Hindi', 'Action/Adventure/Fantasy/Mystery/Thriller', '2026-10-11', 'UA', '/images/movies/The_Vvaan.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'The Vvaan - Force of the Forrest',
        Description = 'An ancient forest entity awakens to confront modern exploitation, leading to a clash between primal supernatural forces and humankind.',
        DurationMinutes = 142,
        Language = 'Hindi',
        Genre = 'Action/Adventure/Fantasy/Mystery/Thriller',
        ReleaseDate = '2026-10-11',
        Certificate = 'UA',
        PosterUrl = '/images/movies/The_Vvaan.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 20;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 21)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (21, 'Mitrata', 'A heartwarming, lighthearted Gujarati comedy drama celebrating lifelong friendship, loyalty, and unforgettable hometown moments.', 130, 'Gujarati', 'Comedy/Drama', '2026-10-10', 'U', '/images/movies/Mitrata.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Mitrata',
        Description = 'A heartwarming, lighthearted Gujarati comedy drama celebrating lifelong friendship, loyalty, and unforgettable hometown moments.',
        DurationMinutes = 130,
        Language = 'Gujarati',
        Genre = 'Comedy/Drama',
        ReleaseDate = '2026-10-10',
        Certificate = 'U',
        PosterUrl = '/images/movies/Mitrata.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 21;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 22)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (22, 'Whalefall', 'A diver trapped inside the belly of an eighty-foot sperm whale must race against the ticking clock of depleting oxygen to find an escape.', 108, 'English, Hindi', 'Adventure/Drama/Thriller', '2026-10-09', 'UA', '/images/movies/Whalefall.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END
ELSE
BEGIN
    UPDATE dbo.Movies
    SET Title = 'Whalefall',
        Description = 'A diver trapped inside the belly of an eighty-foot sperm whale must race against the ticking clock of depleting oxygen to find an escape.',
        DurationMinutes = 108,
        Language = 'English, Hindi',
        Genre = 'Adventure/Drama/Thriller',
        ReleaseDate = '2026-10-09',
        Certificate = 'UA',
        PosterUrl = '/images/movies/Whalefall.jpg',
        Status = 'CurrentlyShowing'
    WHERE MovieId = 22;
END

SET IDENTITY_INSERT dbo.Movies OFF;
GO

-- 2. Seed Dynamic Showtimes for Today and Next 7 Days in Rajkot Screens (1 to 4)
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
DECLARE @DayOffset INT = 0;
DECLARE @ShowtimeDate DATE;

WHILE @DayOffset < 7
BEGIN
    SET @ShowtimeDate = DATEADD(DAY, @DayOffset, @Today);

    -- Cosmoplex Multiplex Audi 1 (Screen 1) - Rajini: The Jailer 2 (MovieId 13)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 1 AND MovieId = 13 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 11)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (13, 1, DATEADD(MINUTE, 0, DATEADD(HOUR, 11, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 45, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 250.00, 350.00, 1, SYSUTCDATETIME());
    END

    -- Cosmoplex Multiplex Audi 1 (Screen 1) - Drishyam The Conclusion (MovieId 14)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 1 AND MovieId = 14 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 15)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (14, 1, DATEADD(MINUTE, 15, DATEADD(HOUR, 15, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 47, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 230.00, 320.00, 1, SYSUTCDATETIME());
    END

    -- Cosmoplex Multiplex Audi 2 (Screen 2) - Nayyi Navelli (MovieId 15)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 2 AND MovieId = 15 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 12)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (15, 2, DATEADD(MINUTE, 0, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 15, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), 140.00, 200.00, 280.00, 1, SYSUTCDATETIME());
    END

    -- Cosmoplex Multiplex Audi 2 (Screen 2) - Prahaar (MovieId 16)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 2 AND MovieId = 16 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 16)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (16, 2, DATEADD(MINUTE, 30, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 50, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 210.00, 290.00, 1, SYSUTCDATETIME());
    END

    -- INOX Multiplex Screen 1 (Screen 3) - Street Fighter (MovieId 17)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 3 AND MovieId = 17 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 11)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (17, 3, DATEADD(MINUTE, 0, DATEADD(HOUR, 11, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 5, DATEADD(HOUR, 13, CAST(@ShowtimeDate AS DATETIME2))), 150.00, 220.00, 300.00, 1, SYSUTCDATETIME());
    END

    -- INOX Multiplex Screen 1 (Screen 3) - Hanuman Ansh (MovieId 18)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 3 AND MovieId = 18 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 17)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (18, 3, DATEADD(MINUTE, 0, DATEADD(HOUR, 17, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 38, DATEADD(HOUR, 19, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 230.00, 320.00, 1, SYSUTCDATETIME());
    END

    -- PVR Crystal Mall Audi 1 (Screen 4) - Doraemon: Castle of the Undersea Devil (MovieId 19)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 4 AND MovieId = 19 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 10)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (19, 4, DATEADD(MINUTE, 30, DATEADD(HOUR, 10, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 8, DATEADD(HOUR, 12, CAST(@ShowtimeDate AS DATETIME2))), 170.00, 240.00, 330.00, 1, SYSUTCDATETIME());
    END

    -- PVR Crystal Mall Audi 1 (Screen 4) - The Vvaan - Force of the Forrest (MovieId 20)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 4 AND MovieId = 20 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 14)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (20, 4, DATEADD(MINUTE, 0, DATEADD(HOUR, 14, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 22, DATEADD(HOUR, 16, CAST(@ShowtimeDate AS DATETIME2))), 180.00, 250.00, 340.00, 1, SYSUTCDATETIME());
    END

    -- PVR Crystal Mall Audi 1 (Screen 4) - Mitrata (MovieId 21)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 4 AND MovieId = 21 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 18)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (21, 4, DATEADD(MINUTE, 0, DATEADD(HOUR, 18, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 10, DATEADD(HOUR, 20, CAST(@ShowtimeDate AS DATETIME2))), 160.00, 230.00, 320.00, 1, SYSUTCDATETIME());
    END

    -- PVR Crystal Mall Audi 1 (Screen 4) - Whalefall (MovieId 22)
    IF NOT EXISTS (SELECT 1 FROM dbo.Showtimes WHERE ScreenId = 4 AND MovieId = 22 AND CAST(StartTime AS DATE) = @ShowtimeDate AND DATEPART(HOUR, StartTime) = 21)
    BEGIN
        INSERT INTO dbo.Showtimes (MovieId, ScreenId, StartTime, EndTime, SilverPrice, GoldPrice, PlatinumPrice, IsActive, CreatedAtUtc)
        VALUES (22, 4, DATEADD(MINUTE, 30, DATEADD(HOUR, 21, CAST(@ShowtimeDate AS DATETIME2))), DATEADD(MINUTE, 18, DATEADD(HOUR, 23, CAST(@ShowtimeDate AS DATETIME2))), 170.00, 240.00, 330.00, 1, SYSUTCDATETIME());
    END

    SET @DayOffset = @DayOffset + 1;
END;
GO

-- 3. Populate Physical ShowtimeSeats for all newly added showtimes
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

PRINT 'Successfully seeded 10 BookMyShow Rajkot movies (MovieId 13 to 22) with active showtimes and seat maps.';
GO
