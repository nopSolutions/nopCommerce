IF NOT EXISTS (
    SELECT 1
    FROM [dbo].[Nop_CustomerAction]
    WHERE [SystemKeyword] = N'ManageEmailSettings')
BEGIN
  INSERT [dbo].[Nop_CustomerAction] ([Name], [SystemKeyword], [Comment], [DisplayOrder])
  VALUES (N'Manage Email Settings', N'ManageEmailSettings', N'',20)
END
GO 


UPDATE [dbo].[Nop_Currency]
SET [CustomFormatting]=N'€0.00'
WHERE [CurrencyCode]=N'EUR'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SearchPage.ProductsPerPage')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SearchPage.ProductsPerPage', N'10', N'')
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'Customer.NewPM')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'Customer.NewPM')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'Customer.NewPM' 

	IF (@MessageTemplateID > 0)
	BEGIN

	--do it for each existing language
	DECLARE @ExistingLanguageID int
	DECLARE cur_existinglanguage CURSOR FOR
	SELECT LanguageID
	FROM [Nop_Language]
	OPEN cur_existinglanguage
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--insert localized message template
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. You have received a new private message',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />
		You have received a new private message.
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewCustomer.Notification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewCustomer.Notification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewCustomer.Notification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		--do it for each existing language
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT LanguageID
		FROM [Nop_Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'New customer registration',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />A new customer registered with your store. Below are the customer''s details:
		<br />Full name: %Customer.FullName%
		<br />Email: %Customer.Email%
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PaymentMethod.PaypalStandard.ValidateOrderTotal')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PaymentMethod.PaypalStandard.ValidateOrderTotal', N'true', N'')
END
GO


--new discount type
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountType]
		WHERE [DiscountTypeID] = 20)
BEGIN
	INSERT [dbo].[Nop_DiscountType] ([DiscountTypeID], [Name])
	VALUES (20, N'Assigned to order subtotal')
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountInclTax')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountInclTax] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountInclTax] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountInclTaxInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountInclTaxInCustomerCurrency] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountInclTaxInCustomerCurrency] DEFAULT ((0))
END
GO


IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountExclTax')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountExclTax] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountExclTax] DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Order]') and NAME='OrderSubTotalDiscountExclTaxInCustomerCurrency')
BEGIN
	ALTER TABLE [dbo].[Nop_Order] 
	ADD [OrderSubTotalDiscountExclTaxInCustomerCurrency] money NOT NULL CONSTRAINT [DF_Nop_Order_OrderSubTotalDiscountExclTaxInCustomerCurrency] DEFAULT ((0))
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.StoreClosed.AllowAdminAccess')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.StoreClosed.AllowAdminAccess', N'True', N'')
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 5)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (5, N'Must be registered')
END
GO

--return requets message templates
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewReturnRequest.StoreOwnerNotification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'NewReturnRequest.StoreOwnerNotification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewReturnRequest.StoreOwnerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		--do it for each existing language
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT LanguageID
		FROM [Nop_Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. New return request.',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />
		%Customer.FullName% (%Customer.Email%) has just submitted a new return request. Details are below:
		<br />
		Request ID: %ReturnRequest.ID%
		<br />
		Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%
		<br />
		Reason for return: %ReturnRequest.Reason%
		<br />
		Requested action: %ReturnRequest.RequestedAction%
		<br />
		Customer comments:
		<br />
		%ReturnRequest.CustomerComment%
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'ReturnRequestStatusChanged.CustomerNotification')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'ReturnRequestStatusChanged.CustomerNotification')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'ReturnRequestStatusChanged.CustomerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN
		--do it for each existing language
		DECLARE @ExistingLanguageID int
		DECLARE cur_existinglanguage CURSOR FOR
		SELECT LanguageID
		FROM [Nop_Language]
		OPEN cur_existinglanguage
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		WHILE @@FETCH_STATUS = 0
		BEGIN

		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. Return request status was changed.',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
		<br />
		Hello %Customer.FullName%,
		<br />
		Your return request #%ReturnRequest.ID% status has been changed.
		</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO

