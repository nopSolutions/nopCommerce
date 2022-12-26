
use [itjobsupport]

use [nopcommerce46]

SELECT * FROM [dbo].[LocaleStringResource] WHERE ResourceValue like '%options%'
SELECT * FROM [dbo].[LocaleStringResource] WHERE ResourceName like '%.product(s)'
SELECT * FROM [dbo].[Setting] WHERE [Name] like '%reviews%';

---------------------------------------------------------
-- ***  SCHEMA SCRIPTS ****
---------------------------------------------------------

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Customer]') AND NAME = 'CustomerProfileTypeId')
BEGIN
	ALTER TABLE [Customer]
	ADD CustomerProfileTypeId [int] NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'SenderSubject')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD SenderSubject [nvarchar](450) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'SenderBodyText')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD SenderBodyText [nvarchar](max) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'RecipientBodyText')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD RecipientBodyText [nvarchar](max) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'IsSystemGenerated')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD IsSystemGenerated Bit NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[Forums_PrivateMessage]') AND NAME = 'ParentMessageId')
BEGIN
	ALTER TABLE [Forums_PrivateMessage]
	ADD ParentMessageId [int] NULL
END
GO


---------------------------------------------------------
-- ***  END SCHEMA SCRIPTS ****
---------------------------------------------------------

---------------  Local String update queries    ------------------------------------------

UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Recently viewed profiles' WHERE resourcename = 'products.recentlyviewedproducts';
-- org: Recently viewed products
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Shortlisted' WHERE resourcename='pagetitle.wishlist';
-- org: Wishlist
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='The profile has been added to your <a href="{0}">shortlist</a>'  WHERE resourcename='products.producthasbeenaddedtothewishlist.link';
-- org: The product has been added to your <a href="{0}">wishlist</a>
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Shortlisted'  WHERE resourcename='wishlist';
-- org: Wishlist
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='There are no shortlisted profiles' WHERE resourcename='wishlist.cartisempty';
-- org: The wishlist is empty!
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='My profile reviews' WHERE resourcename='account.customerproductreviews';
-- org: My product reviews
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Profile specifications' WHERE resourcename='products.specs';
-- org: The wishlist is empty!
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='New Profiles' WHERE resourcename='pagetitle.newproducts';
-- org: New Products
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='New profiles' WHERE resourcename='products.newproducts';
-- org: New products
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.text.label';
-- org: Search store
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.tooltip';
-- org: Search store
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.tooltip';
-- org: Search store
UPDATE [dbo].[LocaleStringResource]SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.tooltip';
-- org: Search store
UPDATE [dbo].[LocaleStringResource]SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.tooltip';
-- org: Search store
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.tooltip';
-- org: Search store
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='Search profiles' WHERE resourcename='search.searchbox.tooltip';
-- org: Search store
UPDATE [dbo].[LocaleStringResource] SET ResourceValue='No profiles were found that matched your criteria.Please adjust your filter criteria to see more profiles.'  WHERE resourcename = 'catalog.products.noresult';
-- org: No products were found that matched your criteria.Please adjust your filter criteria to see more profiles.
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile review for' WHERE [ResourceName]='account.customerproductreviews.productreviewfor'
--orginal : Product review for
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='You will see the profile review after approving by administrator.' WHERE [ResourceName]='reviews.seeafterapproving'
--orginal : You will see the product review after approving by a store administrator.
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile review for' WHERE [ResourceName]='account.customerproductreviews.productreviewfor'
--orginal : Product review for
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile review for' WHERE [ResourceName]='account.customerproductreviews.productreviewfor'
--orginal : Product review for
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Subscription(s)' WHERE [ResourceName]='shoppingcart.product(s)'
--orginal : Product(s)
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='By creating an account on our website, you will be able to communicate those who needs/provide proxy support in an efficent manner with seamless experiance, and also you can switch from one proxy support to another with out worrying too much.' WHERE [ResourceName]='account.login.newcustomertext'
--orginal : By creating an account on our website, you will be able to shop faster, be up to date on an orders status, and keep track of the orders you have previously made.
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Subscription(s)' WHERE [ResourceName]='shoppingcart.product(s)'
-- orginal : Product(s)
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Featured Profiles' WHERE [ResourceName]='homepage.products'
-- orginal : Featured products
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Technical Details' WHERE [ResourceName]='account.options'
-- Options
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Mobile Number' WHERE [ResourceName]='account.fields.phone'
-- Phone
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Mobile number is not valid' WHERE [ResourceName]='account.fields.phone.notvalid'
-- Phone number is not valid
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Mobile number is required' WHERE [ResourceName]='account.fields.phone.required'
-- Phone is required
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile Photo' WHERE [ResourceName]='account.avatar'
-- Avatar
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Remove Photo' WHERE [ResourceName]='account.avatar.removeavatar'
-- Remove avatar
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Photo must be in GIF or JPEG format with the maximum size of 1 MB' 
WHERE [ResourceName]='account.avatar.uploadrules'
-- Remove avatar
UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile reviews for' 
WHERE [ResourceName]='reviews.productreviewsfor'
-- Product reviews for

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Profile tags' 
WHERE [ResourceName]='products.tags'
-- Product tags

UPDATE [dbo].[LocaleStringResource] SET [ResourceValue]='Sign In Here' 
WHERE [ResourceName]='account.login.returningcustomer'
-- Returning Customer

Update [dbo].[LocaleStringResource] SET ResourceValue='Similar Profiles' WHERE ResourceName='products.relatedproducts'	 
Update [dbo].[LocaleStringResource] SET ResourceValue='You are already subscribed for this profile back in available notification' WHERE ResourceName='backinstocksubscriptions.alreadysubscribed'
-- You're already subscribed for this product back in stock notification
Update [dbo].[LocaleStringResource] SET ResourceValue='Receive an email when profile is available' WHERE ResourceName='backinstocksubscriptions.popuptitle'
--Receive an email when this arrives in stock
Update [dbo].[LocaleStringResource] SET ResourceValue='You will receive a onetime e-mail when this profile is available' WHERE ResourceName='backinstocksubscriptions.tooltip'
-- You'll receive a onetime e-mail when this product is available for ordering again. We will not send you any other e-mails or add you to our newsletter; you will only be e-mailed about this product!
Update [dbo].[LocaleStringResource] SET ResourceValue='You will receive an e-mail when a particular profile is back to available.' WHERE ResourceName='account.backinstocksubscriptions.description'
	-- You will receive an e-mail when a particular product is back in stock.

SELECT * FROM [dbo].[LocaleStringResource] WHERE [ResourceName] like 'Admin.Configuration.Stores.Fields.%'
SELECT * FROM [dbo].[LocaleStringResource] WHERE [ResourceName] like 'Products.Tags%'
SELECT * FROM [dbo].[LocaleStringResource] WHERE [ResourceValue] like '%Returning Customer%'

--------------- End: [LocaleStringResource] Table Scripts--------------------------------------------------------------

------------------ Start: [Setting] Table Scripts---------------------------------------------------------------------

SELECT * FROM [dbo].[Setting] WHERE [Name] like '%catalogsettings%';
SELECT * FROM [dbo].[Setting] WHERE [Value] like '%20%';

UPDATE [dbo].[Setting] SET Value='<p>Mail Personal or Business Check, Cashiers Check or money order to:</p><p><br /><b>On Job Support</b> <br /><b>Hitech City,</b> <br /><b>Hyderabad,500018 </b> <br /><b>INDIA</b></p><p>Notice that if you pay by Personal or Business Check, your order may be held for up to 10 days after we receive your check to allow enough time for the check to clear. If you want us to server faster upon receipt of your payment, then we recommend your send a money order or Cashiers check.</p>' WHERE Name = 'checkmoneyorderpaymentsettings.descriptiontext';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'shippingsettings.hideshippingtotal';

UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.showgiftcardbox';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.showproductimagesonshoppingcart';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.allowcartitemediting';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.moveitemsfromwishlisttocart';

-- catalogsettings
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.showskuonproductdetailspage';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.allowproductviewmodechanging';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.allowproductsorting';
UPDATE [dbo].[Setting] SET Value='list'  WHERE Name = 'catalogsettings.defaultviewmode';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.showsharebutton';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.productreviewsmustbeapproved';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.productreviewpossibleonlyafterpurchasing';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.notifystoreowneraboutnewproductreviews';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.notifycustomeraboutproductreviewreply';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.showproductreviewstabonaccountpage';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.emailafriendenabled';
UPDATE [dbo].[Setting] SET Value='10'	 WHERE Name = 'catalogsettings.recentlyviewedproductsnumber';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.compareproductsenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'catalogsettings.showlinktoallresultinsearchautocomplete';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.allowproductsorting';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.searchpageallowcustomerstoselectpagesize';

