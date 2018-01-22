--upgrade scripts from nopCommerce 4.00 to 4.10

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Currencies.Fields.CurrencyCode.Hint">
    <Value>The currency code. For a list of currency codes, go to: https://en.wikipedia.org/wiki/ISO_4217</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Avatar">
    <Value>Avatar</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages">
    <Value>Export/Import products. Allow download images</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportAllowDownloadImages.Hint">
    <Value>Check if images can be downloaded from remote server when exporting products</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.ContentManagement.Topics.List.SearchKeywords">
    <Value>Search keywords</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.List.SearchKeywords.Hint">
    <Value>Search topic(s) by specific keywords.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore.Hint">
    <Value>Check to display reviews written in the current store only (on a product details page and on the account product reviews page).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ViewMode.Grid">
    <Value>Grid</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ViewMode.List">
    <Value>List</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DefaultViewMode">
    <Value>Default view mode</Value>
  </LocaleResource>    
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.DefaultViewMode.Hint">
    <Value>Choose the default view mode for catalog pages.</Value>
  </LocaleResource>     
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchEndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchEndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchStartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.List.SearchStartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.AdminApproval">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.Disabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.EmailValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Security.UserRegistrationType.Standard">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.AdminApproval">
    <Value>A customer should be approved by administrator</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.Disabled">
    <Value>Registration is disabled</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.EmailValidation">
    <Value>Email validation is required after registration</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Customers.UserRegistrationType.Standard">
    <Value>Standard account creation</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.Sku.Reserved">
    <Value>The entered SKU is already reserved for the product ''{0}''</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved">
    <Value>The entered SKU is already reserved for one of combinations of the product ''{0}''</Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.Hint">
    <Value>Search by a "Published" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.All">
    <Value>All</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.PublishedOnly">
    <Value>Published only</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.LowStockReport.SearchPublished.UnpublishedOnly">
    <Value>Unpublished only</Value>
  </LocaleResource>
  <LocaleResource Name="Products.Availability.SelectRequiredAttributes">
    <Value>Please select required attribute(s)</Value>
  </LocaleResource>
  <LocaleResource Name="PDFInvoice.VendorName">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Order.Product(s).VendorName">
    <Value>Vendor name</Value>
  </LocaleResource> 
  <LocaleResource Name="ShoppingCart.VendorName">
    <Value>Vendor name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.ShowVendorOnOrderDetailsPage">
    <Value>Show vendor name on order details page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.ShowVendorOnOrderDetailsPage.Hint">
    <Value>Check to show vendor name of product on the order details page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Edit">
    <Value>Edit combination</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture">
    <Value>Picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture.Hint">
    <Value>Choose a picture associated to this attribute combination. This picture will replace the main product image when this product attribute combination is selected.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Picture.NoPicture">
    <Value>No picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountUrl">
    <Value>URL with coupon code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.DiscountUrl.Hint">
    <Value>The sample link that includes a discount coupon code, so that customers do not have to input the coupon code at checkout. You can also use this query parameter with any other link to your store, for example link to certain product or category.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NotifyCustomerAboutProductReviewReply">
    <Value>Notify customer about product review reply</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.NotifyCustomerAboutProductReviewReply.Hint">
    <Value>Check to notify customer about product review reply.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.ProductReview.Reply.CustomerNotification">
    <Value><![CDATA[This message template is used to notify customers when a store owner (or vendor) replies to their product reviews. You can set up this option by ticking the checkbox <strong>Notify customer about product review reply</strong> in Configuration - Settings - Catalog settings.]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPaid.AffiliateNotification">
	  <Value>This message template is used to notify an affiliate that the certain order was paid. The order gets the status Paid when the amount was charged.</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.OrderPlaced.AffiliateNotification">
	  <Value>This message template is used to notify an affiliate that the certain order was placed.</Value>
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

--new index
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_GetLowStockProducts' and object_id=object_id(N'[dbo].[Product]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_GetLowStockProducts] ON [Product] (Deleted ASC, VendorId ASC, ProductTypeId ASC, ManageInventoryMethodId ASC, MinStockQuantity ASC, UseMultipleWarehouses ASC)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.exportimportallowdownloadimages')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.exportimportallowdownloadimages', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ActivityLog]') AND NAME = 'EntityId')
BEGIN
	ALTER TABLE [ActivityLog]
	ADD [EntityId] INT NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ActivityLog]') AND NAME = 'EntityName')
