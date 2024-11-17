
--use [itjobsupport]
--use [nopcommerce46]
-- use [onjobsupport47]
-- use [qaonjobsupport47]

SELECT * FROM [dbo].[LocaleStringResource] WHERE ResourceValue like '%options%'
SELECT * FROM [dbo].[LocaleStringResource] WHERE ResourceName like '%.product(s)'

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

SELECT * FROM [dbo].[LocaleStringResource] WHERE [ResourceName] like 'Admin.Configuration.Stores.Fields.%';
SELECT * FROM [dbo].[LocaleStringResource] WHERE [ResourceName] like 'Products.Tags%';

--------------- End: [LocaleStringResource] Table Scripts--------------------------------------------------------------


---------------------	End: [Setting] Table Scripts---------------------------------------------------------------------

---------Start: [Countries] Tab7, 15, 20, 50, 100le Scripts : Update countries table that we only support ---------------------------------

SELECT * FROM [Country] WHERE Published=1

-- Update [Country] SET Published=0 WHERE [TwoLetterIsoCode] NOT IN ('US','GB','AU','CA','IN','NZ','DE','IT','SG','FR','AE')

Update [Country] SET Published=1 WHERE [TwoLetterIsoCode] IN ('US')
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode] IN ('AE')
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode]='IT'
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode]='SG'
Update [Country] SET Published=0 WHERE [TwoLetterIsoCode]='NZ'

Update [Country] SET DisplayOrder=0 WHERE [TwoLetterIsoCode]='IN'
Update [Country] SET DisplayOrder=10 WHERE [TwoLetterIsoCode]='US'
Update [Country] SET DisplayOrder=20 WHERE [TwoLetterIsoCode]='CA'
Update [Country] SET DisplayOrder=30 WHERE [TwoLetterIsoCode]='AU'
Update [Country] SET DisplayOrder=40 WHERE [TwoLetterIsoCode]='FR'
Update [Country] SET DisplayOrder=50 WHERE [TwoLetterIsoCode]='DE'
Update [Country] SET DisplayOrder=60 WHERE [TwoLetterIsoCode]='IT'
Update [Country] SET DisplayOrder=70 WHERE [TwoLetterIsoCode]='SG'
Update [Country] SET DisplayOrder=80 WHERE [TwoLetterIsoCode]='GB'                              

-----------	End: [Countries] Table Scripts -----------------------------------------------------------------------------------

SELECT * FROM [CustomerAttribute] Where Id>=7

-- Delete [CustomerAttribute]
--DBCC CHECKIDENT ('[CustomerAttribute]', RESEED, 0);
--DBCC CHECKIDENT ('[SpecificationAttributeOption]', RESEED, 4);

-- Make all feilds mandatory except secondary technology which is not used presently
UPDATE [CustomerAttribute] SET IsRequired=1 WHERE Id Not In (8)

-- delete [SpecificationAttributeOption] WHERE id=6

--DBCC CHECKIDENT ('SpecificationAttributeOption', NORESEED);
--DBCC CHECKIDENT ('SpecificationAttributeOption', RESEED, 27);

--  Store SEO Settings
UPDATE [dbo].[Store] SET DefaultTitle='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET DefaultMetaDescription='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET DefaultMetaKeywords='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET HomepageDescription='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET HomepageTitle='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1

-- Topics Settings
UPDATE [dbo].[Topic] SET [Body] = '<p>Please contact us for any queries you have. We will be happy to assist you.</p>' WHERE Id=4 -- Contact us page
UPDATE [dbo].[Topic] SET [Body] = '<p></p>', Title='' WHERE Id=7 -- Login Page Info