--rename 'Extension' to 'MimeType' (Nop_Picture)
IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_Picture]') and NAME='Extension')
BEGIN
 ALTER TABLE [dbo].[Nop_Picture] ADD MimeType nvarchar(20) NOT NULL CONSTRAINT [DF_Nop_Picture_MimeType] DEFAULT ((''))
 EXEC('UPDATE [dbo].[Nop_Picture] SET MimeType=Extension')
 ALTER TABLE [dbo].[Nop_Picture] DROP COLUMN Extension
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PictureLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PictureLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_PictureLoadAllPaged]
(
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	CREATE TABLE #PageIndex 
	(
		IndexID int IDENTITY (1, 1) NOT NULL,
		PictureID int NOT NULL		 
	)
	INSERT INTO #PageIndex (PictureID)
	SELECT
		PictureID
	FROM [Nop_Picture]
	ORDER BY PictureID DESC

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn

	SELECT 
		[p].PictureId,
		[p].PictureBinary,
		[p].MimeType,
		[p].IsNew
	FROM [Nop_Picture] [p]
		INNER JOIN #PageIndex [pi]
		ON [p].PictureID = [pi].PictureID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound

	SET ROWCOUNT 0
	
	DROP TABLE #PageIndex

END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Display.Products.ShowCategoryProductNumber.IncludeSubCategories')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Display.Products.ShowCategoryProductNumber.IncludeSubCategories', N'True', N'')
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PDFInvoice.RenderOrderNotes')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PDFInvoice.RenderOrderNotes', N'False', N'')
END
GO



--ACL per object
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].[Nop_ACLPerObject]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [dbo].[Nop_ACLPerObject](
	[ACLPerObjectId] int IDENTITY(1,1) NOT NULL,
	[ObjectId] int NOT NULL,
	[ObjectTypeId] int NOT NULL,
	[CustomerRoleId] int NOT NULL,
	[Deny] bit NOT NULL,
 CONSTRAINT [PK_ACLPerObject] PRIMARY KEY CLUSTERED 
(
	[ACLPerObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ACLPerObject_Nop_CustomerRole'
           AND parent_obj = Object_id('Nop_ACLPerObject')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ACLPerObject
DROP CONSTRAINT FK_Nop_ACLPerObject_Nop_CustomerRole
GO
ALTER TABLE [dbo].[Nop_ACLPerObject]  WITH CHECK ADD  CONSTRAINT [FK_Nop_ACLPerObject_Nop_CustomerRole] FOREIGN KEY([CustomerRoleId])
REFERENCES [dbo].[Nop_CustomerRole] ([CustomerRoleId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PromotionProvider.BecomeCom.ProductThumbnailImageSize')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PromotionProvider.BecomeCom.ProductThumbnailImageSize', N'125', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PromotionProvider.Froogle.ProductThumbnailImageSize')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PromotionProvider.Froogle.ProductThumbnailImageSize', N'125', N'')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'PromotionProvider.PriceGrabber.ProductThumbnailImageSize')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'PromotionProvider.PriceGrabber.ProductThumbnailImageSize', N'125', N'')
END
GO



IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Common.EmailWishlist')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Common.EmailWishlist', N'True', N'')
END
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'Wishlist.EmailAFriend')
BEGIN
	INSERT [dbo].[Nop_MessageTemplate] ([Name])
	VALUES (N'Wishlist.EmailAFriend')

	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'Wishlist.EmailAFriend' 

	IF (@MessageTemplateID > 0)
	BEGIN

	--do it for each existing language
	DECLARE @ExistingLanguageID int
	DECLARE cur_existinglanguage CURSOR FOR
	SELECT LanguageID
	FROM [Nop_Language]
	OPEN cur_existinglanguage
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--insert localized message template
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, @ExistingLanguageID, N'', N'%Store.Name%. Wishlist',  N'<p><a href="%Store.URL%"> %Store.Name%</a> <br />
<br />
%Customer.Email% was shopping on %Store.Name% and wanted to share a wishlist with you. <br />
<br />
<br />
For more info click <a target="_blank" href="%Wishlist.URLForCustomer%">here</a> <br />
<br />
<br />
%EmailAFriend.PersonalMessage%<br />
<br />
%Store.Name%</p>')

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO

