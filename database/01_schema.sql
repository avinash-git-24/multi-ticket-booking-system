-- ============================================================================
-- ShowSphere Database Schema: Part 1 - Users, Roles, Cities, and Audit
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

IF DB_ID('ShowSphereDb') IS NULL
BEGIN
    CREATE DATABASE ShowSphereDb;
END
GO

USE ShowSphereDb;
GO

-- 1. Cities Table
IF OBJECT_ID('dbo.Cities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cities (
        CityId INT IDENTITY(1,1) NOT NULL,
        CityName NVARCHAR(100) NOT NULL,
        StateName NVARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Cities_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Cities_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Cities PRIMARY KEY CLUSTERED (CityId),
        CONSTRAINT UQ_Cities_CityName UNIQUE (CityName)
    );
END
GO

-- 2. Roles Table
IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles (
        RoleId INT IDENTITY(1,1) NOT NULL,
        RoleName NVARCHAR(50) NOT NULL,
        Description NVARCHAR(250) NULL,
        CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId),
        CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
    );
END
GO

-- 3. Users Table
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId INT IDENTITY(1,1) NOT NULL,
        FullName NVARCHAR(150) NOT NULL,
        Email NVARCHAR(256) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        PhoneNumber NVARCHAR(20) NULL,
        PreferredCityId INT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Users_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        UpdatedAtUtc DATETIME2(7) NULL,
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT FK_Users_PreferredCity FOREIGN KEY (PreferredCityId) REFERENCES dbo.Cities(CityId)
    );
END
GO

-- 4. UserRoles Table (Many-to-Many)
IF OBJECT_ID('dbo.UserRoles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles (
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        AssignedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_UserRoles_AssignedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UserRoles PRIMARY KEY CLUSTERED (UserId, RoleId),
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE,
        CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId) ON DELETE CASCADE
    );
END
GO

