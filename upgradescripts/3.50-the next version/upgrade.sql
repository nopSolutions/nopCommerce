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
  <LocaleResource Name="Admin.Catalog.Products.Fields.IsTelecommunicationsOrBroadcastingOrElectronicServices.Hint">
    <Value>Check if it''s telecommunications, broadcasting and electronic services. It''s used for tax calculation in Europe Union.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToContactVendors">
    <Value>Allow customers to contact vendors</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Vendor.AllowCustomersToContactVendors.Hint">
    <Value>Check to allow customers to contact vendors.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.ContactVendor">
    <Value>Contact Vendor - {0}</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor">
    <Value>Contact vendor</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Button">
    <Value>Submit</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Email">
    <Value>Your email</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Email.Hint">
    <Value>Enter your email address</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Email.Required">
    <Value>Enter email</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.EmailSubject">
    <Value>{0}. Contact us</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Enquiry">
    <Value>Enquiry</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Enquiry.Hint">
    <Value>Enter your enquiry</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.Enquiry.Required">
    <Value>Enter enquiry</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.FullName">
    <Value>Your name</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.FullName.Hint">
    <Value>Enter your name</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.FullName.Required">
    <Value>Enter your name</Value>
  </LocaleResource>
  <LocaleResource Name="ContactVendor.YourEnquiryHasBeenSent">
    <Value>Your enquiry has been successfully sent to the vendor.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic">
    <Value>Topic templates</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.Name">
    <Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.Name.Required">
    <Value>Name is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.ViewPath">
    <Value>View path</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.Templates.Topic.ViewPath.Required">
    <Value>View path is required</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.TopicTemplate">
    <Value>Topic template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.TopicTemplate.Hint">
    <Value>Choose a topic template. This template defines how this topic will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Address.AddressAttributes.Fields.IsRequired.Hint">
	<Value>When an attribute is required, the customer must choose an appropriate attribute value before they can continue.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test">
	<Value>Test template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.BackToTemplate">
	<Value>back to template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Send">
	<Value>Send</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.SendTo">
	<Value>Send email to</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.SendTo.Hint">
	<Value>Send test email to ensure that everything is properly configured.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Success">
	<Value>Email has been successfully queued.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Tokens">
	<Value>Tokens</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Tokens.Description">
	<Value>Please enter tokens you want to be replaced below</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Test.Tokens.Hint">
	<Value>Enter tokens.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.TestDetails">
	<Value>Send test email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.SearchPagePageSizeOptions">
    <Value>Search page. Page size options (comma separated)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToExcel.All">
    <Value>Export to Excel (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.ExportToXml.All">
    <Value>Export to XML (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.PrintPackagingSlip.All">
    <Value>Print packaging slips (all found)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Warehouse">
    <Value>Warehouse</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Shipping.ByWeight.Fields.Warehouse.Hint">
    <Value>If an asterisk is selected, then this shipping rate will apply to all warehouses.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.CustomGoods">
    <Value>Custom goods (no identifier exists)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.TopCategoryMenuSubcategoryLevelsToDisplay">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.TopCategoryMenuSubcategoryLevelsToDisplay.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.System.QueuedEmails.Fields.Priority.Range">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.QueuedEmailPriority.Low">
    <Value>Low</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Messages.QueuedEmailPriority.High">
    <Value>High</Value>
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


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'vendorsettings.allowcustomerstocontactvendors')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'vendorsettings.allowcustomerstocontactvendors', N'true', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'externalauthenticationsettings.requireemailvalidation')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'externalauthenticationsettings.requireemailvalidation', N'false', 0)
END
GO




--Topic templates
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TopicTemplate]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[TopicTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[ViewPath] [nvarchar](400) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[TopicTemplate]
		WHERE [Name] = N'Default template')
BEGIN
	INSERT [dbo].[TopicTemplate] ([Name], [ViewPath], [DisplayOrder])
	VALUES (N'Default template', N'TopicDetails', 1)
END
GO



--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Topic]') and NAME='TopicTemplateId')
BEGIN
	ALTER TABLE [Topic]
	ADD [TopicTemplateId] int NULL
END
GO

UPDATE [Topic]
SET [TopicTemplateId] = 1
WHERE [TopicTemplateId] IS NULL
GO

ALTER TABLE [Topic] ALTER COLUMN [TopicTemplateId] int NOT NULL
GO



--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'frooglesettings.expirationnumberofdays')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'frooglesettings.expirationnumberofdays', N'28', 0)
END
GO


--shipping by weight plugin
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[ShippingByWeight]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--new [StoreId] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[ShippingByWeight]'') and NAME=''WarehouseId'')
	BEGIN
		ALTER TABLE [ShippingByWeight]
		ADD [WarehouseId] int NULL

		exec(''UPDATE [ShippingByWeight] SET [WarehouseId] = 0'')
		
		EXEC (''ALTER TABLE [ShippingByWeight] ALTER COLUMN [WarehouseId] int NOT NULL'')
	END')
END
GO


--froogle plugin
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[GoogleProduct]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
	--new [StoreId] column
	EXEC ('IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id(''[GoogleProduct]'') and NAME=''CustomGoods'')
	BEGIN
		ALTER TABLE [GoogleProduct]
		ADD [CustomGoods] bit NULL

		exec(''UPDATE [GoogleProduct] SET [CustomGoods] = 0'')
		
		EXEC (''ALTER TABLE [GoogleProduct] ALTER COLUMN [CustomGoods] bit NOT NULL'')
	END')
END
GO

--delete setting
DELETE FROM [Setting] 
WHERE [name] = N'catalogsettings.topcategorymenusubcategorylevelstodisplay'
GO


--queued email priority
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[QueuedEmail]') and NAME='Priority')
BEGIN
	EXEC sp_rename 'QueuedEmail.Priority', 'PriorityId', 'COLUMN';
	
	EXEC ('UPDATE [QueuedEmail] SET [PriorityId] = 0 WHERE [PriorityId] <> 5')
END
GO