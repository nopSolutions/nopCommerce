
USE  nopcommerce46

-- nop 4.6 version related scripts

-- Author : Sateesh Munagala
-- Created Date : July 15th 2022
-- Updated Date : July 15th 2022


-- General Scripts for testing and not for production run
------------------------------------------------------------------

SELECT * FROM [dbo].[Setting] WHERE [Name] like '%storesettings%';
SELECT * FROM [dbo].Store

--update [dbo].Store set SslEnabled=1
--update [dbo].Store set Url='https://localhost:51796/'
--update [dbo].Store set Hosts='yourstore.com,www.yourstore.com'
