BEGIN
	ALTER TABLE [ActivityLog]
	ADD [EntityName] NVARCHAR(400) NULL
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.showvendoronorderdetailspage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.showvendoronorderdetailspage', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'addresssettings.preselectcountryifonlyone')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'addresssettings.preselectcountryifonlyone', N'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ProductAttributeCombination]') AND NAME = 'PictureId')
BEGIN
	ALTER TABLE [ProductAttributeCombination]
	ADD [PictureId] INT NULL
END
GO

UPDATE [ProductAttributeCombination]
SET [PictureId] = 0
WHERE [PictureId] IS NULL

ALTER TABLE [ProductAttributeCombination] ALTER COLUMN [PictureId] INT NOT NULL
GO

-- new message template
 IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'ProductReview.Reply.CustomerNotification')
 BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
	INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
	VALUES (N'ProductReview.Reply.CustomerNotification', NULL, N'%Store.Name%. Product review reply.', N'<p>' + @NewLine + '<a href="%Store.URL%">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Hello %Customer.FullName%,' + @NewLine + '<br />' + @NewLine + 'You received a reply from the store administration to your review for product "%ProductReview.ProductName%".' + @NewLine + '</p>' + @NewLine, 0, 0, 0, 0, 0)
 END
 GO
 
--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[ProductReview]') AND NAME = 'CustomerNotifiedOfReply')
BEGIN
	ALTER TABLE [ProductReview]
	ADD [CustomerNotifiedOfReply] BIT NULL
END
GO

UPDATE [ProductReview]
SET [CustomerNotifiedOfReply] = 0
WHERE [CustomerNotifiedOfReply] IS NULL

ALTER TABLE [ProductReview] ALTER COLUMN [CustomerNotifiedOfReply] BIT NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.notifycustomeraboutproductreviewreply')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.notifycustomeraboutproductreviewreply', N'false', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.uselinksinrequiredproductwarnings')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'catalogsettings.uselinksinrequiredproductwarnings', N'true', 0)
END
GO

-- new message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'OrderPlaced.AffiliateNotification')
BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
    INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
    VALUES (N'OrderPlaced.AffiliateNotification', NULL, N'%Store.Name%. Order placed', N'<p>' + @NewLine + '<a href=\"%Store.URL%\">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Customer.FullName% (%Customer.Email%) has just placed an order.' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Order Number: %Order.OrderNumber%' + @NewLine + '<br />' + @NewLine + 'Date Ordered: %Order.CreatedOn%' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Order.Product(s)%' + @NewLine + '</p>' + @NewLine, 0, 0, 0, 0, 0)
END
GO

-- new message template
IF NOT EXISTS (SELECT 1 FROM [dbo].[MessageTemplate] WHERE [Name] = N'OrderPaid.AffiliateNotification')
BEGIN
    DECLARE @NewLine AS CHAR(2) = CHAR(13) + CHAR(10)
    INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores], [DelayPeriodId]) 
    VALUES (N'OrderPaid.AffiliateNotification', NULL, N'%Store.Name%. Order #%Order.OrderNumber% paid', N'<p>' + @NewLine + '<a href=\"%Store.URL%\">%Store.Name%</a>' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Order #%Order.OrderNumber% has been just paid.' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + 'Order Number: %Order.OrderNumber%' + @NewLine + '<br />' + @NewLine + 'Date Ordered: %Order.CreatedOn%' + @NewLine + '<br />' + @NewLine + '<br />' + @NewLine + '%Order.Product(s)%' + @NewLine + '</p>' + @NewLine, 0, 0, 0, 0, 0)
END
GO
