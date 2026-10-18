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
