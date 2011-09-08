--upgrade scripts from nopCommerce 2.10 to nopCommerce 2.20

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagPageSize">
    <Value>''Products by tag'' page. Products per page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ProductsByTagPageSize.Hint">
    <Value>Set the page size for products on ''Products by tag'' page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.PictureThumbnailUrl">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.NumberOfDaysReturnRequestAvailable">
    <Value>Number of days that the return request is available</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Order.NumberOfDaysReturnRequestAvailable.Hint">
    <Value>Set a certain number of days that the Return Request Link will be available in the customer area. For example if the store owner allows returns within 30 days after purchase, then they would set this to 30. When the customer logs into the website and looks at "My Account" any orders completed more than 30 days ago would not show a Return Request button.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.Download.DownloadCount">
    <Value>Number of downloads: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.Download.ResetDownloadCount">
    <Value>Reset</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Products.Download.ResetDownloadCount.Title">
    <Value>Click to reset download count</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.CategoryTemplate">
    <Value>Category template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.CategoryTemplate.Hint">
    <Value>Choose a category template. This template defines how this category (and it''s products) will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.ManufacturerTemplate">
    <Value>Manufacturer template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.ManufacturerTemplate.Hint">
    <Value>Choose a manufacturer template. This template defines how this manufacturer (and it''s products) will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.CurrentWishlists">
    <Value>Current wishlists</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequireOtherProducts">
    <Value>Require other product variants are added to the cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequireOtherProducts.Hint">
    <Value>Check if this product variant requires that other product variants are added to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequiredProductVariantIds">
    <Value>Required product variant IDs</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.RequiredProductVariantIds.Hint">
    <Value>Specify comma separated list of required product variant IDs. NOTE: Ensure that there are no circular references (for example, A requires B, and B requires A).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AutomaticallyAddRequiredProductVariants">
    <Value>Automatically add these product variants to the cart</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.AutomaticallyAddRequiredProductVariants.Hint">
    <Value>Check to automatically add this product variants to the cart.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.RequiredProductWarning">
    <Value>This product requires the following product is added to the cart: {0}</Value>
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



IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.allowunicodecharsinurls')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'seosettings.allowunicodecharsinurls', N'true')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'securitysettings.hideadminmenuitemsbasedonpermissions')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'securitysettings.hideadminmenuitemsbasedonpermissions', N'false')
END
GO



--missed 'Manage Plugins' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManagePlugins')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Manage Plugins', N'ManagePlugins', N'Configuration')

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


--ProductsByTags page size
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsbytagpagesize')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.productsbytagpagesize', N'4')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.displayproductpictures')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'adminareasettings.displayproductpictures', N'true')
END
GO

--home page product box size
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.usesmallproductboxonhomepage')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'catalogsettings.usesmallproductboxonhomepage', N'true')
END
GO


--return requsts
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'ordersettings.numberofdaysreturnrequestavailable')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'ordersettings.numberofdaysreturnrequestavailable', N'365')
END
GO




--Category templates
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[CategoryTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[CategoryTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[ViewPath] [nvarchar](400) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[CategoryTemplate]
		WHERE [Name] = N'Products in Grid or Lines')
BEGIN
	INSERT [dbo].[CategoryTemplate] ([Name], [ViewPath], [DisplayOrder])
	VALUES (N'Products in Grid or Lines', N'CategoryTemplate.ProductsInGridOrLines', 1)
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Category]') and NAME='CategoryTemplateId')
BEGIN
	ALTER TABLE [dbo].[Category] 
	ADD [CategoryTemplateId] int NULL
END
GO

UPDATE [dbo].[Category]
SET [CategoryTemplateId]=1
WHERE [CategoryTemplateId] is null
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [CategoryTemplateId] int NOT NULL
GO




--Manufacturer templates
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[ManufacturerTemplate]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ManufacturerTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[ViewPath] [nvarchar](400) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ManufacturerTemplate]
		WHERE [Name] = N'Products in Grid or Lines')
BEGIN
	INSERT [dbo].[ManufacturerTemplate] ([Name], [ViewPath], [DisplayOrder])
	VALUES (N'Products in Grid or Lines', N'ManufacturerTemplate.ProductsInGridOrLines', 1)
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Manufacturer]') and NAME='ManufacturerTemplateId')
BEGIN
	ALTER TABLE [dbo].[Manufacturer] 
	ADD [ManufacturerTemplateId] int NULL
END
GO

UPDATE [dbo].[Manufacturer]
SET [ManufacturerTemplateId]=1
WHERE [ManufacturerTemplateId] is null
GO

ALTER TABLE [dbo].[Manufacturer] ALTER COLUMN [ManufacturerTemplateId] int NOT NULL
GO


--new 'RewardPointsWereAdded' or 'Order' entity
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Order]') and NAME='RewardPointsWereAdded')
BEGIN
	ALTER TABLE [dbo].[Order] 
	ADD [RewardPointsWereAdded] bit NULL
END
GO

UPDATE [dbo].[Order]
SET [RewardPointsWereAdded]=0
WHERE [RewardPointsWereAdded] is null
GO

ALTER TABLE [dbo].[Order] ALTER COLUMN [RewardPointsWereAdded] bit NOT NULL
GO



--Products can require that other products are added to the cart
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[ProductVariant]') and NAME='RequireOtherProducts')
BEGIN
	ALTER TABLE [dbo].[ProductVariant] 
	ADD [RequireOtherProducts] bit NULL
END
GO

UPDATE [dbo].[ProductVariant]
SET [RequireOtherProducts]=0
WHERE [RequireOtherProducts] is null
GO

ALTER TABLE [dbo].[ProductVariant] ALTER COLUMN [RequireOtherProducts] bit NOT NULL
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[ProductVariant]') and NAME='RequiredProductVariantIds')
BEGIN
	ALTER TABLE [dbo].[ProductVariant] 
	ADD [RequiredProductVariantIds] nvarchar(1000) NULL
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[ProductVariant]') and NAME='AutomaticallyAddRequiredProductVariants')
BEGIN
	ALTER TABLE [dbo].[ProductVariant] 
	ADD [AutomaticallyAddRequiredProductVariants] bit NULL
END
GO

UPDATE [dbo].[ProductVariant]
SET [AutomaticallyAddRequiredProductVariants]=0
WHERE [AutomaticallyAddRequiredProductVariants] is null
GO

ALTER TABLE [dbo].[ProductVariant] ALTER COLUMN [AutomaticallyAddRequiredProductVariants] bit NOT NULL
GO