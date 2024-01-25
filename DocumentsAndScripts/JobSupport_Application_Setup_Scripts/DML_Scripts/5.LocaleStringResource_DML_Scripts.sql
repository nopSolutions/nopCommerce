
-- use onjobsupport47


-- SELECT * FROM [dbo].[LocaleStringResource] WHERE ResourceValue like '%options%'
-- SELECT * FROM [dbo].[LocaleStringResource] WHERE ResourceName like '%.product(s)'

---------------  Local String update queries    ------------------------------------------

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Recently viewed profiles' WHERE [ResourceName] = 'products.recentlyviewedproducts';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Shortlisted' WHERE [ResourceName]='pagetitle.wishlist';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='The profile has been added to your <a href="{0}">shortlist</a>'  WHERE [ResourceName]='products.producthasbeenaddedtothewishlist.link';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Shortlisted'  WHERE [ResourceName]='wishlist';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='There are no shortlisted profiles' WHERE [ResourceName]='wishlist.cartisempty';

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='My profile reviews' WHERE [ResourceName]='account.customerproductreviews';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile specifications' WHERE [ResourceName]='products.specs';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='New Profiles' WHERE [ResourceName]='pagetitle.newproducts';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='New profiles' WHERE [ResourceName]='products.newproducts';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Search profiles' WHERE [ResourceName]='search.searchbox.text.label';

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Search profiles' WHERE [ResourceName]='search.searchbox.tooltip';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='No profiles were found that matched your criteria.Please adjust your filter criteria to see more profiles.'  WHERE resourcename = 'catalog.products.noresult';
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile review for' WHERE [ResourceName]='account.customerproductreviews.productreviewfor'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='You will see the profile review after approving by administrator.' WHERE [ResourceName]='reviews.seeafterapproving'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile review for' WHERE [ResourceName]='account.customerproductreviews.productreviewfor'

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Subscription(s)' WHERE [ResourceName]='shoppingcart.product(s)'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='By creating an account on our website, you will be able to communicate those who needs/provide proxy support in an efficent manner with seamless experiance, and also you can switch from one proxy support to another with out worrying too much.' WHERE [ResourceName]='account.login.newcustomertext'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Featured Profiles' WHERE [ResourceName]='homepage.products'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Technical Details' WHERE [ResourceName]='account.options'

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Mobile Number' WHERE [ResourceName]='account.fields.phone'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Mobile number is not valid' WHERE [ResourceName]='account.fields.phone.notvalid'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Mobile number is required' WHERE [ResourceName]='account.fields.phone.required'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile Photo' WHERE [ResourceName]='account.avatar'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Remove Photo' WHERE [ResourceName]='account.avatar.removeavatar'

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Photo must be in GIF or JPEG format with the maximum size of 1 MB' WHERE [ResourceName]='account.avatar.uploadrules'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile reviews for' WHERE [ResourceName]='reviews.productreviewsfor'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile tags' WHERE [ResourceName]='products.tags'
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Sign In Here' WHERE [ResourceName]='account.login.returningcustomer'
Update [dbo].[LocaleStringResource] SET [ResourceValue]='Similar Profiles' WHERE [ResourceName]='products.relatedproducts'

Update [dbo].[LocaleStringResource] SET [ResourceValue]='You are already subscribed for this profile back in available notification' WHERE [ResourceName]='backinstocksubscriptions.alreadysubscribed'
Update [dbo].[LocaleStringResource] SET [ResourceValue]='Receive an email when profile is available' WHERE [ResourceName]='backinstocksubscriptions.popuptitle'
Update [dbo].[LocaleStringResource] SET [ResourceValue]='You will receive a onetime e-mail when this profile is available' WHERE [ResourceName]='backinstocksubscriptions.tooltip'
Update [dbo].[LocaleStringResource] SET [ResourceValue]='You will receive an e-mail when a particular profile is back to available.' WHERE [ResourceName]='account.backinstocksubscriptions.description'
Update [dbo].[LocaleStringResource] SET [ResourceValue]='Partner Consultancies' WHERE [ResourceName]='manufacturers'
--Update [dbo].[LocaleStringResource] SET ResourceValue='Your Shopping Cart is empty!' WHERE [ResourceName]='shoppingcart.cartisempty';


IF NOT EXISTS (SELECT * FROM [LocaleStringResource] WHERE [ResourceName]='Orders.UpgradeSubscription.Message')
   BEGIN
		INSERT INTO [dbo].[LocaleStringResource]([ResourceName],[ResourceValue],[LanguageId]) 
		VALUES('Orders.UpgradeSubscription.Message','Please upgrade to Subscription to View Mobile Number ,Send the messages.',1)
   END


Update [dbo].[LocaleStringResource] SET [ResourceValue]='Show profiles in category {0}' WHERE [ResourceName]='media.category.imagelinktitleformat'
-- Show products in category {0}

IF NOT EXISTS (SELECT * FROM [LocaleStringResource] WHERE [ResourceName]='rewardpoints.fields.date')
   BEGIN
		INSERT INTO [dbo].[LocaleStringResource]([ResourceName],[ResourceValue],[LanguageId]) 
		VALUES('rewardpoints.fields.date','Date',1)
   END