-- ============================================================================
-- ShowSphere Event Seed Data: Venues, Events, Sessions, TicketCategories, DataSources
-- For College Live Demonstration in Rajkot and Ahmedabad, Gujarat
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Seed Venues
SET IDENTITY_INSERT dbo.Venues ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 1)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive, CreatedAtUtc)
    VALUES (1, 'Hemu Gadhvi Natyagruh', 1, 'Tagore Road, Near Race Course, Rajkot, Gujarat 360001', 1000, 1, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 2)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive, CreatedAtUtc)
    VALUES (2, 'Saurashtra Cricket Association Stadium', 1, 'Jamnagar Highway, Khandheri, Rajkot, Gujarat 360110', 28000, 1, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 3)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive, CreatedAtUtc)
    VALUES (3, 'Pramukh Swami Auditorium', 1, 'Opposite Raiya Telephone Exchange, University Road, Rajkot, Gujarat 360005', 800, 1, SYSUTCDATETIME());
END

IF NOT EXISTS (SELECT 1 FROM dbo.Venues WHERE VenueId = 4)
BEGIN
    INSERT INTO dbo.Venues (VenueId, VenueName, CityId, Address, Capacity, IsActive, CreatedAtUtc)
    VALUES (4, 'Narendra Modi Stadium', 2, 'Stadium Road, Motera, Ahmedabad, Gujarat 380005', 132000, 1, SYSUTCDATETIME());
END

SET IDENTITY_INSERT dbo.Venues OFF;
GO

-- 2. Seed Events
SET IDENTITY_INSERT dbo.Events ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 1)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        1, 
        'Arijit Singh: Soulful Symphony Live Tour', 
        'Experience the magical evening with India''s most celebrated playback singer Arijit Singh accompanied by a grand 45-piece symphony orchestra performing chartbusters and acoustic renditions.',
        'Concert',
        1,
        2,
        'https://images.unsplash.com/photo-1501386761578-eac5c94b800a?w=1200&auto=format&fit=crop&q=80',
        DATEADD(DAY, 7, CAST(GETDATE() AS DATETIME2)),
        DATEADD(DAY, 7, DATEADD(HOUR, 4, CAST(GETDATE() AS DATETIME2))),
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 2)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        2, 
        'Zakir Khan: Tathastu Live Comedy Special', 
        'The Sakht Launda Zakir Khan brings his critically acclaimed heartwarming standup show to Rajkot with fresh stories about family, childhood, and navigating adulthood.',
        'Comedy',
        1,
        1,
        'https://images.unsplash.com/photo-1514525253161-7a46d19cd819?w=1200&auto=format&fit=crop&q=80',
        DATEADD(DAY, 5, CAST(GETDATE() AS DATETIME2)),
        DATEADD(DAY, 5, DATEADD(HOUR, 3, CAST(GETDATE() AS DATETIME2))),
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 3)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        3, 
        'Full-Stack Cloud & AI Architecture Masterclass 2026', 
        'Intensive hands-on technical workshop for computer engineering students and developers covering .NET 10 microservices, SQL Server concurrency patterns, and local AI integration.',
        'Workshop',
        1,
        3,
        'https://images.unsplash.com/photo-1531482615713-2afd69097998?w=1200&auto=format&fit=crop&q=80',
        DATEADD(DAY, 10, CAST(GETDATE() AS DATETIME2)),
        DATEADD(DAY, 10, DATEADD(HOUR, 6, CAST(GETDATE() AS DATETIME2))),
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 4)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        4, 
        'Ranji Trophy 2026: Saurashtra vs Mumbai Elite Clash', 
        'Watch defending domestic giants Saurashtra take on 42-time champions Mumbai at the iconic SCA Stadium in a premier 4-day first-class red-ball cricket match.',
        'Sports',
        1,
        2,
        'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=1200&auto=format&fit=crop&q=80',
        DATEADD(DAY, 14, CAST(GETDATE() AS DATETIME2)),
        DATEADD(DAY, 17, CAST(GETDATE() AS DATETIME2)),
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 5)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        5, 
        'Rangilo Gujarat: Folk Drama & Cultural Fest', 
        'An exuberant celebration of Gujarati cultural heritage, traditional Bhavai folk drama, classical Dayro musical storytelling, and vibrant folk performances.',
        'Theatre',
        1,
        1,
        'https://images.unsplash.com/photo-1469488865564-c2de10f69f96?w=1200&auto=format&fit=crop&q=80',
        DATEADD(DAY, 18, CAST(GETDATE() AS DATETIME2)),
        DATEADD(DAY, 18, DATEADD(HOUR, 4, CAST(GETDATE() AS DATETIME2))),
        'Upcoming',
        SYSUTCDATETIME()
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.Events WHERE EventId = 6)
BEGIN
    INSERT INTO dbo.Events (EventId, Title, Description, Category, CityId, VenueId, BannerUrl, StartDate, EndDate, Status, CreatedAtUtc)
    VALUES (
        6, 
        'Coldplay: Music of the Spheres Ahmedabad Tour', 
        'The global stadium sensation Coldplay performs live under the lights of Narendra Modi Stadium with mind-bending kinetic floors, laser displays, and sustainable LED wristbands.',
        'Concert',
        2,
        4,
        'https://images.unsplash.com/photo-1470225620780-dba8ba36b745?w=1200&auto=format&fit=crop&q=80',
        DATEADD(DAY, 30, CAST(GETDATE() AS DATETIME2)),
        DATEADD(DAY, 30, DATEADD(HOUR, 5, CAST(GETDATE() AS DATETIME2))),
        'Upcoming',
        SYSUTCDATETIME()
    );
END

SET IDENTITY_INSERT dbo.Events OFF;
GO

