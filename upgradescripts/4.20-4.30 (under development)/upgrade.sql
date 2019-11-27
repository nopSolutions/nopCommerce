--upgrade scripts from nopCommerce 4.20 to 4.30

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableHtmlMinification">
    <Value>HTML minification</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Hint">
    <Value>
      Specification attributes are product features i.e, screen size, number of USB-ports, visible on product details page. Specification attributes can be used for filtering products on the category details page. Unlike product attributes, specification attributes are used for information purposes only.
      You can add attribute for your product using existing list of attributes, or if you need to create a new one go to Catalog &gt; Attributes &gt; Specification attributes.
    </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Tax.DefaultTaxCategory.Hint">
    <Value>Select default tax category for products. It''ll be pre-selected on the "Add new product" page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Attributes.CheckoutAttributes.Fields.TaxCategory.Hint">
    <Value>The tax classification for this attribute (used to calculate tax). You can manage tax categories by selecting Configuration &gt; Tax &gt; Tax Categories.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute.Hint">
    <Value>Choose a product specification attribute. You can manage specification attributes from Catalog &gt; Attributes &gt; Product Specifications.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.OrderTotalDiscount.Hint">
    <Value>The total discount applied to this order. Manage your discounts from Promotions &gt; Discounts.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.PaymentMethod.Hint">
    <Value>The payment method used for this transaction. You can manage Payment Methods from Configuration &gt; Payment &gt; Payment Methods.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.ShippingMethod.Hint">
    <Value>The customers chosen shipping method for this order. You can manage shipping methods from Configuration &gt; Shipping &gt; Shipping Methods.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Fields.Tax.Hint">
    <Value>Total tax applied to this order. Manage your tax settings from Configuration &gt; Tax.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue">
    <Value>Error while save attribute combinations. Attribute value not specified.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum">
    <Value>Show on forum</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForum.Hint">
    <Value>Check to show CAPTCHA on forum, when editing and creating a topic or post.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanDeliver">
    <Value>Delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanDeliver.Hint">
    <Value>Check to apply current date to delivery.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanShip">
    <Value>Shipped</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.CanShip.Hint">
    <Value>Check to apply current date to shipment.</Value>
  </LocaleResource>
  <LocaleResource Name="ShippingMethod.NotAvailableMethodsError">
    <Value>Your order cannot be completed at this time as there is no shipping methods available for it. Please make necessary changes in your shipping address.</Value>
  </LocaleResource>
  <LocaleResource Name="ShippingMethod.SpecifyMethodError">
    <Value>Please specify shipping method.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.NotAvailableMethodsError">
    <Value>Your order cannot be completed at this time as there is no payment methods available for it.</Value>
  </LocaleResource>
  <LocaleResource Name="PaymentMethod.SpecifyMethodError">
    <Value>Please specify payment method.</Value>
  </LocaleResource>
  <LocaleResource Name="AjaxCart.Failure">
    <Value>Failed to add the product. Please refresh the page and try one more time.</Value>
  </LocaleResource>
  <LocaleResource Name="MainMenu.AjaxFailure">
    <Value>Failed to open menu. Please refresh the page and try one more time.</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Token.Key">
    <Value>Verification token</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Use3ds">
    <Value>Use 3D-Secure</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Use3ds.Hint">
    <Value>Determine whether to use 3D-Secure feature. Used for Strong customer authentication (SCA). SCA is generally friction-free for the buyer, but a card-issuing bank may require additional authentication for some payments. In those cases, the buyer must verify their identiy with the bank using an additional secure dialog.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.AclCustomerRoles.Hint">
    <Value>Choose one or several customer roles i.e. administrators, vendors, guests, who will be able to see this product in catalog. If you don''t need this option just leave this field empty. In order to use this functionality, you have to disable the following setting: Configuration &gt; Settings &gt; Catalog &gt; Ignore ACL rules (sitewide).</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.CannotBeFound">
    <Value>The coupon code cannot be found</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Empty">
    <Value>The coupon code is empty</Value>
  </LocaleResource>
  <LocaleResource Name="PrivateMessages.Inbox.NoItems">
    <Value>No inbox messages</Value>
  </LocaleResource>
  <LocaleResource Name="PrivateMessages.Sent.NoItems">
    <Value>No sent messages</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude">
    <Value>Latitude</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.Hint">
    <Value>Specify a latitude (DD.dddddddd°).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange">
    <Value>Latitude should be in range -90 to 90</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision">
    <Value>Precision should be less then 8</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue">
    <Value>Latitude and Longitude should be specify together</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude">
    <Value>Longitude</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.Hint">
    <Value>Specify a longitude (DD.dddddddd°).</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange">
    <Value>Longitude should be in range -180 to 180</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision">
    <Value>Precision should be less then 8</Value>
  </LocaleResource>
    <LocaleResource Name="Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue">
    <Value>Latitude and Longitude should be specify together</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.LoadNotDelivered">
    <Value>Load not delivered</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.Shipments.List.LoadNotDelivered.Hint">
    <Value>Load only undelivered shipments</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Fields.Newsletter.Hint">
    <Value>Choose stores to subscribe to newsletter.</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.RedirectMessage">
    <Value>Redirected</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Instructions">
    <Value>
        <![CDATA[
            <div style="margin: 0 0 10px;">
                <em><b>Warning: Square sandbox data has been changed. For more information visit our <a href="https://docs.nopcommerce.com/user-guide/configuring/settingup/payments/methods/square.html" target="_blank">documentation</a>.</em></b><br />
                <br />
                For plugin configuration, follow these steps:<br />
                <br />
                1. You will need a Square Merchant account. If you don''t already have one, you can sign up here: <a href="http://squ.re/nopcommerce" target="_blank">https://squareup.com/signup/</a><br />
                2. Sign in to ''Square Merchant Dashboard''. Go to ''Account & Settings'' &#8594; ''Locations'' tab and create new location.<br />
                <em>   Important: Your merchant account must have at least one location with enabled credit card processing. Please refer to the Square customer support if you have any questions about how to set this up.</em><br />
                3. Sign in to your ''Square Developer Dashboard'' at <a href="http://squ.re/nopcommerce1" target="_blank">https://connect.squareup.com/apps</a>; use the same login credentials as your merchant account.<br />
                4. Click on ''Create Your First Application'' and fill in the ''Application Name''. This name is for you to recognize the application in the developer portal and is not used by the plugin. Click ''Create Application'' at the bottom of the page.<br />
                5. Now you are on the details page of the previously created application. On the ''Credentials'' tab click on the ''Change Version'' button and choose ''2019-09-25''.<br />
                6. Make sure you uncheck ''Use sandbox'' below.<br />
                7. In the ''Square Developer Dashboard'' go to the details page of the your previously created application:
                    <ul>
                        <li>On the ''Credentials'' tab make sure the ''Application mode'' setting value is ''Production''</li>
                        <li>On the ''Credentials'' tab copy the ''Application ID'' and paste it into ''Application ID'' below</li>
                        <li>Go to ''OAuth'' tab. Click ''Show'' on the ''Application Secret'' field. Copy the ''Application Secret'' and paste it into ''Application Secret'' below</li>
                        <li>Copy this URL: <em>{0}</em>. On the ''OAuth'' tab paste this URL into ''Redirect URL''. Click ''Save''</li>
                    </ul>
                8. Click ''Save'' below to save the plugin configuration.<br />
                9. Click ''Obtain access token'' below; the Access token field should populate.<br />
                <em>Note: If for whatever reason you would like to disable an access to your accounts, simply ''Revoke access tokens'' below.</em><br />
                10. Choose the previously created location. ''Location'' is a required parameter for payment requests.<br />
                11. Fill in the remaining fields and click ''Save'' to complete the configuration.<br />
                <br />
                <em>Note: The payment form must be generated only on a webpage that uses HTTPS.</em><br />
            </div>
        ]]>
    </Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.UploadIconsArchive.Hint">
    <Value>Upload archive with favicon and app icons for different operating systems and devices. You can see an example of the favicon and app icons archive in /icons/samples in the root of the site. Your favicon and app icons path is "/icons/icons_{0}"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.MessageTemplates.Description.Wishlist.EmailAFriend">
    <Value><![CDATA[This message template is used when a customer wants to share some product from the wishlist with a friend by sending an email. You can set up this option by ticking the checkbox Allow customers to email their wishlists in Configuration - Settings - Shopping cart settings.]]></Value>
  </LocaleResource>
