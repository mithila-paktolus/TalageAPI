-- Create Database
CREATE DATABASE TalengeIntegration;
GO

USE [TalengeIntegration];
GO

-- =========================
-- NLog Table
-- =========================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NLog')
BEGIN
    CREATE TABLE [dbo].[NLog](
        [NLogId] INT IDENTITY(1,1) NOT NULL,
        [ApplicationName] NVARCHAR(255) NOT NULL,
        [LogMessage] NVARCHAR(MAX) NOT NULL,
        [LogDate] DATETIME NOT NULL,
        CONSTRAINT [PK_NLog] PRIMARY KEY CLUSTERED ([NLogId])
    );
END
GO

-- =========================
-- AccessTokens Table
-- =========================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AccessTokens')
BEGIN
    CREATE TABLE [dbo].[AccessTokens](
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Token] NVARCHAR(MAX) NOT NULL,
        [TokenType] NVARCHAR(50) NOT NULL,
        [RefreshToken] NVARCHAR(MAX) NULL,
        [ExpiresDate] DATETIME NOT NULL,
        [CreatedDate] DATETIME NOT NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL,
        [LastUpdated] DATETIME NULL,
        [UpdatedBy] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL,
        CONSTRAINT [PK_AccessTokens] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- =========================
-- ApplicationCreationLogs Table
-- =========================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApplicationCreationLogs' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[ApplicationCreationLogs](
        [Id] BIGINT IDENTITY(1,1) NOT NULL,
        [ApplicationId] NVARCHAR(50) NOT NULL,
        [ResponseJson] NVARCHAR(MAX) NOT NULL,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_ApplicationCreationLogs_IsDeleted] DEFAULT (0),
        [CreatedOn] DATETIME NOT NULL CONSTRAINT [DF_ApplicationCreationLogs_CreatedOn] DEFAULT (GETDATE()),
        [CreatedBy] NVARCHAR(100) NULL,
        [LastUpdated] DATETIME NULL,
        [UpdatedBy] NVARCHAR(100) NULL,
        CONSTRAINT [PK_ApplicationCreationLogs] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE NONCLUSTERED INDEX [IX_ApplicationCreationLogs_ApplicationId]
        ON [dbo].[ApplicationCreationLogs] ([ApplicationId] ASC);
END
GO