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
  <LocaleResource Name="Admin.System.ScheduleTasks">
    <Value>Schedule tasks</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Name.Required">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Seconds">
    <Value>Seconds (run period)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Seconds.Positive">
    <Value>Seconds should be positive</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.Enabled">
    <Value>Enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.StopOnError">
    <Value>Stop on error</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.LastStart">
    <Value>Last start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.LastEnd">
    <Value>Last end date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.LastSuccess">
    <Value>Last success date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.ScheduleTasks.RestartApplication">
    <Value>Do not forgot to restart the application once a task has been modified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.BillingCountry">
    <Value>Billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.Bestsellers.BillingCountry.Hint">
    <Value>Filter by order billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-product'' to make your page URL ''http://www.yourStore.com/the-best-product''. Leave empty to generate it automatically based on the name of the product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-category'' to make your page URL ''http://www.yourStore.com/the-best-category''. Leave empty to generate it automatically based on the name of the category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.SeName.Hint">
    <Value>Set a search engine friendly page name e.g. ''the-best-manufacturer'' to make your page URL ''http://www.yourStore.com/the-best-manufacturer''. Leave empty to generate it automatically based on the name of the manufacturer.</Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Common.PageTitleSeoAdjustment.PagenameAfterStorename">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Common.PageTitleSeoAdjustment.StorenameAfterPagename">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Seo.PageTitleSeoAdjustment.PagenameAfterStorename">
    <Value>Page name comes after store name</Value>
  </LocaleResource>
  <LocaleResource Name="Nop.Core.Domain.Seo.PageTitleSeoAdjustment.StorenameAfterPagename">
    <Value>Store name comes after page name</Value>
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


--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageScheduleTasks')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Schedule Tasks', N'ManageScheduleTasks', N'Configuration')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admin role be default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO

--more SQL indexes
IF NOT EXISTS (SELECT 1 from sysindexes WHERE [NAME]=N'IX_ActivityLog_CreatedOnUtc' and id=object_id(N'[ActivityLog]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
END
GO







--New search engine friendly URLs implementation

IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'seosettings.reservedurlrecordslugs', N'admin,install,recentlyviewedproducts,newproducts,compareproducts,clearcomparelist,setproductreviewhelpfulness,login,register,logout,cart,wishlist,emailwishlist,checkout,onepagecheckout,contactus,passwordrecovery,subscribenewsletter,blog,boards,inboxupdate,sentupdate,news,sitemap,sitemapseo,search,config,eucookielawaccept')
END
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[UrlRecord]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[UrlRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityName] nvarchar(400) NOT NULL,
	[Slug] nvarchar(400) NOT NULL,
	[LanguageId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
--new indexes
IF NOT EXISTS (SELECT 1 from sysindexes WHERE [NAME]=N'IX_UrlRecord_Slug' and id=object_id(N'[UrlRecord]'))
BEGIN
	CREATE UNIQUE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
END
GO


IF EXISTS (
		SELECT *
		FROM sysobjects
		WHERE id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_generate_sename]
GO
CREATE PROCEDURE [temp_generate_sename]
(
    @table_name nvarchar(1000),
    @entity_id int,
    @language_id int = 0, --0 to process main sename column, --language id to process a localized value
    @result nvarchar(1000) OUTPUT
)
AS
BEGIN
	--get current name
	DECLARE @current_sename nvarchar(1000)
	DECLARE @sql nvarchar(4000)
	
	IF (@language_id = 0)
	BEGIN
		SET @sql = 'SELECT @current_sename = [SeName] FROM [' + @table_name + '] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT
		
		--if not empty, se name is already specified by a store owner. if empty, we should use product name
		IF (@current_sename is null or @current_sename = N'')
		BEGIN
			SET @sql = 'SELECT @current_sename = [Name] FROM [' + @table_name + '] WHERE [Id] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
			EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT		
		END
    END
    ELSE
    BEGIN
		SET @sql = 'SELECT @current_sename = [LocaleValue] FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
		EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT
		
		--if not empty, se name is already specified by a store owner. if empty, we should use poduct name
		IF (@current_sename is null or @current_sename = N'')
		BEGIN
			SET @sql = 'SELECT @current_sename = [LocaleValue] FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''Name'' AND [LanguageId] = ' + ISNULL(CAST(@language_id AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0')
			EXEC sp_executesql @sql,N'@current_sename nvarchar(1000) OUTPUT',@current_sename OUTPUT		
		END
		
		--if localized product name is also empty, we exit
		IF (@current_sename is null or @current_sename = N'')
			RETURN
    END
    
    --generate se name    
	DECLARE @new_sename nvarchar(1000)
    SET @new_sename = ''
    --ensure only allowed chars
    DECLARE @allowed_se_chars varchar(4000)
    --Note for store owners: add more chars below if want them to be supported when migrating your data
    SET @allowed_se_chars = N'abcdefghijklmnopqrstuvwxyz1234567890 _-'
    DECLARE @l int
    SET @l = len(@current_sename)
    DECLARE @p int
    SET @p = 1
    WHILE @p <= @l
    BEGIN
		DECLARE @c nvarchar(1)
        SET @c = substring(@current_sename, @p, 1)
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
		SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [Slug] = @sename AND [EntityId] <> ' + ISNULL(CAST(@entity_id AS nvarchar(max)), '0') + ')
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

--update [sename] column for products
IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[Product]') and NAME='SeName')
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Product]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Product'
		
		--main sename
		EXEC	[dbo].[temp_generate_sename]
				@table_name = @table_name,
				@entity_id = @sename_existing_entity_id,
				@language_id = 0,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 0)
		END		

		--localized values
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT [ID]
		FROM [Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			SET @sename = null -- clear cache (variable scope)
			
			EXEC	[dbo].[temp_generate_sename]
					@table_name = @table_name,
					@entity_id = @sename_existing_entity_id,
					@language_id = @ExistingLanguageID,
					@result = @sename OUTPUT
			IF (len(@sename) > 0)
			BEGIN
				
				DECLARE @sql nvarchar(4000)
				--insert
				SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
				BEGIN
					--update
					UPDATE [UrlRecord]
					SET [Slug] = @sename
					WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
					VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
				END
				'				
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
				
				
				--delete
				SET @sql = 'DELETE FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0')
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
			END

			--fetch next language identifier
			FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
		CLOSE cur_existinglanguage
		DEALLOCATE cur_existinglanguage
		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
	
	--drop SeName column
	EXEC('ALTER TABLE [Product] DROP COLUMN [SeName]')
	
END
GO



--update [sename] column for categories
IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[Category]') and NAME='SeName')
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Category]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Category'
		
		--main sename
		EXEC	[dbo].[temp_generate_sename]
				@table_name = @table_name,
				@entity_id = @sename_existing_entity_id,
				@language_id = 0,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 0)
		END		

		--localized values
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT [ID]
		FROM [Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			SET @sename = null -- clear cache (variable scope)
			
			EXEC	[dbo].[temp_generate_sename]
					@table_name = @table_name,
					@entity_id = @sename_existing_entity_id,
					@language_id = @ExistingLanguageID,
					@result = @sename OUTPUT
			IF (len(@sename) > 0)
			BEGIN
				
				DECLARE @sql nvarchar(4000)
				SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
				BEGIN
					--update
					UPDATE [UrlRecord]
					SET [Slug] = @sename
					WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
					VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
				END
				'
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
				
				
				--delete
				SET @sql = 'DELETE FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0')
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
			END
					

			--fetch next language identifier
			FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
		CLOSE cur_existinglanguage
		DEALLOCATE cur_existinglanguage
		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
	
	--drop SeName column
	EXEC('ALTER TABLE [Category] DROP COLUMN [SeName]')