--new discount requirements
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 7)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (7, N'Has all of these product variants in the cart')
END
GO

IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_DiscountRequirement]
		WHERE [DiscountRequirementID] = 8)
BEGIN
	INSERT [dbo].[Nop_DiscountRequirement] ([DiscountRequirementID], [Name])
	VALUES (8, N'Has one of these product variants in the cart')
END
GO

--update NewVATSubmitted.StoreOwnerNotification message template
IF EXISTS (
		SELECT 1
		FROM [dbo].[Nop_MessageTemplate]
		WHERE [Name] = N'NewVATSubmitted.StoreOwnerNotification')
BEGIN
	DECLARE @MessageTemplateID INT 
	SELECT @MessageTemplateID =	mt.MessageTemplateID FROM Nop_MessageTemplate mt
							WHERE mt.Name = N'NewVATSubmitted.StoreOwnerNotification' 

	IF (@MessageTemplateID > 0)
	BEGIN

	--do it for each existing language
	DECLARE @ExistingLanguageID int
	DECLARE cur_existinglanguage CURSOR FOR
	SELECT LanguageID
	FROM [Nop_Language]
	OPEN cur_existinglanguage
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--update localized message template
		UPDATE [Nop_MessageTemplateLocalized]
		SET [Body] = N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
<br />
%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:
<br />
VAT number: %Customer.VatNumber%
<br />
VAT number status: %Customer.VatNumberStatus%
<br />
Received name: %VatValidationResult.Name%
<br />
Received address: %VatValidationResult.Address%
</p>'
		WHERE [MessageTemplateID] = @MessageTemplateID and
			  [LanguageID] = @ExistingLanguageID

		--fetch next language identifier
		FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
		END
	END
	CLOSE cur_existinglanguage
	DEALLOCATE cur_existinglanguage
END
GO


--product tag URL rewrites
IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'SEO.ProductTags.UrlRewriteFormat')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'SEO.ProductTags.UrlRewriteFormat', N'{0}producttag/{1}-{2}.aspx', N'')
END
GO

IF EXISTS (SELECT 1 FROM syscolumns WHERE id=object_id('[dbo].[Nop_PaymentMethod]') and NAME='HidePaymentInfoForZeroOrders')
BEGIN
	ALTER TABLE [dbo].[Nop_PaymentMethod] DROP CONSTRAINT [DF_Nop_PaymentMethod_HidePaymentInfoForZeroOrders]
	ALTER TABLE [dbo].[Nop_PaymentMethod] DROP COLUMN HidePaymentInfoForZeroOrders
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PaymentMethodLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
GO

