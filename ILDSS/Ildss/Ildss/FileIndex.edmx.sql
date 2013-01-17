
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/17/2013 02:37:43
-- Generated from EDMX file: F:\Documents\GitHub\docstore\ILDSS\Ildss\Ildss\FileIndex.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FileIndex];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_DocumentDocPath]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocPaths] DROP CONSTRAINT [FK_DocumentDocPath];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentDocEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocEvents] DROP CONSTRAINT [FK_DocumentDocEvent];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Documents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Documents];
GO
IF OBJECT_ID(N'[dbo].[DocEvents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocEvents];
GO
IF OBJECT_ID(N'[dbo].[DocPaths]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocPaths];
GO
IF OBJECT_ID(N'[dbo].[EventQueueBackups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventQueueBackups];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Documents'
CREATE TABLE [dbo].[Documents] (
    [DocumentHash] nchar(128)  NOT NULL,
    [size] bigint  NOT NULL
);
GO

-- Creating table 'DocEvents'
CREATE TABLE [dbo].[DocEvents] (
    [DocEventId] int IDENTITY(1,1) NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [DocumentDocumentHash] nchar(128)  NOT NULL,
    [path] nvarchar(max)  NOT NULL,
    [old_path] nvarchar(max)  NULL,
    [name] nvarchar(max)  NOT NULL,
    [old_name] nvarchar(max)  NULL,
    [last_access] datetime  NOT NULL,
    [last_write] datetime  NOT NULL
);
GO

-- Creating table 'DocPaths'
CREATE TABLE [dbo].[DocPaths] (
    [DocPathId] int IDENTITY(1,1) NOT NULL,
    [path] nvarchar(max)  NOT NULL,
    [DocumentDocumentHash] nchar(128)  NOT NULL
);
GO

-- Creating table 'EventQueueBackups'
CREATE TABLE [dbo].[EventQueueBackups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [path] nvarchar(max)  NOT NULL,
    [old_path] nvarchar(max)  NULL,
    [name] nvarchar(max)  NOT NULL,
    [old_name] nvarchar(max)  NULL,
    [DocumentDocumentHash] nchar(128)  NOT NULL,
    [date_time] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [DocumentHash] in table 'Documents'
ALTER TABLE [dbo].[Documents]
ADD CONSTRAINT [PK_Documents]
    PRIMARY KEY CLUSTERED ([DocumentHash] ASC);
GO

-- Creating primary key on [DocEventId] in table 'DocEvents'
ALTER TABLE [dbo].[DocEvents]
ADD CONSTRAINT [PK_DocEvents]
    PRIMARY KEY CLUSTERED ([DocEventId] ASC);
GO

-- Creating primary key on [DocPathId] in table 'DocPaths'
ALTER TABLE [dbo].[DocPaths]
ADD CONSTRAINT [PK_DocPaths]
    PRIMARY KEY CLUSTERED ([DocPathId] ASC);
GO

-- Creating primary key on [Id] in table 'EventQueueBackups'
ALTER TABLE [dbo].[EventQueueBackups]
ADD CONSTRAINT [PK_EventQueueBackups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DocumentDocumentHash] in table 'DocPaths'
ALTER TABLE [dbo].[DocPaths]
ADD CONSTRAINT [FK_DocumentDocPath]
    FOREIGN KEY ([DocumentDocumentHash])
    REFERENCES [dbo].[Documents]
        ([DocumentHash])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentDocPath'
CREATE INDEX [IX_FK_DocumentDocPath]
ON [dbo].[DocPaths]
    ([DocumentDocumentHash]);
GO

-- Creating foreign key on [DocumentDocumentHash] in table 'DocEvents'
ALTER TABLE [dbo].[DocEvents]
ADD CONSTRAINT [FK_DocumentDocEvent]
    FOREIGN KEY ([DocumentDocumentHash])
    REFERENCES [dbo].[Documents]
        ([DocumentHash])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentDocEvent'
CREATE INDEX [IX_FK_DocumentDocEvent]
ON [dbo].[DocEvents]
    ([DocumentDocumentHash]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------