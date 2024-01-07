
USE [nopcommerce46]

-- nop 4.6 version related schema scripts

-- Author : Sateesh Munagala
-- Created Date : July 15th 2022
-- Updated Date : July 15th 2022

---------------------------------------------------------
-- ***  SCHEMA SCRIPTS ****
---------------------------------------------------------

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Customer]') AND NAME = 'CustomerProfileTypeId')
BEGIN
	ALTER TABLE [Customer]
	ADD CustomerProfileTypeId [int] NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'SenderSubject')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD SenderSubject [nvarchar](450) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'SenderBodyText')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD SenderBodyText [nvarchar](max) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'RecipientBodyText')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD RecipientBodyText [nvarchar](max) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'IsSystemGenerated')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD IsSystemGenerated Bit NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'ParentMessageId')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD ParentMessageId [int] NULL
END
GO


-- new column 'HelpText' as part of  NOP 4.7 version
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[CustomerAttribute]') AND NAME = 'HelpText')
BEGIN
	ALTER TABLE [CustomerAttribute]
	ADD HelpText [nvarchar](max) NULL
END
GO

-- new column 'ShowOnRegisterPage' as part of  NOP 4.7 version
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[CustomerAttribute]') AND NAME = 'ShowOnRegisterPage')
BEGIN
	ALTER TABLE [CustomerAttribute]
	ADD ShowOnRegisterPage Bit NULL
END
GO


---------------------------------------------------------
-- ***  END SCHEMA SCRIPTS ****
---------------------------------------------------------