</Language>
'

CREATE TABLE #LocaleStringResourceTmp
	(
		[ResourceName] [nvarchar](200) NOT NULL,
		[ResourceValue] [nvarchar](max) NOT NULL
	)

INSERT INTO #LocaleStringResourceTmp (ResourceName, ResourceValue)
SELECT	nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
FROM	@resources.nodes('//Language/LocaleResource') AS R(nref)

--do it for each existing language
DECLARE @ExistingLanguageID int
DECLARE cur_existinglanguage CURSOR FOR
SELECT [ID]
FROM [Language]
OPEN cur_existinglanguage
FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ResourceName nvarchar(200)
	DECLARE @ResourceValue nvarchar(MAX)
	DECLARE cur_localeresource CURSOR FOR
	SELECT ResourceName, ResourceValue
	FROM #LocaleStringResourceTmp
	OPEN cur_localeresource
	FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName))
		BEGIN
			UPDATE [LocaleStringResource]
			SET [ResourceValue]=@ResourceValue
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		ELSE 
		BEGIN
			INSERT INTO [LocaleStringResource]
			(
				[LanguageId],
				[ResourceName],
				[ResourceValue]
			)
			VALUES
			(
				@ExistingLanguageID,
				@ResourceName,
				@ResourceValue
			)
		END
		
		IF (@ResourceValue is null or @ResourceValue = '')
		BEGIN
			DELETE [LocaleStringResource]
			WHERE LanguageID=@ExistingLanguageID AND ResourceName=@ResourceName
		END
		
		FETCH NEXT FROM cur_localeresource INTO @ResourceName, @ResourceValue
	END
	CLOSE cur_localeresource
	DEALLOCATE cur_localeresource


	--fetch next language identifier
	FETCH NEXT FROM cur_existinglanguage INTO @ExistingLanguageID
END
CLOSE cur_existinglanguage
DEALLOCATE cur_existinglanguage

DROP TABLE #LocaleStringResourceTmp
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.showonforum')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.showonforum', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.recaptcharequesttimeout')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.recaptcharequesttimeout', 20, 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'squarepaymentsettings.use3ds')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'squarepaymentsettings.use3ds', 'false', 0)
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[StorePickupPoint]') and NAME='Latitude')
BEGIN
	ALTER TABLE [StorePickupPoint] ADD
	Latitude decimal(18, 8) NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[StorePickupPoint]') and NAME='Longitude')
BEGIN
	ALTER TABLE [StorePickupPoint] ADD
	Longitude decimal(18, 8) NULL
END
GO
