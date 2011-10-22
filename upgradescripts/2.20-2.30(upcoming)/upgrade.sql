--upgrade scripts from nopCommerce 2.20 to nopCommerce 2.30

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.System.SystemInfo.ASPNETInfo.Hint">
        <Value>ASP.NET info</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.System.SystemInfo.IsFullTrust.Hint">
        <Value>Is full trust level</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.System.SystemInfo.NopVersion.Hint">
        <Value>nopCommerce version</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.System.SystemInfo.OperatingSystem.Hint">
        <Value>Operating system</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.System.SystemInfo.ServerLocalTime.Hint">
        <Value>Server local time</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.System.SystemInfo.ServerTimeZone.Hint">
        <Value>Server time zone</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.System.SystemInfo.UTCTime.Hint">
        <Value>Greenwich mean time (GMT/UTC)</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Common.ConfigurationNotRequired">
        <Value>Configuration is not required</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CheckUsernameAvailabilityEnabled">
        <Value>Allow customers to check the availability of usernames</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.CheckUsernameAvailabilityEnabled.Hint">
        <Value>A value indicating whether customers are allowed to check the availability of usernames (when registering or changing in ''My Account'').</Value>
    </LocaleResource>
    <LocaleResource Name="Account.CheckUsernameAvailability.Available">
        <Value>Username available</Value>
    </LocaleResource>
    <LocaleResource Name="Account.CheckUsernameAvailability.CurrentUsername">
        <Value>Current username</Value>
    </LocaleResource>
    <LocaleResource Name="Account.CheckUsernameAvailability.NotAvailable">
        <Value>Username not available</Value>
    </LocaleResource>
    <LocaleResource Name="Account.CheckUsernameAvailability.Button">
        <Value>Check Availability</Value>
    </LocaleResource>
    <LocaleResource Name="Account.Login.WrongCredentials">
        <Value>The credentials provided are incorrect</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Orders.Fields.BillingAddress.Hint">
        <Value>Billing address info</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Orders.Fields.ShippingAddress.Hint">
        <Value>Shipping address info</Value>
    </LocaleResource>
    <LocaleResource Name="Checkout.BillingToThisAddress">
        <Value></Value>
    </LocaleResource>
    <LocaleResource Name="Checkout.BillToThisAddress">
        <Value>Bill to this address</Value>
    </LocaleResource>    
    <LocaleResource Name="Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize">
        <Value>Allow customers to select page size</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize.Hint">
        <Value>Whether customers are allowed to select the page size from a predefined list of options.</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSizeOptions">
        <Value>Page Size options (comma separated)</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSizeOptions.Hint">
        <Value>Comma separated list of page size options (e.g. 10, 5, 15, 20). First option is the default page size if none are selected.</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AllowCustomersToSelectPageSize">
        <Value>Allow customers to select page size</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AllowCustomersToSelectPageSize.Hint">
        <Value>Whether customers are allowed to select the page size from a predefined list of options.</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSizeOptions">
        <Value>Page Size options (comma separated)</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSizeOptions.Hint">
        <Value>Comma separated list of page size options (e.g. 10, 5, 15, 20). First option is the default page size if none are selected.</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagAllowCustomersToSelectPageSize">
        <Value>Allow customers to select ''Products by tag'' page size</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagAllowCustomersToSelectPageSize.Hint">
        <Value>Whether customers are allowed to select the ''Products by tag'' page size from a predefined list of options.</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagPageSizeOptions">
        <Value>''Products by tag'' Page Size options (comma separated)</Value>
    </LocaleResource>
    <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagPageSizeOptions.Hint">
        <Value>Comma separated list of page size options (e.g. 10, 5, 15, 20). First option is the default page size if none are selected.</Value>
    </LocaleResource>
    <LocaleResource Name="Products.Tags.PageSize">
        <Value>Display</Value>
    </LocaleResource>
    <LocaleResource Name="Products.Tags.PageSize.PerPage">
        <Value>per page</Value>
    </LocaleResource>
    <LocaleResource Name="Categories.PageSize">
        <Value>Display</Value>
    </LocaleResource>
    <LocaleResource Name="Categories.PageSize.PerPage">
        <Value>per page</Value>
    </LocaleResource>
    <LocaleResource Name="Manufacturers.PageSize">
        <Value>Display</Value>
    </LocaleResource>
    <LocaleResource Name="Manufacturers.PageSize.PerPage">
        <Value>per page</Value>
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





--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'commonsettings.enablehttpcompression')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'commonsettings.enablehttpcompression', N'true')
END
GO

--customer can't be deleted until it has associated log records
IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'Log_Customer'
           AND parent_obj = Object_id('Log')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.[Log]
DROP CONSTRAINT Log_Customer
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [Log_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.defaultcategorypagesizeoptions')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.defaultcategorypagesizeoptions', N'4, 2, 8, 12')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.defaultmanufacturerpagesizeoptions')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.defaultmanufacturerpagesizeoptions', N'4, 2, 8, 12')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsbytagallowcustomerstoselectpagesize')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.productsbytagallowcustomerstoselectpagesize', N'True')
END
GO

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsbytagpagesizeoptions')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.productsbytagpagesizeoptions', N'4, 2, 8, 12')
END
GO


--Add fields to Category
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Category]') and NAME='AllowCustomersToSelectPageSize')
BEGIN
	ALTER TABLE [dbo].[Category]
	ADD [AllowCustomersToSelectPageSize] bit NULL
END
GO

UPDATE [dbo].[Category]
SET [AllowCustomersToSelectPageSize] = 1
WHERE [AllowCustomersToSelectPageSize] IS NULL
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [AllowCustomersToSelectPageSize] bit NOT NULL
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Category]') and NAME='PageSizeOptions')
BEGIN
	ALTER TABLE [dbo].[Category]
	ADD [PageSizeOptions] nvarchar(200) NULL
END
GO

UPDATE [dbo].[Category]
SET [PageSizeOptions] = N'4, 2, 8, 12'
WHERE [PageSizeOptions] IS NULL
GO

--Add fields to Manufacturer
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Manufacturer]') and NAME='AllowCustomersToSelectPageSize')
BEGIN
	ALTER TABLE [dbo].[Manufacturer]
	ADD [AllowCustomersToSelectPageSize] bit NULL
END
GO

UPDATE [dbo].[Manufacturer]
SET [AllowCustomersToSelectPageSize] = 1
WHERE [AllowCustomersToSelectPageSize] IS NULL
GO

ALTER TABLE [dbo].[Manufacturer] ALTER COLUMN [AllowCustomersToSelectPageSize] bit NOT NULL
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Manufacturer]') and NAME='PageSizeOptions')
BEGIN
	ALTER TABLE [dbo].[Manufacturer]
	ADD [PageSizeOptions] nvarchar(200) NULL
END
GO

UPDATE [dbo].[Manufacturer]
SET [PageSizeOptions] = N'4, 2, 8, 12'
WHERE [PageSizeOptions] IS NULL
GO
