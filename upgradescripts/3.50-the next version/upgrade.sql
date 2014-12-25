--upgrade scripts from nopCommerce 3.50 to 3.60

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.NotifyCustomerAboutShippingFromMultipleLocations">
    <Value>Notify customer about shipping from multiple locations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.NotifyCustomerAboutShippingFromMultipleLocations.Hint">
    <Value>Check if you want customers to be notified when shipping from multiple locations.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.ShippingMethod.ShippingFromMultipleLocations">
    <Value>Please note that your order will be shipped from multiple locations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.SeNames.Details">
    <Value>Edit page</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PricesConsiderPromotions">
    <Value>Prices consider promotions</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.PricesConsiderPromotions.Hint">
    <Value>Check if you want prices to be calculated with promotions (tier prices, discounts, special prices, tax, etc). But please note that it can significantly reduce time required to generate the feed file.</Value>
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
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shippingsettings.notifycustomeraboutshippingfrommultiplelocations')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.notifycustomeraboutshippingfrommultiplelocations', N'false', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.pricesconsiderpromotions')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'frooglesettings.pricesconsiderpromotions', N'false', 0)
END
GO