-- 3. Seed EventSessions
SET IDENTITY_INSERT dbo.EventSessions ON;

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 1)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (1, 1, 'Evening Symphony Show', DATEADD(HOUR, 18, DATEADD(DAY, 7, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 22, DATEADD(DAY, 7, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 2)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (2, 2, 'Prime Time Standup', DATEADD(HOUR, 20, DATEADD(DAY, 5, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 23, DATEADD(DAY, 5, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 3)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (3, 3, 'Full Day Technical Workshop', DATEADD(HOUR, 10, DATEADD(DAY, 10, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 17, DATEADD(DAY, 10, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 4)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (4, 4, 'Day 1 Match Play', DATEADD(HOUR, 9, DATEADD(DAY, 14, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 17, DATEADD(DAY, 14, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 5)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (5, 5, 'Traditional Night Show', DATEADD(HOUR, 19, DATEADD(DAY, 18, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 23, DATEADD(DAY, 18, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.EventSessions WHERE SessionId = 6)
BEGIN
    INSERT INTO dbo.EventSessions (SessionId, EventId, SessionTitle, StartTime, EndTime, IsActive)
    VALUES (6, 6, 'Stadium Mega Spectacle', DATEADD(HOUR, 18, DATEADD(DAY, 30, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(HOUR, 23, DATEADD(DAY, 30, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 1);
END

SET IDENTITY_INSERT dbo.EventSessions OFF;
GO

-- 4. Seed TicketCategories (Tiered with Atomic Capacity Guard)
SET IDENTITY_INSERT dbo.TicketCategories ON;

-- Event 1: Arijit Singh
IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 1)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (1, 1, 1, 'Silver: General Standing', 999.00, 500, 480, 6, 'Ground arena access, standard entry gate');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 2)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (2, 1, 1, 'Gold: Grandstand Reserved', 1999.00, 300, 290, 6, 'Reserved stadium bucket seat, snack box coupon');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 3)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (3, 1, 1, 'VIP Fan Pit: Stage Front', 4999.00, 100, 95, 4, 'Front row stage barricade, commemorative tour laminate, express gate');
END

-- Event 2: Zakir Khan
IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 4)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (4, 2, 2, 'Balcony Class', 499.00, 250, 230, 8, 'Upper balcony tier seating with clear audio-visual view');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 5)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (5, 2, 2, 'Prime Ground Auditorium', 999.00, 350, 340, 8, 'Central auditorium seating, cushioned comfort');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 6)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (6, 2, 2, 'Front Row VIP', 1999.00, 80, 75, 4, 'Rows A to C closest to artist, post-show group photo');
END

-- Event 3: Workshop
IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 7)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (7, 3, 3, 'Student Delegate Pass', 299.00, 200, 190, 4, 'Student ID verification required, lab handbook, participation certificate');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 8)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (8, 3, 3, 'Professional Delegate Pass', 799.00, 150, 145, 6, 'GitHub starter repos, cloud credits voucher, speaker networking buffet');
END

-- Event 4: Ranji Trophy
IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 9)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (9, 4, 4, 'General Stand: East Bay', 150.00, 1000, 980, 10, 'General stadium entry, open seating');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 10)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (10, 4, 4, 'Club House Pavilion', 750.00, 300, 285, 6, 'Air-conditioned pavilion lounge, complimentary high tea');
END

-- Event 5: Rangilo Gujarat
IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 11)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (11, 5, 5, 'Standard Entry', 250.00, 300, 290, 8, 'Standard auditorium seating');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 12)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (12, 5, 5, 'Royal Box', 600.00, 50, 48, 4, 'Elevated private box with traditional hospitality');
END

-- Event 6: Coldplay Ahmedabad
IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 13)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (13, 6, 6, 'Upper Tier Seating', 2500.00, 2000, 1950, 4, 'Stadium upper tier seat, panoramic visual show view');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 14)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (14, 6, 6, 'Pitch Standing Arena', 4500.00, 3000, 2800, 4, 'Field level standing next to main runway and B-stage');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TicketCategories WHERE CategoryId = 15)
BEGIN
    INSERT INTO dbo.TicketCategories (CategoryId, EventId, SessionId, CategoryName, Price, TotalCapacity, RemainingCapacity, MaxPerBooking, Perks)
    VALUES (15, 6, 6, 'VIP Infinity Hospitality', 12500.00, 200, 180, 2, 'Dedicated VIP entrance, pre-show dining lounge, souvenir gift bag');
END

SET IDENTITY_INSERT dbo.TicketCategories OFF;
GO

-- 5. Seed DataSources Provenance for Events
IF NOT EXISTS (SELECT 1 FROM dbo.DataSources WHERE EntityType = 'Event' AND EntityId = 1)
BEGIN
    INSERT INTO dbo.DataSources (EntityType, EntityId, SourceName, SourceUrl, VerifiedAtUtc, VerificationStatus, Notes)
    VALUES ('Event', 1, 'ShowSphere Cultural Operations / SCA Stadium', 'https://www.scastadium.com', SYSUTCDATETIME(), 'Verified', 'Official concert venue authorization and seating capacity certification.');
END

IF NOT EXISTS (SELECT 1 FROM dbo.DataSources WHERE EntityType = 'Event' AND EntityId = 2)
BEGIN
    INSERT INTO dbo.DataSources (EntityType, EntityId, SourceName, SourceUrl, VerifiedAtUtc, VerificationStatus, Notes)
    VALUES ('Event', 2, 'Rajkot Municipal Cultural Department / Hemu Gadhvi Natyagruh', 'https://www.rmc.gov.in', SYSUTCDATETIME(), 'Verified', 'Municipal auditorium scheduling and seating configuration records.');
END
GO
