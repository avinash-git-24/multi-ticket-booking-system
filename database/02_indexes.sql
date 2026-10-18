-- ============================================================================
-- ShowSphere Database Indexes: Performance & Search Optimization
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE ShowSphereDb;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Movie Search and Catalog Indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movies_Status_ReleaseDate' AND object_id = OBJECT_ID('dbo.Movies'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Movies_Status_ReleaseDate 
    ON dbo.Movies (Status, ReleaseDate DESC)
    INCLUDE (Title, DurationMinutes, Language, Genre, Certificate, PosterUrl);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movies_Genre_Language' AND object_id = OBJECT_ID('dbo.Movies'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Movies_Genre_Language 
    ON dbo.Movies (Genre, Language)
    INCLUDE (Title, Status, PosterUrl);
END
GO

-- 2. Showtime Lookup Indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Showtimes_Movie_StartTime' AND object_id = OBJECT_ID('dbo.Showtimes'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Showtimes_Movie_StartTime 
    ON dbo.Showtimes (MovieId, StartTime)
    INCLUDE (ScreenId, SilverPrice, GoldPrice, PlatinumPrice, IsActive);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Showtimes_Screen_StartTime' AND object_id = OBJECT_ID('dbo.Showtimes'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Showtimes_Screen_StartTime 
    ON dbo.Showtimes (ScreenId, StartTime);
END
GO

-- 3. Showtime Seat Map & Hold Verification Indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ShowtimeSeats_Showtime_Status' AND object_id = OBJECT_ID('dbo.ShowtimeSeats'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ShowtimeSeats_Showtime_Status 
    ON dbo.ShowtimeSeats (ShowtimeId, Status)
    INCLUDE (SeatId, HoldExpiresAtUtc, BookingId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ShowtimeSeats_Held_Expiry' AND object_id = OBJECT_ID('dbo.ShowtimeSeats'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ShowtimeSeats_Held_Expiry 
    ON dbo.ShowtimeSeats (Status, HoldExpiresAtUtc)
    WHERE Status = 'Held';
END
GO

-- 4. Event Search & Category Indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Events_Category_City_Status' AND object_id = OBJECT_ID('dbo.Events'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Events_Category_City_Status 
    ON dbo.Events (Category, CityId, Status)
    INCLUDE (Title, StartDate, EndDate, BannerUrl);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Events_StartDate' AND object_id = OBJECT_ID('dbo.Events'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Events_StartDate 
    ON dbo.Events (StartDate)
    INCLUDE (Title, Category, Status);
END
GO

-- 5. Booking Retrieval & Expiry Cleanup Indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bookings_User_Created' AND object_id = OBJECT_ID('dbo.Bookings'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Bookings_User_Created 
    ON dbo.Bookings (UserId, CreatedAtUtc DESC)
    INCLUDE (BookingReference, BookingType, Status, TotalAmount);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bookings_Pending_Expires' AND object_id = OBJECT_ID('dbo.Bookings'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Bookings_Pending_Expires 
    ON dbo.Bookings (Status, ExpiresAtUtc)
    WHERE Status = 'Pending';
END
GO

-- 6. Audit & Venue Lookup
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Theatres_City' AND object_id = OBJECT_ID('dbo.Theatres'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Theatres_City 
    ON dbo.Theatres (CityId)
    INCLUDE (TheatreName, Address);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DataSources_Entity' AND object_id = OBJECT_ID('dbo.DataSources'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_DataSources_Entity 
    ON dbo.DataSources (EntityType, EntityId);
END
GO
