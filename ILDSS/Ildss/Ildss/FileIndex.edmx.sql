
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/12/2013 00:04:48
-- Generated from EDMX file: C:\Users\Max\Desktop\ILDSS\Ildss\Ildss\FileIndex.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ILDSS.FileIndex];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_DirectoryFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Files] DROP CONSTRAINT [FK_DirectoryFile];
GO
IF OBJECT_ID(N'[dbo].[FK_FileEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Events] DROP CONSTRAINT [FK_FileEvent];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Directories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Directories];
GO
IF OBJECT_ID(N'[dbo].[Files]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Files];
GO
IF OBJECT_ID(N'[dbo].[Events]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Events];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Directories'
CREATE TABLE [dbo].[Directories] (
    [DirectoryId] int IDENTITY(1,1) NOT NULL,
    [path] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Files'
CREATE TABLE [dbo].[Files] (
    [FileId] int IDENTITY(1,1) NOT NULL,
    [path] nvarchar(max)  NOT NULL,
    [hash] nvarchar(max)  NOT NULL,
    [DirectoryDirectoryId] int  NOT NULL
);
GO

-- Creating table 'Events'
CREATE TABLE [dbo].[Events] (
    [EventId] int IDENTITY(1,1) NOT NULL,
    [date_time] datetime  NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [FileFileId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [DirectoryId] in table 'Directories'
ALTER TABLE [dbo].[Directories]
ADD CONSTRAINT [PK_Directories]
    PRIMARY KEY CLUSTERED ([DirectoryId] ASC);
GO

-- Creating primary key on [FileId] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [PK_Files]
    PRIMARY KEY CLUSTERED ([FileId] ASC);
GO

-- Creating primary key on [EventId] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [PK_Events]
    PRIMARY KEY CLUSTERED ([EventId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DirectoryDirectoryId] in table 'Files'
ALTER TABLE [dbo].[Files]
ADD CONSTRAINT [FK_DirectoryFile]
    FOREIGN KEY ([DirectoryDirectoryId])
    REFERENCES [dbo].[Directories]
        ([DirectoryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DirectoryFile'
CREATE INDEX [IX_FK_DirectoryFile]
ON [dbo].[Files]
    ([DirectoryDirectoryId]);
GO

-- Creating foreign key on [FileFileId] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [FK_FileEvent]
    FOREIGN KEY ([FileFileId])
    REFERENCES [dbo].[Files]
        ([FileId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FileEvent'
CREATE INDEX [IX_FK_FileEvent]
ON [dbo].[Events]
    ([FileFileId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------