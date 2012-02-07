--upgrade scripts from nopCommerce 2.40 to nopCommerce 2.50

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackByDimensions">
    <Value>Pack by dimensions</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackByOneItemPerPackage">
    <Value>Pack by one item per package</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Plugin.Shipping.Fedex.PackByVolume">
    <Value>Pack by volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingType">
    <Value>Packing type</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingType.Hint">
    <Value>Choose preferred packing type.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingPackageVolume">
    <Value>Package volume</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.Fedex.Fields.PackingPackageVolume.Hint">
    <Value>Enter your package volume.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnFrom">
    <Value>Created from</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnFrom.Hint">
    <Value>The creation from date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnTo">
    <Value>Created to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.CreatedOnTo.Hint">
    <Value>The creation to date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.ApproveSelected">
    <Value>Approve selected</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.DisapproveSelected">
    <Value>Disapprove selected</Value>
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
				[LanguageID],
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

--Customer currency rate issue fix
ALTER TABLE [dbo].[Order] ALTER COLUMN [CurrencyRate] decimal(18, 8) NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.displaytierpriceswithdiscounts')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.displaytierpriceswithdiscounts', N'true')
END
GO
--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'fedexsettings.packingpackagevolume')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'fedexsettings.packingpackagevolume', N'5184')
END
GO