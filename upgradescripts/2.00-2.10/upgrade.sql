--upgrade scripts from nopCommerce 2.00 to nopCommerce 2.10

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.ShowOnHomePage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.ShowOnHomePage.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeadHtmlTag">
    <Value>Head HTML tag</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterBodyStartHtmlTag">
    <Value><![CDATA[After <body> start HTML tag]]></Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeaderLinks">
    <Value>Header links</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeaderSelectors">
    <Value>Header selectors</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.HeaderMenu">
    <Value>Header menu</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeContent">
    <Value>Before content</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeLeftSideColumn">
    <Value>Before left side column</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterLeftSideColumn">
    <Value>After left side column</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeMainColumn">
    <Value>Before main column</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterMainColumn">
    <Value>After main column</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeRightSideColumn">
    <Value>Before right side column</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterRightSideColumn">
    <Value>After right side column</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.AfterContent">
    <Value>After content</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.Footer">
    <Value>Footer</Value>
  </LocaleResource>
  <LocaleResource Name="Enums.Nop.Core.Domain.Cms.WidgetZone.BeforeBodyEndHtmlTag">
    <Value><![CDATA[After <body> end HTML tag]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets">
    <Value>Widgets</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.BackToList">
    <Value>back to widget list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.EditWidgetDetails">
    <Value>Edit widget details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AddNew">
    <Value>Add a new widget</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.LivePersonChat.ButtonCode">
    <Value>Button code(max 2000)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.LivePersonChat.ButtonCode.Hint">
    <Value>Enter your button code here.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.LivePersonChat.MonitoringCode">
    <Value>Monitoring code(max 2000)</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.LivePersonChat.MonitoringCode.Hint">
    <Value>Enter your monitoring code here.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.LiveChat.LivePerson.ButtonCode">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.LiveChat.LivePerson.ButtonCode.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.LiveChat.LivePerson.MonitoringCode">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.LiveChat.LivePerson.MonitoringCode.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AvailableWidgetPlugins">
    <Value>Available Widget Plugins</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AvailableWidgetPlugins.FriendlyName">
    <Value>Plugin</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.AvailableWidgetPlugins.AddToZone">
    <Value>Add to zone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets">
    <Value>Widgets</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets.WidgetZone">
    <Value>Widget zone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets.PluginFriendlyName">
    <Value>Widget name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Widgets.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.WidgetZone">
    <Value>Widget zone</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.WidgetZone.Hint">
    <Value>Select widget zone.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.PluginFriendlyName">
    <Value>Widget name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.PluginFriendlyName.Hint">
    <Value>Used widget name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Fields.DisplayOrder.Hint">
    <Value>The widget display order. 1 represents the first item in the list.</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.AddNewWidget">
    <Value>Added a new widget (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.EditWidget">
    <Value>Edited a widget (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="ActivityLog.DeleteWidget">
    <Value>Deleted a widget (ID = {0})</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Added">
    <Value>The new widget has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Deleted">
    <Value>The widget has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Widgets.Updated">
    <Value>The widget has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.BackToList">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.Configure">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.Fields">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.Fields.FriendlyName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.Fields.SystemName">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.Fields.DisplayOrder">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.LiveChats.Fields.IsActive">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="LiveChats">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.LivePersonChat.LiveChat">
    <Value>Live chat</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalytics">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalytics.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsEnabled">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsEnabled.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsId.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsJavaScript">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsJavaScript.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsPlacement">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsPlacement.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsPlacement.BeforeBody">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.GoogleAnalyticsPlacement.BeforeHead">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.GoogleId">
    <Value>ID</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.GoogleId.Hint">
    <Value>Enter Google Analytics ID.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.JavaScript">
    <Value>Tracking code</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Widgets.GoogleAnalytics.JavaScript.Hint">
    <Value>Paste the tracking code generated by Google Analytics here. This tracking code will be used to track when a new visitor arrives. You can then login to Google Analytics to view the customer''s details including which site they came from and conversion details.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationSettings">
    <Value>External authentication settings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationAutoRegisterEnabled">
    <Value>Auto register enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.ExternalAuthenticationAutoRegisterEnabled.Hint">
    <Value>Check to enable auto registration when using external authentication.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods">
    <Value>External authentication methods</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.BackToList">
    <Value>back to authentication method list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.Configure">
    <Value>Configure</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.Fields.FriendlyName">
    <Value>Friendly name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.Fields.SystemName">
    <Value>System name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.Fields.DisplayOrder">
    <Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.ExternalAuthenticationMethods.Fields.IsActive">
    <Value>Is active</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Twitter.ConsumerKey">
    <Value>Consumer key</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Twitter.ConsumerKey.Hint">
    <Value>Enter your consumer key here.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Twitter.ConsumerSecret">
    <Value>Consumer secret</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Twitter.ConsumerSecret.Hint">
    <Value>Enter your consumer key here.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientKeyIdentifier">
    <Value>Client key identifier</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientKeyIdentifier.Hint">
    <Value>Enter your client key identifier here.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientSecret">
    <Value>Client secret</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.ClientSecret.Hint">
    <Value>Enter your client key here.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AssociatedExternalAuth">
    <Value>External authentication</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AssociatedExternalAuth.Fields.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AssociatedExternalAuth.Fields.ExternalIdentifier">
    <Value>External identifier</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.AssociatedExternalAuth.Fields.AuthMethodName">
    <Value>Authentication method</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth">
    <Value>External authentication</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.Email">
    <Value>Email</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.ExternalIdentifier">
    <Value>External identifier</Value>
  </LocaleResource>
  <LocaleResource Name="Account.AssociatedExternalAuth.AuthMethodName">
    <Value>Authentication method</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.GiftCardCouponCode.Applied">
    <Value>The gift card code was applied</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Applied">
    <Value>The coupon code was applied</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled">
    <Value>SEO friendly URLs with multiple languages enabled</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled.Hint">
    <Value>When enabled, your URLs will be http://www.yourStore.com/en/ or http://www.yourStore.com/ru/ (SEO friendly)</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.UniqueSeoCode">
    <Value>Unique SEO code</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.UniqueSeoCode.Hint">
    <Value>The unique two letter SEO code. It''s used to generate URLs like ''http://www.yourStore.com/en/'' when you have more than one published language. ''SEO friendly URLs with multiple languages'' option should also be enabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.UniqueSeoCode.Length">
    <Value>Two letter SEO code should be 2 characters long.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Languages.Fields.UniqueSeoCode.Required">
    <Value>Please provide an unique SEO code.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.DefaultGoogleCategory">
    <Value>Default Google category</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.DefaultGoogleCategory.Hint">
    <Value>The default Google category will be useds if other one is not specified.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.General">
    <Value>General</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Override">
    <Value>Override product settings</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.ProductName">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Feed.Froogle.Products.GoogleCategory">
    <Value>Google Category</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Day">
    <Value>Day</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Month">
    <Value>Month</Value>
  </LocaleResource>
  <LocaleResource Name="Common.Year">
    <Value>Year</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisableWishlistButton">
    <Value>Disable wishlist button</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.DisableWishlistButton.Hint">
    <Value>Check to disable the wishlist button for this product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTemplate">
    <Value>Product template</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ProductTemplate.Hint">
    <Value>Choose a product template. This template defines how this product (and it''s variants) will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Variants.Fields.Download.Hint">
    <Value>The download file.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem">
    <Value>System customer roles can''t be disabled.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem">
    <Value>The system name of system customer roles can''t be edited.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.CustomerAddresses.NoAddresses">
    <Value>No addresses</Value>
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


--insert your update SQL scripts here





--missed 'Manage Countries' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageCountries')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Manage Countries', N'ManageCountries', N'Configuration')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admni role be default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO









--Widgets
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Widget]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Widget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WidgetZoneId] int NOT NULL,
	[PluginSystemName] [nvarchar](max) NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO

