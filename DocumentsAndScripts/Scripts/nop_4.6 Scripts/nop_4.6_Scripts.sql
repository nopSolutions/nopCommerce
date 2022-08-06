
USE  nopcommerce46

-- nop 4.6 version related scripts

-- Author : Sateesh Munagala
-- Created Date : July 15th 2022
-- Updated Date : July 15th 2022


-- General Scripts for testing and not for production run
------------------------------------------------------------------

SELECT * FROM [dbo].[Setting] WHERE [Name] like '%storesettings%';
SELECT * FROM [dbo].Store

update [dbo].Store set SslEnabled=1
update [dbo].Store set Url='https://localhost:51796/'
update [dbo].Store set Hosts='yourstore.com,www.yourstore.com'




-- Data Scripts
---------------------------------

SELECT * FROM [dbo].[Setting] WHERE [Name] = 'vendorsettings.allowcustomerstoapplyforvendoraccount';
SELECT * FROM [dbo].[Setting] WHERE [Name] = 'catalogsettings.compareproductsenabled';
SELECT * FROM [dbo].[Setting] WHERE [Name] = 'customersettings.allowcustomerstouploadavatars';

SELECT * FROM [dbo].[poll]

SELECT * FROM [dbo].[Setting] WHERE [Name] = 'customersettings.allowcustomerstouploadavatars';
SELECT * FROM [dbo].[Setting] WHERE [Name] = 'customersettings.allowcustomerstouploadavatars';
SELECT * FROM [dbo].[Setting] WHERE [Name] = 'customersettings.allowcustomerstouploadavatars';

-- hide 'compare products' option in footer
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.compareproductsenabled';
-- hide footer option  'Apply for vendor account'
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'vendorsettings.allowcustomerstoapplyforvendoraccount';
-- Allow customer to update avatar photo
UPDATE [dbo].[Setting] SET Value='True' WHERE Name = 'vendorsettings.allowcustomerstoapplyforvendoraccount';
-- un publish all polls
UPDATE [dbo].[Poll] SET Published=0 













