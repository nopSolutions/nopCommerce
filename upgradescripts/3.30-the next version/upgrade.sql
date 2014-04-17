--upgrade scripts from nopCommerce 3.30 to 3.40

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore">
    <Value>"Pick Up in Store" enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.AllowPickUpInStore.Hint">
    <Value>A value indicating whether "Pick Up in Store" option is enabled during checkout.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore">
    <Value>In-Store Pickup</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore.Description">
    <Value>Pick up your items at the store (put your store address here)</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.PickUpInStore.MethodName">
    <Value>In-Store Pickup</Value>
  </LocaleResource>
  <LocaleResource Name="BackInStockSubscriptions.OnlyRegistered">
    <Value>Only registered customers can use this feature</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AdminComment.Hint">
    <Value>Admin comment. For internal use.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.AdminComment.Button">
    <Value>Set admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForMobileDevices">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForMobileDevices.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.MobileDevicesSupported">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.MobileDevicesSupported.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Mobile.ViewFullSite">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Mobile.ViewMobileVersion">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.HeaderQuantity.Mobile">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Wishlist.HeaderQuantity.Mobile">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops.GetMore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreThemeForDesktops.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme">
    <Value>Default store theme</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.GetMore">
    <Value>You can get more themes on</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.Hint">
    <Value>The public store theme. You can download themes from the extensions page at www.nopcommerce.com.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault">
    <Value>Newsletter ticked by default</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.NewsletterTickedByDefault.Hint">
    <Value>A value indicating whether ''Newsletter'' checkbox is ticked by default on the registration page.</Value>
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

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.allowpickupinstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.allowpickupinstore', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='PickUpInStore')
BEGIN
	ALTER TABLE [Order]
	ADD [PickUpInStore] bit NULL
END
GO

UPDATE [Order]
SET [PickUpInStore] = 0
WHERE [PickUpInStore] IS NULL
GO

ALTER TABLE [Order] ALTER COLUMN [PickUpInStore] bit NOT NULL
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Shipment]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Shipment]
	ADD [AdminComment] nvarchar(MAX) NULL
END
GO

--delete some settings
DELETE FROM [Setting]
WHERE [name] = N'storeinformationsettings.emulatemobiledevice'
GO

DELETE FROM [Setting]
WHERE [name] = N'storeinformationsettings.mobiledevicessupported'
GO

DELETE FROM [Setting]
WHERE [name] = N'storeinformationsettings.defaultstorethemeformobiledevices'
GO

UPDATE [GenericAttribute]
SET [key] = N'WorkingThemeName'
WHERE [key] = N'WorkingDesktopThemeName'
GO

UPDATE [Setting]
SET [name] = N'storeinformationsettings.defaultstoretheme'
WHERE [name] = N'storeinformationsettings.defaultstorethemefordesktops'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'customersettings.newslettertickedbydefault')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'customersettings.newslettertickedbydefault', N'true', 0)
END
GO