--'Manage Widgets' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageWidgets')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Manage Widgets', N'ManageWidgets', N'Configuration')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admni role be default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO

--Widgets activity log records
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ActivityLogType]
		WHERE [SystemKeyword] = N'AddNewWidget')
BEGIN
	INSERT [dbo].[ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'AddNewWidget', N'Add a new widget', 1)
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ActivityLogType]
		WHERE [SystemKeyword] = N'EditWidget')
BEGIN
	INSERT [dbo].[ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'EditWidget', N'Edit a widget', 1)
END
GO
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ActivityLogType]
		WHERE [SystemKeyword] = N'DeleteWidget')
BEGIN
	INSERT [dbo].[ActivityLogType] ([SystemKeyword], [Name], [Enabled])
	VALUES (N'DeleteWidget', N'Delete a widget', 1)
END
GO


-- ExternalAuthenticationRecord (OpenId, OAuth, etc)
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExternalAuthenticationRecord]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ExternalAuthenticationRecord](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] int NOT NULL,
	[ExternalIdentifier] [nvarchar](max) NULL,
	[ExternalDisplayIdentifier] [nvarchar](max) NULL,
	[OAuthToken] [nvarchar](max) NULL,
	[OAuthAccessToken] [nvarchar](max) NULL,
	[ProviderSystemName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO


IF EXISTS (SELECT 1
           FROM   sys.objects
           WHERE  name = 'ExternalAuthenticationRecord_Customer'
           AND parent_object_id = Object_id('ExternalAuthenticationRecord')
           AND Objectproperty(object_id,N'IsForeignKey') = 1)
ALTER TABLE dbo.ExternalAuthenticationRecord
DROP CONSTRAINT ExternalAuthenticationRecord_Customer
GO
ALTER TABLE [dbo].[ExternalAuthenticationRecord]  WITH CHECK ADD  CONSTRAINT [ExternalAuthenticationRecord_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO

--'Manage External Authentication Methods' permission record
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageExternalAuthenticationMethods')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Manage External Authentication Methods', N'ManageExternalAuthenticationMethods', N'Configuration')

	DECLARE @PermissionRecordId INT 
	SET @PermissionRecordId = @@IDENTITY


	--add it to admni role be default
	DECLARE @AdminCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'

	INSERT [dbo].[PermissionRecord_Role_Mapping] ([PermissionRecord_Id], [CustomerRole_Id])
	VALUES (@PermissionRecordId, @AdminCustomerRoleId)
END
GO

--[Value] column of [CustomerAttribute] can have up to 4000 chars
ALTER TABLE [dbo].[CustomerAttribute] ALTER COLUMN [Value] nvarchar(4000) NOT NULL
GO

--Add one more column to [ExternalAuthenticationRecord] table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[ExternalAuthenticationRecord]') and NAME='Email')
BEGIN
	ALTER TABLE [dbo].[ExternalAuthenticationRecord] 
	ADD [Email] nvarchar(MAX) NULL
END
GO

DELETE FROM [Setting]
WHERE [NAME]=N'storeinformationsettings.currentversion'
GO


--'returnrequestreasons' and 'returnrequestactions' should be reverted
UPDATE [Setting]
SET [Value] = N'Received Wrong Product,Wrong Product Ordered,There Was A Problem With The Product'
WHERE [name] = N'ordersettings.returnrequestreasons'
GO

UPDATE [Setting]
SET [Value] = N'Repair,Replacement,Store Credit'
WHERE [name] = N'ordersettings.returnrequestactions'
GO

--SEO friendly language URLs
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Language]') and NAME='UniqueSeoCode')
BEGIN
	ALTER TABLE [dbo].[Language] 
	ADD [UniqueSeoCode] [nvarchar](2) NULL
