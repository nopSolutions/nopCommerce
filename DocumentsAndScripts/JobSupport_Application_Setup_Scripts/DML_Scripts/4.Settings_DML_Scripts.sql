
-- use onjobsupport47

------------------ Start: [Setting] Table Scripts---------------------------------------------------------------------

SELECT * FROM [dbo].[Setting] WHERE [Name] like '%catalogsettings%';
SELECT * FROM [dbo].[Setting] WHERE [Value] like '%20%';

-- shipping settings
UPDATE [dbo].[Setting] SET Value='<p>Mail Personal or Business Check, Cashiers Check or money order to:</p><p><br /><b>On Job Support</b> <br /><b>Hitech City,</b> <br /><b>Hyderabad,500018 </b> <br /><b>INDIA</b></p><p>Notice that if you pay by Personal or Business Check, your order may be held for up to 10 days after we receive your check to allow enough time for the check to clear. If you want us to server faster upon receipt of your payment, then we recommend your send a money order or Cashiers check.</p>' WHERE Name = 'checkmoneyorderpaymentsettings.descriptiontext';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'shippingsettings.hideshippingtotal';


-- catalog settings
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

-- catalog settings -- Filter settings
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.enablemanufacturerfiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.enablepricerangefiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.productsbytagpricerangefiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.searchpagepricerangefiltering';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'catalogsettings.allowcustomerstosearchwithmanufacturername';
UPDATE [dbo].[Setting] SET Value='0' WHERE Name = 'catalogsettings.manufacturersblockitemstodisplay';

-- ordersettings
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.anonymouscheckoutallowed';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'ordersettings.disablebillingaddresscheckoutstep';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'ordersettings.termsofserviceonshoppingcartpage';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.termsofserviceonorderconfirmpage';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.onepagecheckoutdisplayordertotalsonpaymentinfotab';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'ordersettings.returnrequestsenabled';
UPDATE [dbo].[Setting] SET Value='{ID}{YYYY}{MM}{DD}' WHERE Name = 'ordersettings.customordernumbermask';

-- admin area settings
UPDATE [dbo].[Setting] SET Value='20, 50, 100' WHERE Name = 'adminareasettings.gridpagesizes';
UPDATE [dbo].[Setting] SET Value='20' WHERE Name = 'adminareasettings.defaultgridpagesize';

-- Customer settings
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.notifynewcustomerregistration';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.showcustomerslocation';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.showcustomersjoindate';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.allowviewingprofiles';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'customersettings.hidenewsletterblock';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.storelastvisitedpage';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.genderenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.countryenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.countryrequired';

UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.phoneenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.phonerequired';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'customersettings.companyenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.hidedownloadableproductstab';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.hidebackinstocksubscriptionstab';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'customersettings.newsletterenabled';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'customersettings.dateofbirthenabled';
UPDATE [dbo].[Setting] SET Value='True'  WHERE Name = 'customersettings.allowcustomerstouploadavatars';

-- Customer settings -- Address Settings -- Address form fields
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
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.showgiftcardbox';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.showproductimagesonshoppingcart';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.allowcartitemediting';
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'shoppingcartsettings.moveitemsfromwishlisttocart';

-- sitemap settings
-- hide manufacture list in sitemap
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'sitemapsettings.sitemapincludemanufacturers';
-- hide cateogories list in sitemap.It is shwoing both take support and give support but we should not show both categories
-- hence hiding categories temporarily until a solution/fix is identified
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'sitemapsettings.sitemapincludecategories';

-- menu settings
-- hide search item in top menu
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'displaydefaultmenuitemsettings.displayproductsearchmenuitem';
-- hide search item in footer
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'displaydefaultfooteritemsettings.displayproductsearchfooteritem';


-- footer settings
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'displaydefaultfooteritemsettings.displayproductsearchfooteritem';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'displaydefaultfooteritemsettings.displayapplyvendoraccountfooteritem';
UPDATE [dbo].[Setting] SET Value='False'  WHERE Name = 'displaydefaultfooteritemsettings.displaycustomeraddressesfooteritem';
-- hide footer option  'Apply for vendor account'
UPDATE [dbo].[Setting] SET Value='False' WHERE Name = 'vendorsettings.allowcustomerstoapplyforvendoraccount';


-- Setting insert scripts
IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='customersettings.genderspecificationattributeid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('customersettings.genderspecificationattributeid','8',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='customersettings.gendermalespecificationattributeoptionid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('customersettings.gendermalespecificationattributeoptionid','28',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='customersettings.genderfemalespecificationattributeoptionid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('customersettings.genderfemalespecificationattributeoptionid','29',0)
   END

 IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='customersettings.showsecondarytechnologyspecificationattribute')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('customersettings.showsecondarytechnologyspecificationattribute','False',0)
   END


------------------------- Shopping cart settings -----------------

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.threemonthsubscriptionproductid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.threemonthsubscriptionproductid','1',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.sixmonthsubscriptionproductid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.sixmonthsubscriptionproductid','2',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.oneyearsubscriptionproductid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.oneyearsubscriptionproductid','3',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.threemonthsubscriptionallottedcount')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.threemonthsubscriptionallottedcount','0',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.sixmonthsubscriptionallottedcount')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.sixmonthsubscriptionallottedcount','25',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.oneyearsubscriptionallottedcount')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.oneyearsubscriptionallottedcount','50',0)
   END