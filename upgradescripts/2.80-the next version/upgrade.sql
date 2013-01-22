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
