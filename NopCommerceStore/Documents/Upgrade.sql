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
		WHERE [DiscountTypeID] = 0)
BEGIN
	INSERT [dbo].[Nop_DiscountType] ([DiscountTypeID], [Name])
	VALUES (0, N'Assigned to order subtotal')
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
    FROM [dbo].[Nop_DiscountType]
    WHERE [DiscountTypeID] = 0)
BEGIN
	UPDATE [Nop_DiscountType]
	SET [DiscountTypeID] = 20
	WHERE [DiscountTypeID] = 0
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