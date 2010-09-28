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
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'', N'%Store.Name%. You have received a new private message',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
<br />
You have received a new private message.
</p>')
	END
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
		INSERT [dbo].[Nop_MessageTemplateLocalized] ([MessageTemplateID], [LanguageID], [BCCEmailAddresses], [Subject], [Body]) 
		VALUES (@MessageTemplateID, 7, N'', N'New customer registration',  N'<p><a href="%Store.URL%">%Store.Name%</a> <br />
<br />A new customer registered with your store. Below are the customer''s details:
<br />Full name: %Customer.FullName%
<br />Email: %Customer.Email%
</p>')
	END
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

UPDATE [Nop_DiscountType]
SET [DiscountTypeID] = 20
WHERE [DiscountTypeID] = 0
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