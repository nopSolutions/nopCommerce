
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') AND NAME='AllowDirectReply')
BEGIN
	ALTER TABLE [MessageTemplate] ADD [AllowDirectReply] bit Default(0)
END
GO

