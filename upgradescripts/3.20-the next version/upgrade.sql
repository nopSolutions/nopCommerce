--upgrade scripts from nopCommerce 3.20 to 3.30

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Catalog.Products.List.ImportFromExcelTip">
    <Value>Imported products are distinguished by SKU. If the SKU already exists, then its corresponding product will be updated.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1">
    <Value>Invoice footer text (left column)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn1.Hint">
    <Value>Enter the text that will appear at the bottom of generated invoices (left column).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2">
    <Value>Invoice footer text (right column)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.InvoiceFooterTextColumn2.Hint">
    <Value>Enter the text that will appear at the bottom of generated invoices (right column).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.DeleteAll">
    <Value>Delete all</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.DeletedAll">
    <Value>All queued emails have been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores.Hint">
	<Value>Determines whether the attribute is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.AvailableStores.Hint">
	<Value>Select stores for which the attribute will be shown.</Value>
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

--'Clear log' schedule task (disabled by default)
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ScheduleTask]
		WHERE [Type] = N'Nop.Services.Logging.ClearLogTask, Nop.Services')
BEGIN
	INSERT [dbo].[ScheduleTask] ([Name], [Seconds], [Type], [Enabled], [StopOnError])
	VALUES (N'Clear log', 3600, N'Nop.Services.Logging.ClearLogTask, Nop.Services', 0, 0)
END
GO

--delete checkout attributes. now they store specific
DELETE FROM [GenericAttribute]
WHERE [KeyGroup] = N'Customer' and [Key] = N'CheckoutAttributes'
GO
--Store mapping for checkout attributes
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CheckoutAttribute]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [CheckoutAttribute]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [CheckoutAttribute]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [CheckoutAttribute] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO
