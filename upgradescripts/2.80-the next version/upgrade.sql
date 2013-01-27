--upgrade scripts from nopCommerce 2.80 to the next version

--new locale resources
declare @resources xml
--a resource will be delete if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.StartDate.Hint">
    <Value>Set the blog post start date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.BlogPosts.Fields.EndDate.Hint">
    <Value>Set the blog post end date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.StartDate.Hint">
    <Value>Set the poll start date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Polls.Fields.EndDate.Hint">
    <Value>Set the poll end date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.StartDate.Hint">
    <Value>Set the news item start date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.NewsItems.Fields.EndDate.Hint">
    <Value>Set the news item end date in Coordinated Universal Time (UTC). You can also leave it empty.</Value>
  </LocaleResource>
  <LocaleResource Name="PageTitle.PageNotFound">
	<Value>Page not found</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Username.Required">
	<Value>Username is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.UsernameIsNotProvided">
	<Value>Username is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Account.Register.Errors.EmailIsNotProvided">
	<Value>Email is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MinimumRewardPointsToUse">
	<Value>Minimum reward points to use</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.RewardPoints.MinimumRewardPointsToUse.Hint">
	<Value>Customers won''t be able to use reward points before they have X amount of points. Set to 0 if you do not want to use this setting.</Value>
  </LocaleResource>
  <LocaleResource Name="RewardPoints.MinimumBalance">
	<Value>Minimum balance allowed to use is {0} reward points ({1}).</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.AddNew">
	<Value>Add a new store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.BackToList">
	<Value>back to store list</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.EditStoreDetails">
	<Value>Edit store details</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Name">
	<Value>Name</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Name.Hint">
	<Value>The name of the store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.Name.Required">
	<Value>Please provide a name.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DisplayOrder">
	<Value>Display order</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Fields.DisplayOrder.Hint">
	<Value>The display order for this store. 1 represents the top of the list.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Added">
	<Value>The new store has been added successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Updated">
	<Value>The store has been updated successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Stores.Deleted">
	<Value>The store has been deleted successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Stores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.LimitedToStores">
	<Value>Limited to stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.LimitedToStores.Hint">
	<Value>Determines whether the manufacturer is available only at certain stores.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AvailableStores">
	<Value>Stores</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Fields.AvailableStores.Hint">
	<Value>Select stores for which the manufacturer will be shown.</Value>
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

--add new "one word" URL to "reservedurlrecordslugs" setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs')
BEGIN
	DECLARE @NewUrlRecord nvarchar(4000)
	SET @NewUrlRecord = N'page-not-found'
	
	DECLARE @reservedurlrecordslugs nvarchar(4000)
	SELECT @reservedurlrecordslugs = [Value] FROM [Setting] WHERE [name] = N'seosettings.reservedurlrecordslugs'
	
	IF (CHARINDEX(@NewUrlRecord, @reservedurlrecordslugs) = 0)
	BEGIN
		UPDATE [Setting]
		SET [Value] = @reservedurlrecordslugs + ',' + @NewUrlRecord
		WHERE [name] = N'seosettings.reservedurlrecordslugs'
	END
END
GO


IF NOT EXISTS (
  SELECT 1
  FROM [dbo].[Topic]
  WHERE [SystemName] = N'PageNotFound')
BEGIN
	INSERT [dbo].[Topic] ([SystemName], [IncludeInSitemap], [IsPasswordProtected],  [Title], [Body])
	VALUES (N'PageNotFound', 0, 0, N'', N'<p><strong>The page you requested was not found, and we have a fine guess why.</strong>
        <ul>
            <li>If you typed the URL directly, please make sure the spelling is correct.</li>
            <li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li>
        </ul></p>')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'rewardpointssettings.minimumrewardpointstouse')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'rewardpointssettings.minimumrewardpointstouse', N'0')
END
GO


IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'mediasettings.multiplethumbdirectories')
BEGIN
	INSERT [Setting] ([Name], [Value])
	VALUES (N'mediasettings.multiplethumbdirectories', N'false')
END
GO



IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[Store]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Store](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(400) NOT NULL,
	[DisplayOrder] int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

--new permission
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[PermissionRecord]
		WHERE [SystemName] = N'ManageStores')
BEGIN
	INSERT [dbo].[PermissionRecord] ([Name], [SystemName], [Category])
	VALUES (N'Admin area. Manage Stores', N'ManageStores', N'Configuration')

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

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[StoreMapping]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[StoreMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityName] nvarchar(400) NOT NULL,
	[StoreId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT 1 from sysindexes WHERE [NAME]=N'IX_StoreMapping_EntityId_EntityName' and id=object_id(N'[StoreMapping]'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_StoreMapping_EntityId_EntityName] ON [StoreMapping] ([EntityId] ASC, [EntityName] ASC)
END
GO

--Store mapping for manufacturers
IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[Manufacturer]') and NAME='LimitedToStores')
BEGIN
	ALTER TABLE [Manufacturer]
	ADD [LimitedToStores] bit NULL
END
GO

UPDATE [Manufacturer]
SET [LimitedToStores] = 0
WHERE [LimitedToStores] IS NULL
GO

ALTER TABLE [Manufacturer] ALTER COLUMN [LimitedToStores] bit NOT NULL
GO

