IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.HideNewsletterBox')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.HideNewsletterBox', N'False', N'')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='IsPasswordProtected')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD IsPasswordProtected bit NOT NULL CONSTRAINT [DF_Nop_Topic_IsPasswordProtected] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Topic]') and NAME='Password')
BEGIN
	ALTER TABLE [dbo].[Nop_Topic] 
	ADD Password nvarchar(200) NOT NULL CONSTRAINT [DF_Nop_Topic_Password] DEFAULT ((''))
END
GO


