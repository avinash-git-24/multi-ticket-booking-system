-- ============================================================================
-- ShowSphere Database Reset Script
-- Drops and recreates the ShowSphereDb database for a clean state
-- SQL Server 2022 / LocalDB Compatible
-- ============================================================================

USE master;
GO

IF DB_ID('ShowSphereDb') IS NOT NULL
BEGIN
    ALTER DATABASE ShowSphereDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ShowSphereDb;
END
GO

CREATE DATABASE ShowSphereDb;
GO

USE ShowSphereDb;
GO

PRINT 'ShowSphereDb has been cleanly dropped and recreated.';
GO