-- catalogsettings -- Filter settings -- nop4.6
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.enablemanufacturerfiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.enablepricerangefiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.productsbytagpricerangefiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.searchpagepricerangefiltering';

-- ordersettings
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.anonymouscheckoutallowed';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'ordersettings.disablebillingaddresscheckoutstep';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'ordersettings.termsofserviceonshoppingcartpage';

UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.termsofserviceonorderconfirmpage';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.onepagecheckoutdisplayordertotalsonpaymentinfotab';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.returnrequestsenabled';
UPDATE [dbo].[Setting] SET Value='{ID}{YYYY}{MM}{DD}' WHERE Name = 'ordersettings.customordernumbermask';


UPDATE [dbo].[Setting] SET Value='20, 50, 100' WHERE Name = 'adminareasettings.gridpagesizes';
UPDATE [dbo].[Setting] SET Value='20' WHERE Name = 'adminareasettings.defaultgridpagesize';

-- Customersettings
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.notifynewcustomerregistration';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.showcustomerslocation';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.showcustomersjoindate';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.allowviewingprofiles';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'customersettings.hidenewsletterblock';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.storelastvisitedpage';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.genderenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.countryenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.countryrequired';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.phoneenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.phonerequired';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'customersettings.companyenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.hidedownloadableproductstab';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.hidebackinstocksubscriptionstab';

-- Customersettings -- AddressSettings -- Address form fields
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.companyenabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.streetaddressenabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.streetaddress2enabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.zippostalcodeenabled';

UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.zippostalcoderequired';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.cityenabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.cityrequired';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.countryenabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.stateprovinceenabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.phoneenabled';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.phonerequired';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'addresssettings.faxenabled';

-- shopping cart settings
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'shoppingcartsettings.displaycartafteraddingproduct';

SELECT * FROM [dbo].[Setting] WHERE [Name] like 'shoppingCartSettings.MiniShoppingCartProductNumber%';
SELECT * FROM [dbo].[Setting] WHERE [Name] like 'customersettings.%stock%';
SELECT * FROM [dbo].[Setting] WHERE [Name] like 'shoppingcartsettings.displaycartafteraddingproduct';
SELECT * FROM [dbo].[Setting] WHERE [Name] like 'seosettings.%';
SELECT * FROM [dbo].[Setting] WHERE [Name] like '%infotab%';
SELECT * FROM [dbo].[Setting] WHERE [Value] like '%Anonymous%';

SELECT * FROM [dbo].[Setting] WHERE [Name] like 'AddressSettings.%';
SELECT * FROM [dbo].[Setting] WHERE Value like '%job%';

-- UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = '';

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
DBCC CHECKIDENT ('[CustomerAttribute]', RESEED, 0);
DBCC CHECKIDENT ('[SpecificationAttributeOption]', RESEED, 4);


--Make all feilds mandatory
UPDATE [CustomerAttribute] SET IsRequired=1

UPDATE [CustomerAttribute] SET IsRequired=1
SELECT * FROM [CustomerAttributeValue]

select * from [SpecificationAttribute]
select * from [SpecificationAttributeOption]

SELECT * FROM [Category]
SELECT * FROM [product]
SELECT * FROM [Topic]

SELECT * FROM [dbo].[Product_SpecificationAttribute_Mapping]

select * from [dbo].[GenericAttribute] WHERE entityid=1

select distinct(keygroup) from [dbo].[GenericAttribute] 

--delete [SpecificationAttributeOption] WHERE id=6

DBCC CHECKIDENT ('SpecificationAttributeOption', NORESEED);
DBCC CHECKIDENT ('SpecificationAttributeOption', RESEED, 27);

--  Store SEO Settings
UPDATE [dbo].[Store] SET DefaultTitle='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET DefaultMetaDescription='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET DefaultMetaKeywords='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET HomepageDescription='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1
UPDATE [dbo].[Store] SET HomepageTitle='On Job support Help |Job support | Proxy Support | Interview Support | USA | INDIA | MS Students | Masters in USA | OPT |CPT |' WHERE Id = 1


Update [dbo].[LocaleStringResource] SET ResourceValue='Partner Consultancies' WHERE ResourceName='manufacturers'
-- Manufacturers

-- Topics Settings
UPDATE [dbo].[Topic] SET [Body] = '<p>Please contact us for any queries you have. We will be happy to assist you.</p>' WHERE Id=4 -- Contact us page
UPDATE [dbo].[Topic] SET [Body] = '<p></p>', Title='' WHERE Id=7 -- Login Page Info