END
GO




--update [sename] column for categories
IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[Manufacturer]') and NAME='SeName')
BEGIN
	DECLARE @sename_existing_entity_id int
	DECLARE cur_sename_existing_entity CURSOR FOR
	SELECT [Id]
	FROM [Manufacturer]
	OPEN cur_sename_existing_entity
	FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @sename nvarchar(1000)	
		SET @sename = null -- clear cache (variable scope)
		
		DECLARE @table_name nvarchar(1000)	
		SET @table_name = N'Manufacturer'
		
		--main sename
		EXEC	[dbo].[temp_generate_sename]
				@table_name = @table_name,
				@entity_id = @sename_existing_entity_id,
				@language_id = 0,
				@result = @sename OUTPUT
				
		IF EXISTS(SELECT 1 FROM [UrlRecord] WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name)
		BEGIN
			UPDATE [UrlRecord]
			SET [Slug] = @sename
			WHERE [LanguageId]=0 AND [EntityId]=@sename_existing_entity_id AND [EntityName]=@table_name
		END
		ELSE
		BEGIN
			INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
			VALUES (@sename_existing_entity_id, @table_name, @sename, 0)
		END		

		--localized values
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT [ID]
		FROM [Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN	
			SET @sename = null -- clear cache (variable scope)
			
			EXEC	[dbo].[temp_generate_sename]
					@table_name = @table_name,
					@entity_id = @sename_existing_entity_id,
					@language_id = @ExistingLanguageID,
					@result = @sename OUTPUT
			IF (len(@sename) > 0)
			BEGIN
				
				DECLARE @sql nvarchar(4000)
				SET @sql = 'IF EXISTS (SELECT 1 FROM [UrlRecord] WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + ')
				BEGIN
					--update
					UPDATE [UrlRecord]
					SET [Slug] = @sename
					WHERE [EntityName]=''' + @table_name + ''' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') + '
				END
				ELSE
				BEGIN
					--insert
					INSERT INTO [UrlRecord] ([EntityId], [EntityName], [Slug], [LanguageId])
					VALUES (' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0') +','''+ @table_name + ''',@sename, ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0')+ ')
				END
				'
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
				
				
				--delete
				SET @sql = 'DELETE FROM [LocalizedProperty] WHERE [LocaleKeyGroup]=''' + @table_name + ''' AND [LocaleKey] = ''SeName'' AND [LanguageId] = ' + ISNULL(CAST(@ExistingLanguageID AS nvarchar(max)), '0') + ' AND [EntityId] = ' + ISNULL(CAST(@sename_existing_entity_id AS nvarchar(max)), '0')
				EXEC sp_executesql @sql,N'@sename nvarchar(1000) OUTPUT',@sename OUTPUT
			END
					

			--fetch next language identifier
			FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
		CLOSE cur_existinglanguage
		DEALLOCATE cur_existinglanguage
		

		--fetch next identifier
		FETCH NEXT FROM cur_sename_existing_entity INTO @sename_existing_entity_id
	END
	CLOSE cur_sename_existing_entity
	DEALLOCATE cur_sename_existing_entity
	
	--drop SeName column
	EXEC('ALTER TABLE [Manufacturer] DROP COLUMN [SeName]')
END
GO

--drop temporary procedures & functions
IF EXISTS (
		SELECT *
		FROM sysobjects
		WHERE id = OBJECT_ID(N'[temp_generate_sename]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [temp_generate_sename]
GO
