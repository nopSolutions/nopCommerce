--upgrade scripts from nopCommerce 3.70 to next version

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.Forums.NotifyAboutPrivateMessages.Hint">
    <Value>Indicates whether a customer should be notified by email about new private messages.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing.Hint">
    <Value>Check to allow customers to edit items already placed in the cart or wishlist. It could be useful when your products have attributes or any other fields entered by a customer.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.AddToWishlist.Update">
    <Value>Update</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Picture.Hint">
    <Value>Choose a picture associated to this attribute value. This picture will replace the main product image when this product attribute value is clicked (selected)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ImageSquaresPicture">
    <Value>Square picture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.ImageSquaresPicture.Hint">
    <Value>Upload a picture to be used with the image squares attribute control</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewTopic">
    <Value>Added a new topic (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditTopic">
    <Value>Edited a topic (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteTopic">
    <Value>Deleted a topic (''{0}'')</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteOrder">
    <Value>Deleted an order (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditOrder">
    <Value>Edited an order (ID = {0}). See order notes for details</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasAllProducts.Fields.Products">
    <Value>Restricted products [and quantity range]</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.DiscountRules.HasOneProduct.Fields.Products">
    <Value>Restricted products [and quantity range]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PageSize">
    <Value>Page size</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.PageSize.Hint">
    <Value>Page size is for history of reward points on my account page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SaveBeforeEdit">
    <Value>You need to save the language before you can make or change resources for this language.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources">
    <Value>String resources</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Localization">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Select">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Fields.LanguageName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.View">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore">
    <Value>Reviews per store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ShowProductReviewsPerStore.Hint">
    <Value>Enable show reviews per store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page Size options should have unique items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page Size options should have unique items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems">
    <Value>Page Size options should not have duplicate items.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Productreviews.Fields.Store">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.Fields.Store.Hint">
	<Value>A store name in which this review was written.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchStore">
	<Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchStore.Hint">
	<Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions">
    <Value>Sort options</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SortOptions.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion">
    <Value>reCAPTCHA version</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.reCaptchaVersion.Hint">
    <Value>Select version of the reCAPTCHA.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.WrongCaptchaV2">
    <Value>The reCAPTCHA response is invalid or malformed. Please try again.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Web.Framework.Security.Captcha.ReCaptchaVersion.Version1">
    <Value>Version 1.0</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Web.Framework.Security.Captcha.ReCaptchaVersion.Version2">
    <Value>Version 2.0</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchProduct">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.ProductReviews.List.SearchProduct.Hint">
    <Value>Search by a specific product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Imported">
    <Value>Products have been imported successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.FlagImageFileName.Hint">
    <Value>The flag image file name. The image should be saved into \images\flags\ directory.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.DontSendBeforeDate">
    <Value>Planned date of sending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.DontSendBeforeDate.Hint">
    <Value>The specific send date and time.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.SendImmediately">
    <Value>Send immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.SendImmediately.Hint">
    <Value>Send message immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.DontSendBeforeDate">
    <Value>Planned date of sending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Campaigns.Fields.DontSendBeforeDate.Hint">
    <Value>Enter a specific date and time to send the campaign. Leave empty to send it immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.DontSendBeforeDate">
    <Value>Planned date of sending</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.DontSendBeforeDate.Hint">
    <Value>The specific send date and time.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.SendImmediately">
    <Value>Send immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.SendEmail.SendImmediately.Hint">
    <Value>Send message immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Category.List.ImportFromExcelTip">
    <Value>Imported categories are distinguished by ID. If the ID already exists, then its corresponding category will be updated. For new categories ID do not need to specify</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturer.List.ImportFromExcelTip">
    <Value>Imported manufacturers are distinguished by ID. If the ID already exists, then its corresponding manufacturer will be updated. For new manufacturers ID do not need to specify</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Category.Imported">
    <Value>Categories have been imported successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturer.Imported">
    <Value>Manufacturers have been imported successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.SearchSettingValue">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.SearchSettingValue.Hint">
    <Value>Search by a specific setting value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceName">
    <Value>Resource name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceName.Hint">
    <Value>Search by a specific resource.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceValue">
    <Value>Value</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.SearchResourceValue.Hint">
    <Value>Search by a specific resource value.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.AllSettings.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Resources.Description">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend">
    <Value>Delay send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend.Hint">
    <Value>The delay before sending message.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.SendImmediately">
    <Value>Send immediately</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Fields.SendImmediately.Hint">
    <Value>Send message immediately.</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.MessageDelayPeriod.Days">
    <Value>Days</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.MessageDelayPeriod.Hours">
    <Value>Hours</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowSearchByVendor">
    <Value>Allow search by vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowSearchByVendor.Hint">
    <Value>Check to allow customers to search by vendor on advanced search page.</Value>
  </LocaleResource>
  <LocaleResource Name="Search.Vendor">
    <Value>Vendor</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Published">
    <Value>Published</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Published.Hint">
    <Value>Determines whether this topic is published (visible) in your store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchStore">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.List.SearchStore.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ImportToExcel.ManyRecordsWarning">
    <Value>Import requires a lot of memory resources. That''s why it''s not recommended to import more than 500 - 1,000 records at once. If you have more records, it''s better to split them to multiple Excel files and import separately.</Value>
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



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductAttributeValue]') and NAME='ImageSquaresPictureId')
BEGIN
	ALTER TABLE [ProductAttributeValue]
	ADD [ImageSquaresPictureId] int NULL
END
GO

UPDATE [ProductAttributeValue]
SET [ImageSquaresPictureId] = 0
WHERE [ImageSquaresPictureId] IS NULL
GO

ALTER TABLE [ProductAttributeValue] ALTER COLUMN [ImageSquaresPictureId] int NOT NULL
GO

--new column
 IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[ProductReview]') and NAME='StoreId')
 BEGIN
 	ALTER TABLE [dbo].[ProductReview] ADD
 	[StoreId] int NULL
 END
 GO

 DECLARE @DefaultStoreId INT
 SET @DefaultStoreId = (SELECT TOP (1) Id FROM [dbo].[Store]);
 UPDATE [dbo].[ProductReview] SET StoreId = @DefaultStoreId WHERE StoreId IS NULL
 GO
 
 ALTER TABLE [dbo].[ProductReview] ALTER COLUMN [StoreId] INT NOT NULL
 GO
 
 IF EXISTS (SELECT 1 FROM   sys.objects WHERE  
 			name = 'ProductReview_Store'
 			AND parent_object_id = Object_id('ProductReview')
 			AND Objectproperty(object_id,N'IsForeignKey') = 1)
 ALTER TABLE dbo.ProductReview
 DROP CONSTRAINT ProductReview_Store
 GO
 
 ALTER TABLE [dbo].[ProductReview]  WITH CHECK ADD  CONSTRAINT [ProductReview_Store] FOREIGN KEY([StoreId])
 REFERENCES [dbo].[Store] ([Id])
 ON DELETE CASCADE
 GO
 
--new setting
 IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.showproductreviewsperstore')
 BEGIN
 	INSERT [Setting] ([Name], [Value], [StoreId])
 	VALUES (N'catalogsettings.showproductreviewsperstore', N'False', 0)
 END
 GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.imagesquarepicturesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'mediasettings.imagesquarepicturesize', N'32', 0)
END
GO

--new activity types
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'AddNewTopic')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewTopic', N'Add a new topic', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteTopic')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteTopic', N'Delete a topic', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditTopic')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditTopic', N'Edit a topic', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'DeleteOrder')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteOrder', N'Delete an order', N'true')
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'EditOrder')
BEGIN
	INSERT [ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditOrder', N'Edit an order', N'true')
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.pagesize')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'rewardpointssettings.pagesize', N'10', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsortingenumdisabled')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'catalogsettings.productsortingenumdisabled',N'',0);
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'catalogsettings.productsortingenumdisplayorder')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'catalogsettings.productsortingenumdisplayorder',N'',0);
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchaversion')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'captchasettings.recaptchaversion',N'1',0);
END
GO

