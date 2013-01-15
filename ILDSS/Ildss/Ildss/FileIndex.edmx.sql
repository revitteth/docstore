
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/15/2013 22:56:04
-- Generated from EDMX file: C:\Users\Max\Documents\GitHub\docstore\ILDSS\Ildss\Ildss\FileIndex.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FileIndexDb];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


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
    [date_time] datetime  NOT NULL,
    [type] nvarchar(max)  NOT NULL,
    [DocumentDocumentHash] nchar(128)  NOT NULL,
    [path] nvarchar(max)  NOT NULL,
    [old_path] nvarchar(max)  NULL,
    [name] nvarchar(max)  NOT NULL,
    [old_name] nvarchar(max)  NULL
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