END
GO
--set defaults
UPDATE [dbo].[Language]
SET [UniqueSeoCode] = SUBSTRING ([LanguageCulture], 1, 2)
GO


--performance optimization (indexes)
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_LocaleStringResource' and object_id=object_id(N'[dbo].[LocaleStringResource]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [dbo].[LocaleStringResource] 
	(
	  [ResourceName] ASC,
	  [LanguageId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductVariant_ProductId' and object_id=object_id(N'[dbo].[ProductVariant]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId]
	ON [dbo].[ProductVariant] ([ProductId])
	INCLUDE ([Price],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc],[Published],[Deleted])
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Country_DisplayOrder' and object_id=object_id(N'[dbo].[Country]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] 
	ON [dbo].[Country] 
	(
		[DisplayOrder] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_StateProvince_CountryId' and object_id=object_id(N'[dbo].[StateProvince]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [dbo].[StateProvince] ([CountryId])
	INCLUDE ([DisplayOrder])
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Currency_DisplayOrder' and object_id=object_id(N'[dbo].[Currency]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Currency_DisplayOrder] ON [dbo].[Currency] 
	(
		[DisplayOrder] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Log_CreatedOnUtc' and object_id=object_id(N'[dbo].[Log]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [dbo].[Log] 
	(
		[CreatedOnUtc] ASC
	)
END
GO

--[Email] column of [Customer] can have up to 1000 chars
ALTER TABLE [dbo].[Customer] ALTER COLUMN [Email] nvarchar(1000) NULL
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Customer_Email' and object_id=object_id(N'[dbo].[Customer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [dbo].[Customer] 
	(
		[Email] ASC
	)
END
GO

--[Username] column of [Customer] can have up to 1000 chars
ALTER TABLE [dbo].[Customer] ALTER COLUMN [Username] nvarchar(1000) NULL
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Customer_Username' and object_id=object_id(N'[dbo].[Customer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [dbo].[Customer] 
	(
		[Username] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Customer_CustomerGuid' and object_id=object_id(N'[dbo].[Customer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [dbo].[Customer] 
	(
		[CustomerGuid] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_QueuedEmail_CreatedOnUtc' and object_id=object_id(N'[dbo].[QueuedEmail]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [dbo].[QueuedEmail] 
	(
		[CreatedOnUtc] ASC
	)
END
GO


IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Order_CustomerId' and object_id=object_id(N'[dbo].[Order]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Order_CustomerId] ON [dbo].[Order] 
	(
		[CustomerId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Language_DisplayOrder' and object_id=object_id(N'[dbo].[Language]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [dbo].[Language] 
	(
		[DisplayOrder] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_CustomerAttribute_CustomerId' and object_id=object_id(N'[dbo].[CustomerAttribute]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_CustomerAttribute_CustomerId] ON [dbo].[CustomerAttribute] 
	(
		[CustomerId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_BlogPost_LanguageId' and object_id=object_id(N'[dbo].[BlogPost]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [dbo].[BlogPost] 
	(
		[LanguageId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_BlogComment_BlogPostId' and object_id=object_id(N'[dbo].[BlogComment]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [dbo].[BlogComment] 
	(
		[BlogPostId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_News_LanguageId' and object_id=object_id(N'[dbo].[News]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_News_LanguageId] ON [dbo].[News] 
	(
		[LanguageId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_NewsComment_NewsItemId' and object_id=object_id(N'[dbo].[NewsComment]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_NewsComment_NewsItemId] ON [dbo].[NewsComment] 
	(
		[NewsItemId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_PollAnswer_PollId' and object_id=object_id(N'[dbo].[PollAnswer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [dbo].[PollAnswer] 
	(
		[PollId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductReview_ProductId' and object_id=object_id(N'[dbo].[ProductReview]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductId] ON [dbo].[ProductReview] 
	(
		[ProductId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_OrderProductVariant_OrderId' and object_id=object_id(N'[dbo].[OrderProductVariant]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_OrderProductVariant_OrderId] ON [dbo].[OrderProductVariant] 
	(
		[OrderId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_OrderNote_OrderId' and object_id=object_id(N'[dbo].[OrderNote]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_OrderNote_OrderId] ON [dbo].[OrderNote] 
	(
		[OrderId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_TierPrice_ProductVariantId' and object_id=object_id(N'[dbo].[TierPrice]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_TierPrice_ProductVariantId] ON [dbo].[TierPrice] 
	(
		[ProductVariantId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId' and object_id=object_id(N'[dbo].[ShoppingCartItem]'))
BEGIN
CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId] ON [dbo].[ShoppingCartItem] 
(
	[ShoppingCartTypeId] ASC,
	[CustomerId] ASC
)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_RelatedProduct_ProductId1' and object_id=object_id(N'[dbo].[RelatedProduct]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_RelatedProduct_ProductId1] ON [dbo].[RelatedProduct] 
	(
		[ProductId1] ASC
	)
END
GO


IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductVariant_DisplayOrder' and object_id=object_id(N'[dbo].[ProductVariant]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductVariant_DisplayOrder] ON [dbo].[ProductVariant] 
	(
		[DisplayOrder] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductVariantAttributeValue_ProductVariantAttributeId' and object_id=object_id(N'[dbo].[ProductVariantAttributeValue]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductVariantAttributeValue_ProductVariantAttributeId] ON [dbo].[ProductVariantAttributeValue] 
	(
		[ProductVariantAttributeId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_ProductVariant_ProductAttribute_Mapping_ProductVariantId' and object_id=object_id(N'[dbo].[ProductVariant_ProductAttribute_Mapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductAttribute_Mapping_ProductVariantId] ON [dbo].[ProductVariant_ProductAttribute_Mapping] 
	(
		[ProductVariantId] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Manufacturer_DisplayOrder' and object_id=object_id(N'[dbo].[Manufacturer]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Manufacturer_DisplayOrder] ON [dbo].[Manufacturer] 
	(
		[DisplayOrder] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Category_DisplayOrder' and object_id=object_id(N'[dbo].[Category]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [dbo].[Category] 
	(
		[DisplayOrder] ASC
	)
END
GO

IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Category_ParentCategoryId' and object_id=object_id(N'[dbo].[Category]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [dbo].[Category] 
	(
		[ParentCategoryId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Group_DisplayOrder' and object_id=object_id(N'[dbo].[Forums_Group]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [dbo].[Forums_Group] 
	(
		[DisplayOrder] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Forum_DisplayOrder' and object_id=object_id(N'[dbo].[Forums_Forum]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [dbo].[Forums_Forum] 
	(
		[DisplayOrder] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Forum_ForumGroupId' and object_id=object_id(N'[dbo].[Forums_Forum]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [dbo].[Forums_Forum] 
	(
		[ForumGroupId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Topic_ForumId' and object_id=object_id(N'[dbo].[Forums_Topic]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [dbo].[Forums_Topic] 
	(
		[ForumId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Post_TopicId' and object_id=object_id(N'[dbo].[Forums_Post]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [dbo].[Forums_Post] 
	(
		[TopicId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Post_CustomerId' and object_id=object_id(N'[dbo].[Forums_Post]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Post_CustomerId] ON [dbo].[Forums_Post] 
	(
		[CustomerId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Subscription_ForumId' and object_id=object_id(N'[dbo].[Forums_Subscription]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [dbo].[Forums_Subscription] 
	(
		[ForumId] ASC
	)
END
GO
IF NOT EXISTS (SELECT 1 from sys.indexes WHERE [NAME]=N'IX_Forums_Subscription_TopicId' and object_id=object_id(N'[dbo].[Forums_Subscription]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [dbo].[Forums_Subscription] 
	(
		[TopicId] ASC
	)
END
GO

--Allow store owner to disable "Add to wishlist" button for a certain product variant
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[ProductVariant]') and NAME='DisableWishlistButton')
BEGIN
	ALTER TABLE [dbo].[ProductVariant] 
	ADD [DisableWishlistButton] [bit] NULL

	EXEC ('UPDATE [dbo].[ProductVariant] SET [DisableWishlistButton] = [DisableBuyButton]')

	ALTER TABLE [dbo].[ProductVariant] ALTER COLUMN [DisableWishlistButton] [bit] NOT NULL
END
GO



--Product templates
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductTemplate]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[ProductTemplate](
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
		FROM [dbo].[ProductTemplate]
		WHERE [Name] = N'Variants in Grid')
BEGIN
	INSERT [dbo].[ProductTemplate] ([Name], [ViewPath], [DisplayOrder])
	VALUES (N'Variants in Grid', N'ProductTemplate.VariantsInGrid', 1)
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[ProductTemplate]
		WHERE [Name] = N'Single Product Variant')
BEGIN
	INSERT [dbo].[ProductTemplate] ([Name], [ViewPath], [DisplayOrder])
	VALUES (N'Single Product Variant', N'ProductTemplate.SingleVariant', 10)
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[dbo].[Product]') and NAME='ProductTemplateId')
BEGIN
	ALTER TABLE [dbo].[Product] 
	ADD [ProductTemplateId] int NULL
END
GO

UPDATE [dbo].[Product]
SET [ProductTemplateId]=1
WHERE [ProductTemplateId] is null
GO

ALTER TABLE [dbo].[Product] ALTER COLUMN [ProductTemplateId] int NOT NULL
GO

--rounding issue
UPDATE [Setting]
SET [Value] = N'true'
WHERE [name] = N'shoppingcartsettings.roundpricesduringcalculation'
GO
