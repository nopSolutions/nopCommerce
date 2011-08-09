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
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Widget]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Widget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WidgetZoneId] int NOT NULL,
	[PluginSystemName] [nvarchar](max) NULL,
	[DisplayOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
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