CREATE PROCEDURE [dbo].[Nop_PaymentMethodLoadAll]
(
	@ShowHidden bit = 0,
	@FilterByCountryID int = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	IF(@FilterByCountryID IS NOT NULL AND @FilterByCountryID != 0)
		BEGIN
			SELECT  
				pm.PaymentMethodId,
				pm.Name,
				pm.VisibleName,
				pm.Description,
				pm.ConfigureTemplatePath,
				pm.UserTemplatePath,
				pm.ClassName,
				pm.SystemKeyword,
				pm.IsActive,
				pm.DisplayOrder
		    FROM 
				[Nop_PaymentMethod] pm
		    WHERE 
                pm.PaymentMethodID NOT IN 
				(
				    SELECT 
						pmc.PaymentMethodID
				    FROM 
						[Nop_PaymentMethod_RestrictedCountries] pmc
				    WHERE 
						pmc.CountryID = @FilterByCountryID AND 
						pm.PaymentMethodID = pmc.PaymentMethodID
				)
				AND
				(IsActive = 1 or @ShowHidden = 1)
		   ORDER BY 
				pm.DisplayOrder
		END
	ELSE
		BEGIN
			SELECT 
				*
			FROM 
				[Nop_PaymentMethod]
			WHERE 
				(IsActive = 1 or @ShowHidden = 1)
			ORDER BY 
				DisplayOrder
		END
END
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
GO
CREATE PROCEDURE [dbo].[Nop_ProductLoadAllPaged]
(
	@CategoryID			int = 0,
	@ManufacturerID		int = 0,
	@ProductTagID		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			money = null,
	@PriceMax			money = null,
	@RelatedToProductID	int = 0,
	@Keywords			nvarchar(MAX),
	@SearchDescriptions bit = 0,
	@ShowHidden			bit = 0,
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@FilteredSpecs		nvarchar(300) = null,	--filter by attributes (comma-separated list). e.g. 14,15,16
	@LanguageID			int = 0,
	@OrderBy			int = 0, --0 position, 5 - Name, 10 - Price, 15 - creation date
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	--init
	DECLARE @SearchKeywords bit
	SET @SearchKeywords = 1
	IF (@Keywords IS NULL OR @Keywords = N'')
		SET @SearchKeywords = 0

	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = '%' + rtrim(ltrim(@Keywords)) + '%'

	--filter by attributes
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionID int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionID)
	SELECT CAST(data as int) FROM dbo.[NOP_splitstring_to_table](@FilteredSpecs, ',');
	
	DECLARE @SpecAttributesCount int	
	SELECT @SpecAttributesCount = COUNT(1) FROM #FilteredSpecs

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[ID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)

	INSERT INTO #DisplayOrderTmp ([ProductID])
	SELECT p.ProductID
	FROM Nop_Product p with (NOLOCK) 
	LEFT OUTER JOIN Nop_Product_Category_Mapping pcm with (NOLOCK) ON p.ProductID=pcm.ProductID
	LEFT OUTER JOIN Nop_Product_Manufacturer_Mapping pmm with (NOLOCK) ON p.ProductID=pmm.ProductID
	LEFT OUTER JOIN Nop_ProductTag_Product_Mapping ptpm with (NOLOCK) ON p.ProductID=ptpm.ProductID
	LEFT OUTER JOIN Nop_RelatedProduct rp with (NOLOCK) ON p.ProductID=rp.ProductID2
	LEFT OUTER JOIN Nop_ProductVariant pv with (NOLOCK) ON p.ProductID = pv.ProductID
	LEFT OUTER JOIN Nop_ProductVariantLocalized pvl with (NOLOCK) ON pv.ProductVariantID = pvl.ProductVariantID AND pvl.LanguageID = @LanguageID
	LEFT OUTER JOIN Nop_ProductLocalized pl with (NOLOCK) ON p.ProductID = pl.ProductID AND pl.LanguageID = @LanguageID
	WHERE 
		(
		   (
				@CategoryID IS NULL OR @CategoryID=0
				OR (pcm.CategoryID=@CategoryID AND (@FeaturedProducts IS NULL OR pcm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ManufacturerID IS NULL OR @ManufacturerID=0
				OR (pmm.ManufacturerID=@ManufacturerID AND (@FeaturedProducts IS NULL OR pmm.IsFeaturedProduct=@FeaturedProducts))
			)
		AND (
				@ProductTagID IS NULL OR @ProductTagID=0
				OR ptpm.ProductTagID=@ProductTagID
			)
		AND (
				@RelatedToProductID IS NULL OR @RelatedToProductID=0
				OR rp.ProductID1=@RelatedToProductID
			)
		AND	(
				@ShowHidden = 1 OR p.Published = 1
			)
		AND 
			(
				p.Deleted=0
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Published = 1
			)
		AND 
			(
				@ShowHidden = 1 OR pv.Deleted = 0
			)
		AND (
				@PriceMin IS NULL OR @PriceMin=0
				OR pv.Price > @PriceMin	
			)
		AND (
				@PriceMax IS NULL OR @PriceMax=2147483644 -- max value
				OR pv.Price < @PriceMax
			)
		AND	(
				@SearchKeywords = 0 or 
				(
					-- search standard content
					patindex(@Keywords, p.name) > 0
					or patindex(@Keywords, pv.name) > 0
					or patindex(@Keywords, pv.sku) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, p.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pv.Description) > 0)					
					-- search language content
					or patindex(@Keywords, pl.name) > 0
					or patindex(@Keywords, pvl.name) > 0
					or (@SearchDescriptions = 1 and patindex(@Keywords, pl.ShortDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pl.FullDescription) > 0)
					or (@SearchDescriptions = 1 and patindex(@Keywords, pvl.Description) > 0)
				)
			)
		AND
			(
				@ShowHidden = 1
				OR
				(getutcdate() between isnull(pv.AvailableStartDateTime, '1/1/1900') and isnull(pv.AvailableEndDateTime, '1/1/2999'))
			)
		AND
			(
				--filter by specs
				@SpecAttributesCount = 0
				OR
				(
					NOT EXISTS(
						SELECT 1 
						FROM #FilteredSpecs [fs]
						WHERE [fs].SpecificationAttributeOptionID NOT IN (
							SELECT psam.SpecificationAttributeOptionID
							FROM dbo.Nop_Product_SpecificationAttribute_Mapping psam
							WHERE psam.AllowFiltering = 1 AND psam.ProductID = p.ProductID
							)
						)
					
				)
			)
		)
	ORDER BY 
		CASE WHEN @OrderBy = 0 AND @CategoryID IS NOT NULL AND @CategoryID > 0
		THEN pcm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @ManufacturerID IS NOT NULL AND @ManufacturerID > 0
		THEN pmm.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0 AND @RelatedToProductID IS NOT NULL AND @RelatedToProductID > 0
		THEN rp.DisplayOrder END ASC,
		CASE WHEN @OrderBy = 0
		THEN p.[Name] END ASC,
		CASE WHEN @OrderBy = 5
		THEN dbo.NOP_getnotnullnotempty(pl.[Name],p.[Name]) END ASC,
		CASE WHEN @OrderBy = 10
		THEN pv.Price END ASC,
		CASE WHEN @OrderBy = 15
		THEN p.CreatedOn END DESC

	DROP TABLE #FilteredSpecs

	CREATE TABLE #PageIndex 
	(
		[IndexID] int IDENTITY (1, 1) NOT NULL,
		[ProductID] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductID])
	SELECT ProductID
	FROM #DisplayOrderTmp with (NOLOCK)
	GROUP BY ProductID
	ORDER BY min([ID])

	--total records
	SET @TotalRecords = @@rowcount	
	SET ROWCOUNT @RowsToReturn
	
	DROP TABLE #DisplayOrderTmp

	--return
	SELECT  
		p.ProductId,
		p.Name,
		p.ShortDescription,
		p.FullDescription,
		p.AdminComment,
		p.TemplateId,
		p.ShowOnHomePage,
		p.MetaKeywords,
		p.MetaDescription,
		p.MetaTitle,
		p.SEName,
		p.AllowCustomerReviews,
		p.AllowCustomerRatings,
		p.RatingSum,
		p.TotalRatingVotes,
		p.Published,
		p.Deleted,
		p.CreatedOn,
		p.UpdatedOn
	FROM
		#PageIndex [pi]
		INNER JOIN Nop_Product p with (NOLOCK) on p.ProductID = [pi].ProductID
	WHERE
		[pi].IndexID > @PageLowerBound AND 
		[pi].IndexID < @PageUpperBound
	ORDER BY
		IndexID
	
	SET ROWCOUNT 0

	DROP TABLE #PageIndex
