IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012070555_CreateEFCoreDB1'
)
BEGIN
    CREATE TABLE [Branches] (
        [BranchId] int NOT NULL IDENTITY,
        [BranchName] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        CONSTRAINT [PK_Branches] PRIMARY KEY ([BranchId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012070555_CreateEFCoreDB1'
)
BEGIN
    CREATE TABLE [Students] (
        [StudentId] int NOT NULL IDENTITY,
        [FirstName] nvarchar(max) NULL,
        [LastName] nvarchar(max) NULL,
        [DateOfBirth] datetime2 NULL,
        [Gender] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [EnrollmentDate] datetime2 NOT NULL,
        [BranchId] int NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId]),
        CONSTRAINT [FK_Students_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([BranchId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012070555_CreateEFCoreDB1'
)
BEGIN
    CREATE INDEX [IX_Students_BranchId] ON [Students] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012070555_CreateEFCoreDB1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251012070555_CreateEFCoreDB1', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125041326_changeInporpertyname'
)
BEGIN
    EXEC sp_rename N'[Students].[StudentId]', N'Students', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125041326_changeInporpertyname'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125041326_changeInporpertyname', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    ALTER TABLE [Students] DROP CONSTRAINT [FK_Students_Branches_BranchId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    ALTER TABLE [Students] DROP CONSTRAINT [PK_Students];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    EXEC sp_rename N'[Students]', N'tb_Students', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    EXEC sp_rename N'[tb_Students].[IX_Students_BranchId]', N'IX_tb_Students_BranchId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    ALTER TABLE [tb_Students] ADD CONSTRAINT [PK_tb_Students] PRIMARY KEY ([Students]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    ALTER TABLE [tb_Students] ADD CONSTRAINT [FK_tb_Students_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([BranchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043526_changedtabeName'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125043526_changedtabeName', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043706_changedcolumnname'
)
BEGIN
    EXEC sp_rename N'[tb_Students].[Email]', N'EmailAddress', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125043706_changedcolumnname'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125043706_changedcolumnname', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044258_changedbranchId'
)
BEGIN
    ALTER TABLE [tb_Students] DROP CONSTRAINT [FK_tb_Students_Branches_BranchId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044258_changedbranchId'
)
BEGIN
    EXEC sp_rename N'[tb_Students].[BranchId]', N'Branch_Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044258_changedbranchId'
)
BEGIN
    EXEC sp_rename N'[tb_Students].[IX_tb_Students_BranchId]', N'IX_tb_Students_Branch_Id', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044258_changedbranchId'
)
BEGIN
    EXEC sp_rename N'[Branches].[BranchId]', N'Branch_Id', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044258_changedbranchId'
)
BEGIN
    ALTER TABLE [tb_Students] ADD CONSTRAINT [FK_tb_Students_Branches_Branch_Id] FOREIGN KEY ([Branch_Id]) REFERENCES [Branches] ([Branch_Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044258_changedbranchId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125044258_changedbranchId', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044345_changerequiredproperty'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Branches]') AND [c].[name] = N'BranchName');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Branches] DROP CONSTRAINT [' + @var + '];');
    EXEC(N'UPDATE [Branches] SET [BranchName] = N'''' WHERE [BranchName] IS NULL');
    ALTER TABLE [Branches] ALTER COLUMN [BranchName] nvarchar(max) NOT NULL;
    ALTER TABLE [Branches] ADD DEFAULT N'' FOR [BranchName];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044345_changerequiredproperty'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125044345_changerequiredproperty', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044731_addedproperty'
)
BEGIN
    ALTER TABLE [tb_Students] ADD [customid] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044731_addedproperty'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125044731_addedproperty', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044942_addedprop'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[tb_Students]') AND [c].[name] = N'customid');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [tb_Students] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [tb_Students] DROP COLUMN [customid];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044942_addedprop'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[tb_Students]') AND [c].[name] = N'EmailAddress');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [tb_Students] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [tb_Students] ALTER COLUMN [EmailAddress] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251125044942_addedprop'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251125044942_addedprop', N'9.0.9');
END;

COMMIT;
GO