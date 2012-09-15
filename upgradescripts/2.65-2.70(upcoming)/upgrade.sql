--upgrade scripts from nopCommerce 2.65 to nopCommerce 2.70

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="GiftCardAttribute.For">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.For.Virtual">
    <Value><![CDATA[For: {0} <{1}>]]></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From.Virtual">
    <Value><![CDATA[From: {0} <{1}>]]></Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.For.Physical">
    <Value>For: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="GiftCardAttribute.From.Physical">
    <Value>From: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart">
    <Value>Move items from wishlist to cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart.Hint">
    <Value>Check to move products from wishlist to the cart when clicking "Add to cart" button. Otherwise, they are copied.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Result.Continue">
    <Value>Continue</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterForeignKeyEq">
    <Value>Is equal to</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterForeignKeyNe">
    <Value>Is not equal to</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterOr">
    <Value>Or</Value>
  </LocaleResource>
  <LocaleResource Name="Telerik.GridLocalization.FilterStringNotSubstringOf">
    <Value>Does not contain</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.Gender">
    <Value>Gender</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.AgeGroup">
    <Value>Age group</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.Color">
    <Value>Color</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.Size">
    <Value>Size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.RecurringPayment">
    <Value>Recurring payment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.RecurringPayment.Hint">
    <Value>This is a recurring order. See the appropriate recurring payment record.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.IsEnabled">
    <Value>Is enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Fields.IsEnabled.Hint">
    <Value>Indicates whether the plugin is enabled/active.</Value>
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







IF EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[TaxRate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
	EXEC('ALTER TABLE [TaxRate] ALTER COLUMN [Percentage] decimal(18, 4) NOT NULL')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'shoppingcartsettings.moveitemsfromwishlisttocart')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'shoppingcartsettings.moveitemsfromwishlisttocart', N'true')
END
GO