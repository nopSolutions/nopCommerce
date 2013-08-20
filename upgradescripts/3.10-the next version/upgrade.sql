--upgrade scripts from nopCommerce 3.10 to 3.20

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Orders.Shipments.List.Country">
    <Value>Country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.Country.Hint">
    <Value>Search by a specific country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StateProvince">
    <Value>State / province</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.StateProvince.Hint">
    <Value>Search by a specific state.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.City">
    <Value>City</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.City.Hint">
    <Value>Search by a specific city.</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Product.ImageLinkTitleFormat.Details">
    <Value>Picture of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Media.Product.ImageAlternateTextFormat.Details">
    <Value>Picture of {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Category">
    <Value>Category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Category.Hint">
    <Value>Search in a specific category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Manufacturer">
    <Value>Manufacturer</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.Manufacturer.Hint">
    <Value>Search in a specific manufacturer.</Value>
  </LocaleResource>
</Language>
'

CREATE TABLE #LocaleStringResourceTmp
	(
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)

INSERT INTO #LocaleStringResourceTmp (ResourceName, ResourceValue)
SELECT	nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

--do it for each existing language
DECLARE @ExistingLanguageID int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [ID]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ResourceName nvarchar(200)
	DECLARE @ResourceValue nvarchar(MAX)
	DECLARE cur_localeresource CURSOR FOR
	SELECT ResourceName, ResourceValue
	FROM #LocaleStringResourceTmp
	OPEN cur_localeresource
	FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [LocaleStringResource]
			(
				[LanguageId],
				[ResourceName],
				[ResourceValue]
			)
			VALUES
			(
				@ExistingLanguageID,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource


	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO


--Add a reference for [StoreMapping] table
--but first, delete abandoned records
DELETE FROM [StoreMapping]
WHERE [StoreId] NOT IN (SELECT [Id] FROM [Store])
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'StoreMapping_Store'
           AND parent_object_id = Object_id('StoreMapping')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[StoreMapping]  WITH CHECK ADD  CONSTRAINT [StoreMapping_Store] FOREIGN KEY([StoreId])
	REFERENCES [dbo].[Store] ([Id])
	ON DELETE CASCADE
END
GO


--Add a reference for [AclRecord] table
--but first, delete abandoned records
DELETE FROM [AclRecord]
WHERE [CustomerRoleId] NOT IN (SELECT [Id] FROM [CustomerRole])
GO

IF NOT EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'AclRecord_CustomerRole'
           AND parent_object_id = Object_id('AclRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
BEGIN
	ALTER TABLE [dbo].[AclRecord]  WITH CHECK ADD  CONSTRAINT [AclRecord_CustomerRole] FOREIGN KEY([CustomerRoleId])
	REFERENCES [dbo].[CustomerRole] ([Id])
	ON DELETE CASCADE
END
GO

DELETE FROM [dbo].[PermissionRecord]
WHERE [SystemName] = N'ManageCustomerRoles'
GO

