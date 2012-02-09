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
  <LocaleResource Name="Admin.Common.ExportToXml.All">
    <Value>Export to XML (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.Selected">
    <Value>Export to XML (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.All">
    <Value>Export to Excel (all)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.Selected">
    <Value>Export to Excel (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExcelFile">
    <Value>Excel file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile">
    <Value>Xml file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile.Note1">
    <Value>NOTE: It can take up to several minutes.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.XmlFile.Note2">
    <Value>NOTE: DO NOT click twice.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.CsvFile">
    <Value>CSV file</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.LoseUnsavedChanges">
    <Value>You are going to lose any unsaved changes. Are you sure?</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.Match">
    <Value>Specified store URL matches this store URL</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.URL.NoMatch">
    <Value>Specified store URL ({0}) doesn''t match this store URL ({1})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.Set">
    <Value>Primary exchange rate currency is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.Rate1">
    <Value>Primary exchange rate currency. The rate should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.ExchangeCurrency.NotSet">
    <Value>Primary exchange rate currency is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PrimaryCurrency.Set">
    <Value>Primary store currency is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PrimaryCurrency.NotSet">
    <Value>Primary store currency is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.Set">
    <Value>Default weight is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.Ratio1">
    <Value>Default weight. The ratio should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultWeight.NotSet">
    <Value>Default weight is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.Set">
    <Value>Default dimension is set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.Ratio1">
    <Value>Default dimension. The ratio should be set to 1.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.DefaultDimension.NotSet">
    <Value>Default dimension is not set</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.Shipping.OnlyOneOffline">
    <Value>Only one offline shipping rate computation method is recommended to use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PaymentMethods.OK">
    <Value>Payment methods are OK</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Warnings.PaymentMethods.NoActive">
    <Value>You don''t have active payment methods</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.CantDeleteExchange">
    <Value>The primary exchange rate currency can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.CantDeletePrimary">
    <Value>The primary store currency can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Weights.CantDeletePrimary">
    <Value>The primary weight can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Measures.Dimensions.CantDeletePrimary">
    <Value>The primary dimension can''t be deleted.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.States.CantDeleteWithAddresses">
    <Value>The state can''t be deleted. It has associated addresses.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description">
    <Value>Manual plugin installation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step1">
    <Value>Upload the plugin to the /plugins folder in your nopCommerce directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step2">
    <Value>Restart your application (or click ''Reload list of plugins'' button).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step3">
    <Value>Scroll down through the list of plugins to find the newly installed plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step4">
    <Value>Click on the ''Install'' link to install the plugin.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.Step5">
    <Value>Note: If you''re running nopCommerce in medium trust, then it''s recommended to clear your \Plugins\bin\ directory</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing">
    <Value>Editing</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Editing.Hint">
    <Value>This grid allows the bulk editing of the ''Friendly name'' and ''Display order'' fields. To enter edit mode just click a cell.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.Installation">
    <Value>Installation</Value>
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