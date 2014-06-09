--upgrade scripts from nopCommerce 1.90 to nopCommerce 2.00

DELETE FROM [Customer]
WHERE IsSystemAccount=0
GO
DELETE FROM [Address]
GO
DELETE FROM [Country]
GO
DELETE FROM [StateProvince]
GO
DELETE FROM [ShippingMethod]
GO
DELETE FROM [TaxCategory]
GO



--temporary table for identifiers
CREATE TABLE #IDs
	(
		[OriginalId] int NOT NULL,
		[NewId] int NOT NULL,
		[EntityName] nvarchar(100) NOT NULL
	)
GO
--But we persist IDs for some of the entities (in order to save URLs):
--product, category, manufacturer, producttag, news, blog, profile, forums




--Update encryption key
UPDATE [Setting]
SET [Value] = (SELECT [Value] FROM [Nop_Setting] WHERE [name] = N'Security.EncryptionPrivateKey')
WHERE [Name] = N'SecuritySettings.EncryptionKey'
GO





--DOWNLOADS
PRINT 'moving downloads'
DECLARE @OriginalDownloadId int
DECLARE cur_originaldownload CURSOR FOR
SELECT DownloadId
FROM [Nop_Download]
ORDER BY [DownloadId]
OPEN cur_originaldownload
FETCH NEXT FROM cur_originaldownload INTO @OriginalDownloadId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving download. ID ' + cast(@OriginalDownloadId as nvarchar(10))
	SET IDENTITY_INSERT [Download] ON
	INSERT INTO [Download] ([Id], [UseDownloadUrl], [DownloadUrl], [DownloadBinary], [ContentType], [Filename], [Extension], [IsNew])
	SELECT [DownloadId], [UseDownloadUrl], [DownloadUrl], [DownloadBinary], [ContentType], [Filename], [Extension], [IsNew]
	FROM [Nop_Download]
	WHERE DownloadId = @OriginalDownloadId
	SET IDENTITY_INSERT [Download] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originaldownload INTO @OriginalDownloadId
END
CLOSE cur_originaldownload
DEALLOCATE cur_originaldownload
GO











--PICTURES
PRINT 'moving pictures'
DECLARE @OriginalPictureId int
DECLARE cur_originalpicture CURSOR FOR
SELECT PictureId
FROM [Nop_Picture]
ORDER BY [PictureId]
OPEN cur_originalpicture
FETCH NEXT FROM cur_originalpicture INTO @OriginalPictureId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving picture. ID ' + cast(@OriginalPictureId as nvarchar(10))
	SET IDENTITY_INSERT [Picture] ON
	INSERT INTO [Picture] ([Id], [PictureBinary], [IsNew], [MimeType])
	SELECT [PictureId], [PictureBinary], [IsNew], [MimeType]
	FROM [Nop_Picture]
	WHERE PictureId = @OriginalPictureId
	SET IDENTITY_INSERT [Picture] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalpicture INTO @OriginalPictureId
END
CLOSE cur_originalpicture
DEALLOCATE cur_originalpicture
GO



--LANGUAGES
PRINT 'moving languages'
DECLARE @NewDefaultLanguageId int
SELECT @NewDefaultLanguageId = Id FROM [Language]
DECLARE @OriginalLanguageId int
DECLARE cur_originallanguage CURSOR FOR
SELECT LanguageId
FROM [Nop_Language]
ORDER BY [LanguageId]
OPEN cur_originallanguage
FETCH NEXT FROM cur_originallanguage INTO @OriginalLanguageId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving language. ID ' + cast(@OriginalLanguageId as nvarchar(10))
	INSERT INTO [Language] ([Name], [LanguageCulture], [FlagImageFileName], [Published], [DisplayOrder])
	SELECT [Name], [LanguageCulture], [FlagImageFileName], [Published], [DisplayOrder]
	FROM [Nop_Language]
	WHERE LanguageId = @OriginalLanguageId

	--new ID
	DECLARE @NewLanguageId int
	SET @NewLanguageId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalLanguageId, @NewLanguageId, N'Language')


	--insert new locale recources (not old ones)
	IF (@NewDefaultLanguageId > 0)
	BEGIN
		INSERT INTO [LocaleStringResource] ([LanguageId], [ResourceName], [ResourceValue])
		SELECT @NewLanguageId, [ResourceName], [ResourceValue]
		FROM [LocaleStringResource]
		WHERE [LanguageId]=@NewDefaultLanguageId ORDER BY [ResourceName]
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originallanguage INTO @OriginalLanguageId
END
CLOSE cur_originallanguage
DEALLOCATE cur_originallanguage
--now delete default language
IF (@NewDefaultLanguageId > 0)
BEGIN
	DELETE FROM [Language]
	WHERE [Id]=@NewDefaultLanguageId
END
GO









--CAMPAIGNS
PRINT 'moving campaigns'
DECLARE @OriginalCampaignId int
DECLARE cur_originalcampaign CURSOR FOR
SELECT CampaignId
FROM [Nop_Campaign]
ORDER BY [CreatedOn]
OPEN cur_originalcampaign
FETCH NEXT FROM cur_originalcampaign INTO @OriginalCampaignId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving campaign. ID ' + cast(@OriginalCampaignId as nvarchar(10))
	SET IDENTITY_INSERT [Campaign] ON
	INSERT INTO [Campaign] ([Id], [Name], [Subject], [Body], [CreatedOnUtc])
	SELECT [CampaignId], [Name], [Subject], [Body], [CreatedOn]
	FROM [Nop_Campaign]
	WHERE CampaignId = @OriginalCampaignId
	SET IDENTITY_INSERT [Campaign] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalcampaign INTO @OriginalCampaignId
END
CLOSE cur_originalcampaign
DEALLOCATE cur_originalcampaign
GO











--COUNTRIES
PRINT 'moving countries'
DECLARE @OriginalCountryId int
DECLARE cur_originalcountry CURSOR FOR
SELECT CountryId
FROM [Nop_Country]
ORDER BY [DisplayOrder], [Name]
OPEN cur_originalcountry
FETCH NEXT FROM cur_originalcountry INTO @OriginalCountryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving country. ID ' + cast(@OriginalCountryId as nvarchar(10))
	INSERT INTO [Country] ([Name], [AllowsBilling], [AllowsShipping], [TwoLetterIsoCode], [ThreeLetterIsoCode], [NumericIsoCode], [SubjectToVat], [Published], [DisplayOrder])
	SELECT [Name], [AllowsBilling], [AllowsShipping], [TwoLetterIsoCode], [ThreeLetterIsoCode], [NumericIsoCode], [SubjectToVat], [Published], [DisplayOrder]
	FROM [Nop_Country]
	WHERE CountryId = @OriginalCountryId

	--new ID
	DECLARE @NewCountryId int
	SET @NewCountryId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCountryId, @NewCountryId, N'Country')
	--fetch next identifier
	FETCH NEXT FROM cur_originalcountry INTO @OriginalCountryId
END
CLOSE cur_originalcountry
DEALLOCATE cur_originalcountry
GO






--STATES
PRINT 'moving states'
DECLARE @OriginalStateProvinceId int
DECLARE cur_originalstateprovince CURSOR FOR
SELECT StateProvinceId
FROM [Nop_StateProvince]
ORDER BY [DisplayOrder], [Name]
OPEN cur_originalstateprovince
FETCH NEXT FROM cur_originalstateprovince INTO @OriginalStateProvinceId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving state. ID ' + cast(@OriginalStateProvinceId as nvarchar(10))
	INSERT INTO [StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Country' and [OriginalId]=original_sp.CountryId), [Name], [Abbreviation], 1 /*published*/, [DisplayOrder]
	FROM [Nop_StateProvince] original_sp
	WHERE StateProvinceId = @OriginalStateProvinceId

	--new ID
	DECLARE @NewStateProvinceId int
	SET @NewStateProvinceId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalStateProvinceId, @NewStateProvinceId, N'StateProvince')
	--fetch next identifier
	FETCH NEXT FROM cur_originalstateprovince INTO @OriginalStateProvinceId
END
CLOSE cur_originalstateprovince
DEALLOCATE cur_originalstateprovince
GO








--AFFILIATES
PRINT 'moving affiliates'
DECLARE @OriginalAffiliateId int
DECLARE cur_originalaffiliate CURSOR FOR
SELECT AffiliateId
FROM [Nop_Affiliate]
ORDER BY [AffiliateId]
OPEN cur_originalaffiliate
FETCH NEXT FROM cur_originalaffiliate INTO @OriginalAffiliateId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving affiliate. ID ' + cast(@OriginalAffiliateId as nvarchar(10))

	INSERT INTO [Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
	SELECT [FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], null /*no state province*/, [ZipPostalCode], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Country' and [OriginalId]=[CountryId]), getutcdate()
	FROM [Nop_Affiliate]
	WHERE AffiliateId = @OriginalAffiliateId

	DECLARE @NewAffiliateAddressId int
	SET @NewAffiliateAddressId = @@IDENTITY

	SET IDENTITY_INSERT [Affiliate] ON
	INSERT INTO [Affiliate] ([Id], [AddressId], [Active], [Deleted])
	SELECT [AffiliateId], @NewAffiliateAddressId, [Active], [Deleted]
	FROM [Nop_Affiliate]
	WHERE AffiliateId = @OriginalAffiliateId
	SET IDENTITY_INSERT [Affiliate] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalaffiliate INTO @OriginalAffiliateId
END
CLOSE cur_originalaffiliate
DEALLOCATE cur_originalaffiliate
GO








--CUSTOMER ROLES
PRINT 'moving customer roles'
DECLARE @OriginalCustomerRoleId int
DECLARE cur_originalcustomerrole CURSOR FOR
SELECT CustomerRoleId
FROM [Nop_CustomerRole]
WHERE Deleted=0
ORDER BY CustomerRoleId
OPEN cur_originalcustomerrole
FETCH NEXT FROM cur_originalcustomerrole INTO @OriginalCustomerRoleId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving customer role. ID ' + cast(@OriginalCustomerRoleId as nvarchar(10))
	INSERT INTO [CustomerRole] ([Name], [FreeShipping], [TaxExempt], [Active], [IsSystemRole])
	SELECT [Name], [FreeShipping], [TaxExempt], [Active], 0
	FROM [Nop_CustomerRole]
	WHERE CustomerRoleId = @OriginalCustomerRoleId

	--new ID
	DECLARE @NewCustomerRoleId int
	SET @NewCustomerRoleId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCustomerRoleId, @NewCustomerRoleId, N'CustomerRole')
	--fetch next identifier
	FETCH NEXT FROM cur_originalcustomerrole INTO @OriginalCustomerRoleId
END
CLOSE cur_originalcustomerrole
DEALLOCATE cur_originalcustomerrole
GO










--CUSTOMERS
PRINT 'moving customers'
DECLARE @OriginalCustomerId int
DECLARE cur_originalcustomer CURSOR FOR
SELECT CustomerId
FROM [Nop_Customer]
ORDER BY CustomerId
OPEN cur_originalcustomer
FETCH NEXT FROM cur_originalcustomer INTO @OriginalCustomerId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving customer. ID ' + cast(@OriginalCustomerId as nvarchar(10))
	SET IDENTITY_INSERT [Customer] ON
	INSERT INTO [Customer] ([Id], [CustomerGuid], [Username], [Email], [Password], [PasswordFormatId], [PasswordSalt], [AffiliateId], [AdminComment], [TaxDisplayTypeId], [IsTaxExempt], [VatNumberStatusId], [UseRewardPointsDuringCheckout], [TimeZoneId], [Active], [Deleted], [IsSystemAccount], [CreatedOnUtc], [LastActivityDateUtc])
	SELECT [CustomerId], [CustomerGuid], [Username], [Email], [PasswordHash], 1 /*hashed*/, [SaltKey], (SELECT CASE WHEN [AffiliateId]=0 THEN NULL ELSE [AffiliateId] END), [AdminComment], 0 /*IncludingTax now*/, [IsTaxExempt], 10 /*Empty now*/, 0, [TimeZoneId], [Active], [Deleted], 0, [RegistrationDate], [RegistrationDate]
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId
	SET IDENTITY_INSERT [Customer] OFF

	--move customer attributes (Gender, Firstname, Lastname, Company, PasswordRecoveryToken, AccountActivationToken)
	INSERT INTO [CustomerAttribute] ([CustomerId], [Key], [Value])
	SELECT @OriginalCustomerId, [key], [value]
	FROM [Nop_CustomerAttribute]
	WHERE (CustomerID = @OriginalCustomerId) and 
	([key] = N'Gender' or [key] = N'FirstName' or [key] = N'LastName' or [key] = N'Company' or [key] = N'PasswordRecoveryToken' or [key] = N'AccountActivationToken') and
	([value] is not null)
	--move 'ForumPostCount' customer attribute
	DECLARE @ForumPostCount int
	SET @ForumPostCount = null -- clear cache (variable scope)
	SELECT @ForumPostCount = [TotalForumPosts]
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId 
	IF (@ForumPostCount > 0)
	BEGIN
		INSERT INTO [CustomerAttribute] ([CustomerId], [Key], [Value])
		VALUES (@OriginalCustomerId, 'ForumPostCount', cast(@ForumPostCount as nvarchar(100)))
	END
	--move 'Signature' customer attribute
	DECLARE @Signature nvarchar(1000)
	SET @Signature = null -- clear cache (variable scope)
	SELECT @Signature = [Signature]
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId 
	IF (len(@Signature) > 0)
	BEGIN
		INSERT INTO [CustomerAttribute] ([CustomerId], [Key], [Value])
		VALUES (@OriginalCustomerId, 'Signature', @Signature)
	END
	--move 'Avatar' customer attribute
	DECLARE @AvatarPictureId int
	SET @AvatarPictureId = null -- clear cache (variable scope)
	SELECT @AvatarPictureId = [AvatarID] FROM [Nop_Customer] WHERE CustomerId = @OriginalCustomerId
	IF (@AvatarPictureId > 0)
	BEGIN
		INSERT INTO [CustomerAttribute] ([CustomerId], [Key], [Value])
		VALUES (@OriginalCustomerId, 'AvatarPictureId', cast(@AvatarPictureId as nvarchar(100)))
	END
	--move 'Location' customer attribute
	DECLARE @LocationCountryId int
	SET @LocationCountryId = null -- clear cache (variable scope)	
	DECLARE @OldLocationCountryId nvarchar(100)
	SET @OldLocationCountryId = null -- clear cache (variable scope)
	SELECT @OldLocationCountryId=[Value]
	FROM [Nop_CustomerAttribute]
	WHERE CustomerId = @OriginalCustomerId and [key]=N'CountryID'
	IF (@OldLocationCountryId is not null and len(@OldLocationCountryId) > 0)
	BEGIN
		SELECT @LocationCountryId = [NewId]
		FROM #IDs
		WHERE [EntityName]=N'Country' and [OriginalId]=(cast(@OldLocationCountryId as int))
		IF (@LocationCountryId > 0)
		BEGIN
			INSERT INTO [CustomerAttribute] ([CustomerId], [Key], [Value])
			VALUES (@OriginalCustomerId, 'Location', cast(@LocationCountryId as nvarchar(100)))
		END
	END

	




	--map customer to customer roles (new system roles)
	DECLARE @IsAdmin bit
	DECLARE @IsGuest bit
	DECLARE @IsRegistered bit
	DECLARE @IsForumModerator bit
	SELECT @IsAdmin = IsAdmin, @IsGuest = IsGuest, @IsRegistered = ~IsGuest, @IsForumModerator = IsForumModerator
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId
	DECLARE @AdminCustomerRoleId int
	DECLARE @GuestCustomerRoleId int
	DECLARE @RegisteredCustomerRoleId int
	DECLARE @ForumModeratorCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'
	SELECT @GuestCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Guests'
	SELECT @RegisteredCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Registered'
	SELECT @ForumModeratorCustomerRoleId = Id
	FROM [CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'ForumModerators'
	IF (@IsAdmin = 1)
	BEGIN
		INSERT INTO [Customer_CustomerRole_Mapping] ([CustomerRole_Id], [Customer_Id])
		VALUES (@AdminCustomerRoleId, @OriginalCustomerId)
	END	
	IF (@IsGuest = 1)
	BEGIN
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@GuestCustomerRoleId, @OriginalCustomerId)
	END	
	IF (@IsRegistered = 1)
	BEGIN
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@RegisteredCustomerRoleId, @OriginalCustomerId)
	END	
	IF (@IsForumModerator = 1)
	BEGIN
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@ForumModeratorCustomerRoleId, @OriginalCustomerId)
	END
	--map customer to customer roles(old roles)
	INSERT INTO [Customer_CustomerRole_Mapping] ([CustomerRole_Id],[Customer_Id])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'CustomerRole' and [OriginalId]=original_ccrm.CustomerRoleId), @OriginalCustomerId
	FROM [Nop_Customer_CustomerRole_Mapping] original_ccrm
	WHERE original_ccrm.CustomerID = @OriginalCustomerId

	--fetch next identifier
	FETCH NEXT FROM cur_originalcustomer INTO @OriginalCustomerId
