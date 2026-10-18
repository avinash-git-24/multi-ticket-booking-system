-- ============================================================================
-- ShowSphere Seed: Real Theatrical Posters, Full Premieres Rail & High-Res Events
-- Local asset paths for 100% offline reliability during college live demonstration
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Update Existing Movies with Local High-Resolution Theatrical Posters
UPDATE dbo.Movies
SET PosterUrl = '/images/posters/Singham_Again.jpg',
    BannerUrl = '/images/banners/blockbuster_hero.jpg'
WHERE MovieId = 1;

UPDATE dbo.Movies
SET PosterUrl = '/images/posters/Kasoombo.jpg',
    BannerUrl = '/images/banners/navratri_hero.jpg'
WHERE MovieId = 2;

UPDATE dbo.Movies
SET PosterUrl = '/images/posters/Stree_2.jpg',
    BannerUrl = '/images/banners/blockbuster_hero.jpg'
WHERE MovieId = 3;

UPDATE dbo.Movies
SET PosterUrl = '/images/posters/Kalki_2898_AD.jpg',
    BannerUrl = '/images/banners/blockbuster_hero.jpg'
WHERE MovieId = 4;

UPDATE dbo.Movies
SET PosterUrl = '/images/posters/Bhool_Bhulaiyaa_3.jpg',
    BannerUrl = '/images/banners/blockbuster_hero.jpg'
WHERE MovieId = 5;

UPDATE dbo.Movies
SET PosterUrl = '/images/posters/Pushpa_2_The_Rule.jpg',
    BannerUrl = '/images/banners/blockbuster_hero.jpg',
    Status = 'Upcoming'
WHERE MovieId = 6;
GO

-- 2. Insert Additional Blockbusters & Premiere Titles
SET IDENTITY_INSERT dbo.Movies ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 7)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (7, 'Devara: Part 1', 'An epic coastal struggle between fierce sea warriors where loyalty clashes with survival and ruthless power.', 170, 'Hindi, Telugu', 'Action, Drama', '2024-09-27', 'A', '/images/posters/Devara_Part_1.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 8)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (8, 'Jigra', 'A fiercely protective sister embarks on a dangerous rescue mission to break her brother out of a foreign prison.', 155, 'Hindi', 'Action, Thriller', '2024-10-11', 'UA', '/images/posters/Jigra.jpg', '/images/banners/blockbuster_hero.jpg', 'CurrentlyShowing', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 9)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (9, 'Transformers One', 'The untold origin story of Optimus Prime and Megatron, exploring how bonded brothers became sworn enemies on Cybertron.', 104, 'English, Hindi', 'Animation, Sci-Fi', '2024-09-20', 'U', '/images/posters/Transformers_One.jpg', '/images/banners/blockbuster_hero.jpg', 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 10)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (10, 'Venom: The Last Dance', 'Eddie and Venom are on the run, hunted by both worlds. As the net closes, they face a devastating final choice.', 109, 'English, Hindi', 'Action, Sci-Fi', '2024-10-25', 'UA', '/images/posters/Venom_The_Last_Dance.jpg', '/images/banners/blockbuster_hero.jpg', 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 11)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (11, 'Alien: Romulus', 'While scavenging the deep ends of a derelict space station, young colonizers face the most terrifying life form in the universe.', 119, 'English', 'Horror, Sci-Fi', '2024-08-16', 'A', '/images/posters/Alien_Romulus.jpg', '/images/banners/blockbuster_hero.jpg', 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Movies WHERE MovieId = 12)
BEGIN
    INSERT INTO dbo.Movies (MovieId, Title, Description, DurationMinutes, Language, Genre, ReleaseDate, Certificate, PosterUrl, BannerUrl, Status, CreatedAtUtc)
    VALUES (12, 'A Quiet Place: Day One', 'Experience the terrifying day the world went quiet in New York City as ultrasonic alien predators invade.', 99, 'English', 'Horror, Drama', '2024-06-28', 'UA', '/images/posters/A_Quiet_Place_Day_One.jpg', '/images/banners/blockbuster_hero.jpg', 'Upcoming', SYSUTCDATETIME());
END

SET IDENTITY_INSERT dbo.Movies OFF;
GO

-- 3. Update Existing Events with Local High-Resolution Imagery
UPDATE dbo.Events
SET BannerUrl = '/images/events/Arijit_Singh_Live.jpg'
WHERE EventId = 1;

UPDATE dbo.Events
SET BannerUrl = '/images/events/Zakir_Khan_Comedy.jpg'
WHERE EventId = 2;

UPDATE dbo.Events
SET BannerUrl = '/images/events/Cloud_Architect_Workshop.jpg'
WHERE EventId = 3;

UPDATE dbo.Events
SET BannerUrl = '/images/events/Ranji_Trophy_Cricket.jpg'
WHERE EventId = 4;

UPDATE dbo.Events
SET BannerUrl = '/images/events/Rangilo_Gujarat_Natak.jpg'
WHERE EventId = 5;

UPDATE dbo.Events
SET BannerUrl = '/images/events/Coldplay_Live.jpg'
WHERE EventId = 6;
GO

-- 4. Seed Additional Events for Complete Event Rails
SET IDENTITY_INSERT dbo.Events ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 7)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        7, 
        'Sunidhi Chauhan: I Am Home Tour', 
        'High-voltage energetic Bollywood live concert featuring Sunidhi Chauhan with a 40-piece live orchestra band.', 
        'Concert', 
        1, 
        1, 
        '/images/events/Sunidhi_Chauhan.jpg', 
        DATEADD(DAY, 12, CAST(GETDATE() AS DATETIME2)), 
        DATEADD(DAY, 12, DATEADD(HOUR, 4, CAST(GETDATE() AS DATETIME2))), 
        'Upcoming', 
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 8)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        8, 
        'Anubhav Singh Bassi: Kisi Ko Batana Mat', 
        'Anubhav Singh Bassi brings an all-new hilarious standup special filled with relatable stories and raw comedic timing.', 
        'Comedy', 
        1, 
        3, 
        '/images/events/Bassi_Comedy.jpg', 
        DATEADD(DAY, 9, CAST(GETDATE() AS DATETIME2)), 
        DATEADD(DAY, 9, DATEADD(HOUR, 2, CAST(GETDATE() AS DATETIME2))), 
        'Upcoming', 
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 9)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        9, 
        'Samay Raina: Unfiltered Live', 
        'Supreme Leader Samay Raina brings an evening of sharp crowd work, witty observations, and unfiltered humor.', 
        'Comedy', 
        1, 
        3, 
        '/images/events/Samay_Raina.jpg', 
        DATEADD(DAY, 15, CAST(GETDATE() AS DATETIME2)), 
        DATEADD(DAY, 15, DATEADD(HOUR, 2, CAST(GETDATE() AS DATETIME2))), 
        'Upcoming', 
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 10)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        10, 
        'Hu Chandrakant Bakshi: Iconic Monologue', 
        'Pratik Gandhi stars in this celebrated biographical drama on the legendary rebel Gujarati author Chandrakant Bakshi.', 
        'Theatre', 
        1, 
        1, 
        '/images/events/Hu_Chandrakant_Bakshi.jpg', 
        DATEADD(DAY, 22, CAST(GETDATE() AS DATETIME2)), 
        DATEADD(DAY, 22, DATEADD(HOUR, 3, CAST(GETDATE() AS DATETIME2))), 
        'Upcoming', 
        SYSUTCDATETIME()
    );
