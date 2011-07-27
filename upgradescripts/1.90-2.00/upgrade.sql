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






--move campaigns
PRINT 'moving campaign'
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











--move countries
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






--move states
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








--move customer roles
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










--move customers
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
	--TODO insert AffiliateId
	INSERT INTO [Customer] ([CustomerGuid], [Username], [Email], [Password], [PasswordFormatId], [PasswordSalt], [AdminComment], [TaxDisplayTypeId], [IsTaxExempt], [VatNumberStatusId], [UseRewardPointsDuringCheckout], [TimeZoneId], [Active], [Deleted], [IsSystemAccount], [CreatedOnUtc], [LastActivityDateUtc])
	SELECT [CustomerGuid], [Username], [Email], [PasswordHash], 1 /*hashed*/, [SaltKey], [AdminComment], 0 /*IncludingTax now*/, [IsTaxExempt], 10 /*Empty now*/, 0, [TimeZoneId], [Active], [Deleted], 0, [RegistrationDate], [RegistrationDate]
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








--move customer addresses
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













--drop temporary table
DROP TABLE #IDs
GO

--TODO drop old tables, store procedures, functions