END
CLOSE cur_originalcustomer
DEALLOCATE cur_originalcustomer
GO








--CUSTOMER ADDRESSES
PRINT 'moving customer addresses'
DECLARE @OriginalAddressId int
DECLARE cur_originaladdress CURSOR FOR
SELECT [AddressId]
FROM [Nop_Address]
WHERE [IsBillingAddress]=1 --move only billing addresses
ORDER BY AddressId
OPEN cur_originaladdress
FETCH NEXT FROM cur_originaladdress INTO @OriginalAddressId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving addresses. ID ' + cast(@OriginalAddressId as nvarchar(10))
	INSERT INTO [Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
	SELECT [FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'StateProvince' and [OriginalId]=[StateProvinceID]), [ZipPostalCode], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Country' and [OriginalId]=[CountryId]), [CreatedOn]
	FROM [Nop_Address]
	WHERE AddressId = @OriginalAddressId

	--new ID
	DECLARE @NewAddressId int
	SET @NewAddressId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalAddressId, @NewAddressId, N'Address')

	
	--map customers to addresses (now we have a new CustomerAddresses table)
	INSERT INTO [CustomerAddresses] ([Customer_Id],[Address_Id])
	SELECT [CustomerID], @NewAddressId
	FROM [Nop_Address]
	WHERE [AddressId] = @OriginalAddressId


	--fetch next identifier
	FETCH NEXT FROM cur_originaladdress INTO @OriginalAddressId
END
CLOSE cur_originaladdress
DEALLOCATE cur_originaladdress
GO







--BLOG POSTS
PRINT 'moving blog posts'
DECLARE @OriginalBlogPostId int
DECLARE cur_originalblogpost CURSOR FOR
SELECT BlogPostId
FROM [Nop_BlogPost]
ORDER BY [CreatedOn]
OPEN cur_originalblogpost
FETCH NEXT FROM cur_originalblogpost INTO @OriginalBlogPostId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving blog post. ID ' + cast(@OriginalBlogPostId as nvarchar(10))
	SET IDENTITY_INSERT [BlogPost] ON
	INSERT INTO [BlogPost] ([Id], [LanguageId], [Title], [Body], [AllowComments], [Tags], [CreatedOnUtc])
	SELECT [BlogPostId], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), [BlogPostTitle], [BlogPostBody], [BlogPostAllowComments], [Tags], [CreatedOn]
	FROM [Nop_BlogPost]
	WHERE BlogPostId = @OriginalBlogPostId
	SET IDENTITY_INSERT [BlogPost] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalblogpost INTO @OriginalBlogPostId
END
CLOSE cur_originalblogpost
DEALLOCATE cur_originalblogpost
GO

--BLOG COMMENTS
PRINT 'moving blog comments'
DECLARE @OriginalBlogCommentId int
DECLARE cur_originalblogcomment CURSOR FOR
SELECT BlogCommentId
FROM [Nop_BlogComment]
ORDER BY [CreatedOn]
OPEN cur_originalblogcomment
FETCH NEXT FROM cur_originalblogcomment INTO @OriginalBlogCommentId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving blog comment. ID ' + cast(@OriginalBlogCommentId as nvarchar(10))

	DECLARE @BlogCommentCustomerId int
	SET @BlogCommentCustomerId = null -- clear cache (variable scope)
	SELECT @BlogCommentCustomerId = CustomerID FROM [Nop_BlogComment] WHERE BlogCommentId = @OriginalBlogCommentId
	--ensure that @BlogCommentCustomerId is not null
	IF ((@BlogCommentCustomerId is null) or (@BlogCommentCustomerId = 0))
	BEGIN
		--insert guest customer	
		print 'inserting guest customer for blog comment'
		INSERT INTO [Customer] ([CustomerGuid], [PasswordFormatId], [TaxDisplayTypeId], [IsTaxExempt], [VatNumberStatusId], [UseRewardPointsDuringCheckout], [Active], [Deleted], [IsSystemAccount], [CreatedOnUtc], [LastActivityDateUtc])
		VALUES (NEWID(), 0 /*clear*/, 0 /*IncludingTax now*/, 0, 10 /*Empty now*/, 0, 1, 0, 0, getutcdate(),getutcdate())
		SET @BlogCommentCustomerId = @@IDENTITY
				
		DECLARE @GuestCustomerRoleId int
		SELECT @GuestCustomerRoleId = Id
		FROM [CustomerRole]
		WHERE IsSystemRole=1 and [SystemName] = N'Guests'
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@GuestCustomerRoleId, @BlogCommentCustomerId)

	END

	INSERT INTO [CustomerContent] ([CustomerId], [IpAddress], [IsApproved], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT @BlogCommentCustomerId, [IpAddress], 1 /*approved*/, [CreatedOn], [CreatedOn]
	FROM [Nop_BlogComment]
	WHERE BlogCommentId = @OriginalBlogCommentId

	--new ID
	DECLARE @NewBlogCommentId int
	SET @NewBlogCommentId = @@IDENTITY


	INSERT INTO [BlogComment] ([Id], [CommentText], [BlogPostId])
	SELECT @NewBlogCommentId, [CommentText], [BlogPostID]
	FROM [Nop_BlogComment]
	WHERE BlogCommentId = @OriginalBlogCommentId	


	--fetch next identifier
	FETCH NEXT FROM cur_originalblogcomment INTO @OriginalBlogCommentId
END
CLOSE cur_originalblogcomment
DEALLOCATE cur_originalblogcomment
GO







--NEWS ITEMS
PRINT 'moving news items'
DECLARE @OriginalNewsItemId int
DECLARE cur_originalnewsitem CURSOR FOR
SELECT NewsId
FROM [Nop_News]
ORDER BY [CreatedOn]
OPEN cur_originalnewsitem
FETCH NEXT FROM cur_originalnewsitem INTO @OriginalNewsItemId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving news item. ID ' + cast(@OriginalNewsItemId as nvarchar(10))
	SET IDENTITY_INSERT [News] ON
	INSERT INTO [News] ([Id], [LanguageId], [Title], [Short], [Full], [Published], [AllowComments], [CreatedOnUtc])
	SELECT [NewsId], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), [Title], [Short], [Full], [Published], [AllowComments], [CreatedOn]
	FROM [Nop_News]
	WHERE NewsId = @OriginalNewsItemId
	SET IDENTITY_INSERT [News] OFF



	--fetch next identifier
	FETCH NEXT FROM cur_originalnewsitem INTO @OriginalNewsItemId
END
CLOSE cur_originalnewsitem
DEALLOCATE cur_originalnewsitem
GO

--NEWS COMMENTS
PRINT 'moving news comments'
DECLARE @OriginalNewsCommentId int
DECLARE cur_originalnewscomment CURSOR FOR
SELECT NewsCommentId
FROM [Nop_NewsComment]
ORDER BY [CreatedOn]
OPEN cur_originalnewscomment
FETCH NEXT FROM cur_originalnewscomment INTO @OriginalNewsCommentId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving news comment. ID ' + cast(@OriginalNewsCommentId as nvarchar(10))

	DECLARE @NewsCommentCustomerId int
	SET @NewsCommentCustomerId = null -- clear cache (variable scope)
	SELECT @NewsCommentCustomerId = CustomerID FROM [Nop_NewsComment] WHERE NewsCommentId = @OriginalNewsCommentId
	--ensure that @NewsCommentCustomerId is not null
	IF ((@NewsCommentCustomerId is null) or (@NewsCommentCustomerId = 0))
	BEGIN
		--insert guest customer	
		print 'inserting guest customer for news comment'
		INSERT INTO [Customer] ([CustomerGuid], [PasswordFormatId], [TaxDisplayTypeId], [IsTaxExempt], [VatNumberStatusId], [UseRewardPointsDuringCheckout], [Active], [Deleted], [IsSystemAccount], [CreatedOnUtc], [LastActivityDateUtc])
		VALUES (NEWID(), 0 /*clear*/, 0 /*IncludingTax now*/, 0, 10 /*Empty now*/, 0, 1, 0, 0, getutcdate(),getutcdate())
		SET @NewsCommentCustomerId = @@IDENTITY
				
		DECLARE @GuestCustomerRoleId int
		SELECT @GuestCustomerRoleId = Id
		FROM [CustomerRole]
		WHERE IsSystemRole=1 and [SystemName] = N'Guests'
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@GuestCustomerRoleId, @NewsCommentCustomerId)

	END

	INSERT INTO [CustomerContent] ([CustomerId], [IpAddress], [IsApproved], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT @NewsCommentCustomerId, [IpAddress], 1 /*approved*/, [CreatedOn], [CreatedOn]
	FROM [Nop_NewsComment]
	WHERE NewsCommentId = @OriginalNewsCommentId

	--new ID
	DECLARE @NewNewsCommentId int
	SET @NewNewsCommentId = @@IDENTITY

	INSERT INTO [NewsComment] ([Id], [CommentTitle], [CommentText], [NewsItemId])
	SELECT @NewNewsCommentId, [Title], [Comment], [NewsID]
	FROM [Nop_NewsComment]
	WHERE NewsCommentId = @OriginalNewsCommentId	


	--fetch next identifier
	FETCH NEXT FROM cur_originalnewscomment INTO @OriginalNewsCommentId
END
CLOSE cur_originalnewscomment
DEALLOCATE cur_originalnewscomment
GO







--POLLS
PRINT 'moving polls'
DECLARE @OriginalPollId int
DECLARE cur_originalpoll CURSOR FOR
SELECT PollId
FROM [Nop_Poll]
ORDER BY [PollId]
OPEN cur_originalpoll
FETCH NEXT FROM cur_originalpoll INTO @OriginalPollId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving poll. ID ' + cast(@OriginalPollId as nvarchar(10))
	SET IDENTITY_INSERT [Poll] ON
	INSERT INTO [Poll] ([Id], [LanguageId], [Name], [SystemKeyword], [Published], [ShowOnHomePage], [DisplayOrder], [StartDateUtc], [EndDateUtc])
	SELECT [PollId], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), [Name], [SystemKeyword], [Published], [ShowOnHomePage], [DisplayOrder], [StartDate], [EndDate]
	FROM [Nop_Poll]
	WHERE PollId = @OriginalPollId
	SET IDENTITY_INSERT [Poll] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalpoll INTO @OriginalPollId
END
CLOSE cur_originalpoll
DEALLOCATE cur_originalpoll
GO

--POLL ANSWERS
PRINT 'moving poll answers'
DECLARE @OriginalPollAnswerId int
DECLARE cur_originalpollanswer CURSOR FOR
SELECT PollAnswerId
FROM [Nop_PollAnswer]
ORDER BY [DisplayOrder]
OPEN cur_originalpollanswer
FETCH NEXT FROM cur_originalpollanswer INTO @OriginalPollAnswerId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving poll answer. ID ' + cast(@OriginalPollAnswerId as nvarchar(10))
	INSERT INTO [PollAnswer] ([PollId], [Name], [NumberOfVotes], [DisplayOrder])
	SELECT [PollId], [Name], [Count], [DisplayOrder]
	FROM [Nop_PollAnswer]
	WHERE PollAnswerId = @OriginalPollAnswerId

	--new ID
	DECLARE @NewPollAnswerId int
	SET @NewPollAnswerId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalPollAnswerId, @NewPollAnswerId, N'PollAnswer')



	--fetch next identifier
	FETCH NEXT FROM cur_originalpollanswer INTO @OriginalPollAnswerId
END
CLOSE cur_originalpollanswer
DEALLOCATE cur_originalpollanswer
GO