END
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ActivityLogClearAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ActivityLogClearAll]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_BlogPostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_BlogPostLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionLoadNonEmpty]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerSessionLoadNonEmpty]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PrivateMessageLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PrivateMessageLoadAll]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_SearchLogClear]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_SearchLogClear]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_LogClear]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_LogClear]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_NewsLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_NewsLoadAll]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_PictureLoadAllPaged]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_PictureLoadAllPaged]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_ProductRatingCreate]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_ProductRatingCreate]
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_RewardPointsHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_RewardPointsHistoryLoadAll]
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.PaymentManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.PaymentManager.CacheEnabled', N'true', N'')
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.CreditCardTypeManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.PaymentMethodManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.PaymentStatusManager.CacheEnabled'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.BlacklistManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.BlacklistManager.CacheEnabled', N'true', N'')
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.IpBlacklistManager.CacheEnabled'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.ShippingManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.ShippingManager.CacheEnabled', N'true', N'')
END
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.ShippingRateComputationMethodManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.ShippingStatusManager.CacheEnabled'
GO

DELETE FROM [dbo].[Nop_Setting]
WHERE [Name] = N'Cache.ShippingMethodManager.CacheEnabled'
GO


IF NOT EXISTS (
		SELECT 1
		FROM [dbo].[Nop_Setting]
		WHERE [Name] = N'Cache.CustomerActivityManager.CacheEnabled')
