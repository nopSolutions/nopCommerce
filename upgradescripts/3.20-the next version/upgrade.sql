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
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SeName">
	<Value>Search engine friendly page name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.SeName.Hint">
	<Value>Set a search engine friendly page name e.g. ''some-topic-name'' to make your page URL ''http://www.yourStore.com/some-topic-name''. Leave empty to generate it automatically based on the title of the topic.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.OrderID.Hint">
	<Value>The order associated to this shipment.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowAddingOnlyExistingAttributeCombinations">
	<Value>Allow only existing attribute combinations</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AllowAddingOnlyExistingAttributeCombinations.Hint">
	<Value>Check to allow adding to the cart/wishlist only attribute combinations that exist and have stock greater than zero. In this case you have to create all existing product attribute combinations that you have in stock.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.LimitedToStores.Hint">
	<Value>Determines whether the country is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Countries.Fields.AvailableStores.Hint">
	<Value>Select stores for which the country will be shown.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage">
	<Value>Automatically detect language</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.AutomaticallyDetectLanguage.Hint">
	<Value>Check to automatically detect language based on a customer browser settings.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct">
	<Value>Purchased with product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Hint">
	<Value>A customer is added to this customer role once a specified product is purchased (paid). Please note that in case of refund or order cancellation you have to manually remove a customer from this role.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Choose">
	<Value>Choose product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Remove">
	<Value>Remove</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Registered">
	<Value>You cannot specify "Purchased with product" value for "Registered" customer role</Value>
  </LocaleResource>
  <LocaleResource Name="Common.OK">
	<Value>OK</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.OK">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Cancel">
	<Value></Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.Description2">
	<Value>Cookies help us deliver our services. By using our services, you agree to our use of cookies.</Value>
  </LocaleResource>
  <LocaleResource Name="EUCookieLaw.LearnMore">
	<Value>Learn more</Value>
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

--topic SEO names
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_topic_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[temp_topic_generate_sename]
GO
CREATE PROCEDURE [dbo].[temp_topic_generate_sename]
(
	@entity_id int,
    @topic_system_name nvarchar(1000),
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
	--get current name
	DECLARE @sql nvarchar(4000)
	
	--if system name is empty, we exit
	IF (@topic_system_name is null or @topic_system_name = N'')
		RETURN
    
    --generate se name    
	DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars varchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@topic_system_name)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
		DECLARE @c nvarchar(1)
        SET @c = substring(@topic_system_name, @p, 1)
        IF CHARINDEX(@c,@allowed_se_chars) > 0
        BEGIN
			SET @new_sename = @new_sename + @c
		END
		SET @p = @p + 1
	END
	--replace spaces with '-'
	SELECT @new_sename = REPLACE(@new_sename,' ','-');
    WHILE CHARINDEX('--',@new_sename) > 0
		SELECT @new_sename = REPLACE(@new_sename,'--','-');
    WHILE CHARINDEX('__',@new_sename) > 0
		SELECT @new_sename = REPLACE(@new_sename,'__','_');
    --ensure not empty
    IF (@new_sename is null or @new_sename = '')
		SELECT @new_sename = ISNULL(CAST(@entity_id AS nvarchar(max)), '0');
    --lowercase
	SELECT @new_sename = LOWER(@new_sename)
	--ensure this sename is not reserved
	WHILE (1=1)
	BEGIN
		DECLARE @sename_is_already_reserved bit
		SET @sename_is_already_reserved = 0
		SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename)
					BEGIN
						SELECT @sename_is_already_reserved = 1
					END'
		EXEC sp_executesql @sql,N'@sename nvarchar(1000), @sename_is_already_reserved nvarchar(4000) OUTPUT',@new_sename,@sename_is_already_reserved OUTPUT
		
		IF (@sename_is_already_reserved > 0)
		BEGIN
			--add some digit to the end in this case
			SET @new_sename = @new_sename + '-1'
		END
		ELSE
		BEGIN
			BREAK
		END
	END
	
	--return
    SET @result = @new_sename
END
GO



--update [sename] column for topics
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Topic]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Topic'
		
		DECLARE @topic_system_name nvarchar(1000)
		SET @topic_system_name = null -- clear cache (variable scope)
		SELECT @topic_system_name = [SystemName] FROM [Topic] WHERE [Id] = @sename_existing_entity_id
		
		--main sename
		EXEC	[dbo].[temp_topic_generate_sename]
				@entity_id = @sename_existing_entity_id,
				@topic_system_name = @topic_system_name,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [IsActive], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 1, 0)
		END		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
END
GO

--drop temporary procedures & functions
IF EXISTS (
		SELECT *
		FROM sys.objects
		WHERE object_id = OBJECT_ID(N'[temp_topic_generate_sename]') AND OBJECTPROPERTY(object_id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_topic_generate_sename]
GO

--New column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Product]') and NAME='AllowAddingOnlyExistingAttributeCombinations')
BEGIN
	ALTER TABLE [Product]
	ADD [AllowAddingOnlyExistingAttributeCombinations] bit NULL
END
GO

UPDATE [Product]
SET [AllowAddingOnlyExistingAttributeCombinations] = 0
WHERE [AllowAddingOnlyExistingAttributeCombinations] IS NULL
GO

ALTER TABLE [Product] ALTER COLUMN [AllowAddingOnlyExistingAttributeCombinations] bit NOT NULL
GO

--Store mapping for countries
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Country]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Country]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Country]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Country] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO

--a new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'localizationsettings.automaticallydetectlanguage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'localizationsettings.automaticallydetectlanguage', N'false', 0)
END
GO


--New "customer role" column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[CustomerRole]') and NAME='PurchasedWithProductId')
BEGIN
	ALTER TABLE [CustomerRole]
	ADD [PurchasedWithProductId] int NULL
END
GO

UPDATE [CustomerRole]
SET [PurchasedWithProductId] = 0
WHERE [PurchasedWithProductId] IS NULL
GO

ALTER TABLE [CustomerRole] ALTER COLUMN [PurchasedWithProductId] int NOT NULL
GO
