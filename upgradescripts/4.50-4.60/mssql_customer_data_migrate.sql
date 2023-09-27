IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='FirstName')
BEGIN
	ALTER TABLE [Customer] ADD FirstName nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='LastName')
BEGIN
	ALTER TABLE [Customer] ADD LastName nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='Gender')
BEGIN
	ALTER TABLE [Customer] ADD Gender nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='Company')
BEGIN
	ALTER TABLE [Customer] ADD Company nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='StreetAddress')
BEGIN
	ALTER TABLE [Customer] ADD StreetAddress nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='StreetAddress2')
BEGIN
	ALTER TABLE [Customer] ADD StreetAddress2 nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='ZipPostalCode')
BEGIN
	ALTER TABLE [Customer] ADD ZipPostalCode nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='TimeZoneId')
BEGIN
	ALTER TABLE [Customer] ADD TimeZoneId nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='City')
BEGIN
	ALTER TABLE [Customer] ADD City nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='County')
BEGIN
	ALTER TABLE [Customer] ADD County nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='Phone')
BEGIN
	ALTER TABLE [Customer] ADD Phone nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='Fax')
BEGIN
	ALTER TABLE [Customer] ADD Fax nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='VatNumber')
BEGIN
	ALTER TABLE [Customer] ADD VatNumber nvarchar(1000) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='CustomCustomerAttributesXML')
BEGIN
	ALTER TABLE [Customer] ADD CustomCustomerAttributesXML nvarchar(max) NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='CountryId')
BEGIN
	ALTER TABLE [Customer] ADD CountryId int NOT NULL DEFAULT(0)
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='StateProvinceId')
BEGIN
	ALTER TABLE [Customer] ADD StateProvinceId int NOT NULL DEFAULT(0)
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='VatNumberStatusId')
BEGIN
	ALTER TABLE [Customer] ADD VatNumberStatusId int NOT NULL DEFAULT(0)
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='CurrencyId')
BEGIN
	ALTER TABLE [Customer] ADD CurrencyId int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='LanguageId')
BEGIN
	ALTER TABLE [Customer] ADD LanguageId int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='TaxDisplayTypeId')
BEGIN
	ALTER TABLE [Customer] ADD TaxDisplayTypeId int NULL
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Customer]') AND NAME='DateOfBirth')
BEGIN
	ALTER TABLE [Customer] ADD DateOfBirth datetime2 NULL
END
GO

--add FKs
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_Customer_CurrencyId_Currency_Id') AND parent_object_id = OBJECT_ID(N'Customer'))
BEGIN
ALTER TABLE [Customer] WITH CHECK ADD CONSTRAINT [FK_Customer_CurrencyId_Currency_Id] FOREIGN KEY([CurrencyId])
REFERENCES [Currency] ([Id])
ON DELETE SET NULL

ALTER TABLE [Customer] CHECK CONSTRAINT [FK_Customer_CurrencyId_Currency_Id]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_Customer_LanguageId_Language_Id') AND parent_object_id = OBJECT_ID(N'Customer'))
BEGIN
ALTER TABLE [Customer] WITH CHECK ADD CONSTRAINT [FK_Customer_LanguageId_Language_Id] FOREIGN KEY([LanguageId])
REFERENCES [Language] ([Id])
ON DELETE SET NULL

ALTER TABLE [Customer] CHECK CONSTRAINT [FK_Customer_LanguageId_Language_Id]
END
GO

-- delete duplicate entities
With cte_duplicates AS 
(SELECT [KeyGroup], [Key], [EntityId], row_number() 
	OVER(PARTITION BY [KeyGroup], [Key], [EntityId] order by [KeyGroup], [Key], [EntityId]) rownumber 
	FROM [GenericAttribute] 
	WHERE [KeyGroup] = 'Customer' AND 
	[Key] in ('FirstName', 'LastName', 'Gender', 'Company', 
		'StreetAddress', 'StreetAddress2', 'ZipPostalCode', 'City', 'County', 'Phone', 'Fax', 'VatNumber', 
		'TimeZoneId', 'CustomCustomerAttributes', 'CountryId', 'StateProvinceId', 'VatNumberStatusId', 
		'CurrencyId', 'LanguageId', 'TaxDisplayTypeId', 'DateOfBirth')) 
delete FROM cte_duplicates where rownumber != 1

-- delete invalid country ids
DELETE FROM [GenericAttribute] WHERE KeyGroup = 'Customer' AND [Key] = 'CountryId' AND [Value] NOT IN (SELECT Id FROM Country)