-- 5. AuditLog Table
IF OBJECT_ID('dbo.AuditLog', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLog (
        AuditLogId BIGINT IDENTITY(1,1) NOT NULL,
        UserId INT NULL,
        ActionType NVARCHAR(100) NOT NULL,
        EntityName NVARCHAR(100) NOT NULL,
        EntityId NVARCHAR(100) NULL,
        Details NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_AuditLog_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_AuditLog PRIMARY KEY CLUSTERED (AuditLogId),
        CONSTRAINT FK_AuditLog_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE SET NULL
    );
END
GO

-- 6. Theatres Table
IF OBJECT_ID('dbo.Theatres', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Theatres (
        TheatreId INT IDENTITY(1,1) NOT NULL,
        TheatreName NVARCHAR(200) NOT NULL,
        CityId INT NOT NULL,
        Address NVARCHAR(500) NOT NULL,
        Latitude DECIMAL(9,6) NULL,
        Longitude DECIMAL(9,6) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Theatres_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Theatres_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Theatres PRIMARY KEY CLUSTERED (TheatreId),
        CONSTRAINT FK_Theatres_Cities FOREIGN KEY (CityId) REFERENCES dbo.Cities(CityId)
    );
END
GO

-- 7. Screens Table
IF OBJECT_ID('dbo.Screens', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Screens (
        ScreenId INT IDENTITY(1,1) NOT NULL,
        TheatreId INT NOT NULL,
        ScreenName NVARCHAR(100) NOT NULL,
        ScreenType NVARCHAR(50) NOT NULL CONSTRAINT DF_Screens_ScreenType DEFAULT ('2D'),
        TotalSeats INT NOT NULL CONSTRAINT CK_Screens_TotalSeats CHECK (TotalSeats > 0),
        IsActive BIT NOT NULL CONSTRAINT DF_Screens_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Screens_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Screens PRIMARY KEY CLUSTERED (ScreenId),
        CONSTRAINT FK_Screens_Theatres FOREIGN KEY (TheatreId) REFERENCES dbo.Theatres(TheatreId) ON DELETE CASCADE
    );
END
GO

-- 8. Seats Table
IF OBJECT_ID('dbo.Seats', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Seats (
        SeatId INT IDENTITY(1,1) NOT NULL,
        ScreenId INT NOT NULL,
        RowLabel VARCHAR(5) NOT NULL,
        SeatNumber INT NOT NULL CONSTRAINT CK_Seats_SeatNumber CHECK (SeatNumber > 0),
        SeatTier NVARCHAR(50) NOT NULL CONSTRAINT DF_Seats_SeatTier DEFAULT ('Gold'),
        IsActive BIT NOT NULL CONSTRAINT DF_Seats_IsActive DEFAULT (1),
        CONSTRAINT PK_Seats PRIMARY KEY CLUSTERED (SeatId),
        CONSTRAINT UQ_Seats_Screen_Row_Number UNIQUE (ScreenId, RowLabel, SeatNumber),
        CONSTRAINT CK_Seats_SeatTier CHECK (SeatTier IN ('Silver', 'Gold', 'Platinum')),
        CONSTRAINT FK_Seats_Screens FOREIGN KEY (ScreenId) REFERENCES dbo.Screens(ScreenId) ON DELETE CASCADE
    );
END
GO

-- 9. Movies Table
IF OBJECT_ID('dbo.Movies', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Movies (
        MovieId INT IDENTITY(1,1) NOT NULL,
        Title NVARCHAR(250) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        DurationMinutes INT NOT NULL CONSTRAINT CK_Movies_Duration CHECK (DurationMinutes > 0),
        Language NVARCHAR(50) NOT NULL,
        Genre NVARCHAR(100) NOT NULL,
        ReleaseDate DATE NOT NULL,
        Certificate NVARCHAR(10) NOT NULL CONSTRAINT DF_Movies_Certificate DEFAULT ('UA'),
        PosterUrl NVARCHAR(1000) NULL,
        BannerUrl NVARCHAR(1000) NULL,
        TrailerUrl NVARCHAR(1000) NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Movies_Status DEFAULT ('CurrentlyShowing'),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Movies_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        UpdatedAtUtc DATETIME2(7) NULL,
        RowVersion ROWVERSION NOT NULL,
        CONSTRAINT PK_Movies PRIMARY KEY CLUSTERED (MovieId),
        CONSTRAINT CK_Movies_Certificate CHECK (Certificate IN ('U', 'UA', 'A', 'S')),
        CONSTRAINT CK_Movies_Status CHECK (Status IN ('CurrentlyShowing', 'Upcoming', 'Archived'))
    );
END
GO

-- 10. Showtimes Table
IF OBJECT_ID('dbo.Showtimes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Showtimes (
        ShowtimeId INT IDENTITY(1,1) NOT NULL,
        MovieId INT NOT NULL,
        ScreenId INT NOT NULL,
        StartTime DATETIME2(7) NOT NULL,
        EndTime DATETIME2(7) NOT NULL,
        SilverPrice DECIMAL(10,2) NOT NULL CONSTRAINT DF_Showtimes_SilverPrice DEFAULT (150.00),
        GoldPrice DECIMAL(10,2) NOT NULL CONSTRAINT DF_Showtimes_GoldPrice DEFAULT (220.00),
        PlatinumPrice DECIMAL(10,2) NOT NULL CONSTRAINT DF_Showtimes_PlatinumPrice DEFAULT (300.00),
        IsActive BIT NOT NULL CONSTRAINT DF_Showtimes_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Showtimes_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Showtimes PRIMARY KEY CLUSTERED (ShowtimeId),
        CONSTRAINT FK_Showtimes_Movies FOREIGN KEY (MovieId) REFERENCES dbo.Movies(MovieId) ON DELETE CASCADE,
        CONSTRAINT FK_Showtimes_Screens FOREIGN KEY (ScreenId) REFERENCES dbo.Screens(ScreenId) ON DELETE CASCADE,
        CONSTRAINT CK_Showtimes_Times CHECK (EndTime > StartTime),
        CONSTRAINT CK_Showtimes_SilverPrice CHECK (SilverPrice >= 0),
        CONSTRAINT CK_Showtimes_GoldPrice CHECK (GoldPrice >= 0),
        CONSTRAINT CK_Showtimes_PlatinumPrice CHECK (PlatinumPrice >= 0)
    );
END
GO

-- 11. ShowtimeSeats Table (Seat Inventory per Showtime)
IF OBJECT_ID('dbo.ShowtimeSeats', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ShowtimeSeats (
        ShowtimeSeatId BIGINT IDENTITY(1,1) NOT NULL,
        ShowtimeId INT NOT NULL,
        SeatId INT NOT NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_ShowtimeSeats_Status DEFAULT ('Available'),
        HoldExpiresAtUtc DATETIME2(7) NULL,
        BookingId INT NULL,
        RowVersion ROWVERSION NOT NULL,
        UpdatedAtUtc DATETIME2(7) NULL,
        CONSTRAINT PK_ShowtimeSeats PRIMARY KEY CLUSTERED (ShowtimeSeatId),
        CONSTRAINT UQ_ShowtimeSeats_Showtime_Seat UNIQUE (ShowtimeId, SeatId),
        CONSTRAINT CK_ShowtimeSeats_Status CHECK (Status IN ('Available', 'Held', 'Booked')),
        CONSTRAINT FK_ShowtimeSeats_Showtimes FOREIGN KEY (ShowtimeId) REFERENCES dbo.Showtimes(ShowtimeId) ON DELETE CASCADE,
        CONSTRAINT FK_ShowtimeSeats_Seats FOREIGN KEY (SeatId) REFERENCES dbo.Seats(SeatId)
    );
END
GO

-- 12. Venues Table
IF OBJECT_ID('dbo.Venues', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Venues (
        VenueId INT IDENTITY(1,1) NOT NULL,
        VenueName NVARCHAR(250) NOT NULL,
        CityId INT NOT NULL,
        Address NVARCHAR(500) NOT NULL,
        Capacity INT NOT NULL CONSTRAINT CK_Venues_Capacity CHECK (Capacity > 0),
        IsActive BIT NOT NULL CONSTRAINT DF_Venues_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Venues_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Venues PRIMARY KEY CLUSTERED (VenueId),
        CONSTRAINT FK_Venues_Cities FOREIGN KEY (CityId) REFERENCES dbo.Cities(CityId)
    );
END
GO

-- 13. Events Table
IF OBJECT_ID('dbo.Events', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Events (
        EventId INT IDENTITY(1,1) NOT NULL,
        Title NVARCHAR(250) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        CityId INT NOT NULL,
        VenueId INT NULL,
        BannerUrl NVARCHAR(1000) NULL,
        StartDate DATETIME2(7) NOT NULL,
        EndDate DATETIME2(7) NOT NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Events_Status DEFAULT ('Upcoming'),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Events_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        UpdatedAtUtc DATETIME2(7) NULL,
        RowVersion ROWVERSION NOT NULL,
        CONSTRAINT PK_Events PRIMARY KEY CLUSTERED (EventId),
        CONSTRAINT FK_Events_Cities FOREIGN KEY (CityId) REFERENCES dbo.Cities(CityId),
        CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId) REFERENCES dbo.Venues(VenueId),
        CONSTRAINT CK_Events_Dates CHECK (EndDate >= StartDate),
        CONSTRAINT CK_Events_Status CHECK (Status IN ('Upcoming', 'Past', 'Cancelled')),
        CONSTRAINT CK_Events_Category CHECK (Category IN ('Concert', 'Comedy', 'Workshop', 'Sports', 'Theatre'))
    );
END
GO

-- 14. EventSessions Table
IF OBJECT_ID('dbo.EventSessions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EventSessions (
        SessionId INT IDENTITY(1,1) NOT NULL,
        EventId INT NOT NULL,
        SessionTitle NVARCHAR(150) NOT NULL,
        StartTime DATETIME2(7) NOT NULL,
        EndTime DATETIME2(7) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_EventSessions_IsActive DEFAULT (1),
        CONSTRAINT PK_EventSessions PRIMARY KEY CLUSTERED (SessionId),
        CONSTRAINT FK_EventSessions_Events FOREIGN KEY (EventId) REFERENCES dbo.Events(EventId) ON DELETE CASCADE,
        CONSTRAINT CK_EventSessions_Times CHECK (EndTime > StartTime)
    );
END
GO

-- 15. TicketCategories Table (Tiered Inventory with Capacity Guard)
IF OBJECT_ID('dbo.TicketCategories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TicketCategories (
        CategoryId INT IDENTITY(1,1) NOT NULL,
        EventId INT NOT NULL,
        SessionId INT NULL,
        CategoryName NVARCHAR(100) NOT NULL,
        Price DECIMAL(10,2) NOT NULL,
        TotalCapacity INT NOT NULL,
        RemainingCapacity INT NOT NULL,
        MaxPerBooking INT NOT NULL CONSTRAINT DF_TicketCategories_MaxPerBooking DEFAULT (10),
        Perks NVARCHAR(500) NULL,
        RowVersion ROWVERSION NOT NULL,
        CONSTRAINT PK_TicketCategories PRIMARY KEY CLUSTERED (CategoryId),
        CONSTRAINT FK_TicketCategories_Events FOREIGN KEY (EventId) REFERENCES dbo.Events(EventId) ON DELETE CASCADE,
        CONSTRAINT FK_TicketCategories_EventSessions FOREIGN KEY (SessionId) REFERENCES dbo.EventSessions(SessionId),
        CONSTRAINT CK_TicketCategories_Price CHECK (Price >= 0),
        CONSTRAINT CK_TicketCategories_TotalCapacity CHECK (TotalCapacity > 0),
        CONSTRAINT CK_TicketCategories_RemainingCapacity CHECK (RemainingCapacity >= 0 AND RemainingCapacity <= TotalCapacity),
        CONSTRAINT CK_TicketCategories_MaxPerBooking CHECK (MaxPerBooking > 0)
    );
END
GO

-- 16. Bookings Table
IF OBJECT_ID('dbo.Bookings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings (
        BookingId INT IDENTITY(1,1) NOT NULL,
        BookingReference NVARCHAR(50) NOT NULL,
        UserId INT NOT NULL,
        BookingType NVARCHAR(20) NOT NULL,
        ShowtimeId INT NULL,
        EventId INT NULL,
        TicketCategoryId INT NULL,
        TotalQuantity INT NOT NULL,
        BaseAmount DECIMAL(10,2) NOT NULL,
        ConvenienceFee DECIMAL(10,2) NOT NULL,
        TaxAmount DECIMAL(10,2) NOT NULL,
        TotalAmount DECIMAL(10,2) NOT NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Bookings_Status DEFAULT ('Pending'),
        CreatedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Bookings_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        ExpiresAtUtc DATETIME2(7) NOT NULL,
        ConfirmedAtUtc DATETIME2(7) NULL,
        CancelledAtUtc DATETIME2(7) NULL,
        CancellationReason NVARCHAR(500) NULL,
        RowVersion ROWVERSION NOT NULL,
        CONSTRAINT PK_Bookings PRIMARY KEY CLUSTERED (BookingId),
        CONSTRAINT UQ_Bookings_BookingReference UNIQUE (BookingReference),
        CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Bookings_Showtimes FOREIGN KEY (ShowtimeId) REFERENCES dbo.Showtimes(ShowtimeId),
        CONSTRAINT FK_Bookings_Events FOREIGN KEY (EventId) REFERENCES dbo.Events(EventId),
        CONSTRAINT FK_Bookings_TicketCategories FOREIGN KEY (TicketCategoryId) REFERENCES dbo.TicketCategories(CategoryId),
        CONSTRAINT CK_Bookings_BookingType CHECK (BookingType IN ('Movie', 'Event')),
        CONSTRAINT CK_Bookings_Status CHECK (Status IN ('Pending', 'Confirmed', 'Failed', 'Cancelled', 'Expired')),
        CONSTRAINT CK_Bookings_TotalQuantity CHECK (TotalQuantity > 0),
        CONSTRAINT CK_Bookings_BaseAmount CHECK (BaseAmount >= 0),
        CONSTRAINT CK_Bookings_ConvenienceFee CHECK (ConvenienceFee >= 0),
        CONSTRAINT CK_Bookings_TaxAmount CHECK (TaxAmount >= 0),
        CONSTRAINT CK_Bookings_TotalAmount CHECK (TotalAmount >= 0)
    );
END
GO

-- Add Foreign Key from ShowtimeSeats to Bookings now that Bookings exists
IF OBJECT_ID('dbo.FK_ShowtimeSeats_Bookings', 'F') IS NULL
BEGIN
    ALTER TABLE dbo.ShowtimeSeats
    ADD CONSTRAINT FK_ShowtimeSeats_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(BookingId);
END
GO

-- 17. BookingItems Table (Detail Line Items)
IF OBJECT_ID('dbo.BookingItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingItems (
        BookingItemId BIGINT IDENTITY(1,1) NOT NULL,
        BookingId INT NOT NULL,
        SeatId INT NULL,
        SeatLabel NVARCHAR(20) NOT NULL,
        TicketCategoryId INT NULL,
        CategoryName NVARCHAR(100) NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        CONSTRAINT PK_BookingItems PRIMARY KEY CLUSTERED (BookingItemId),
        CONSTRAINT FK_BookingItems_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(BookingId) ON DELETE CASCADE,
        CONSTRAINT FK_BookingItems_Seats FOREIGN KEY (SeatId) REFERENCES dbo.Seats(SeatId),
        CONSTRAINT FK_BookingItems_TicketCategories FOREIGN KEY (TicketCategoryId) REFERENCES dbo.TicketCategories(CategoryId),
        CONSTRAINT CK_BookingItems_UnitPrice CHECK (UnitPrice >= 0)
    );
END
GO

-- 18. Payments Table (Simulated Payment Transaction Ledger)
IF OBJECT_ID('dbo.Payments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments (
        PaymentId INT IDENTITY(1,1) NOT NULL,
        BookingId INT NOT NULL,
        TransactionReference NVARCHAR(100) NOT NULL,
        PaymentGateway NVARCHAR(50) NOT NULL CONSTRAINT DF_Payments_Gateway DEFAULT ('SimulatedGateway'),
        PaymentMode NVARCHAR(50) NOT NULL,
        Amount DECIMAL(10,2) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        FailureReason NVARCHAR(500) NULL,
        ProcessedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_Payments_ProcessedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_Payments PRIMARY KEY CLUSTERED (PaymentId),
        CONSTRAINT UQ_Payments_TransactionReference UNIQUE (TransactionReference),
        CONSTRAINT FK_Payments_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(BookingId),
        CONSTRAINT CK_Payments_Amount CHECK (Amount >= 0),
        CONSTRAINT CK_Payments_Status CHECK (Status IN ('Success', 'Failed', 'Timeout'))
    );
END
GO

-- 19. DataSources Table (Catalogue Provenance & Verification Metadata)
IF OBJECT_ID('dbo.DataSources', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DataSources (
        DataSourceId INT IDENTITY(1,1) NOT NULL,
        EntityType NVARCHAR(50) NOT NULL,
        EntityId INT NOT NULL,
        SourceName NVARCHAR(200) NOT NULL,
        SourceUrl NVARCHAR(1000) NOT NULL,
        VerifiedAtUtc DATETIME2(7) NOT NULL CONSTRAINT DF_DataSources_VerifiedAtUtc DEFAULT (SYSUTCDATETIME()),
        VerificationStatus NVARCHAR(50) NOT NULL CONSTRAINT DF_DataSources_VerificationStatus DEFAULT ('Demo'),
        Notes NVARCHAR(MAX) NULL,
        CONSTRAINT PK_DataSources PRIMARY KEY CLUSTERED (DataSourceId),
        CONSTRAINT CK_DataSources_EntityType CHECK (EntityType IN ('Movie', 'Event', 'Venue')),
        CONSTRAINT CK_DataSources_VerificationStatus CHECK (VerificationStatus IN ('Verified', 'Partially Verified', 'Demo'))
    );
END
GO