END

SET IDENTITY_INSERT dbo.Events OFF;
GO

-- 5. Seed Sessions for Events 7 to 10
SET IDENTITY_INSERT dbo.EventSessions ON;

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 7)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (7, 7, 'Sunidhi Chauhan Grand Tour', DATEADD(HOUR, 19, DATEADD(DAY, 12, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 23, DATEADD(DAY, 12, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 8)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (8, 8, 'Bassi Comedy Evening', DATEADD(HOUR, 18, DATEADD(DAY, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 20, DATEADD(DAY, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 9)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (9, 9, 'Samay Raina Unfiltered', DATEADD(HOUR, 20, DATEADD(DAY, 15, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 22, DATEADD(DAY, 15, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 10)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (10, 10, 'Hu Chandrakant Bakshi Natak', DATEADD(HOUR, 20, DATEADD(DAY, 22, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 23, DATEADD(DAY, 22, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

SET IDENTITY_INSERT dbo.EventSessions OFF;
GO

-- 6. Ticket Categories for Sessions of Events 7 to 10
SET IDENTITY_INSERT dbo.TicketCategories ON;

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 16)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES 
    (16, 7, 7, 'Silver: General Standing', 699.00, 500, 480, 8, 'Ground standing pass with direct view of main stage'),
    (17, 7, 7, 'Gold: Fan Pit Front', 1499.00, 200, 190, 4, 'Front row barricade access, concert wristband');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 18)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES 
    (18, 8, 8, 'Silver: Balcony Class', 499.00, 250, 240, 8, 'Auditorium upper tier seating'),
    (19, 8, 8, 'Platinum: Front Rows', 999.00, 100, 95, 4, 'Central front row seats');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 20)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES 
    (20, 9, 9, 'Standard Pass', 399.00, 300, 280, 8, 'General auditorium seating'),
    (21, 9, 9, 'VIP Meet & Greet', 899.00, 60, 50, 4, 'Reserved front seats and artist interaction');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 22)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES 
    (22, 10, 10, 'General Balcony', 250.00, 200, 190, 8, 'Hemu Gadhvi auditorium seating'),
    (23, 10, 10, 'Executive Front', 500.00, 150, 140, 4, 'VIP front rows reserved seats');
END

SET IDENTITY_INSERT dbo.TicketCategories OFF;
GO

-- 7. Populate ShowtimeSeats for Any Showtimes Missing Inventory
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
    SELECT 
        @StId,
        st.SeatId,
        'Available'
    FROM dbo.Seats st
    WHERE st.ScreenId = @ScId;

    FETCH NEXT FROM st_cursor INTO @StId, @ScId;
END;

CLOSE st_cursor;
DEALLOCATE st_cursor;
GO
