IF OBJECT_ID(N'[__InvestmentMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__InvestmentMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___InvestmentMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    IF SCHEMA_ID(N'Funds') IS NULL EXEC(N'CREATE SCHEMA [Funds];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[Allocations] (
        [Name] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Allocations] PRIMARY KEY ([Name])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[Funds] (
        [FundCode] nvarchar(450) NOT NULL,
        [MsUrl] nvarchar(max) NULL,
        [MsCode] nvarchar(max) NULL,
        CONSTRAINT [PK_Funds] PRIMARY KEY ([FundCode])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[Option] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [OptionNumber] int NOT NULL,
        CONSTRAINT [PK_Option] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[VersionDrafts] (
        [Id] int NOT NULL IDENTITY,
        [Version] int NOT NULL,
        [Date] datetimeoffset NOT NULL,
        [Draft] xml NULL,
        [Notes] nvarchar(max) NULL,
        CONSTRAINT [PK_VersionDrafts] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[AllocationVersions] (
        [Name] nvarchar(450) NOT NULL,
        [Version] int NOT NULL,
        [ScoreRange_Min] int NULL,
        [ScoreRange_Max] int NULL,
        [AllocationName] nvarchar(450) NULL,
        CONSTRAINT [PK_AllocationVersions] PRIMARY KEY ([Name], [Version]),
        CONSTRAINT [FK_AllocationVersions_Allocations_AllocationName] FOREIGN KEY ([AllocationName]) REFERENCES [Funds].[Allocations] ([Name]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[AllocationOptions] (
        [Id] int NOT NULL IDENTITY,
        [OptionId] int NULL,
        [AllocationVersionName] nvarchar(450) NULL,
        [AllocationVersionVersion] int NULL,
        CONSTRAINT [PK_AllocationOptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AllocationOptions_Option_OptionId] FOREIGN KEY ([OptionId]) REFERENCES [Funds].[Option] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AllocationOptions_AllocationVersions_AllocationVersionName_AllocationVersionVersion] FOREIGN KEY ([AllocationVersionName], [AllocationVersionVersion]) REFERENCES [Funds].[AllocationVersions] ([Name], [Version]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE TABLE [Funds].[CompositionPart] (
        [OptionId] int NOT NULL,
        [Id] int NOT NULL IDENTITY,
        [Percent] int NOT NULL,
        [FundCode] nvarchar(450) NULL,
        CONSTRAINT [PK_CompositionPart] PRIMARY KEY ([OptionId], [Id]),
        CONSTRAINT [FK_CompositionPart_Funds_FundCode] FOREIGN KEY ([FundCode]) REFERENCES [Funds].[Funds] ([FundCode]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CompositionPart_AllocationOptions_OptionId] FOREIGN KEY ([OptionId]) REFERENCES [Funds].[AllocationOptions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name') AND [object_id] = OBJECT_ID(N'[Funds].[Allocations]'))
        SET IDENTITY_INSERT [Funds].[Allocations] ON;
    INSERT INTO [Funds].[Allocations] ([Name])
    VALUES (N'100i'),
    (N'20i80e'),
    (N'30i70e'),
    (N'40i60e'),
    (N'50i50e'),
    (N'60i40e'),
    (N'70i30e'),
    (N'80i20e'),
    (N'100e');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name') AND [object_id] = OBJECT_ID(N'[Funds].[Allocations]'))
        SET IDENTITY_INSERT [Funds].[Allocations] OFF;
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE INDEX [IX_AllocationOptions_OptionId] ON [Funds].[AllocationOptions] ([OptionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE INDEX [IX_AllocationOptions_AllocationVersionName_AllocationVersionVersion] ON [Funds].[AllocationOptions] ([AllocationVersionName], [AllocationVersionVersion]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE INDEX [IX_AllocationVersions_AllocationName] ON [Funds].[AllocationVersions] ([AllocationName]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE INDEX [IX_CompositionPart_FundCode] ON [Funds].[CompositionPart] ([FundCode]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    CREATE INDEX [IX_VersionDrafts_Version] ON [Funds].[VersionDrafts] ([Version]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__InvestmentMigrationsHistory] WHERE [MigrationId] = N'20190517131848_AllocationsMigration')
BEGIN
    INSERT INTO [__InvestmentMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190517131848_AllocationsMigration', N'3.0.0-preview5.19227.1');
END;

GO

