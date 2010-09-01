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