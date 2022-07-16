
USE [nopcommerce46]

-- nop 4.6 version related schema scripts

-- Author : Sateesh Munagala
-- Created Date : July 15th 2022
-- Updated Date : July 15th 2022

---------------------------------------------------------
-- ***  SCHEMA SCRIPTS ****
---------------------------------------------------------

ALTER TABLE [dbo].[Customer] ADD CustomerProfileTypeId [int] NULL;

ALTER TABLE [dbo].[Forums_PrivateMessage] ADD SenderSubject [nvarchar](450) NULL;
ALTER TABLE [dbo].[Forums_PrivateMessage] ADD SenderBodyText [nvarchar](max) NULL;
ALTER TABLE [dbo].[Forums_PrivateMessage] ADD RecipientBodyText [nvarchar](max) NULL;
ALTER TABLE [dbo].[Forums_PrivateMessage] ADD IsSystemGenerated Bit NULL;


IF NOT EXISTS(SELECT * FROM   INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Discount'AND COLUMN_NAME = 'IsActive')
    BEGIN
        ALTER TABLE [Discount] ADD [IsActive] [bit] NOT NULL
    END






---------------------------------------------------------
-- ***  END SCHEMA SCRIPTS ****
---------------------------------------------------------


