--upgrade scripts from nopCommerce 4.10 to 4.20

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Title.Required">
    <Value>Title is required</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisableBillingAddressCheckoutStep.Hint">
    <Value>Check to disable "Billing address" step during checkout. Billing address will be pre-filled and saved using the default registration data (this option cannot be used with guest checkout enabled). Also ensure that appropriate address fields that cannot be pre-filled are not required (or disabled). If a customer doesn''t have a billing address, then the billing address step will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.RelativeDateTime.Past">
    <Value>{0} ago</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore">
    <Value>Ignore additional shipping charge for pick up in store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore.Hint">
    <Value>Check if you want ignore additional shipping charge for pick up in store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseResponseCompression">
    <Value>Use response compression</Value>
  </LocaleResource>  
   <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseResponseCompression.Hint">
    <Value>Enable to compress response (gzip by default). You can disable it if you have an active IIS Dynamic Compression Module configured at the server level.</Value>
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

UPDATE [Topic] 
SET [IncludeInFooterColumn1] = 0
WHERE [SystemName] = 'VendorTermsOfService'
GO

UPDATE [Topic]
SET [Title] = ISNULL([SystemName], '')
WHERE [Title] IS NULL OR [Title] = ''
GO

ALTER TABLE [Topic] ALTER COLUMN [Title] nvarchar(max) NOT NULL
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.ignoreadditionalshippingchargeforpickupinstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.ignoreadditionalshippingchargeforpickupinstore', N'true', 0)
END
GO