--POLL VOTING RECORDS
PRINT 'moving poll voting records'
DECLARE @OriginalPollVotingRecordId int
DECLARE cur_originalpollvotingrecord CURSOR FOR
SELECT PollVotingRecordId
FROM [Nop_PollVotingRecord]
ORDER BY [PollVotingRecordID]
OPEN cur_originalpollvotingrecord
FETCH NEXT FROM cur_originalpollvotingrecord INTO @OriginalPollVotingRecordId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving poll voting record. ID ' + cast(@OriginalPollVotingRecordId as nvarchar(10))

	INSERT INTO [CustomerContent] ([CustomerId], [IpAddress], [IsApproved], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [CustomerId], null, 1 /*approved*/, getutcdate(), getutcdate()
	FROM [Nop_PollVotingRecord]
	WHERE PollVotingRecordId = @OriginalPollVotingRecordId

	--new ID
	DECLARE @NewPollVotingRecordId int
	SET @NewPollVotingRecordId = @@IDENTITY


	INSERT INTO [PollVotingRecord] ([Id], [PollAnswerId])
	SELECT @NewPollVotingRecordId, (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'PollAnswer' and [OriginalId]=[PollAnswerID])
	FROM [Nop_PollVotingRecord]
	WHERE PollVotingRecordId = @OriginalPollVotingRecordId	


	--fetch next identifier
	FETCH NEXT FROM cur_originalpollvotingrecord INTO @OriginalPollVotingRecordId
END
CLOSE cur_originalpollvotingrecord
DEALLOCATE cur_originalpollvotingrecord
GO




--FORUM GROUPS
PRINT 'moving forum groups'
DECLARE @OriginalForumGroupId int
DECLARE cur_originalforumgroup CURSOR FOR
SELECT ForumGroupId
FROM [Nop_Forums_Group]
ORDER BY [DisplayOrder]
OPEN cur_originalforumgroup
FETCH NEXT FROM cur_originalforumgroup INTO @OriginalForumGroupId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving forum group. ID ' + cast(@OriginalForumGroupId as nvarchar(10))
	SET IDENTITY_INSERT [Forums_Group] ON
	INSERT INTO [Forums_Group] ([Id], [Name], [Description], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [ForumGroupId], [Name], [Description], [DisplayOrder], [CreatedOn], [UpdatedOn]
	FROM [Nop_Forums_Group]
	WHERE ForumGroupId = @OriginalForumGroupId
	SET IDENTITY_INSERT [Forums_Group] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalforumgroup INTO @OriginalForumGroupId
END
CLOSE cur_originalforumgroup
DEALLOCATE cur_originalforumgroup
GO







--FORUMS
PRINT 'moving forums'
DECLARE @OriginalForumId int
DECLARE cur_originalforum CURSOR FOR
SELECT ForumId
FROM [Nop_Forums_Forum]
ORDER BY [CreatedOn]
OPEN cur_originalforum
FETCH NEXT FROM cur_originalforum INTO @OriginalForumId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving forum. ID ' + cast(@OriginalForumId as nvarchar(10))

	SET IDENTITY_INSERT [Forums_Forum] ON
	INSERT INTO [Forums_Forum] ([Id], [ForumGroupId], [Name], [Description], [NumTopics], [NumPosts], [LastTopicId], [LastPostId], [LastPostCustomerId], [LastPostTime], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [ForumId], [ForumGroupId], [Name], [Description], [NumTopics], [NumPosts], [LastTopicId],  [LastPostId], [LastPostUserID], [LastPostTime], [DisplayOrder], [CreatedOn], [UpdatedOn]
	FROM [Nop_Forums_Forum]
	WHERE ForumId = @OriginalForumId
	SET IDENTITY_INSERT [Forums_Forum] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalforum INTO @OriginalForumId
END
CLOSE cur_originalforum
DEALLOCATE cur_originalforum
GO







--FORUM TOPICS
PRINT 'moving forum topics'
DECLARE @OriginalForumTopicId int
DECLARE cur_originalforumtopic CURSOR FOR
SELECT TopicId
FROM [Nop_Forums_Topic]
ORDER BY [CreatedOn]
OPEN cur_originalforumtopic
FETCH NEXT FROM cur_originalforumtopic INTO @OriginalForumTopicId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving forum topic. ID ' + cast(@OriginalForumTopicId as nvarchar(10))

	SET IDENTITY_INSERT [Forums_Topic] ON
	INSERT INTO [Forums_Topic] ([Id], [ForumId], [CustomerId], [TopicTypeId], [Subject], [NumPosts], [Views], [LastPostId], [LastPostCustomerId], [LastPostTime], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [TopicId], [ForumId], [UserID], [TopicTypeId], [Subject], [NumPosts], [Views], [LastPostId], [LastPostUserID], [LastPostTime], [CreatedOn], [UpdatedOn]
	FROM [Nop_Forums_Topic]
	WHERE TopicId = @OriginalForumTopicId
	SET IDENTITY_INSERT [Forums_Topic] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalforumtopic INTO @OriginalForumTopicId
END
CLOSE cur_originalforumtopic
DEALLOCATE cur_originalforumtopic
GO







--FORUM POSTS
PRINT 'moving forum posts'
DECLARE @OriginalForumPostId int
DECLARE cur_originalforumpost CURSOR FOR
SELECT PostId
FROM [Nop_Forums_Post]
ORDER BY [CreatedOn]
OPEN cur_originalforumpost
FETCH NEXT FROM cur_originalforumpost INTO @OriginalForumPostId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving forum post. ID ' + cast(@OriginalForumPostId as nvarchar(10))

	SET IDENTITY_INSERT [Forums_Post] ON
	INSERT INTO [Forums_Post] ([Id], [TopicId], [CustomerId], [Text], [IPAddress], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [PostID], [TopicId], [UserID], [Text], [IPAddress], [CreatedOn], [UpdatedOn]
	FROM [Nop_Forums_Post]
	WHERE PostId = @OriginalForumPostId
	SET IDENTITY_INSERT [Forums_Post] OFF


	--fetch next identifier
	FETCH NEXT FROM cur_originalforumpost INTO @OriginalForumPostId
END
CLOSE cur_originalforumpost
DEALLOCATE cur_originalforumpost
GO



















--FORUM SUBSCRIPTIONS
PRINT 'moving forum subscriptions'
DECLARE @OriginalForumSubscriptionId int
DECLARE cur_originalforumsubscription CURSOR FOR
SELECT SubscriptionID
FROM [Nop_Forums_Subscription]
ORDER BY [SubscriptionID]
OPEN cur_originalforumsubscription
FETCH NEXT FROM cur_originalforumsubscription INTO @OriginalForumSubscriptionId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving forum subscription. ID ' + cast(@OriginalForumSubscriptionId as nvarchar(10))
	INSERT INTO [Forums_Subscription] ([SubscriptionGuid], [CustomerId], [ForumId], [TopicId], [CreatedOnUtc])
	SELECT [SubscriptionGuid], [UserId], [ForumId], [TopicId], [CreatedOn]
	FROM [Nop_Forums_Subscription]
	WHERE SubscriptionID = @OriginalForumSubscriptionId

	--fetch next identifier
	FETCH NEXT FROM cur_originalforumsubscription INTO @OriginalForumSubscriptionId
END
CLOSE cur_originalforumsubscription
DEALLOCATE cur_originalforumsubscription
GO











--PRIVATE MESSAGES
PRINT 'moving private messages'
DECLARE @OriginalPrivateMessageId int
DECLARE cur_originalprivatemessage CURSOR FOR
SELECT PrivateMessageID
FROM [Nop_Forums_PrivateMessage]
ORDER BY [PrivateMessageID]
OPEN cur_originalprivatemessage
FETCH NEXT FROM cur_originalprivatemessage INTO @OriginalPrivateMessageId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving private message. ID ' + cast(@OriginalPrivateMessageId as nvarchar(10))
	INSERT INTO [Forums_PrivateMessage] ([FromCustomerId], [ToCustomerId], [Subject], [Text], [IsRead], [IsDeletedByAuthor], [IsDeletedByRecipient], [CreatedOnUtc])
	SELECT [FromUserID], [ToUserID], [Subject], [Text], [IsRead], [IsDeletedByAuthor], [IsDeletedByRecipient], [CreatedOn]
	FROM [Nop_Forums_PrivateMessage]
	WHERE PrivateMessageID = @OriginalPrivateMessageId

	--fetch next identifier
	FETCH NEXT FROM cur_originalprivatemessage INTO @OriginalPrivateMessageId
END
CLOSE cur_originalprivatemessage
DEALLOCATE cur_originalprivatemessage
GO











--NEWSLETTER SUBSCRIPTIONS
PRINT 'moving newsletter subscriptions'
DECLARE @OriginalNewsLetterSubscriptionId int
DECLARE cur_originalnewslettersubscription CURSOR FOR
SELECT NewsLetterSubscriptionId
FROM [Nop_NewsLetterSubscription]
ORDER BY [NewsLetterSubscriptionId]
OPEN cur_originalnewslettersubscription
FETCH NEXT FROM cur_originalnewslettersubscription INTO @OriginalNewsLetterSubscriptionId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving newsletter subscription. ID ' + cast(@OriginalNewsLetterSubscriptionId as nvarchar(10))
	INSERT INTO [NewsLetterSubscription] ([NewsLetterSubscriptionGuid], [Email], [Active], [CreatedOnUtc])
	SELECT [NewsLetterSubscriptionGuid], [Email], [Active], [CreatedOn]
	FROM [Nop_NewsLetterSubscription]
	WHERE NewsLetterSubscriptionId = @OriginalNewsLetterSubscriptionId

	--fetch next identifier
	FETCH NEXT FROM cur_originalnewslettersubscription INTO @OriginalNewsLetterSubscriptionId
END
CLOSE cur_originalnewslettersubscription
DEALLOCATE cur_originalnewslettersubscription
GO











--QUEUED EMAILS
PRINT 'moving queued emails'
DECLARE @DefaultEmailAccount int
SELECT @DefaultEmailAccount = cast([value] as int) FROM [Setting] WHERE [name] = 'emailaccountsettings.defaultemailaccountid'
DECLARE @OriginalQueuedEmailId int
DECLARE cur_originalqueuedemail CURSOR FOR
SELECT QueuedEmailId
FROM [Nop_QueuedEmail]
ORDER BY [QueuedEmailId]
OPEN cur_originalqueuedemail
FETCH NEXT FROM cur_originalqueuedemail INTO @OriginalQueuedEmailId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving queued email. ID ' + cast(@OriginalQueuedEmailId as nvarchar(10))
	INSERT INTO [QueuedEmail] ([Priority], [From], [FromName], [To], [ToName], [Cc], [Bcc], [Subject], [Body], [CreatedOnUtc], [SentTries], [SentOnUtc], [EmailAccountId])
	SELECT [Priority], [From], [FromName], [To], [ToName], [Cc], [Bcc], [Subject], [Body], [CreatedOn], [SendTries], [SentOn], @DefaultEmailAccount
	FROM [Nop_QueuedEmail]
	WHERE QueuedEmailId = @OriginalQueuedEmailId

	--fetch next identifier
	FETCH NEXT FROM cur_originalqueuedemail INTO @OriginalQueuedEmailId
END
CLOSE cur_originalqueuedemail
DEALLOCATE cur_originalqueuedemail
GO











--SHIPPING METHODS
PRINT 'moving shipping methods'
DECLARE @OriginalShippingMethodId int
DECLARE cur_originalshippingmethod CURSOR FOR
SELECT ShippingMethodId
FROM [Nop_ShippingMethod]
ORDER BY [ShippingMethodId]
OPEN cur_originalshippingmethod
FETCH NEXT FROM cur_originalshippingmethod INTO @OriginalShippingMethodId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving shipping method. ID ' + cast(@OriginalShippingMethodId as nvarchar(10))
	INSERT INTO [ShippingMethod] ([Name], [Description], [DisplayOrder])
	SELECT [Name], [Description], [DisplayOrder]
	FROM [Nop_ShippingMethod]
	WHERE ShippingMethodId = @OriginalShippingMethodId

	--new ID
	DECLARE @NewShippingMethodId int
	SET @NewShippingMethodId = @@IDENTITY

	--shipping method restrictions
	INSERT INTO [ShippingMethodRestrictions] ([ShippingMethod_Id], [Country_Id])
	SELECT @NewShippingMethodId, (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Country' and [OriginalId]=[CountryID])
	FROM [Nop_ShippingMethod_RestrictedCountries]
	WHERE ShippingMethodId = @OriginalShippingMethodId

	--fetch next identifier
	FETCH NEXT FROM cur_originalshippingmethod INTO @OriginalShippingMethodId
END
CLOSE cur_originalshippingmethod
DEALLOCATE cur_originalshippingmethod
GO











--TAX CATEGORIES
PRINT 'moving tax categories'
DECLARE @OriginalTaxCategoryId int
DECLARE cur_originaltaxcategory CURSOR FOR
SELECT TaxCategoryId
FROM [Nop_TaxCategory]
ORDER BY [TaxCategoryId]
OPEN cur_originaltaxcategory
FETCH NEXT FROM cur_originaltaxcategory INTO @OriginalTaxCategoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving tax category. ID ' + cast(@OriginalTaxCategoryId as nvarchar(10))
	SET IDENTITY_INSERT [TaxCategory] ON
	INSERT INTO [TaxCategory] ([Id], [Name], [DisplayOrder])
	SELECT [TaxCategoryId], [Name], [DisplayOrder]
	FROM [Nop_TaxCategory]
	WHERE TaxCategoryId = @OriginalTaxCategoryId
	SET IDENTITY_INSERT [TaxCategory] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originaltaxcategory INTO @OriginalTaxCategoryId
END
CLOSE cur_originaltaxcategory
DEALLOCATE cur_originaltaxcategory
GO











--CATEGORIES
PRINT 'moving categories'
DECLARE @OriginalCategoryId int
DECLARE cur_originalcategory CURSOR FOR
SELECT CategoryId
FROM [Nop_Category]
ORDER BY [CategoryId]
OPEN cur_originalcategory
FETCH NEXT FROM cur_originalcategory INTO @OriginalCategoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving category. ID ' + cast(@OriginalCategoryId as nvarchar(10))
	SET IDENTITY_INSERT [Category] ON
	INSERT INTO [Category] ([Id], [Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [SeName], [ParentCategoryId], [PictureId], [PageSize], [PriceRanges], [ShowOnHomePage], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [CategoryId], [Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [SeName], [ParentCategoryId], [PictureId], [PageSize], [PriceRanges], [ShowOnHomePage], [Published], [Deleted], [DisplayOrder], [CreatedOn], [UpdatedOn]
	FROM [Nop_Category]
	WHERE CategoryId = @OriginalCategoryId
	SET IDENTITY_INSERT [Category] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalcategory INTO @OriginalCategoryId
END
CLOSE cur_originalcategory
DEALLOCATE cur_originalcategory
GO



--LOCALIZED CATEGORIES
PRINT 'moving localized categories'
DECLARE @OriginalCategoryLocalizedId int
DECLARE cur_originalcategorylocalized CURSOR FOR
SELECT CategoryLocalizedID
FROM [Nop_CategoryLocalized]
ORDER BY [CategoryLocalizedID]
OPEN cur_originalcategorylocalized
FETCH NEXT FROM cur_originalcategorylocalized INTO @OriginalCategoryLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized category. ID ' + cast(@OriginalCategoryLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @Description nvarchar(MAX)
	SET @Description = null -- clear cache (variable scope)
	DECLARE @MetaKeywords nvarchar(MAX)
	SET @MetaKeywords = null -- clear cache (variable scope)
	DECLARE @MetaDescription nvarchar(MAX)
	SET @MetaDescription = null -- clear cache (variable scope)
	DECLARE @MetaTitle nvarchar(MAX)
	SET @MetaTitle = null -- clear cache (variable scope)
	DECLARE @SEName nvarchar(MAX)
	SET @SEName = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@Description=[Description],
			@MetaKeywords = [MetaKeywords], 
			@MetaDescription = [MetaDescription], 
			@MetaTitle = [MetaTitle], 
			@SEName = [SEName]
	FROM [Nop_CategoryLocalized]
	WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CategoryID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'Name', @Name
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@Description) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CategoryID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'Description', @Description
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@MetaKeywords) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CategoryID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'MetaKeywords', @MetaKeywords
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@MetaDescription) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CategoryID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'MetaDescription', @MetaDescription
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@MetaTitle) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CategoryID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'MetaTitle', @MetaTitle
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@SEName) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CategoryID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'SEName', @SEName
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalcategorylocalized INTO @OriginalCategoryLocalizedId
END
CLOSE cur_originalcategorylocalized
DEALLOCATE cur_originalcategorylocalized
GO









--MANUFACTURERS
PRINT 'moving manufacturers'
DECLARE @OriginalManufacturerId int
DECLARE cur_originalmanufacturer CURSOR FOR
SELECT ManufacturerId
FROM [Nop_Manufacturer]
ORDER BY [ManufacturerId]
OPEN cur_originalmanufacturer
FETCH NEXT FROM cur_originalmanufacturer INTO @OriginalManufacturerId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving manufacturer. ID ' + cast(@OriginalManufacturerId as nvarchar(10))
	SET IDENTITY_INSERT [Manufacturer] ON
	INSERT INTO [Manufacturer] ([Id], [Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [SeName], [PictureId], [PageSize], [PriceRanges], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [ManufacturerId], [Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [SeName], [PictureId], [PageSize], [PriceRanges], [Published], [Deleted], [DisplayOrder], [CreatedOn], [UpdatedOn]
	FROM [Nop_Manufacturer]
	WHERE ManufacturerId = @OriginalManufacturerId
	SET IDENTITY_INSERT [Manufacturer] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalmanufacturer INTO @OriginalManufacturerId
END
CLOSE cur_originalmanufacturer
DEALLOCATE cur_originalmanufacturer
GO



--LOCALIZED MANUFACTURERS
PRINT 'moving localized manufacturers'
DECLARE @OriginalManufacturerLocalizedId int
DECLARE cur_originalmanufacturerlocalized CURSOR FOR
SELECT ManufacturerLocalizedID
FROM [Nop_ManufacturerLocalized]
ORDER BY [ManufacturerLocalizedID]
OPEN cur_originalmanufacturerlocalized
FETCH NEXT FROM cur_originalmanufacturerlocalized INTO @OriginalManufacturerLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized manufacturer. ID ' + cast(@OriginalManufacturerLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @Description nvarchar(MAX)
	SET @Description = null -- clear cache (variable scope)
	DECLARE @MetaKeywords nvarchar(MAX)
	SET @MetaKeywords = null -- clear cache (variable scope)
	DECLARE @MetaDescription nvarchar(MAX)
	SET @MetaDescription = null -- clear cache (variable scope)
	DECLARE @MetaTitle nvarchar(MAX)
	SET @MetaTitle = null -- clear cache (variable scope)
	DECLARE @SEName nvarchar(MAX)
	SET @SEName = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@Description=[Description],
			@MetaKeywords = [MetaKeywords], 
			@MetaDescription = [MetaDescription], 
			@MetaTitle = [MetaTitle], 
			@SEName = [SEName]
	FROM [Nop_ManufacturerLocalized]
	WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ManufacturerID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'Name', @Name
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@Description) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ManufacturerID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'Description', @Description
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@MetaKeywords) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ManufacturerID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'MetaKeywords', @MetaKeywords
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@MetaDescription) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ManufacturerID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'MetaDescription', @MetaDescription
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@MetaTitle) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ManufacturerID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'MetaTitle', @MetaTitle
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@SEName) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ManufacturerID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'SEName', @SEName
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalmanufacturerlocalized INTO @OriginalManufacturerLocalizedId
END
CLOSE cur_originalmanufacturerlocalized
DEALLOCATE cur_originalmanufacturerlocalized
GO











--PRODUCT TAGS
PRINT 'moving product tags'
DECLARE @OriginalProductTagId int
DECLARE cur_originalproducttag CURSOR FOR
SELECT ProductTagId
FROM [Nop_ProductTag]
ORDER BY [ProductTagId]
OPEN cur_originalproducttag
FETCH NEXT FROM cur_originalproducttag INTO @OriginalProductTagId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product tag. ID ' + cast(@OriginalProductTagId as nvarchar(10))
	SET IDENTITY_INSERT [ProductTag] ON
	INSERT INTO [ProductTag] ([Id], [Name], [ProductCount])
	SELECT [ProductTagId], [Name], [ProductCount]
	FROM [Nop_ProductTag]
	WHERE ProductTagId = @OriginalProductTagId
	SET IDENTITY_INSERT [ProductTag] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalproducttag INTO @OriginalProductTagId
END
CLOSE cur_originalproducttag
DEALLOCATE cur_originalproducttag
GO











--PRODUCTS
PRINT 'moving products'
DECLARE @OriginalProductId int
DECLARE cur_originalproduct CURSOR FOR
SELECT ProductId
FROM [Nop_Product]
ORDER BY [ProductId]
OPEN cur_originalproduct
FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product. ID ' + cast(@OriginalProductId as nvarchar(10))
	SET IDENTITY_INSERT [Product] ON
	INSERT INTO [Product] ([Id], [Name], [ShortDescription], [FullDescription], [AdminComment], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [SeName], [AllowCustomerReviews], [ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [ProductId], [Name], [ShortDescription], [FullDescription], [AdminComment], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [SeName], [AllowCustomerReviews], 0, 0, 0, 0, [Published], [Deleted], [CreatedOn], [UpdatedOn]
	FROM [Nop_Product]
	WHERE ProductId = @OriginalProductId
	SET IDENTITY_INSERT [Product] OFF

	--category mappings
	INSERT INTO [Product_Category_Mapping] ([ProductId], [CategoryId], [IsFeaturedProduct], [DisplayOrder])
	SELECT [ProductId], [CategoryId], [IsFeaturedProduct], [DisplayOrder]
	FROM [Nop_Product_Category_Mapping]
	WHERE ProductId = @OriginalProductId

	--manufacturer mappings
	INSERT INTO [Product_Manufacturer_Mapping] ([ProductId], [ManufacturerId], [IsFeaturedProduct], [DisplayOrder])
	SELECT [ProductId], [ManufacturerId], [IsFeaturedProduct], [DisplayOrder]
	FROM [Nop_Product_Manufacturer_Mapping]
	WHERE ProductId = @OriginalProductId

	--picture mappings
	INSERT INTO [Product_Picture_Mapping] ([ProductId], [PictureId], [DisplayOrder])
	SELECT [ProductId], [PictureId], [DisplayOrder]
	FROM [Nop_ProductPicture]
	WHERE ProductId = @OriginalProductId

	--product tag mappings
	INSERT INTO [Product_ProductTag_Mapping] ([Product_Id], [ProductTag_Id])
	SELECT [ProductId], [ProductTagId]
	FROM [Nop_ProductTag_Product_Mapping]
	WHERE ProductId = @OriginalProductId


	

	--fetch next identifier
	FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
END
CLOSE cur_originalproduct
DEALLOCATE cur_originalproduct
GO


--LOCALIZED PRODUCTS
PRINT 'moving localized products'
DECLARE @OriginalProductLocalizedId int
DECLARE cur_originalproductlocalized CURSOR FOR
SELECT ProductLocalizedID
FROM [Nop_ProductLocalized]
ORDER BY [ProductLocalizedID]
OPEN cur_originalproductlocalized
FETCH NEXT FROM cur_originalproductlocalized INTO @OriginalProductLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized product. ID ' + cast(@OriginalProductLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @ShortDescription nvarchar(MAX)
	SET @ShortDescription = null -- clear cache (variable scope)
	DECLARE @FullDescription nvarchar(MAX)
	SET @FullDescription = null -- clear cache (variable scope)
	DECLARE @MetaKeywords nvarchar(MAX)
	SET @MetaKeywords = null -- clear cache (variable scope)
	DECLARE @MetaDescription nvarchar(MAX)
	SET @MetaDescription = null -- clear cache (variable scope)
	DECLARE @MetaTitle nvarchar(MAX)
	SET @MetaTitle = null -- clear cache (variable scope)
	DECLARE @SEName nvarchar(MAX)
	SET @SEName = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@ShortDescription=[ShortDescription],
			@FullDescription = [FullDescription], 
			@MetaKeywords = [MetaKeywords], 
			@MetaDescription = [MetaDescription], 
			@MetaTitle = [MetaTitle], 
			@SEName = [SEName]
	FROM [Nop_ProductLocalized]
	WHERE [ProductLocalizedID] = @OriginalProductLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'Name', @Name
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@ShortDescription) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'ShortDescription', @ShortDescription
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@FullDescription) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'FullDescription', @FullDescription
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@MetaKeywords) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'MetaKeywords', @MetaKeywords
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@MetaDescription) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'MetaDescription', @MetaDescription
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@MetaTitle) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'MetaTitle', @MetaTitle
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@SEName) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'SEName', @SEName
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductlocalized INTO @OriginalProductLocalizedId
END
CLOSE cur_originalproductlocalized
DEALLOCATE cur_originalproductlocalized
GO