BEGIN
	INSERT [dbo].[Nop_Setting] ([Name], [Value], [Description])
	VALUES (N'Cache.CustomerActivityManager.CacheEnabled', N'true', N'')
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountLimitation'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountLimitation
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountLimitation]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_DiscountLimitation]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountRequirement'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountRequirement
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountRequirement]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_DiscountRequirement]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Discount_Nop_DiscountType'
           AND parent_obj = Object_id('Nop_Discount')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Discount
DROP CONSTRAINT FK_Nop_Discount_Nop_DiscountType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_DiscountType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_DiscountType]
END
GO


IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Log_Nop_LogType'
           AND parent_obj = Object_id('Nop_Log')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Log
DROP CONSTRAINT FK_Nop_Log_Nop_LogType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_LogType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_LogType]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ProductVariant_Nop_LowStockActivity'
           AND parent_obj = Object_id('Nop_ProductVariant')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ProductVariant
DROP CONSTRAINT FK_Nop_ProductVariant_Nop_LowStockActivity
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_LowStockActivity]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_LowStockActivity]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_ShoppingCart_Nop_ShoppingCartType'
           AND parent_obj = Object_id('Nop_ShoppingCartItem')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_ShoppingCartItem
DROP CONSTRAINT FK_Nop_ShoppingCart_Nop_ShoppingCartType
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ShoppingCartType]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_ShoppingCartType]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Order_Nop_ShippingStatus'
           AND parent_obj = Object_id('Nop_Order')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Order
DROP CONSTRAINT FK_Nop_Order_Nop_ShippingStatus
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_ShippingStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_ShippingStatus]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Order_Nop_PaymentStatus'
           AND parent_obj = Object_id('Nop_Order')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Order
DROP CONSTRAINT FK_Nop_Order_Nop_PaymentStatus
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_PaymentStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_PaymentStatus]
END
GO

IF EXISTS (SELECT 1
           FROM   sysobjects
           WHERE  name = 'FK_Nop_Order_Nop_OrderStatus'
           AND parent_obj = Object_id('Nop_Order')
           AND Objectproperty(id,N'IsForeignKey') = 1)
ALTER TABLE dbo.Nop_Order
DROP CONSTRAINT FK_Nop_Order_Nop_OrderStatus
GO
if exists (select 1 from sysobjects where id = object_id(N'[dbo].[Nop_OrderStatus]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
DROP TABLE [Nop_OrderStatus]
END
GO

UPDATE dbo.Nop_Log
SET LogTypeId=20
WHERE LogTypeId=0
GO


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_CustomerSessionDeleteExpired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_CustomerSessionDeleteExpired]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_DiscountUsageHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_DiscountUsageHistoryLoadAll]
GO 

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_PostLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_PostLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_Forums_SubscriptionLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_Forums_SubscriptionLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardLoadAll]
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID(N'[dbo].[Nop_GiftCardUsageHistoryLoadAll]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Nop_GiftCardUsageHistoryLoadAll]
GO
