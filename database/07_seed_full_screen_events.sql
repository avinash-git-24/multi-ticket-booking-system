-- ============================================================================
-- ShowSphere Seed: Comprehensive Event Catalogue for Full-Screen Grid Experience
-- Seeds additional high-demand events so all rails have 5+ cards with zero empty slots
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

SET IDENTITY_INSERT dbo.Events ON;

-- 1. Additional Music Events
IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 11)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (11, 'Diljit Dosanjh: Dil-Luminati India Tour', 'Global Punjabi sensation Diljit Dosanjh brings his explosive arena tour with mind-blowing live production and chartbuster tracks.', 'Concert', 1, 2, '/images/events/Diljit_Dosanjh.jpg', DATEADD(DAY, 14, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 14, DATEADD(HOUR, 4, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 12)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (12, 'Shreya Ghoshal: All Hearts Symphony Tour', 'Melodious musical spectacle featuring Shreya Ghoshal performing immortal romantic melodies with a 35-piece acoustic orchestra.', 'Concert', 1, 1, '/images/events/Shreya_Ghoshal.jpg', DATEADD(DAY, 21, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 21, DATEADD(HOUR, 3, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 13)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (13, 'Amit Trivedi: Indradhanush Live Experience', 'National award-winning composer Amit Trivedi live with energetic folk-electronic fusion and soulful folk melodies.', 'Concert', 1, 2, '/images/events/Amit_Trivedi.jpg', DATEADD(DAY, 28, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 28, DATEADD(HOUR, 4, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

-- 2. Additional Standup Comedy Events
IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 14)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (14, 'Gaurav Kapoor: Live Standup Tour Rajkot', 'Relatable middle-class stories, razor-sharp everyday observations, and infectious high-energy comedy.', 'Comedy', 1, 3, '/images/events/Gaurav_Kapoor.jpg', DATEADD(DAY, 11, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 11, DATEADD(HOUR, 2, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 15)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (15, 'Harsh Gujral: Jo Bolta Hai Wohi Hota Hai', 'Brand new standup solo special by Harsh Gujral with spontaneous crowd interactions and witty punchlines.', 'Comedy', 1, 3, '/images/events/Harsh_Gujral.jpg', DATEADD(DAY, 17, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 17, DATEADD(HOUR, 2, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 16)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (16, 'Abhishek Upmanyu: Jealous of Sabziwala', 'Abhishek Upmanyu returns to Rajkot with his critically acclaimed hilarious routine on modern urban dilemmas.', 'Comedy', 1, 1, '/images/events/Abhishek_Upmanyu.jpg', DATEADD(DAY, 24, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 24, DATEADD(HOUR, 2, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

-- 3. Additional Gujarati & Hindi Theatre Plays
IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 17)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (17, 'Welcome Zindagi: Iconic Family Drama', 'Heart-touching story about father-son relationship, generational understanding, and the true essence of living life.', 'Theatre', 1, 1, '/images/events/Welcome_Zindagi.jpg', DATEADD(DAY, 13, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 13, DATEADD(HOUR, 3, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 18)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (18, '102 Not Out: Evergreen Comedy Drama', 'A 102-year-old father who wants to break the world record for the oldest living person and his 75-year-old traditional son.', 'Theatre', 1, 1, '/images/events/102_Not_Out.jpg', DATEADD(DAY, 20, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 20, DATEADD(HOUR, 3, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 19)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (19, 'Baa Retire Thay Chhe: Heartwarming Drama', 'Classic Gujarati masterpiece depicting a dedicated mother deciding to retire from household duties, making her family realize her value.', 'Theatre', 1, 3, '/images/events/Rangilo_Gujarat_Natak.jpg', DATEADD(DAY, 27, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 27, DATEADD(HOUR, 3, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

-- 4. Additional Sports & Arena Events
IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 20)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (20, 'Rajkot Heritage Half Marathon 2026', 'Official AIMS-certified city run celebrating Rajkot history, scenic routes around Race Course, and athletic medals.', 'Sports', 1, 2, '/images/events/Rajkot_Marathon.jpg', DATEADD(DAY, 10, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 10, DATEADD(HOUR, 5, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 21)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (21, 'Saurashtra Premier League: Rajkot vs Jamnagar', 'High-voltage T20 regional cricket championship under lights at SCA Stadium with live DJ and spectator fanfare.', 'Sports', 1, 2, '/images/events/Saurashtra_Premier_League.jpg', DATEADD(DAY, 16, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 16, DATEADD(HOUR, 4, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 22)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (22, 'Gujarat State Badminton Championship 2026', 'Premier state-level badminton tournament featuring elite singles and doubles players competing for national qualifications.', 'Sports', 1, 3, '/images/events/Ranji_Trophy_Cricket.jpg', DATEADD(DAY, 23, CAST(GETDATE() AS DATETIME2)), DATEADD(DAY, 23, DATEADD(HOUR, 6, CAST(GETDATE() AS DATETIME2))), 'Upcoming', SYSUTCDATETIME());
END

SET IDENTITY_INSERT dbo.Events OFF;
GO

-- 5. Seed EventSessions for Events 11 to 22
SET IDENTITY_INSERT dbo.EventSessions ON;

DECLARE @i INT = 11;
WHILE @i <= 22
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE EventId = @i)
    BEGIN
        INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
        VALUES (@i, @i, 'Main Show', DATEADD(HOUR, 19, DATEADD(DAY, @i, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 22, DATEADD(DAY, @i, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
    END
    SET @i = @i + 1;
END;

SET IDENTITY_INSERT dbo.EventSessions OFF;
GO

-- 6. Seed TicketCategories for Sessions 11 to 22
SET IDENTITY_INSERT dbo.TicketCategories ON;

DECLARE @cId INT = 30;
DECLARE @evId INT = 11;

WHILE @evId <= 22
BEGIN
    DECLARE @sessId INT = (SELECT TOP 1 SessionId FROM dbo.EventSessions WHERE EventId = @evId);
    IF @sessId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE SessionId = @sessId)
    BEGIN
        INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
        VALUES 
        (@cId, @evId, @sessId, 'General Admission', 499.00, 300, 280, 8, 'Full venue admission with clear view of stage'),
        (@cId + 1, @evId, @sessId, 'VIP Prime Experience', 1299.00, 100, 90, 4, 'Front row priority seating and welcome kit');
        SET @cId = @cId + 2;
    END
    SET @evId = @evId + 1;
END;

SET IDENTITY_INSERT dbo.TicketCategories OFF;
GO