--RELATED PRODUCTS
PRINT 'moving related products'
DECLARE @OriginalRelatedProductId int
DECLARE cur_originalrelatedproduct CURSOR FOR
SELECT RelatedProductId
FROM [Nop_RelatedProduct]
ORDER BY [RelatedProductId]
OPEN cur_originalrelatedproduct
FETCH NEXT FROM cur_originalrelatedproduct INTO @OriginalRelatedProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving related product. ID ' + cast(@OriginalRelatedProductId as nvarchar(10))

	INSERT INTO [RelatedProduct] ([ProductId1], [ProductId2], [DisplayOrder])
	SELECT [ProductID1], [ProductID2], [DisplayOrder]
	FROM [Nop_RelatedProduct]
	WHERE RelatedProductId = @OriginalRelatedProductId
	
	--fetch next identifier
	FETCH NEXT FROM cur_originalrelatedproduct INTO @OriginalRelatedProductId
END
CLOSE cur_originalrelatedproduct
DEALLOCATE cur_originalrelatedproduct
GO











--CROSSSELL PRODUCTS
PRINT 'moving crosssell products'
DECLARE @OriginalCrossSellProductId int
DECLARE cur_originalcrosssellproduct CURSOR FOR
SELECT CrossSellProductId
FROM [Nop_CrossSellProduct]
ORDER BY [CrossSellProductId]
OPEN cur_originalcrosssellproduct
FETCH NEXT FROM cur_originalcrosssellproduct INTO @OriginalCrossSellProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving crosssell product. ID ' + cast(@OriginalCrossSellProductId as nvarchar(10))

	INSERT INTO [CrossSellProduct] ([ProductId1], [ProductId2])
	SELECT [ProductID1], [ProductID2]
	FROM [Nop_CrossSellProduct]
	WHERE CrossSellProductId = @OriginalCrossSellProductId
	
	--fetch next identifier
	FETCH NEXT FROM cur_originalcrosssellproduct INTO @OriginalCrossSellProductId
END
CLOSE cur_originalcrosssellproduct
DEALLOCATE cur_originalcrosssellproduct
GO











--SPECIFICATION ATTRIBUTES
PRINT 'moving specification attributes'
DECLARE @OriginalSpecificationAttributeId int
DECLARE cur_originalspecificationattribute CURSOR FOR
SELECT SpecificationAttributeId
FROM [Nop_SpecificationAttribute]
ORDER BY [SpecificationAttributeId]
OPEN cur_originalspecificationattribute
FETCH NEXT FROM cur_originalspecificationattribute INTO @OriginalSpecificationAttributeId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving specification attribute. ID ' + cast(@OriginalSpecificationAttributeId as nvarchar(10))
	SET IDENTITY_INSERT [SpecificationAttribute] ON
	INSERT INTO [SpecificationAttribute] ([Id], [Name], [DisplayOrder])
	SELECT [SpecificationAttributeId], [Name], [DisplayOrder]
	FROM [Nop_SpecificationAttribute]
	WHERE SpecificationAttributeId = @OriginalSpecificationAttributeId
	SET IDENTITY_INSERT [SpecificationAttribute] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalspecificationattribute INTO @OriginalSpecificationAttributeId
END
CLOSE cur_originalspecificationattribute
DEALLOCATE cur_originalspecificationattribute
GO