-- delete invalid language ids
DELETE FROM [GenericAttribute] WHERE KeyGroup = 'Customer' AND [Key] = 'LanguageId' AND [Value] NOT IN (SELECT Id FROM [Language])

-- truncate if length is more than 1000
Update [GenericAttribute] Set [Value] = SUBSTRING([Value], 1, 1000) WHERE [KeyGroup] = 'Customer' AND 
	[Key] in ('FirstName', 'LastName', 'Gender', 'Company', 'StreetAddress', 'StreetAddress2',
		'ZipPostalCode', 'City', 'County', 'Phone', 'Fax', 'VatNumber', 'TimeZoneId')
		
-- data type cast
Update [GenericAttribute] Set [Value] = ISNULL(TRY_CAST([Value] AS int), 0) WHERE [KeyGroup] = 'Customer' AND 
	[Key] in ('CountryId', 'StateProvinceId', 'VatNumberStatusId')
Update [GenericAttribute] Set [Value] = TRY_CAST([Value] AS int) WHERE [KeyGroup] = 'Customer' AND 
	[Key] in ('CurrencyId', 'LanguageId', 'TaxDisplayTypeId')
Update [GenericAttribute] Set [Value] = TRY_PARSE([Value] AS datetime USING 'en-gb') WHERE [KeyGroup] = 'Customer' AND
	[Key] = 'DateOfBirth'

-- Move data FROM GA to Customer table
-- FirstName
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'FirstName'
WHEN MATCHED THEN UPDATE SET c.FirstName = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'FirstName'

-- LastName
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'LastName'
WHEN MATCHED THEN UPDATE SET c.LastName = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'LastName'

-- Gender
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'Gender'
WHEN MATCHED THEN UPDATE SET c.Gender = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'Gender'

-- Company
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'Company'
WHEN MATCHED THEN UPDATE SET c.Company = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'Company'

-- StreetAddress
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'StreetAddress'
WHEN MATCHED THEN UPDATE SET c.StreetAddress = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'StreetAddress'

-- StreetAddress2
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'StreetAddress2'
WHEN MATCHED THEN UPDATE SET c.StreetAddress2 = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'StreetAddress2'

-- ZipPostalCode
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'ZipPostalCode'
WHEN MATCHED THEN UPDATE SET c.ZipPostalCode = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'ZipPostalCode'

-- City
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'City'
WHEN MATCHED THEN UPDATE SET c.City = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'City'

-- County
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'County'
WHEN MATCHED THEN UPDATE SET c.County = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'County'

-- Phone
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'Phone'
WHEN MATCHED THEN UPDATE SET c.Phone = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'Phone'

-- Fax
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'Fax'
WHEN MATCHED THEN UPDATE SET c.Fax = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'Fax'

-- VatNumber
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'VatNumber'
WHEN MATCHED THEN UPDATE SET c.VatNumber = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'VatNumber'

-- TimeZoneId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'TimeZoneId'
WHEN MATCHED THEN UPDATE SET c.TimeZoneId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'TimeZoneId'

-- CustomCustomerAttributesXML
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'CustomCustomerAttributes'
WHEN MATCHED THEN UPDATE SET c.CustomCustomerAttributesXML = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'CustomCustomerAttributes'

-- CountryId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'CountryId'
WHEN MATCHED THEN UPDATE SET c.CountryId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'CountryId'

-- StateProvinceId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'StateProvinceId'
WHEN MATCHED THEN UPDATE SET c.StateProvinceId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'StateProvinceId'

-- VatNumberStatusId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'VatNumberStatusId'
WHEN MATCHED THEN UPDATE SET c.VatNumberStatusId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'VatNumberStatusId'

-- CurrencyId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'CurrencyId'
WHEN MATCHED THEN UPDATE SET c.CurrencyId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'CurrencyId'

-- LanguageId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'LanguageId'
WHEN MATCHED THEN UPDATE SET c.LanguageId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'LanguageId'

-- TaxDisplayTypeId
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'TaxDisplayTypeId'
WHEN MATCHED THEN UPDATE SET c.TaxDisplayTypeId = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'TaxDisplayTypeId'

-- DateOfBirth
MERGE Customer AS c USING [GenericAttribute] AS ga
ON ga.EntityId = c.Id AND ga.KeyGroup = 'Customer' AND ga.[Key] = 'DateOfBirth'
WHEN MATCHED THEN UPDATE SET c.DateOfBirth = ga.[Value];

DELETE FROM [GenericAttribute] where [KeyGroup] = 'Customer' AND [Key] = 'DateOfBirth'