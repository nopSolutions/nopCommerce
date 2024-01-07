
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


-- Customer related DDL scripts for updating tooltip values in register and info pages
UPDATE [CustomerAttribute]
SET HelpText='Select Take Support if you want to take support else select Give Support'
WHERE Id=1

UPDATE [CustomerAttribute]
SET HelpText='Select Avialibilty so that people can contact you if you are only available.'
WHERE Id=2

UPDATE [CustomerAttribute]
SET HelpText='Provide Your relavent experiance with respect to technical skill you mentioned.'
WHERE Id=3

UPDATE [CustomerAttribute]
SET HelpText='Provide Your mother tongue so that others can contact you <br/> if they want to contact same language'
WHERE Id=4

UPDATE [CustomerAttribute]
SET HelpText='Provide short description like  "I have 5 years experaince in xyz technologies" OR "I am looking for some one who can provide support on XYZ technologies"'
WHERE Id=5

UPDATE [CustomerAttribute]
SET HelpText='Provide your technical skill set ,exp etc OR what are you looking for. Please do not share your personally identifiable information such as contact nos,email ids, fb ids, linked in ids. This is strictly against our policy.  '
WHERE Id=6

UPDATE [CustomerAttribute]
SET HelpText='Select TOP 5 skills that you are expert in OR TOP 5 skill that are you looking for in a procpective match'
WHERE Id=7

UPDATE [CustomerAttribute]
SET HelpText='Select TOP 5 optional skills that you have OR TOP 5 optional skills that are you looking for in a procpective match'
WHERE Id=8

UPDATE [CustomerAttribute]
SET HelpText='Provide your linkedin url. We will never show this to anyone . This is to verify your experiance by our internal team and once verified you will get higher ranking in user searchs '
WHERE Id=9

---------------------------------------------------------------------------------------------------












