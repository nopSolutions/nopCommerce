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




--temporary table for identifiers
CREATE TABLE #IDs
	(
		[OriginalId] int NOT NULL,
		[NewId] int NOT NULL,
		[EntityName] nvarchar(100) NOT NULL
	)
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
	INSERT INTO [Campaign] ([Name], [Subject], [Body], [CreatedOnUtc])
	SELECT [Name], [Subject], [Body], [CreatedOn]
	FROM [Nop_Campaign]
	WHERE CampaignId = @OriginalCampaignId

	--new ID
	DECLARE @NewCampaignId int
	SET @NewCampaignId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCampaignId, @NewCampaignId, N'Campaign')
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

	INSERT INTO [Affiliate] ([AddressId], [Active], [Deleted])
	SELECT @NewAffiliateAddressId, [Active], [Deleted]
	FROM [Nop_Affiliate]
	WHERE AffiliateId = @OriginalAffiliateId

	--new ID
	DECLARE @NewAffiliateId int
	SET @NewAffiliateId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalAffiliateId, @NewAffiliateId, N'Affiliate')
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

	INSERT INTO [Customer] ([CustomerGuid], [Username], [Email], [Password], [PasswordFormatId], [PasswordSalt], [AffiliateId], [AdminComment], [TaxDisplayTypeId], [IsTaxExempt], [VatNumberStatusId], [UseRewardPointsDuringCheckout], [TimeZoneId], [Active], [Deleted], [IsSystemAccount], [CreatedOnUtc], [LastActivityDateUtc])
	SELECT [CustomerGuid], [Username], [Email], [PasswordHash], 1 /*hashed*/, [SaltKey], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Affiliate' and [OriginalId]=[AffiliateId]) ,[AdminComment], 0 /*IncludingTax now*/, [IsTaxExempt], 10 /*Empty now*/, 0, [TimeZoneId], [Active], [Deleted], 0, [RegistrationDate], [RegistrationDate]
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId

	--new ID
	DECLARE @NewCustomerId int
	SET @NewCustomerId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCustomerId, @NewCustomerId, N'Customer')

	--move customer attributes (Gender, Firstname, Lastname, Company, PasswordRecoveryToken, AccountActivationToken)
	INSERT INTO [CustomerAttribute] ([CustomerId], [Key], [Value])
	SELECT @NewCustomerId, [key], [value]
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
		VALUES (@NewCustomerId, 'ForumPostCount', cast(@ForumPostCount as nvarchar(100)))
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
		VALUES (@NewCustomerId, 'Signature', @Signature)
	END
	--TODO move other customer attributes (AvatarPictureId, LocationCountryId)

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
		VALUES (@AdminCustomerRoleId, @NewCustomerId)
	END	
	IF (@IsGuest = 1)
	BEGIN
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@GuestCustomerRoleId, @NewCustomerId)
	END	
	IF (@IsRegistered = 1)
	BEGIN
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@RegisteredCustomerRoleId, @NewCustomerId)
	END	
	IF (@IsForumModerator = 1)
	BEGIN
		INSERT INTO Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@ForumModeratorCustomerRoleId, @NewCustomerId)
	END
	--map customer to customer roles(old roles)
	INSERT INTO [Customer_CustomerRole_Mapping] ([CustomerRole_Id],[Customer_Id])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'CustomerRole' and [OriginalId]=original_ccrm.CustomerRoleId), @NewCustomerId
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
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Customer' and [OriginalId]=original_a.CustomerID), @NewAddressId
	FROM [Nop_Address] original_a
	WHERE original_a.AddressId = @OriginalAddressId


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
	INSERT INTO [BlogPost] ([LanguageId], [Title], [Body], [AllowComments], [Tags], [CreatedOnUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), [BlogPostTitle], [BlogPostBody], [BlogPostAllowComments], [Tags], [CreatedOn]
	FROM [Nop_BlogPost]
	WHERE BlogPostId = @OriginalBlogPostId

	--new ID
	DECLARE @NewBlogPostId int
	SET @NewBlogPostId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalBlogPostId, @NewBlogPostId, N'BlogPost')



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
	SELECT @BlogCommentCustomerId = [NewId] 
	FROM #IDs 
	WHERE [EntityName]=N'Customer' and [OriginalId]=(SELECT CustomerID FROM [Nop_BlogComment] WHERE BlogCommentId = @OriginalBlogCommentId)
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

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalBlogCommentId, @NewBlogCommentId, N'BlogComment')


	INSERT INTO [BlogComment] ([Id], [CommentText], [BlogPostId])
	SELECT @NewBlogCommentId, [CommentText], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'BlogPost' and [OriginalId]=[BlogPostID])
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
	INSERT INTO [News] ([LanguageId], [Title], [Short], [Full], [Published], [AllowComments], [CreatedOnUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), [Title], [Short], [Full], [Published], [AllowComments], [CreatedOn]
	FROM [Nop_News]
	WHERE NewsId = @OriginalNewsItemId

	--new ID
	DECLARE @NewNewsItemId int
	SET @NewNewsItemId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalNewsItemId, @NewNewsItemId, N'NewsItem')



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
	SELECT @NewsCommentCustomerId = [NewId] 
	FROM #IDs 
	WHERE [EntityName]=N'Customer' and [OriginalId]=(SELECT CustomerID FROM [Nop_NewsComment] WHERE NewsCommentId = @OriginalNewsCommentId)
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

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalNewsCommentId, @NewNewsCommentId, N'NewsComment')


	INSERT INTO [NewsComment] ([Id], [CommentTitle], [CommentText], [NewsItemId])
	SELECT @NewNewsCommentId, [Title], [Comment], (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'NewsItem' and [OriginalId]=[NewsID])
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
	INSERT INTO [Poll] ([LanguageId], [Name], [SystemKeyword], [Published], [ShowOnHomePage], [DisplayOrder], [StartDateUtc], [EndDateUtc])
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), [Name], [SystemKeyword], [Published], [ShowOnHomePage], [DisplayOrder], [StartDate], [EndDate]
	FROM [Nop_Poll]
	WHERE PollId = @OriginalPollId

	--new ID
	DECLARE @NewPollId int
	SET @NewPollId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalPollId, @NewPollId, N'Poll')



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
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Poll' and [OriginalId]=[PollId]), [Name], [Count], [DisplayOrder]
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
	SELECT (SELECT [NewId] FROM #IDs WHERE [EntityName]=N'Customer' and [OriginalId]=[CustomerId]), null, 1 /*approved*/, getutcdate(), getutcdate()
	FROM [Nop_PollVotingRecord]
	WHERE PollVotingRecordId = @OriginalPollVotingRecordId

	--new ID
	DECLARE @NewPollVotingRecordId int
	SET @NewPollVotingRecordId = @@IDENTITY

	INSERT INTO #IDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalPollVotingRecordId, @NewPollVotingRecordId, N'PollVotingRecord')


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







--drop temporary table
DROP TABLE #IDs
GO

--TODO drop old tables, store procedures, functions