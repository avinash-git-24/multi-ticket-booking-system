-- ============================================================================
-- ShowSphere Seed: BookMyShow Exact Rajkot Events
-- Replicates the exact 4 top events from BookMyShow Rajkot discovery screenshot
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Venues
SET IDENTITY_INSERT dbo.Venues ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 5)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive, CreatedAtUtc)
    VALUES (5, 'Gift City: Ahmedabad', 2, 'GIFT City, Gandhinagar, Gujarat 382355', 5000, 1, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 6)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive, CreatedAtUtc)
    VALUES (6, 'HB Minitorium: Rajkot', 1, 'Near Astron Cinema, Sardar Nagar, Rajkot, Gujarat 360001', 350, 1, SYSUTCDATETIME());
END

SET IDENTITY_INSERT dbo.Venues OFF;
GO

-- 2. Events
SET IDENTITY_INSERT dbo.Events ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 23)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        23,
        'AR Rahman The Wonderment Tour Live in Concert',
        'Academy and Grammy Award winner A. R. Rahman brings his monumental The Wonderment Tour to Gujarat with state-of-the-art immersive visual production and his legendary musical ensemble.',
        'Concert',
        1,
        5,
        '/images/events/AR_Rahman_Wonderment.jpg',
        '2027-03-27 19:30:00',
        '2027-03-27 23:30:00',
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 24)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        24,
        'Arshaan Live in Concert',
        '1st time in Gujarat! Arshaan live with an unplugged melody musical concert featuring chartbuster playback tracks and soulful sufi fusion.',
        'Concert',
        1,
        1,
        '/images/events/Arshaan_Concert.jpg',
        '2026-10-30 20:00:00',
        '2026-10-30 23:00:00',
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 25)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        25,
        'Hit''s of A R RAHMAN',
        'Presented by Shiva Friends Musical Group - Rajkot. A grand musical tribute to the Mozart of Madras featuring top instrumentalists and vocalists.',
        'Concert',
        1,
        1,
        '/images/events/Hits_Of_AR_Rahman.jpg',
        '2026-10-31 21:00:00',
        '2026-10-31 23:45:00',
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 26)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        26,
        'Unfiltered Bakajiki - Panel Standup Comedy Show',
        'Ace Comedy Club presents Unfiltered Bakajiki. Inspired from panel comedy shows with unexpected punchlines, unfiltered crowd work, and surprise acts.',
        'Comedy',
        1,
        6,
        '/images/events/Unfiltered_Bakajiki.jpg',
        '2026-10-25 18:00:00',
        '2026-10-25 20:30:00',
        'Upcoming',
        SYSUTCDATETIME()
    );
END

SET IDENTITY_INSERT dbo.Events OFF;
GO

-- 3. Event Sessions
SET IDENTITY_INSERT dbo.EventSessions ON;

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 23)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (23, 23, 'Main Arena Concert', '2026-10-20 19:30:00', '2026-10-20 23:30:00', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 24)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (24, 24, 'Evening Musical Night', '2026-10-21 20:00:00', '2026-10-21 23:00:00', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 25)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (25, 25, 'Tribute Gala Show', '2026-10-22 21:00:00', '2026-10-22 23:45:00', 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 26)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (26, 26, 'Standup Comedy Night', '2026-10-23 18:00:00', '2026-10-23 20:30:00', 1);
END

SET IDENTITY_INSERT dbo.EventSessions OFF;
GO

-- 4. Ticket Categories
SET IDENTITY_INSERT dbo.TicketCategories ON;

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 2030)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (2030, 23, 23, 'General Access', 999.00, 300, 280, 6, 'Arena standing entry, standard stage view');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 2031)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (2031, 24, 24, 'Balcony Pass', 300.00, 200, 195, 6, 'Auditorium balcony seating');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 2032)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (2032, 25, 25, 'Standard Pass', 99.00, 250, 240, 6, 'Auditorium entry ticket');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 2033)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (2033, 26, 26, 'Entry Pass', 120.00, 150, 142, 6, 'Unfiltered comedy club seating');
END

SET IDENTITY_INSERT dbo.TicketCategories OFF;
GO
