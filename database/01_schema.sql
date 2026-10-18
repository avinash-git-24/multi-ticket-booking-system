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