--LOCALIZED SPECIFICATION ATTRIBUTE
PRINT 'moving localized specification attributes'
DECLARE @OriginalSpecificationAttributeLocalizedId int
DECLARE cur_originalspecificationattributelocalized CURSOR FOR
SELECT SpecificationAttributeLocalizedID
FROM [Nop_SpecificationAttributeLocalized]
ORDER BY [SpecificationAttributeLocalizedID]
OPEN cur_originalspecificationattributelocalized
FETCH NEXT FROM cur_originalspecificationattributelocalized INTO @OriginalSpecificationAttributeLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized specification attribute. ID ' + cast(@OriginalSpecificationAttributeLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	SELECT  @Name = [Name]
	FROM [Nop_SpecificationAttributeLocalized]
	WHERE [SpecificationAttributeLocalizedID] = @OriginalSpecificationAttributeLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [SpecificationAttributeID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'SpecificationAttribute', 'Name', @Name
		FROM [Nop_SpecificationAttributeLocalized]
		WHERE [SpecificationAttributeLocalizedID] = @OriginalSpecificationAttributeLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalspecificationattributelocalized INTO @OriginalSpecificationAttributeLocalizedId
END
CLOSE cur_originalspecificationattributelocalized
DEALLOCATE cur_originalspecificationattributelocalized
GO











--SPECIFICATION ATTRIBUTE OPTIONS
PRINT 'moving specification attribute options'
DECLARE @OriginalSpecificationAttributeOptionId int
DECLARE cur_originalspecificationattributeoption CURSOR FOR
SELECT SpecificationAttributeOptionId
FROM [Nop_SpecificationAttributeOption]
ORDER BY [SpecificationAttributeOptionId]
OPEN cur_originalspecificationattributeoption
FETCH NEXT FROM cur_originalspecificationattributeoption INTO @OriginalSpecificationAttributeOptionId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving specification attribute option. ID ' + cast(@OriginalSpecificationAttributeOptionId as nvarchar(10))
	SET IDENTITY_INSERT [SpecificationAttributeOption] ON
	INSERT INTO [SpecificationAttributeOption] ([Id], [SpecificationAttributeId], [Name], [DisplayOrder])
	SELECT [SpecificationAttributeOptionId], [SpecificationAttributeId], [Name], [DisplayOrder]
	FROM [Nop_SpecificationAttributeOption]
	WHERE SpecificationAttributeOptionId = @OriginalSpecificationAttributeOptionId
	SET IDENTITY_INSERT [SpecificationAttributeOption] OFF

	--product mapping
	INSERT INTO [Product_SpecificationAttribute_Mapping] ([ProductId], [SpecificationAttributeOptionId], [AllowFiltering], [ShowOnProductPage], [DisplayOrder])
	SELECT [ProductId], [SpecificationAttributeOptionId], [AllowFiltering], [ShowOnProductPage], [DisplayOrder]
	FROM [Nop_Product_SpecificationAttribute_Mapping]
	WHERE SpecificationAttributeOptionId = @OriginalSpecificationAttributeOptionId


	--fetch next identifier
	FETCH NEXT FROM cur_originalspecificationattributeoption INTO @OriginalSpecificationAttributeOptionId
END
CLOSE cur_originalspecificationattributeoption
DEALLOCATE cur_originalspecificationattributeoption
GO


--LOCALIZED SPECIFICATION ATTRIBUTE OPTION
PRINT 'moving localized specification attribute option'
DECLARE @OriginalSpecificationAttributeOptionLocalizedId int
DECLARE cur_originalspecificationattributeoptionlocalized CURSOR FOR
SELECT SpecificationAttributeOptionLocalizedID
FROM [Nop_SpecificationAttributeOptionLocalized]
ORDER BY [SpecificationAttributeOptionLocalizedID]
OPEN cur_originalspecificationattributeoptionlocalized
FETCH NEXT FROM cur_originalspecificationattributeoptionlocalized INTO @OriginalSpecificationAttributeOptionLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized product variant attribute option. ID ' + cast(@OriginalSpecificationAttributeOptionLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	SELECT  @Name = [Name]
	FROM [Nop_SpecificationAttributeOptionLocalized]
	WHERE [SpecificationAttributeOptionLocalizedID] = @OriginalSpecificationAttributeOptionLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [SpecificationAttributeOptionID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'SpecificationAttributeOption', 'Name', @Name
		FROM [Nop_SpecificationAttributeOptionLocalized]
		WHERE [SpecificationAttributeOptionLocalizedID] = @OriginalSpecificationAttributeOptionLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalspecificationattributeoptionlocalized INTO @OriginalSpecificationAttributeOptionLocalizedId
END
CLOSE cur_originalspecificationattributeoptionlocalized
DEALLOCATE cur_originalspecificationattributeoptionlocalized
GO











--CHECKOUT ATTRIBUTES
PRINT 'moving checkout attributes'
DECLARE @OriginalCheckoutAttributeId int
DECLARE cur_originalcheckoutattribute CURSOR FOR
SELECT CheckoutAttributeId
FROM [Nop_CheckoutAttribute]
ORDER BY [CheckoutAttributeId]
OPEN cur_originalcheckoutattribute
FETCH NEXT FROM cur_originalcheckoutattribute INTO @OriginalCheckoutAttributeId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving checkout attribute. ID ' + cast(@OriginalCheckoutAttributeId as nvarchar(10))
	SET IDENTITY_INSERT [CheckoutAttribute] ON
	INSERT INTO [CheckoutAttribute] ([Id], [Name], [TextPrompt], [IsRequired], [ShippableProductRequired], [IsTaxExempt], [TaxCategoryID], [AttributeControlTypeID], [DisplayOrder])
	SELECT [CheckoutAttributeId], [Name], [TextPrompt], [IsRequired], [ShippableProductRequired], [IsTaxExempt], [TaxCategoryID], [AttributeControlTypeID], [DisplayOrder]
	FROM [Nop_CheckoutAttribute]
	WHERE CheckoutAttributeId = @OriginalCheckoutAttributeId
	SET IDENTITY_INSERT [CheckoutAttribute] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalcheckoutattribute INTO @OriginalCheckoutAttributeId
END
CLOSE cur_originalcheckoutattribute
DEALLOCATE cur_originalcheckoutattribute
GO



--LOCALIZED CHECKOUT ATTRIBUTES
PRINT 'moving localized checkout attributes'
DECLARE @OriginalCheckoutAttributeLocalizedId int
DECLARE cur_originalcheckoutattributelocalized CURSOR FOR
SELECT CheckoutAttributeLocalizedID
FROM [Nop_CheckoutAttributeLocalized]
ORDER BY [CheckoutAttributeLocalizedID]
OPEN cur_originalcheckoutattributelocalized
FETCH NEXT FROM cur_originalcheckoutattributelocalized INTO @OriginalCheckoutAttributeLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized checkout attribute. ID ' + cast(@OriginalCheckoutAttributeLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @TextPrompt nvarchar(MAX)
	SET @TextPrompt = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@TextPrompt=[TextPrompt]
	FROM [Nop_CheckoutAttributeLocalized]
	WHERE [CheckoutAttributeLocalizedID] = @OriginalCheckoutAttributeLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CheckoutAttributeID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'CheckoutAttribute', 'Name', @Name
		FROM [Nop_CheckoutAttributeLocalized]
		WHERE [CheckoutAttributeLocalizedID] = @OriginalCheckoutAttributeLocalizedId
	END

	IF (len(@TextPrompt) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CheckoutAttributeID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'CheckoutAttribute', 'TextPrompt', @TextPrompt
		FROM [Nop_CheckoutAttributeLocalized]
		WHERE [CheckoutAttributeLocalizedID] = @OriginalCheckoutAttributeLocalizedId
	END


	--fetch next identifier
	FETCH NEXT FROM cur_originalcheckoutattributelocalized INTO @OriginalCheckoutAttributeLocalizedId
END
CLOSE cur_originalcheckoutattributelocalized
DEALLOCATE cur_originalcheckoutattributelocalized
GO











--CHECKOUT ATTRIBUTE VALUES
PRINT 'moving checkout attribute values'
DECLARE @OriginalCheckoutAttributeValueId int
DECLARE cur_originalcheckoutattributevalue CURSOR FOR
SELECT CheckoutAttributeValueId
FROM [Nop_CheckoutAttributeValue]
ORDER BY [CheckoutAttributeValueId]
OPEN cur_originalcheckoutattributevalue
FETCH NEXT FROM cur_originalcheckoutattributevalue INTO @OriginalCheckoutAttributeValueId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving checkout attribute value. ID ' + cast(@OriginalCheckoutAttributeValueId as nvarchar(10))
	SET IDENTITY_INSERT [CheckoutAttributeValue] ON
	INSERT INTO [CheckoutAttributeValue] ([Id], [CheckoutAttributeId], [Name], [PriceAdjustment], [WeightAdjustment], [IsPreSelected], [DisplayOrder])
	SELECT [CheckoutAttributeValueId], [CheckoutAttributeId], [Name], [PriceAdjustment], [WeightAdjustment], [IsPreSelected], [DisplayOrder]
	FROM [Nop_CheckoutAttributeValue]
	WHERE CheckoutAttributeValueId = @OriginalCheckoutAttributeValueId
	SET IDENTITY_INSERT [CheckoutAttributeValue] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalcheckoutattributevalue INTO @OriginalCheckoutAttributeValueId
END
CLOSE cur_originalcheckoutattributevalue
DEALLOCATE cur_originalcheckoutattributevalue
GO



--LOCALIZED CHECKOUT ATTRIBUTE VALUE
PRINT 'moving localized checkout attribute value'
DECLARE @OriginalCheckoutAttributeValueLocalizedId int
DECLARE cur_originalcheckoutattributevaluelocalized CURSOR FOR
SELECT CheckoutAttributeValueLocalizedID
FROM [Nop_CheckoutAttributeValueLocalized]
ORDER BY [CheckoutAttributeValueLocalizedID]
OPEN cur_originalcheckoutattributevaluelocalized
FETCH NEXT FROM cur_originalcheckoutattributevaluelocalized INTO @OriginalCheckoutAttributeValueLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized checkout attribute value. ID ' + cast(@OriginalCheckoutAttributeValueLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	SELECT  @Name = [Name]
	FROM [Nop_CheckoutAttributeValueLocalized]
	WHERE [CheckoutAttributeValueLocalizedID] = @OriginalCheckoutAttributeValueLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [CheckoutAttributeValueID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'CheckoutAttributeValue', 'Name', @Name
		FROM [Nop_CheckoutAttributeValueLocalized]
		WHERE [CheckoutAttributeValueLocalizedID] = @OriginalCheckoutAttributeValueLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalcheckoutattributevaluelocalized INTO @OriginalCheckoutAttributeValueLocalizedId
END
CLOSE cur_originalcheckoutattributevaluelocalized
DEALLOCATE cur_originalcheckoutattributevaluelocalized
GO











--PRODUCT VARIANTS
PRINT 'moving product variants'
DECLARE @OriginalProductVariantId int
DECLARE cur_originalproductvariant CURSOR FOR
SELECT ProductVariantId
FROM [Nop_ProductVariant]
ORDER BY [ProductVariantId]
OPEN cur_originalproductvariant
FETCH NEXT FROM cur_originalproductvariant INTO @OriginalProductVariantId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product variant. ID ' + cast(@OriginalProductVariantId as nvarchar(10))
	SET IDENTITY_INSERT [ProductVariant] ON
	INSERT INTO [ProductVariant] ([Id], [ProductId], [Name], [Sku], [Description], [AdminComment], [ManufacturerPartNumber], [IsGiftCard], [GiftCardTypeId], [IsDownload], [DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload], [SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles], [IsShipEnabled], [IsFreeShipping], [AdditionalShippingCharge], [IsTaxExempt], [TaxCategoryId], [ManageInventoryMethodId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity], [MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [OrderMinimumQuantity], [OrderMaximumQuantity], [DisableBuyButton], [CallForPrice], [Price], [OldPrice], [ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [Weight], [Length], [Width], [Height], [PictureId], [AvailableStartDateTimeUtc], [AvailableEndDateTimeUtc], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [ProductVariantId], [ProductId], [Name], [Sku], [Description], [AdminComment], [ManufacturerPartNumber], [IsGiftCard], [GiftCardType], [IsDownload], [DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationType], [HasSampleDownload], [SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [CycleLength], [CyclePeriod], [TotalCycles], [IsShipEnabled], [IsFreeShipping], [AdditionalShippingCharge], [IsTaxExempt], [TaxCategoryID], [ManageInventory], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity], [MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [Backorders], [OrderMinimumQuantity], [OrderMaximumQuantity], [DisableBuyButton], [CallForPrice], [Price], [OldPrice], [ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [Weight], [Length], [Width], [Height], [PictureId], [AvailableStartDateTime], [AvailableEndDateTime], [Published], [Deleted], [DisplayOrder], [CreatedOn], [UpdatedOn]
	FROM [Nop_ProductVariant]
	WHERE ProductVariantId = @OriginalProductVariantId
	SET IDENTITY_INSERT [ProductVariant] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvariant INTO @OriginalProductVariantId
END
CLOSE cur_originalproductvariant
DEALLOCATE cur_originalproductvariant
GO



--LOCALIZED PRODUCTS
PRINT 'moving localized product variants'
DECLARE @OriginalProductVariantLocalizedId int
DECLARE cur_originalproductvarriantlocalized CURSOR FOR
SELECT ProductVariantLocalizedID
FROM [Nop_ProductVariantLocalized]
ORDER BY [ProductVariantLocalizedID]
OPEN cur_originalproductvarriantlocalized
FETCH NEXT FROM cur_originalproductvarriantlocalized INTO @OriginalProductVariantLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized product variant. ID ' + cast(@OriginalProductVariantLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @Description nvarchar(MAX)
	SET @Description = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@Description=[Description]
	FROM [Nop_ProductVariantLocalized]
	WHERE [ProductVariantLocalizedID] = @OriginalProductVariantLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductVariantID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'ProductVariant', 'Name', @Name
		FROM [Nop_ProductVariantLocalized]
		WHERE [ProductVariantLocalizedID] = @OriginalProductVariantLocalizedId
	END

	IF (len(@Description) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductVariantID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'ProductVariant', 'Description', @Description
		FROM [Nop_ProductVariantLocalized]
		WHERE [ProductVariantLocalizedID] = @OriginalProductVariantLocalizedId
	END


	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvarriantlocalized INTO @OriginalProductVariantLocalizedId
END
CLOSE cur_originalproductvarriantlocalized
DEALLOCATE cur_originalproductvarriantlocalized
GO










--PRODUCT ATTRIBUTES
PRINT 'moving product attributes'
DECLARE @OriginalProductAttributeId int
DECLARE cur_originalproductattribute CURSOR FOR
SELECT ProductAttributeId
FROM [Nop_ProductAttribute]
ORDER BY [ProductAttributeId]
OPEN cur_originalproductattribute
FETCH NEXT FROM cur_originalproductattribute INTO @OriginalProductAttributeId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product attribute. ID ' + cast(@OriginalProductAttributeId as nvarchar(10))
	SET IDENTITY_INSERT [ProductAttribute] ON
	INSERT INTO [ProductAttribute] ([Id], [Name], [Description])
	SELECT [ProductAttributeId], [Name], [Description]
	FROM [Nop_ProductAttribute]
	WHERE ProductAttributeId = @OriginalProductAttributeId
	SET IDENTITY_INSERT [ProductAttribute] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductattribute INTO @OriginalProductAttributeId
END
CLOSE cur_originalproductattribute
DEALLOCATE cur_originalproductattribute
GO



--LOCALIZED PRODUCT ATTRIBUTES
PRINT 'moving localized product attribute'
DECLARE @OriginalProductAttributeLocalizedId int
DECLARE cur_originalproductattributelocalized CURSOR FOR
SELECT ProductAttributeLocalizedID
FROM [Nop_ProductAttributeLocalized]
ORDER BY [ProductAttributeLocalizedID]
OPEN cur_originalproductattributelocalized
FETCH NEXT FROM cur_originalproductattributelocalized INTO @OriginalProductAttributeLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized product attribute. ID ' + cast(@OriginalProductAttributeLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @Description nvarchar(MAX)
	SET @Description = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@Description=[Description]
	FROM [Nop_ProductAttributeLocalized]
	WHERE [ProductAttributeLocalizedID] = @OriginalProductAttributeLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductAttributeID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'ProductAttribute', 'Name', @Name
		FROM [Nop_ProductAttributeLocalized]
		WHERE [ProductAttributeLocalizedID] = @OriginalProductAttributeLocalizedId
	END

	IF (len(@Description) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductAttributeID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'ProductAttribute', 'Description', @Description
		FROM [Nop_ProductAttributeLocalized]
		WHERE [ProductAttributeLocalizedID] = @OriginalProductAttributeLocalizedId
	END


	--fetch next identifier
	FETCH NEXT FROM cur_originalproductattributelocalized INTO @OriginalProductAttributeLocalizedId
END
CLOSE cur_originalproductattributelocalized
DEALLOCATE cur_originalproductattributelocalized
GO












--PRODUCT VARIANT ATTRIBUTES (MAPPING)
PRINT 'moving product variant attributes'
DECLARE @OriginalProductVariantAttributeId int
DECLARE cur_originalproductvariantattribute CURSOR FOR
SELECT ProductVariantAttributeID
FROM [Nop_ProductVariant_ProductAttribute_Mapping]
ORDER BY [ProductVariantAttributeID]
OPEN cur_originalproductvariantattribute
FETCH NEXT FROM cur_originalproductvariantattribute INTO @OriginalProductVariantAttributeId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product variant attribute. ID ' + cast(@OriginalProductVariantAttributeId as nvarchar(10))
	SET IDENTITY_INSERT [ProductVariant_ProductAttribute_Mapping] ON
	INSERT INTO [ProductVariant_ProductAttribute_Mapping] ([Id], [ProductVariantId], [ProductAttributeId], [TextPrompt], [IsRequired], [AttributeControlTypeId], [DisplayOrder])
	SELECT [ProductVariantAttributeID], [ProductVariantId], [ProductAttributeId], [TextPrompt], [IsRequired], [AttributeControlTypeId], [DisplayOrder]
	FROM [Nop_ProductVariant_ProductAttribute_Mapping]
	WHERE ProductVariantAttributeID = @OriginalProductVariantAttributeId
	SET IDENTITY_INSERT [ProductVariant_ProductAttribute_Mapping] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvariantattribute INTO @OriginalProductVariantAttributeId
END
CLOSE cur_originalproductvariantattribute
DEALLOCATE cur_originalproductvariantattribute
GO











--PRODUCT VARIANT ATTRIBUTE VALUES
PRINT 'moving product variant attribute values'
DECLARE @OriginalProductVariantAttributeValueId int
DECLARE cur_originalproductvariantattributevalue CURSOR FOR
SELECT ProductVariantAttributeValueId
FROM [Nop_ProductVariantAttributeValue]
ORDER BY [ProductVariantAttributeValueId]
OPEN cur_originalproductvariantattributevalue
FETCH NEXT FROM cur_originalproductvariantattributevalue INTO @OriginalProductVariantAttributeValueId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product variant attribute value. ID ' + cast(@OriginalProductVariantAttributeValueId as nvarchar(10))
	SET IDENTITY_INSERT [ProductVariantAttributeValue] ON
	INSERT INTO [ProductVariantAttributeValue] ([Id], [ProductVariantAttributeId], [Name], [PriceAdjustment], [WeightAdjustment], [IsPreSelected], [DisplayOrder])
	SELECT [ProductVariantAttributeValueId], [ProductVariantAttributeId], [Name], [PriceAdjustment], [WeightAdjustment], [IsPreSelected], [DisplayOrder]
	FROM [Nop_ProductVariantAttributeValue]
	WHERE ProductVariantAttributeValueId = @OriginalProductVariantAttributeValueId
	SET IDENTITY_INSERT [ProductVariantAttributeValue] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvariantattributevalue INTO @OriginalProductVariantAttributeValueId
END
CLOSE cur_originalproductvariantattributevalue
DEALLOCATE cur_originalproductvariantattributevalue
GO



--LOCALIZED PRODUCT VARIANT ATTRIBUTE VALUE
PRINT 'moving localized product variant attribute value'
DECLARE @OriginalProductVariantAttributeValueLocalizedId int
DECLARE cur_originalproductvariantattributevaluelocalized CURSOR FOR
SELECT ProductVariantAttributeValueLocalizedID
FROM [Nop_ProductVariantAttributeValueLocalized]
ORDER BY [ProductVariantAttributeValueLocalizedID]
OPEN cur_originalproductvariantattributevaluelocalized
FETCH NEXT FROM cur_originalproductvariantattributevaluelocalized INTO @OriginalProductVariantAttributeValueLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized product variant attribute value. ID ' + cast(@OriginalProductVariantAttributeValueLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	SELECT  @Name = [Name]
	FROM [Nop_ProductVariantAttributeValueLocalized]
	WHERE [ProductVariantAttributeValueLocalizedID] = @OriginalProductVariantAttributeValueLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO [LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT [ProductVariantAttributeValueID], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'ProductVariantAttributeValue', 'Name', @Name
		FROM [Nop_ProductVariantAttributeValueLocalized]
		WHERE [ProductVariantAttributeValueLocalizedID] = @OriginalProductVariantAttributeValueLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvariantattributevaluelocalized INTO @OriginalProductVariantAttributeValueLocalizedId
END
CLOSE cur_originalproductvariantattributevaluelocalized
DEALLOCATE cur_originalproductvariantattributevaluelocalized
GO











--PRODUCT VARIANT ATTRIBUTE COMBINATIONS
PRINT 'moving product variant attribute combinations'
DECLARE @OriginalProductVariantAttributeCombinationId int
DECLARE cur_originalproductvariantattributecombination CURSOR FOR
SELECT ProductVariantAttributeCombinationId
FROM [Nop_ProductVariantAttributeCombination]
ORDER BY [ProductVariantAttributeCombinationId]
OPEN cur_originalproductvariantattributecombination
FETCH NEXT FROM cur_originalproductvariantattributecombination INTO @OriginalProductVariantAttributeCombinationId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product variant attribute combination. ID ' + cast(@OriginalProductVariantAttributeCombinationId as nvarchar(10))
	SET IDENTITY_INSERT [ProductVariantAttributeCombination] ON
	INSERT INTO [ProductVariantAttributeCombination] ([Id], [ProductVariantID], [AttributesXML], [StockQuantity], [AllowOutOfStockOrders])
	SELECT [ProductVariantAttributeCombinationId], [ProductVariantID], cast([AttributesXML] as nvarchar(MAX)), [StockQuantity], [AllowOutOfStockOrders]
	FROM [Nop_ProductVariantAttributeCombination]
	WHERE ProductVariantAttributeCombinationId = @OriginalProductVariantAttributeCombinationId
	SET IDENTITY_INSERT [ProductVariantAttributeCombination] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvariantattributecombination INTO @OriginalProductVariantAttributeCombinationId
END
CLOSE cur_originalproductvariantattributecombination
DEALLOCATE cur_originalproductvariantattributecombination
GO











--TIER PRICES
PRINT 'moving tier prices'
DECLARE @OriginalTierPriceId int
DECLARE cur_originaltierprice CURSOR FOR
SELECT TierPriceId
FROM [Nop_TierPrice]
ORDER BY [TierPriceId]
OPEN cur_originaltierprice
FETCH NEXT FROM cur_originaltierprice INTO @OriginalTierPriceId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving tier price. ID ' + cast(@OriginalTierPriceId as nvarchar(10))
	SET IDENTITY_INSERT [TierPrice] ON
	INSERT INTO [TierPrice] ([Id], [ProductVariantId], [Quantity], [Price])
	SELECT [TierPriceId], [ProductVariantID], [Quantity], [Price]
	FROM [Nop_TierPrice]
	WHERE TierPriceId = @OriginalTierPriceId
	SET IDENTITY_INSERT [TierPrice] OFF

	--fetch next identifier
	FETCH NEXT FROM cur_originaltierprice INTO @OriginalTierPriceId
END
CLOSE cur_originaltierprice
DEALLOCATE cur_originaltierprice
GO











--DISCOUNTS
--first delete already [Deleted] discounts
DELETE FROM [Nop_Discount]
WHERE [Deleted]=1
GO
--move discounts
PRINT 'moving discounts'
DECLARE @OriginalDiscountId int
DECLARE cur_originaldiscount CURSOR FOR
SELECT DiscountId
FROM [Nop_Discount]
WHERE Deleted=0
ORDER BY [DiscountId]
OPEN cur_originaldiscount
FETCH NEXT FROM cur_originaldiscount INTO @OriginalDiscountId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving discount. ID ' + cast(@OriginalDiscountId as nvarchar(10))

	DECLARE @NewDiscountLimitationId int
	SET @NewDiscountLimitationId = null -- clear cache (variable scope)
	DECLARE @NewLimitationTimes int
	SET @NewLimitationTimes = null -- clear cache (variable scope)
	SELECT @NewDiscountLimitationId = [DiscountLimitationId], @NewLimitationTimes = [LimitationTimes]
	FROM [Nop_Discount]
	WHERE DiscountId = @OriginalDiscountId
	--we removed the following discount limitation types: OneTimeOnly, OneTimePerCustomer
	IF (@NewDiscountLimitationId = 10) -- OneTimeOnly
	BEGIN
		SET @NewDiscountLimitationId  = 15
		SET @NewLimitationTimes  = 1
	END
	IF (@NewDiscountLimitationId = 20) -- OneTimePerCustomer
	BEGIN
		SET @NewDiscountLimitationId  = 25
		SET @NewLimitationTimes  = 1
	END

	SET IDENTITY_INSERT [Discount] ON
	INSERT INTO [Discount] ([Id], [Name], [DiscountTypeId], [UsePercentage], [DiscountPercentage], [DiscountAmount], [StartDateUtc], [EndDateUtc], [RequiresCouponCode], [CouponCode], [DiscountLimitationId], [LimitationTimes])
	SELECT [DiscountId], [Name], [DiscountTypeId], [UsePercentage], [DiscountPercentage], [DiscountAmount], [StartDate], [EndDate], [RequiresCouponCode], [CouponCode], @NewDiscountLimitationId, @NewLimitationTimes
	FROM [Nop_Discount]
	WHERE DiscountId = @OriginalDiscountId
	SET IDENTITY_INSERT [Discount] OFF

	--category mappings
	INSERT INTO [Discount_AppliedToCategories] ([Discount_Id], [Category_Id])
	SELECT [DiscountId], [CategoryID]
	FROM [Nop_Category_Discount_Mapping]
	WHERE DiscountID = @OriginalDiscountId

	--product variant mappings
	INSERT INTO [Discount_AppliedToProductVariants] ([Discount_Id], [ProductVariant_Id])
	SELECT [DiscountId], [ProductVariantID]
	FROM [Nop_ProductVariant_Discount_Mapping]
	WHERE DiscountID = @OriginalDiscountId



	--fetch next identifier
	FETCH NEXT FROM cur_originaldiscount INTO @OriginalDiscountId
END
CLOSE cur_originaldiscount
DEALLOCATE cur_originaldiscount
GO











--ORDERS
PRINT 'moving orders'
DECLARE @OriginalOrderId int
DECLARE cur_originalorder CURSOR FOR
SELECT OrderId
FROM [Nop_Order]
ORDER BY [OrderId]
OPEN cur_originalorder
FETCH NEXT FROM cur_originalorder INTO @OriginalOrderId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving order. ID ' + cast(@OriginalOrderId as nvarchar(10))

	--set @PaymentMethodSystemName
	--it's set according to IDs in old [Nop_PaymentMethod] table
	DECLARE @PaymentMethodSystemName nvarchar(100)
	SET @PaymentMethodSystemName = null -- clear cache (variable scope)
	DECLARE @OldPaymentMethodId nvarchar(100)
	SET @OldPaymentMethodId = null -- clear cache (variable scope)
	SELECT @OldPaymentMethodId = [PaymentMethodId]
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	SELECT @PaymentMethodSystemName = CASE @OldPaymentMethodId
	WHEN 9 THEN N'Payments.AuthorizeNet' 
	WHEN 15 THEN N'Payments.CashOnDelivery' 
	WHEN 17 THEN N'Payments.CheckMoneyOrder' 
	WHEN 14 THEN N'Payments.GoogleCheckout' 
	WHEN 1 THEN N'Payments.Manual' 
	WHEN 25 THEN N'Payments.PayInStore'
	WHEN 7 THEN N'Payments.PayPalDirect'
	WHEN 2 THEN N'Payments.PayPalStandard'
	WHEN 18 THEN N'Payments.PurchaseOrder'
	END
	

	--calculate exchange rate
	DECLARE @CurrencyRate decimal(18, 4)
	SET @CurrencyRate = null -- clear cache (variable scope)
	DECLARE @OldOrderTotalInCustomerCurrency decimal(18, 4)
	DECLARE @OldOrderTotal decimal(18, 4)
	SELECT @OldOrderTotalInCustomerCurrency=[OrderTotalInCustomerCurrency],
		@OldOrderTotal=[OrderTotal]
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	IF (@OldOrderTotalInCustomerCurrency > 0 and @OldOrderTotal > 0)
	BEGIN
		--use order total
		SET @CurrencyRate = @OldOrderTotalInCustomerCurrency / @OldOrderTotal
	END
	ELSE 
	BEGIN
		--order total can be 0. in this case let's use subtotal
		DECLARE @OldOrderSubTotalInCustomerCurrency decimal(18, 4)
		DECLARE @OldOrderSubTotal decimal(18, 4)
		SELECT @OldOrderSubTotalInCustomerCurrency=[OrderSubTotalInclTaxInCustomerCurrency],
			@OldOrderSubTotal=[OrderSubTotalInclTax]
		FROM [Nop_Order]
		WHERE OrderId = @OriginalOrderId
		IF (@OldOrderSubTotalInCustomerCurrency > 0 and @OldOrderSubTotal > 0)
		BEGIN
			--use order subtotal
			SET @CurrencyRate = @OldOrderSubTotalInCustomerCurrency / @OldOrderSubTotal
		END
		ELSE 
		BEGIN
			--order total can be 0. in this case let's use subtotal
			DECLARE @OldOrderShippingInCustomerCurrency decimal(18, 4)
			DECLARE @OldOrderShipping decimal(18, 4)
			SELECT @OldOrderShippingInCustomerCurrency=[OrderShippingInclTaxInCustomerCurrency],
				@OldOrderShipping=[OrderShippingInclTax]
			FROM [Nop_Order]
			WHERE OrderId = @OriginalOrderId			
			IF (@OldOrderShippingInCustomerCurrency > 0 and @OldOrderShipping > 0)
			BEGIN
				SET @CurrencyRate = @OldOrderShippingInCustomerCurrency / @OldOrderShipping
			END
		END
	END
	--some exchange rate validation
	IF (@CurrencyRate is null or @CurrencyRate = 0)
	BEGIN
		SET @CurrencyRate = 1
	END

	--TODO set @ShippingRateComputationMethodSystemName (although it's not used)
	DECLARE @ShippingRateComputationMethodSystemName  nvarchar(100)
	SET @ShippingRateComputationMethodSystemName = null -- clear cache (variable scope)

	--insert billing address (now stored into [Address] table
	DECLARE @BillingAddressId int
	SET @BillingAddressId = null -- clear cache (variable scope)
	INSERT INTO [Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
	SELECT [BillingFirstName], [BillingLastName], [BillingPhoneNumber], [BillingEmail], [BillingFaxNumber], [BillingCompany], [BillingAddress1], [BillingAddress2], [BillingCity], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'StateProvince' and [OriginalId]=[BillingStateProvinceID]), [BillingZipPostalCode], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Country' and [OriginalId]=[BillingCountryID]), getutcdate()
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	SET @BillingAddressId = @@IDENTITY

	--insert shipping address
	DECLARE @ShippingStatusId int
	SELECT @ShippingStatusId = ShippingStatusId
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId

	DECLARE @ShippingAddressId int
	SET @ShippingAddressId = null -- clear cache (variable scope)

	IF (@ShippingStatusId <> 10)
	BEGIN
		--shipping is required
		INSERT INTO [Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
		SELECT [ShippingFirstName], [ShippingLastName], [ShippingPhoneNumber], [ShippingEmail], [ShippingFaxNumber], [ShippingCompany], [ShippingAddress1], [ShippingAddress2], [ShippingCity], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'StateProvince' and [OriginalId]=[ShippingStateProvinceID]), [ShippingZipPostalCode], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Country' and [OriginalId]=[ShippingCountryID]), getutcdate()
		FROM [Nop_Order]
		WHERE OrderId = @OriginalOrderId
		SET @ShippingAddressId = @@IDENTITY
	END

	--customer tax display type
	DECLARE @CustomerTaxDisplayTypeId int
	SET @CustomerTaxDisplayTypeId = null -- clear cache (variable scope)
	SELECT @CustomerTaxDisplayTypeId=[CustomerTaxDisplayTypeId]
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	IF (@CustomerTaxDisplayTypeId = 1)
	BEGIN
		-- Including tax
		SET @CustomerTaxDisplayTypeId = 0 --now 0
	END
	ELSE 
	BEGIN
		-- Excluding tax
		SET @CustomerTaxDisplayTypeId = 10 --now 10
	END


	SET IDENTITY_INSERT [Order] ON	
	INSERT INTO [Order] ([Id], [OrderGuid], [CustomerId], [OrderStatusId], [ShippingStatusId], [PaymentStatusId], [PaymentMethodSystemName], [CustomerCurrencyCode], [CurrencyRate], [CustomerTaxDisplayTypeId], [VatNumber], [OrderSubtotalInclTax], [OrderSubtotalExclTax], [OrderSubTotalDiscountInclTax], [OrderSubTotalDiscountExclTax], [OrderShippingInclTax], [OrderShippingExclTax], [PaymentMethodAdditionalFeeInclTax], [PaymentMethodAdditionalFeeExclTax], [TaxRates], [OrderTax], [OrderDiscount], [OrderTotal], [RefundedAmount], [CheckoutAttributeDescription], [CheckoutAttributesXml], [CustomerLanguageId], [AffiliateId], [CustomerIp], [AllowStoringCreditCardNumber], [CardType], [CardName], [CardNumber], [MaskedCreditCardNumber], [CardCvv2], [CardExpirationMonth], [CardExpirationYear], [AuthorizationTransactionId], [AuthorizationTransactionCode], [AuthorizationTransactionResult], [CaptureTransactionId], [CaptureTransactionResult], [SubscriptionTransactionId], [PurchaseOrderNumber], [PaidDateUtc], [ShippingMethod], [ShippingRateComputationMethodSystemName], [ShippedDateUtc], [DeliveryDateUtc], [OrderWeight], [TrackingNumber], [Deleted], [CreatedOnUtc], [BillingAddressId], [ShippingAddressId])
	SELECT [OrderId], [OrderGuid], [CustomerId], [OrderStatusId], [ShippingStatusId], [PaymentStatusId], @PaymentMethodSystemName, [CustomerCurrencyCode], @CurrencyRate, @CustomerTaxDisplayTypeId, [VatNumber], [OrderSubtotalInclTax], [OrderSubtotalExclTax], [OrderSubTotalDiscountInclTax], [OrderSubTotalDiscountExclTax], [OrderShippingInclTax], [OrderShippingExclTax], [PaymentMethodAdditionalFeeInclTax], [PaymentMethodAdditionalFeeExclTax], [TaxRates], [OrderTax], [OrderDiscount], [OrderTotal], [RefundedAmount], [CheckoutAttributeDescription], cast([CheckoutAttributesXml] as nvarchar(MAX)), COALESCE((SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[CustomerLanguageId]), 0), (SELECT CASE WHEN [AffiliateId]=0 THEN NULL ELSE [AffiliateId] END), [CustomerIp], [AllowStoringCreditCardNumber], [CardType], [CardName], [CardNumber], [MaskedCreditCardNumber], [CardCvv2], [CardExpirationMonth], [CardExpirationYear], [AuthorizationTransactionId], [AuthorizationTransactionCode], [AuthorizationTransactionResult], [CaptureTransactionId], [CaptureTransactionResult], [SubscriptionTransactionId], [PurchaseOrderNumber], [PaidDate], [ShippingMethod], @ShippingRateComputationMethodSystemName, [ShippedDate], [DeliveryDate], [OrderWeight], [TrackingNumber], [Deleted], [CreatedOn], @BillingAddressId, @ShippingAddressId
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	SET IDENTITY_INSERT [Order] OFF
	
	--fetch next identifier
	FETCH NEXT FROM cur_originalorder INTO @OriginalOrderId
END
CLOSE cur_originalorder
DEALLOCATE cur_originalorder
GO











--ORDER PRODUCT VARIANTS
PRINT 'moving order product variants'
DECLARE @OriginalOrderProductVariantId int
DECLARE cur_originalorderproductvariant CURSOR FOR
SELECT OrderProductVariantId
FROM [Nop_OrderProductVariant]
ORDER BY [OrderProductVariantId]
OPEN cur_originalorderproductvariant
FETCH NEXT FROM cur_originalorderproductvariant INTO @OriginalOrderProductVariantId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving order product variant. ID ' + cast(@OriginalOrderProductVariantId as nvarchar(10))
	
	INSERT INTO [OrderProductVariant] ([OrderProductVariantGuid], [OrderId], [ProductVariantId], [Quantity], [UnitPriceInclTax], [UnitPriceExclTax], [PriceInclTax], [PriceExclTax], [DiscountAmountInclTax], [DiscountAmountExclTax], [AttributeDescription], [AttributesXml], [DownloadCount], [IsDownloadActivated], [LicenseDownloadId])
	SELECT [OrderProductVariantGuid], [OrderId], [ProductVariantId], [Quantity], [UnitPriceInclTax], [UnitPriceExclTax], [PriceInclTax], [PriceExclTax], [DiscountAmountInclTax], [DiscountAmountExclTax], [AttributeDescription], cast([AttributesXml] as nvarchar(MAX)), [DownloadCount], [IsDownloadActivated], (SELECT CASE WHEN [LicenseDownloadId]=0 THEN NULL ELSE [LicenseDownloadId] END)
	FROM [Nop_OrderProductVariant]
	WHERE OrderProductVariantId = @OriginalOrderProductVariantId

	--new ID
	DECLARE @NewOrderProductVariantId int
	SET @NewOrderProductVariantId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalOrderProductVariantId, @NewOrderProductVariantId, N'OrderProductVariant')
	--fetch next identifier
	FETCH NEXT FROM cur_originalorderproductvariant INTO @OriginalOrderProductVariantId
END
CLOSE cur_originalorderproductvariant
DEALLOCATE cur_originalorderproductvariant
GO











--ORDER NOTES
PRINT 'moving order notes'
DECLARE @OriginalOrderNoteId int
DECLARE cur_originalordernote CURSOR FOR
SELECT OrderNoteId
FROM [Nop_OrderNote]
ORDER BY [OrderNoteId]
OPEN cur_originalordernote
FETCH NEXT FROM cur_originalordernote INTO @OriginalOrderNoteId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving order note. ID ' + cast(@OriginalOrderNoteId as nvarchar(10))
	INSERT INTO [OrderNote] ([OrderId], [Note], [DisplayToCustomer], [CreatedOnUtc])
	SELECT [OrderId], [Note], [DisplayToCustomer], [CreatedOn]
	FROM [Nop_OrderNote]
	WHERE OrderNoteId = @OriginalOrderNoteId

	--fetch next identifier
	FETCH NEXT FROM cur_originalordernote INTO @OriginalOrderNoteId
END
CLOSE cur_originalordernote
DEALLOCATE cur_originalordernote
GO











--DISCOUNT USAGE HISTORY
PRINT 'moving discount usage history'
DECLARE @OriginalDiscountUsageHistoryId int
DECLARE cur_originaldiscountusagehistory CURSOR FOR
SELECT DiscountUsageHistoryId
FROM [Nop_DiscountUsageHistory]
ORDER BY [DiscountUsageHistoryId]
OPEN cur_originaldiscountusagehistory
FETCH NEXT FROM cur_originaldiscountusagehistory INTO @OriginalDiscountUsageHistoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving discount usage history. ID ' + cast(@OriginalDiscountUsageHistoryId as nvarchar(10))
	INSERT INTO [DiscountUsageHistory] ([DiscountId], [OrderId], [CreatedOnUtc])
	SELECT [DiscountId], [OrderId], [CreatedOn]
	FROM [Nop_DiscountUsageHistory]
	WHERE DiscountUsageHistoryId = @OriginalDiscountUsageHistoryId

	--fetch next identifier
	FETCH NEXT FROM cur_originaldiscountusagehistory INTO @OriginalDiscountUsageHistoryId
END
CLOSE cur_originaldiscountusagehistory
DEALLOCATE cur_originaldiscountusagehistory
GO











--GIFT CARDS
PRINT 'moving gift cards'
DECLARE @OriginalGiftCardId int
DECLARE cur_originalgiftcard CURSOR FOR
SELECT GiftCardId
FROM [Nop_GiftCard]
ORDER BY [GiftCardId]
OPEN cur_originalgiftcard
FETCH NEXT FROM cur_originalgiftcard INTO @OriginalGiftCardId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving gift card. ID ' + cast(@OriginalGiftCardId as nvarchar(10))

	DECLARE @GiftCardTypeId int
	SET @GiftCardTypeId = null -- clear cache (variable scope)
	
	SELECT @GiftCardTypeId = pv.[GiftCardType]
	FROM [Nop_OrderProductVariant] opv
	INNER JOIN [Nop_ProductVariant] pv ON opv.ProductVariantId=pv.ProductVariantId
	INNER JOIN [Nop_GiftCard] gc ON gc.PurchasedOrderProductVariantID = opv.OrderProductVariantID
	WHERE gc.GiftCardId = @OriginalGiftCardId

	INSERT INTO [GiftCard] ([PurchasedWithOrderProductVariantId], [GiftCardTypeId], [Amount], [IsGiftCardActivated], [GiftCardCouponCode], [RecipientName], [RecipientEmail], [SenderName], [SenderEmail], [Message], [IsRecipientNotified], [CreatedOnUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'OrderProductVariant' and [OriginalId]=gc.[PurchasedOrderProductVariantID]), @GiftCardTypeId, gc.[Amount], gc.[IsGiftCardActivated], gc.[GiftCardCouponCode], gc.[RecipientName], gc.[RecipientEmail], gc.[SenderName], gc.[SenderEmail], gc.[Message], gc.[IsRecipientNotified], gc.[CreatedOn]
	FROM [Nop_GiftCard] gc
	WHERE GiftCardId = @OriginalGiftCardId

	--new ID
	DECLARE @NewGiftCardId int
	SET @NewGiftCardId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalGiftCardId, @NewGiftCardId, N'GiftCard')
	--fetch next identifier
	FETCH NEXT FROM cur_originalgiftcard INTO @OriginalGiftCardId
END
CLOSE cur_originalgiftcard
DEALLOCATE cur_originalgiftcard
GO











--GIFT CARD USAGE HISTORY
PRINT 'moving gift card usage history'
DECLARE @OriginalGiftCardUsageHistoryId int
DECLARE cur_originalgiftcardusagehistory CURSOR FOR
SELECT GiftCardUsageHistoryId
FROM [Nop_GiftCardUsageHistory]
ORDER BY [GiftCardUsageHistoryId]
OPEN cur_originalgiftcardusagehistory
FETCH NEXT FROM cur_originalgiftcardusagehistory INTO @OriginalGiftCardUsageHistoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving gift card usage history. ID ' + cast(@OriginalGiftCardUsageHistoryId as nvarchar(10))

	INSERT INTO [GiftCardUsageHistory] ([GiftCardId], [UsedWithOrderId], [UsedValue], [CreatedOnUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'GiftCard' and [OriginalId]=[GiftCardID]), [OrderId], [UsedValue], [CreatedOn]
	FROM [Nop_GiftCardUsageHistory]
	WHERE GiftCardUsageHistoryId = @OriginalGiftCardUsageHistoryId

	--fetch next identifier
	FETCH NEXT FROM cur_originalgiftcardusagehistory INTO @OriginalGiftCardUsageHistoryId
END
CLOSE cur_originalgiftcardusagehistory
DEALLOCATE cur_originalgiftcardusagehistory
GO











--RECURRING PAYMENTS
PRINT 'moving recurring payments'
DECLARE @OriginalRecurringPaymentId int
DECLARE cur_originalrecurringpayment CURSOR FOR
SELECT RecurringPaymentId
FROM [Nop_RecurringPayment]
ORDER BY [RecurringPaymentId]
OPEN cur_originalrecurringpayment
FETCH NEXT FROM cur_originalrecurringpayment INTO @OriginalRecurringPaymentId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving recurring payment. ID ' + cast(@OriginalRecurringPaymentId as nvarchar(10))
	INSERT INTO [RecurringPayment] ([CycleLength], [CyclePeriodId], [TotalCycles], [StartDateUtc], [IsActive], [Deleted], [CreatedOnUtc], [InitialOrderId])
	SELECT [CycleLength], [CyclePeriod], [TotalCycles], [StartDate], [IsActive], [Deleted], [CreatedOn], [InitialOrderId]
	FROM [Nop_RecurringPayment]
	WHERE RecurringPaymentId = @OriginalRecurringPaymentId

	--new ID
	DECLARE @NewRecurringPaymentId int
	SET @NewRecurringPaymentId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalRecurringPaymentId, @NewRecurringPaymentId, N'RecurringPayment')
	--fetch next identifier
	FETCH NEXT FROM cur_originalrecurringpayment INTO @OriginalRecurringPaymentId
END
CLOSE cur_originalrecurringpayment
DEALLOCATE cur_originalrecurringpayment
GO











--RECURRING PAYMENT USAGE HISTORY
PRINT 'moving recurring payment history'
DECLARE @OriginalRecurringPaymentHistoryId int
DECLARE cur_originalrecurringpaymenthistory CURSOR FOR
SELECT RecurringPaymentHistoryId
FROM [Nop_RecurringPaymentHistory]
ORDER BY [RecurringPaymentHistoryId]
OPEN cur_originalrecurringpaymenthistory
FETCH NEXT FROM cur_originalrecurringpaymenthistory INTO @OriginalRecurringPaymentHistoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving recurring payment history. ID ' + cast(@OriginalRecurringPaymentHistoryId as nvarchar(10))

	INSERT INTO [RecurringPaymentHistory] ([RecurringPaymentId], [OrderId], [CreatedOnUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'RecurringPayment' and [OriginalId]=[RecurringPaymentId]), [OrderId], [CreatedOn]
	FROM [Nop_RecurringPaymentHistory]
	WHERE RecurringPaymentHistoryId = @OriginalRecurringPaymentHistoryId

	--fetch next identifier
	FETCH NEXT FROM cur_originalrecurringpaymenthistory INTO @OriginalRecurringPaymentHistoryId
END
CLOSE cur_originalrecurringpaymenthistory
DEALLOCATE cur_originalrecurringpaymenthistory
GO











--RETURN REQUESTS
PRINT 'moving return requests'
DECLARE @OriginalReturnRequestId int
DECLARE cur_originalreturnrequest CURSOR FOR
SELECT ReturnRequestId
FROM [Nop_ReturnRequest]
ORDER BY [ReturnRequestId]
OPEN cur_originalreturnrequest
FETCH NEXT FROM cur_originalreturnrequest INTO @OriginalReturnRequestId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving return request. ID ' + cast(@OriginalReturnRequestId as nvarchar(10))
	INSERT INTO [ReturnRequest] ([OrderProductVariantId], [CustomerId], [Quantity], [ReasonForReturn], [RequestedAction], [CustomerComments], [StaffNotes], [ReturnRequestStatusId], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'OrderProductVariant' and [OriginalId]=[OrderProductVariantId]), [CustomerId], [Quantity], [ReasonForReturn], [RequestedAction], [CustomerComments], [StaffNotes], [ReturnStatusId], [CreatedOn], [UpdatedOn]
	FROM [Nop_ReturnRequest]
	WHERE ReturnRequestId = @OriginalReturnRequestId

	--fetch next identifier
	FETCH NEXT FROM cur_originalreturnrequest INTO @OriginalReturnRequestId
END
CLOSE cur_originalreturnrequest
DEALLOCATE cur_originalreturnrequest
GO











--REWARD POINTS HISTORY
PRINT 'moving reward points history'
DECLARE @OriginalRewardPointsHistoryId int
DECLARE cur_originalrewardpointshistory CURSOR FOR
SELECT RewardPointsHistoryId
FROM [Nop_RewardPointsHistory]
ORDER BY [RewardPointsHistoryId]
OPEN cur_originalrewardpointshistory
FETCH NEXT FROM cur_originalrewardpointshistory INTO @OriginalRewardPointsHistoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving reward points history. ID ' + cast(@OriginalRewardPointsHistoryId as nvarchar(10))
	INSERT INTO [RewardPointsHistory] ([CustomerId], [Points], [PointsBalance], [UsedAmount], [Message], [CreatedOnUtc], [UsedWithOrder_Id])
	SELECT [CustomerId], [Points], [PointsBalance], [UsedAmount], [Message], [CreatedOn], (SELECT CASE WHEN [OrderId]=0 THEN NULL ELSE [OrderId] END)
	FROM [Nop_RewardPointsHistory]
	WHERE RewardPointsHistoryId = @OriginalRewardPointsHistoryId

	--fetch next identifier
	FETCH NEXT FROM cur_originalrewardpointshistory INTO @OriginalRewardPointsHistoryId
END
CLOSE cur_originalrewardpointshistory
DEALLOCATE cur_originalrewardpointshistory
GO












--PRODUCT REVIEWS
PRINT 'moving product reviews'
DECLARE @OriginalProductReviewId int
DECLARE cur_originalproductreview CURSOR FOR
SELECT ProductReviewId
FROM [Nop_ProductReview]
ORDER BY [ProductReviewId]
OPEN cur_originalproductreview
FETCH NEXT FROM cur_originalproductreview INTO @OriginalProductReviewId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product review. ID ' + cast(@OriginalProductReviewId as nvarchar(10))

	INSERT INTO [CustomerContent] ([CustomerId], [IpAddress], [IsApproved], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [CustomerId], [IpAddress], [IsApproved], [CreatedOn], [CreatedOn]
	FROM [Nop_ProductReview]
	WHERE ProductReviewId = @OriginalProductReviewId

	--new ID
	DECLARE @NewProductReviewId int
	SET @NewProductReviewId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalProductReviewId, @NewProductReviewId, N'ProductReview')


	INSERT INTO [ProductReview] ([Id], [ProductId], [Title], [ReviewText], [Rating], [HelpfulYesTotal], [HelpfulNoTotal])
	SELECT @NewProductReviewId, [ProductId], [Title], [ReviewText], [Rating], [HelpfulYesTotal], [HelpfulNoTotal]
	FROM [Nop_ProductReview]
	WHERE ProductReviewId = @OriginalProductReviewId	


	--fetch next identifier
	FETCH NEXT FROM cur_originalproductreview INTO @OriginalProductReviewId
END
CLOSE cur_originalproductreview
DEALLOCATE cur_originalproductreview
GO

--PRODUCT REVIEW HELPFULNESS
PRINT 'moving product review helpfulness'
DECLARE @OriginalProductReviewHelpfulnessId int
DECLARE cur_originalproductreviewhelpfulness CURSOR FOR
SELECT ProductReviewHelpfulnessId
FROM [Nop_ProductReviewHelpfulness]
ORDER BY [ProductReviewHelpfulnessId]
OPEN cur_originalproductreviewhelpfulness
FETCH NEXT FROM cur_originalproductreviewhelpfulness INTO @OriginalProductReviewHelpfulnessId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product review helpfulness. ID ' + cast(@OriginalProductReviewHelpfulnessId as nvarchar(10))
		
	INSERT INTO [CustomerContent] ([CustomerId], [IpAddress], [IsApproved], [CreatedOnUtc], [UpdatedOnUtc])
	SELECT [CustomerId], null, 1 /*approved*/, getutcdate(), getutcdate()
	FROM [Nop_ProductReviewHelpfulness]
	WHERE ProductReviewHelpfulnessId = @OriginalProductReviewHelpfulnessId

	--new ID
	DECLARE @NewProductReviewHelpfulnessId int
	SET @NewProductReviewHelpfulnessId = @@IDENTITY

	INSERT INTO [ProductReviewHelpfulness] ([Id], [ProductReviewId], [WasHelpful])
	SELECT @NewProductReviewHelpfulnessId, (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'ProductReview' and [OriginalId]=[ProductReviewID]), [WasHelpful]
	FROM [Nop_ProductReviewHelpfulness]
	WHERE ProductReviewHelpfulnessId = @OriginalProductReviewHelpfulnessId	


	--fetch next identifier
	FETCH NEXT FROM cur_originalproductreviewhelpfulness INTO @OriginalProductReviewHelpfulnessId
END
CLOSE cur_originalproductreviewhelpfulness
DEALLOCATE cur_originalproductreviewhelpfulness
GO

--UPDATE PRODUCT REVIEW TOTALS
PRINT 'updating product review totals'
DECLARE @OriginalProductId int
DECLARE cur_originalproduct CURSOR FOR
SELECT ProductId
FROM [Nop_Product]
ORDER BY [ProductId]
OPEN cur_originalproduct
FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'product review total. ID ' + cast(@OriginalProductId as nvarchar(10))

	DECLARE @ApprovedTotalReviews int
	SELECT @ApprovedTotalReviews = COUNT(1)
	FROM [Nop_ProductReview]
	WHERE [ProductId] = @OriginalProductId and [IsApproved]=1

	DECLARE @NotApprovedTotalReviews int
	SELECT @NotApprovedTotalReviews = COUNT(1)
	FROM [Nop_ProductReview]
	WHERE [ProductId] = @OriginalProductId and [IsApproved]=0

	DECLARE @ApprovedRatingSum int
	SELECT @ApprovedRatingSum = SUM(Rating)
	FROM [Nop_ProductReview]
	WHERE [ProductId] = @OriginalProductId and [IsApproved]=1
	IF (@ApprovedRatingSum is null) --ensure it's not null
	BEGIN
		SET @ApprovedRatingSum = 0
	END

	DECLARE @NotApprovedRatingSum int
	SELECT @NotApprovedRatingSum = SUM(Rating)
	FROM [Nop_ProductReview]
	WHERE [ProductId] = @OriginalProductId and [IsApproved]=0
	IF (@NotApprovedRatingSum is null) --ensure it's not null
	BEGIN
		SET @NotApprovedRatingSum = 0
	END

	UPDATE [Product]
	SET [ApprovedTotalReviews] = @ApprovedTotalReviews,
	[NotApprovedTotalReviews] = @NotApprovedTotalReviews,
	[ApprovedRatingSum] = @ApprovedRatingSum,
	[NotApprovedRatingSum] = @NotApprovedRatingSum
	WHERE [Id]=@OriginalProductId


	--fetch next identifier
	FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
END
CLOSE cur_originalproduct
DEALLOCATE cur_originalproduct
GO









--drop temporary table
DROP TABLE #IDs
GO

--drop old tables, store procedures, functions
DROP TABLE [Nop_ACL]
GO
DROP TABLE [Nop_ActivityLog]
GO
DROP TABLE [Nop_ActivityLogType]
GO
DROP TABLE [Nop_BannedIpAddress]
GO
DROP TABLE [Nop_BannedIpNetwork]
GO
DROP TABLE [Nop_BlogComment]
GO
DROP TABLE [Nop_BlogPost]
GO
DROP TABLE [Nop_Campaign]
GO
DROP TABLE [Nop_Category_Discount_Mapping]
GO
DROP TABLE [Nop_CategoryLocalized]
GO
DROP TABLE [Nop_CheckoutAttributeValueLocalized]
GO
DROP TABLE [Nop_CheckoutAttributeValue]
GO
DROP TABLE [Nop_CheckoutAttributeLocalized]
GO
DROP TABLE [Nop_CheckoutAttribute]
GO
DROP TABLE [Nop_CreditCardType]
GO
DROP TABLE [Nop_CrossSellProduct]
GO
DROP TABLE [Nop_Currency]
GO
DROP TABLE [Nop_Customer_CustomerRole_Mapping]
GO
DROP TABLE [Nop_CustomerAction]
GO
DROP TABLE [Nop_CustomerAttribute]
GO
DROP TABLE [Nop_CustomerRole_Discount_Mapping]
GO
DROP TABLE [Nop_CustomerRole_ProductPrice]
GO
DROP TABLE [Nop_DiscountRestriction]
GO
DROP TABLE [Nop_DiscountUsageHistory]
GO
DROP TABLE [Nop_Download]
GO
DROP TABLE [Nop_EmailAccount]
GO
DROP TABLE [Nop_Forums_Subscription]
GO
DROP TABLE [Nop_Forums_PrivateMessage]
GO
DROP TABLE [Nop_GiftCardUsageHistory]
GO
DROP TABLE [Nop_LocaleStringResource]
GO
DROP TABLE [Nop_Log]
GO
DROP TABLE [Nop_ManufacturerLocalized]
GO
DROP TABLE [Nop_MeasureDimension]
GO
DROP TABLE [Nop_MeasureWeight]
GO
DROP TABLE [Nop_MessageTemplateLocalized]
GO
DROP TABLE [Nop_MessageTemplate]
GO
DROP TABLE [Nop_NewsComment]
GO
DROP TABLE [Nop_News]
GO
DROP TABLE [Nop_NewsLetterSubscription]
GO
DROP TABLE [Nop_OrderNote]
GO
DROP TABLE [Nop_PaymentMethod_RestrictedCountries]
GO
DROP TABLE [Nop_PaymentMethod]
GO
DROP TABLE [Nop_PollVotingRecord]
GO
DROP TABLE [Nop_PollAnswer]
GO
DROP TABLE [Nop_Poll]
GO
DROP TABLE [Nop_Product_Category_Mapping]
GO
DROP TABLE [Nop_Product_Manufacturer_Mapping]
GO
DROP TABLE [Nop_Product_SpecificationAttribute_Mapping]
GO
DROP TABLE [Nop_ProductAttributeLocalized]
GO
DROP TABLE [Nop_ProductLocalized]
GO
DROP TABLE [Nop_ProductPicture]
GO
DROP TABLE [Nop_ProductReviewHelpfulness]
GO
DROP TABLE [Nop_ProductReview]
GO
DROP TABLE [Nop_ProductRating]
GO
DROP TABLE [Nop_ProductTag_Product_Mapping]
GO
DROP TABLE [Nop_ProductTag]
GO
DROP TABLE [Nop_ProductVariant_Discount_Mapping]
GO
DROP TABLE [Nop_ProductVariant_Pricelist_Mapping]
GO
DROP TABLE [Nop_ProductVariantAttributeValueLocalized]
GO
DROP TABLE [Nop_ProductVariantAttributeValue]
GO
DROP TABLE [Nop_ProductVariantAttributeCombination]
GO
DROP TABLE [Nop_ProductVariant_ProductAttribute_Mapping]
GO
DROP TABLE [Nop_ProductVariantLocalized]
GO
DROP TABLE [Nop_QBEntity]
GO
DROP TABLE [Nop_QueuedEmail]
GO
DROP TABLE [Nop_RecurringPaymentHistory]
GO
DROP TABLE [Nop_RecurringPayment]
GO
DROP TABLE [Nop_RelatedProduct]
GO
DROP TABLE [Nop_ReturnRequest]
GO
DROP TABLE [Nop_RewardPointsHistory]
GO
DROP TABLE [Nop_SearchLog]
GO
DROP TABLE [Nop_Setting]
GO
DROP TABLE [Nop_ShippingByTotal]
GO
DROP TABLE [Nop_ShippingByWeight]
GO
DROP TABLE [Nop_ShippingByWeightAndCountry]
GO
DROP TABLE [Nop_ShippingMethod_RestrictedCountries]
GO
DROP TABLE [Nop_ShippingMethod]
GO
DROP TABLE [Nop_ShippingRateComputationMethod]
GO
DROP TABLE [Nop_SMSProvider]
GO
DROP TABLE [Nop_SpecificationAttributeOptionLocalized]
GO
DROP TABLE [Nop_SpecificationAttributeOption]
GO
DROP TABLE [Nop_SpecificationAttributeLocalized]
GO
DROP TABLE [Nop_SpecificationAttribute]
GO
DROP TABLE [Nop_StateProvince]
GO
DROP TABLE [Nop_TaxProvider]
GO
DROP TABLE [Nop_TaxRate]
GO
DROP TABLE [Nop_TierPrice]
GO
DROP TABLE [Nop_TopicLocalized]
GO
DROP TABLE [Nop_Topic]
GO
DROP TABLE [Nop_Warehouse]
GO
DROP TABLE [Nop_Manufacturer]
GO
DROP TABLE [Nop_ManufacturerTemplate]
GO
DROP TABLE [Nop_Category]
GO
DROP TABLE [Nop_CategoryTemplate]
GO
DROP TABLE [Nop_ACLPerObject]
GO
DROP TABLE [Nop_Discount]
GO
DROP TABLE [Nop_Affiliate]
GO
DROP TABLE [Nop_Address]
GO
DROP TABLE [Nop_Country]
GO
DROP TABLE [Nop_GiftCard]
GO
DROP TABLE [Nop_Language]
GO
DROP TABLE [Nop_Picture]
GO
DROP TABLE [Nop_ShoppingCartItem]
GO
DROP TABLE [Nop_CustomerSession]
GO
DROP TABLE [Nop_Customer]
GO
DROP TABLE [Nop_CustomerRole]
GO
DROP TABLE [Nop_ProductAttribute]
GO
DROP TABLE [Nop_OrderProductVariant]
GO
DROP TABLE [Nop_Order]
GO
DROP TABLE [Nop_ProductVariant]
GO
DROP TABLE [Nop_Product]
GO
DROP TABLE [Nop_ProductTemplate]
GO
DROP TABLE [Nop_TaxCategory]
GO
DROP TABLE [Nop_Forums_Post]
GO
DROP TABLE [Nop_Forums_Topic]
GO
DROP TABLE [Nop_Forums_Forum]
GO
DROP TABLE [Nop_Forums_Group]
GO
DROP TABLE [Nop_Pricelist]
GO
DROP PROCEDURE [Nop_CustomerBestReport]
GO
DROP PROCEDURE [Nop_CustomerReportByAttributeKey]
GO
DROP PROCEDURE [Nop_LanguagePackExport]
GO
DROP PROCEDURE [Nop_LanguagePackImport]
GO
DROP PROCEDURE [Nop_Maintenance_ReindexTables]
GO
DROP PROCEDURE [Nop_OrderProductVariantReport]
GO
DROP PROCEDURE [Nop_ProductAlsoPurchasedLoadByProductID]
GO
DROP PROCEDURE [Nop_ProductLoadAllPaged]
GO
DROP PROCEDURE [Nop_ProductVariantLoadAll]
GO
DROP PROCEDURE [Nop_SalesBestSellersReport]
GO
DROP PROCEDURE [Nop_SpecificationAttributeOptionFilter_LoadByFilter]
GO
DROP FUNCTION [NOP_splitstring_to_table]
GO
DROP FUNCTION [NOP_getcustomerattributevalue]
GO
DROP FUNCTION [NOP_getnotnullnotempty]
GO