--new or update setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchatheme')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'captchasettings.recaptchatheme',N'',0);
END
ELSE
BEGIN
	UPDATE [Setting] 
	SET [Value] = N'' 
	WHERE [Name] = N'captchasettings.recaptchatheme'
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'captchasettings.recaptchalanguage')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'captchasettings.recaptchalanguage',N'',0);
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='DontSendBeforeDateUtc')
BEGIN
	ALTER TABLE [QueuedEmail]
	ADD [DontSendBeforeDateUtc] DATETIME NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Campaign]') and NAME='DontSendBeforeDateUtc')
BEGIN
	ALTER TABLE [Campaign]
	ADD [DontSendBeforeDateUtc] DATETIME NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') and NAME='DelayBeforeSend')
BEGIN
	ALTER TABLE [MessageTemplate]
	ADD [DelayBeforeSend] INT NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[MessageTemplate]') and NAME='DelayPeriodId')
BEGIN
	ALTER TABLE [MessageTemplate]
	ADD [DelayPeriodId] INT NULL
END
GO

UPDATE [MessageTemplate]
SET [DelayPeriodId] = 0
WHERE [DelayPeriodId] IS NULL
GO

ALTER TABLE [MessageTemplate] ALTER COLUMN [DelayPeriodId] int NOT NULL
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowsearchbyvendor')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId]) 
	VALUES (N'vendorsettings.allowsearchbyvendor',N'False',0);
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='Published')
BEGIN
	ALTER TABLE [Topic]
	ADD [Published] bit NULL
END
GO

UPDATE [Topic]
SET [Published] = 1
WHERE [Published] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [Published] bit NOT NULL
GO