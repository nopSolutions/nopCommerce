


	SELECT * FROM .[dbo].[LocaleStringResource] WHERE ResourceValue like '%stock';


	Update [dbo].[LocaleStringResource] SET ResourceValue='Similar Profiles' WHERE ResourceName='products.relatedproducts'	 
	Update [dbo].[LocaleStringResource] SET ResourceValue='You are already subscribed for this profile back in available notification' WHERE ResourceName='backinstocksubscriptions.alreadysubscribed'
	-- You're already subscribed for this product back in stock notification
	Update [dbo].[LocaleStringResource] SET ResourceValue='Receive an email when profile is available' WHERE ResourceName='backinstocksubscriptions.popuptitle'
	--Receive an email when this arrives in stock
	Update [dbo].[LocaleStringResource] SET ResourceValue='You will receive a onetime e-mail when this profile is available' WHERE ResourceName='backinstocksubscriptions.tooltip'
	-- You'll receive a onetime e-mail when this product is available for ordering again. We will not send you any other e-mails or add you to our newsletter; you will only be e-mailed about this product!
	Update [dbo].[LocaleStringResource] SET ResourceValue='You will receive an e-mail when a particular profile is back to available.' WHERE ResourceName='account.backinstocksubscriptions.description'
	-- You will receive an e-mail when a particular product is back in stock.

	-- You are not currently subscribed to any Back In Stock notification lists
	-- Back in stock subscriptions
	-- Product xyz is in stock.
	-- on job support. Back in stock notification

	--=============== ActivityLogtype ================

	INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
	VALUES ('PublicStore.EditCustomerAvailabilityToTrue','PublicStore.EditCustomerAvailabilityToTrue',1)
	GO




	--=============== ActivityLogtype ================