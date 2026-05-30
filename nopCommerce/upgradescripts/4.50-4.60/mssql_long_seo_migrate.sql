IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') AND NAME='DefaultTitle')
BEGIN
	ALTER TABLE [Store] ADD [DefaultTitle] nvarchar(MAX) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') AND NAME='DefaultMetaDescription')
BEGIN
	ALTER TABLE [Store] ADD [DefaultMetaDescription] nvarchar(MAX) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') AND NAME='DefaultMetaKeywords')
BEGIN
	ALTER TABLE [Store] ADD [DefaultMetaKeywords] nvarchar(MAX) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') AND NAME='HomepageDescription')
BEGIN
	ALTER TABLE [Store] ADD [HomepageDescription] nvarchar(MAX) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Store]') AND NAME='HomepageTitle')
BEGIN
	ALTER TABLE [Store] ADD [HomepageTitle] nvarchar(MAX) NULL
END
GO

