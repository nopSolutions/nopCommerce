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
  <LocaleResource Name="Admin.Configuration.Payment.Methods.DownloadMorePlugins">
    <Value><![CDATA[You can download more plugins in our <a href="https://www.nopcommerce.com/extensions?category=payment-modules&utm_source=admin-panel&utm_medium=payment-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.Description.DownloadMorePlugins">
    <Value><![CDATA[You can download more nopCommerce plugins in our <a href="https://www.nopcommerce.com/marketplace?utm_source=admin-panel&utm_medium=plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins.OfficialFeed.Instructions">
    <Value><![CDATA[Here you can find third-party extensions and themes which are developed by our community and partners. They are also available in our <a href="https://www.nopcommerce.com/marketplace?utm_source=admin-panel&utm_medium=official-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.DefaultStoreTheme.GetMore">
    <Value><![CDATA[You can get more themes in our <a href="https://www.nopcommerce.com/themes?utm_source=admin-panel&utm_medium=theme-settings&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Providers.DownloadMorePlugins">
    <Value><![CDATA[You can download more plugins in our <a href="https://www.nopcommerce.com/extensions?category=shipping-delivery&utm_source=admin-panel&utm_medium=shipping-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Tax.Providers.DownloadMorePlugins">
    <Value><![CDATA[You can download more plugins in our <a href="https://www.nopcommerce.com/extensions?category=taxes&utm_source=admin-panel&utm_medium=tax-plugins&utm_campaign=admin-panel" target="_blank">marketplace</a>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Microdata">
    <Value>Microdata tags</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Microdata.Hint">
    <Value>Check to generate Microdata tags on the product details page.</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Promotions.Discounts.Fields.AdminComment">
    <Value>Admin comment</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Fields.AdminComment.Hint">
    <Value>This comment is for internal use only, not visible for customers.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Localization.UploadLocalePattern">
    <Value>Set CLDR for current culture</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.Localization.Description">
    <Value><![CDATA[Sets the <a href="http://cldr.unicode.org/" target="_blank">CLDR</a> pattern for localization of client-side validation according to the current culture]]></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.LocalePattern.SuccessUpload">
    <Value>Localization patterns for the current culture loaded successfully.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForAdminArea.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.EnableXSRFProtectionForPublicStore.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Products.Qty.AriaLabel">
    <Value>Enter a quantity</Value>
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
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[StorePickupPoint]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[StorePickupPoint]') AND NAME = 'Latitude')
BEGIN
	ALTER TABLE [StorePickupPoint]
	ADD Latitude decimal(18, 8) NULL
END
GO

--new column
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[StorePickupPoint]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
and NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = object_id('[StorePickupPoint]') AND NAME = 'Longitude')
BEGIN
	ALTER TABLE [StorePickupPoint]
	ADD Longitude decimal(18, 8) NULL
END
GO

-- update the "DeleteGuests" stored procedure
ALTER PROCEDURE [DeleteGuests]
(
	@OnlyWithoutShoppingCart bit = 1,
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (CustomerId int)
		
	INSERT #tmp_guests (CustomerId)
	SELECT c.[Id] 
	FROM [Customer] c with (NOLOCK)
		LEFT JOIN [ShoppingCartItem] sci with (NOLOCK) ON sci.[CustomerId] = c.[Id]
		INNER JOIN (
			--guests only
			SELECT ccrm.[Customer_Id] 
			FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK)
				INNER JOIN [CustomerRole] cr with (NOLOCK) ON cr.[Id] = ccrm.[CustomerRole_Id]
			WHERE cr.[SystemName] = N'Guests'
		) g ON g.[Customer_Id] = c.[Id]
		LEFT JOIN [Order] o with (NOLOCK) ON o.[CustomerId] = c.[Id]
		LEFT JOIN [BlogComment] bc with (NOLOCK) ON bc.[CustomerId] = c.[Id]
		LEFT JOIN [NewsComment] nc with (NOLOCK) ON nc.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReview] pr with (NOLOCK) ON pr.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReviewHelpfulness] prh with (NOLOCK) ON prh.[CustomerId] = c.[Id]
		LEFT JOIN [PollVotingRecord] pvr with (NOLOCK) ON pvr.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Topic] ft with (NOLOCK) ON ft.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Post] fp with (NOLOCK) ON fp.[CustomerId] = c.[Id]
	WHERE 1 = 1
		--no orders
		AND (o.Id is null)
		--no blog comments
		AND (bc.Id is null)
		--no news comments
		AND (nc.Id is null)
		--no product reviews
		AND (pr.Id is null)
		--no product reviews helpfulness
		AND (prh.Id is null)
		--no poll voting
		AND (pvr.Id is null)
		--no forum topics
		AND (ft.Id is null)
		--no forum topics
		AND (fp.Id is null)
		--no system accounts
		AND (c.IsSystemAccount = 0)
		--created from
		AND ((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
		--created to
		AND ((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
		--shopping cart items
		AND ((@OnlyWithoutShoppingCart = 0) OR (sci.Id is null))
	
	--delete guests
	DELETE [Customer]
	WHERE [Id] IN (SELECT [CustomerId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [CustomerId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'Customer')
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'captchasettings.recaptchaapiurl')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'captchasettings.recaptchaapiurl', N'https://www.google.com/recaptcha/', 0)
END
GO


--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'seosettings.microdataenabled')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'seosettings.microdataenabled', 'true', 0)
END
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'securitysettings.enablexsrfprotectionforadminarea')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'securitysettings.enablexsrfprotectionforadminarea'
END
GO

--delete setting
IF EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'securitysettings.enablexsrfprotectionforpublicstore')
BEGIN
    DELETE FROM [Setting]
    WHERE [Name] = N'securitysettings.enablexsrfprotectionforpublicstore'
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Order]') and NAME='RedeemedRewardPointsEntryId')
BEGIN
	ALTER TABLE [Order] ADD	RedeemedRewardPointsEntryId int NULL
END
GO

--new column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[Discount]') and NAME='AdminComment')
BEGIN
	ALTER TABLE [Discount] ADD 	AdminComment nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[RewardPointsHistory]') and NAME='OrderId')
BEGIN
	ALTER TABLE [RewardPointsHistory] ADD OrderId int NULL
END
GO

--fluent migrator
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = object_id(N'[MigrationVersionInfo]') AND objectproperty(object_id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [MigrationVersionInfo](
	[AppliedOn] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Version] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddAffiliateAddressFK', 637097594562551771)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogCommentBlogPostFK', 637097605404497785)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogCommentCustomerFK', 637097605404497786)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogCommentStoreFK', 637097605404497787)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBlogPostLanguageFK', 637097607595956342)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:42.0000000' AS DateTime2), N'AddBackInStockSubscriptionProductFK', 637097608748261630)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddBackInStockSubscriptionCustomerFK', 637097608748261631)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddPredefinedProductAttributeValueProductAttributeFK', 637097611590754490)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductAttributeMappingProductFK', 637097615386806324)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductAttributeMappingProductAttributeFK', 637097615386806325)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductAttributeValueProductAttributeMappingFK', 637097616507544540)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductCategoryCategoryFK', 637097618625689396)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductCategoryProductFK', 637097618625689397)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductManufacturerManufacturerFK', 637097620539067594)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductManufacturerProductFK', 637097620539067595)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductPictureProductPictureFK', 637097627662625749)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddProductPictureProductProductFK', 637097627662625750)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddAddProductProductTagProductFK', 637097631880193450)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:43.0000000' AS DateTime2), N'AddAddProductProductTagProductTagFK', 637097631880193451)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewHelpfulnessProductReviewFK', 637097639558603530)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewProductFK', 637097639998948304)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewCustomerFK', 637097639998948305)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewStoreFK', 637097639998948306)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewReviewTypeMappingProductReviewFK', 637097643602513441)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductReviewReviewTypeMappingReviewTypeFK', 637097643602513442)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductSpecificationAttributeSpecificationAttributeOptionFK', 637097645462261985)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductSpecificationAttributeProductFK', 637097645462261986)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductWarehouseInventoryProductFK', 637097650980051780)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddProductWarehouseInventoryWarehouseFK', 637097650980051781)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddSpecificationAttributeOptionSpecificationAttributeFK', 637097653366619708)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddStockQuantityHistoryProductFK', 637097656165419186)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddStockQuantityHistoryWarehouseFK', 637097656165419187)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddTierPriceProductFK', 637097657438051844)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddTierPriceCustomerRoleFK', 637097657438051845)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddAddressAttributeValueAddressAttributeValueFk', 637097693526459118)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddAddressCountryFK', 637097696240659480)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddAddressStateProvinceFK', 637097696240659481)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:44.0000000' AS DateTime2), N'AddCustomerAddressCustomerFK', 637097698595245358)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerAddressAddressFK', 637097698595245359)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerAttributeValueCustomerAttributeFK', 637097701504308129)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerCustomerRoleCustomerFK', 637097703237489896)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerCustomerRoleCustomerRoleFK', 637097703237489897)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerBillingAddressFK', 637097705651641381)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerShippingAddressFK', 637097705651641382)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddCustomerPasswordCustomerFK', 637097707461276491)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddExternalAuthenticationRecordCustomerFK', 637097708449096139)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddRewardPointsHistoryCustomerFK', 637097709252342366)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddRewardPointsHistoryOrderFK', 637097709252342367)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddStateProvinceCountryFK', 637097713433797964)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountCategoryDiscountFK', 637097771695936887)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountCategoryCategoryFK', 637097771695936888)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountManufacturerDiscountFK', 637097774149883528)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountManufacturerManufacturerFK', 637097774149883529)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountProductDiscountFK', 637097778951975256)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountProductProductFK', 637097778951975257)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountUsageHistoryDiscountFK', 637097780041180783)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddDiscountUsageHistoryOrderFK', 637097780041180784)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumForumGroupFK', 637097783627313370)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumPostForumTopicFK', 637097784463004325)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumPostCustomerFK', 637097784463004326)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumPostVoteForumPostFK', 637097787633262801)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumSubscriptionCustomerFK', 637097788387699848)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumTopicForumFK', 637097789101910240)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:45.0000000' AS DateTime2), N'AddForumTopicCustomerFK', 637097789101910241)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddPrivateMessageFromCustomerFK', 637097790373669695)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddPrivateMessageToCustomerFK', 637097790373669696)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddLocaleStringResourceLanguageFK', 637097792951964555)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddLocalizedPropertyLanguageFK', 637097793590515436)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddActivityLogActivityLogTypeFK', 637097794508380329)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddActivityLogCustomerFK', 637097794508380330)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddLogCustomerFK', 637097795893561926)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddPictureBinaryPictureFK', 637097796695631609)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddQueuedEmailEmailAccountFK', 637097797031655781)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsCommentNewsItemFK', 637097798362530772)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsCommentCustomerFK', 637097798362530773)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsCommentStoreFK', 637097798362530774)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddNewsItemLanguageFK', 637097800094361423)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddCheckoutAttributeValueCheckoutAttributeFK', 637097801078553212)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddGiftCardOrderItemFK', 637097802922130581)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddGiftCardUsageHistoryGiftCardFK', 637097803156452475)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddGiftCardUsageHistoryOrderFK', 637097803156452476)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddOrderItemOrderFK', 637097804609436788)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:46.0000000' AS DateTime2), N'AddOrderItemProductFK', 637097804609436789)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderCustomerFK', 637097805896028942)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderBillingAddressFK', 637097805896028943)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderShippingAddressFK', 637097805896028944)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderPickupAddressFK', 637097805896028945)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderNoteOrderFK', 637097808997123308)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddRecurringPaymentHistoryRecurringPaymentFK', 637097810210887644)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddRecurringPaymentOrderFK', 637097811410960207)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddReturnRequestCustomerFK', 637097812291248082)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShoppingCartItemCustomerFK', 637097813093371767)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShoppingCartItemProductFK', 637097813093371768)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollAnswerPollFK', 637097815487520229)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollLanguageFK', 637097816025962851)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollVotingRecordPollAnswerFK', 637097817036693383)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPollVotingRecordCustomerFK', 637097817036693384)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddAclRecordCustomerRoleFK', 637097818436073081)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPermissionRecordCustomerRoleCustomerRoleFK', 637097819107801301)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddPermissionRecordCustomerRolePermissionRecordFK', 637097819107801302)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShipmentItemShipmentFK', 637097820921984734)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShipmentOrderFK', 637097821681126845)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShippingMethodCountryCountryFK', 637097822410528356)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddShippingMethodCountryShippingMethodFK', 637097822410528357)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddStoreMappingStoreFK', 637097823639005655)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddVendorAttributeValueVendorAttributeFK', 637097824346411077)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddVendorNoteVendorFK', 637097824991868645)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddDiscountRequirementDiscountFK', 637118390520043560)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddDiscountRequirementDiscountRequirementFK', 637118390520043561)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddOrderRewardPointsHistoryFK', 637121109617140897)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddProductAttributeCombinationProductFk', 637121110600830411)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddLocaleStringResourceIX', 637123449689037677)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddProductPriceDatesEtcIX', 637123449689037678)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddCountryDisplayOrderIX', 637123449689037679)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddLogCreatedOnUtcIX', 637123449689037680)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddCustomerEmailIX', 637123449689037681)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:47.0000000' AS DateTime2), N'AddCustomerUsernameIX', 637123449689037682)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerCustomerGuidIX', 637123449689037683)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerSystemNameIX', 637123449689037684)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerCreatedOnUtcIX', 637123449689037685)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddGenericAttributeEntityIdKeyGroupIX', 637123449689037686)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddQueuedEmailCreatedOnUtcIX', 637123449689037687)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddOrderCreatedOnUtcIX', 637123449689037688)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddLanguageDisplayOrderIX', 637123449689037689)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddNewsletterSubscriptionEmailStoreIdIX', 637123449689037690)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddShoppingCartItemShoppingCartTypeIdCustomerIdIX', 637123449689037691)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddRelatedProductProductId1IX', 637123449689037692)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductAttributeValueProductAttributeMappingIdDisplayOrderIX', 637123449689037693)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductProductAttributeMappingProductIdDisplayOrderIX', 637123449689037694)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddManufacturerDisplayOrderIX', 637123449689037695)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryDisplayOrderIX', 637123449689037696)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryParentCategoryIdIX', 637123449689037697)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsGroupDisplayOrderIX', 637123449689037698)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsForumDisplayOrderIX', 637123449689037699)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsSubscriptionForumIdIX', 637123449689037700)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddForumsSubscriptionTopicIdIX', 637123449689037701)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductDeletedPublishedIX', 637123449689037702)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductPublishedIX', 637123449689037703)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductShowOnHomepageIX', 637123449689037704)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductParentGroupedProductIdIX', 637123449689037705)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductVisibleIndividuallyIX', 637123449689037706)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPCMProductCategoryIX', 637123449689037707)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCurrencyDisplayOrderIX', 637123449689037708)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductTagNameIX', 637123521091647925)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddActivityLogCreatedOnUtcIX', 637123521091647926)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddUrlRecordSlugIX', 637123521091647927)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddUrlRecordCustom1IX', 637123521091647928)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddAclRecordEntityIdEntityNameIX', 637123521091647929)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddStoreMappingEntityIdEntityNameIX', 637123521091647930)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryLimitedToStoresIX', 637123521091647931)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddManufacturerLimitedToStoresIX', 637123521091647932)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductLimitedToStoresIX', 637123521091647933)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategorSubjectToAclIX', 637123521091647934)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddManufacturerSubjectToAclIX', 637123521091647935)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductSubjectToAclIX', 637123521091647936)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductCategoryMappingIsFeaturedProductIX', 637123521091647937)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductManufacturerMappingIsFeaturedProductIX', 637123521091647938)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCustomerCustomerRoleMappingCustomerIdIX', 637123521091647939)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductDeleteIdIX', 637123521091647940)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddGetLowStockProductsIX', 637123521091647941)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPMMProductManufacturerIX', 637123521091647942)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPCMProductIdExtendedIX', 637123537559280389)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPMMProductIdExtendedIX', 637123537559280390)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPSAMAllowFilteringIX', 637123537559280391)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddPSAMSpecificationAttributeOptionIdAllowFilteringIX', 637123537559280392)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddQueuedEmailSentOnUtcDontSendBeforeDateUtcExtendedIX', 637123537559280393)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddProductVisibleIndividuallyPublishedDeletedExtendedIX', 637123537559280394)
INSERT [MigrationVersionInfo] ([AppliedOn], [Description], [Version]) VALUES (CAST(N'2019-12-30T09:12:48.0000000' AS DateTime2), N'AddCategoryDeletedExtendedIX', 637123537559280395)
END
GO

-- update the "ProductLoadAllPaged" stored procedure
ALTER PROCEDURE [ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
	@ProductTagId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchManufacturerPartNumber bit = 0, -- a value indicating whether to search by a specified "keyword" in manufacturer part number
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@OverridePublished	bit = null, --null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@FilterableSpecificationAttributeOptionIds nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	/* Products that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ProductId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@OriginalKeywords nvarchar(4000),
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	SET @OriginalKeywords = @Keywords
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, LEN(@Keywords) - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, LEN(@Keywords)) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--product name
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Name]) > 0 '

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			IF @UseFullTextSearch = 1
			BEGIN
				SET @sql = @sql + 'OR CONTAINS(p.[ShortDescription], @Keywords) '
				SET @sql = @sql + 'OR CONTAINS(p.[FullDescription], @Keywords) '
			END
			ELSE
			BEGIN
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[ShortDescription]) > 0 '
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[FullDescription]) > 0 '
			END
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + 'OR p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + 'OR p.[Sku] = @OriginalKeywords '
		END

		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND ( (lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

		IF @SearchDescriptions = 1
		BEGIN
			--localized product short description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

			--localized product full description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '
		END

		SET @sql = @sql + ' ) '

		IF @SearchProductTags = 1
		BEGIN
			--product tags (exact match)
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE pt.[Name] = @OriginalKeywords '

			--localized product tags
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name''
				AND lp.[LocaleValue] = @OriginalKeywords '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by customer role IDs (access control list)
	SET @AllowedCustomerRoleIds = isnull(@AllowedCustomerRoleIds, '')	
	CREATE TABLE #FilteredCustomerRoleIds
	(
		CustomerRoleId int not null
	)
	INSERT INTO #FilteredCustomerRoleIds (CustomerRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AllowedCustomerRoleIds, ',')
	DECLARE @FilteredCustomerRoleIdsCount int	
	SET @FilteredCustomerRoleIdsCount = (SELECT COUNT(1) FROM #FilteredCustomerRoleIds)
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ProductId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN ('
		
		SET @sql = @sql + + CAST(@CategoryIds AS nvarchar(max))

		SET @sql = @sql + ')'

		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pcm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by manufacturer
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		AND pmm.ManufacturerId = ' + CAST(@ManufacturerId AS nvarchar(max))
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pmm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
	END
	
	--filter by warehouse
	IF @WarehouseId > 0
	BEGIN
		--we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		--but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET @sql = @sql + '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory [pwi]
					WHERE [pwi].WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ' AND [pwi].ProductId = p.Id))
			)'
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--filter by product tag
	IF ISNULL(@ProductTagId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTagId AS nvarchar(max))
	END
	
	--"Published" property
	IF (@OverridePublished is null)
	BEGIN
		--process according to "showHidden"
		IF @ShowHidden = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Published = 1'
		END
	END
	ELSE IF (@OverridePublished = 1)
	BEGIN
		--published only
		SET @sql = @sql + '
		AND p.Published = 1'
	END
	ELSE IF (@OverridePublished = 0)
	BEGIN
		--unpublished only
		SET @sql = @sql + '
		AND p.Published = 0'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(p.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')'
	END
	
	--show hidden and ACL
	IF  @ShowHidden = 0 and @FilteredCustomerRoleIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FilteredCustomerRoleIds [fcr]
			WHERE
				[fcr].CustomerRoleId IN (
					SELECT [acl].CustomerRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
    --prepare filterable specification attribute option identifier (if requested)
    IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
        DECLARE @sql_filterableSpecs nvarchar(max)
        SET @sql_filterableSpecs = '
	        INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
	        SELECT DISTINCT [psam].SpecificationAttributeOptionId
	        FROM [Product_SpecificationAttribute_Mapping] [psam] WITH (NOLOCK)
	            WHERE [psam].[AllowFiltering] = 1
	            AND [psam].[ProductId] IN (' + @sql + ')'

        EXEC sp_executesql @sql_filterableSpecs

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',') 

    CREATE TABLE #FilteredSpecsWithAttributes
	(
        SpecificationAttributeId int not null,
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecsWithAttributes (SpecificationAttributeId, SpecificationAttributeOptionId)
	SELECT sao.SpecificationAttributeId, fs.SpecificationAttributeOptionId
    FROM #FilteredSpecs fs INNER JOIN SpecificationAttributeOption sao ON sao.Id = fs.SpecificationAttributeOptionId
    ORDER BY sao.SpecificationAttributeId 

    DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecsWithAttributes)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
        DECLARE @SpecificationAttributeId int
        DECLARE @LastSpecificationAttributeId int
        SET @LastSpecificationAttributeId = 0
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT SpecificationAttributeId, SpecificationAttributeOptionId
		FROM #FilteredSpecsWithAttributes

		OPEN cur_SpecificationAttributeOption
        FOREACH:
            FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeId, @SpecificationAttributeOptionId
            IF (@LastSpecificationAttributeId <> 0 AND @SpecificationAttributeId <> @LastSpecificationAttributeId OR @@FETCH_STATUS <> 0) 
			    SET @sql = @sql + '
        AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId IN (SELECT SpecificationAttributeOptionId FROM #FilteredSpecsWithAttributes WHERE SpecificationAttributeId = ' + CAST(@LastSpecificationAttributeId AS nvarchar(max)) + '))'
            SET @LastSpecificationAttributeId = @SpecificationAttributeId
		IF @@FETCH_STATUS = 0 GOTO FOREACH
		CLOSE cur_SpecificationAttributeOption
		DEALLOCATE cur_SpecificationAttributeOption
	END

	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
		--manufacturer position (display order)
		IF @ManufacturerId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' pmm.DisplayOrder ASC'
		END
		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Name] ASC'
	END
	
	SET @sql = @sql + '
	ORDER BY' + @sql_orderby
	
    SET @sql = '
    INSERT INTO #DisplayOrderTmp ([ProductId])' + @sql

	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
    DROP TABLE #FilteredSpecsWithAttributes
	DROP TABLE #FilteredCustomerRoleIds
	DROP TABLE #KeywordProducts

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

-- update the "DeleteGuests" stored procedure
ALTER PROCEDURE [DeleteGuests]
(
	@OnlyWithoutShoppingCart bit = 1,
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (CustomerId int)
	CREATE TABLE #tmp_adresses (AddressId int)
		
	INSERT #tmp_guests (CustomerId)
	SELECT c.[Id] 
	FROM [Customer] c with (NOLOCK)
		LEFT JOIN [ShoppingCartItem] sci with (NOLOCK) ON sci.[CustomerId] = c.[Id]
		INNER JOIN (
			--guests only
			SELECT ccrm.[Customer_Id] 
			FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK)
				INNER JOIN [CustomerRole] cr with (NOLOCK) ON cr.[Id] = ccrm.[CustomerRole_Id]
			WHERE cr.[SystemName] = N'Guests'
		) g ON g.[Customer_Id] = c.[Id]
		LEFT JOIN [Order] o with (NOLOCK) ON o.[CustomerId] = c.[Id]
		LEFT JOIN [BlogComment] bc with (NOLOCK) ON bc.[CustomerId] = c.[Id]
		LEFT JOIN [NewsComment] nc with (NOLOCK) ON nc.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReview] pr with (NOLOCK) ON pr.[CustomerId] = c.[Id]
		LEFT JOIN [ProductReviewHelpfulness] prh with (NOLOCK) ON prh.[CustomerId] = c.[Id]
		LEFT JOIN [PollVotingRecord] pvr with (NOLOCK) ON pvr.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Topic] ft with (NOLOCK) ON ft.[CustomerId] = c.[Id]
		LEFT JOIN [Forums_Post] fp with (NOLOCK) ON fp.[CustomerId] = c.[Id]
	WHERE 1 = 1
		--no orders
		AND (o.Id is null)
		--no blog comments
		AND (bc.Id is null)
		--no news comments
		AND (nc.Id is null)
		--no product reviews
		AND (pr.Id is null)
		--no product reviews helpfulness
		AND (prh.Id is null)
		--no poll voting
		AND (pvr.Id is null)
		--no forum topics
		AND (ft.Id is null)
		--no forum topics
		AND (fp.Id is null)
		--no system accounts
		AND (c.IsSystemAccount = 0)
		--created from
		AND ((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
		--created to
		AND ((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
		--shopping cart items
		AND ((@OnlyWithoutShoppingCart = 0) OR (sci.Id is null))

	INSERT #tmp_adresses (AddressId)
	SELECT [Address_Id] FROM [CustomerAddresses] WHERE [Customer_Id] IN (SELECT [CustomerId] FROM #tmp_guests)

	--delete guests
	DELETE [Customer]
	WHERE [Id] IN (SELECT [CustomerId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [CustomerId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'Customer')

	--delete addresses
	DELETE [Address]
	WHERE [Id] IN (SELECT [AddressId] FROM #tmp_adresses)
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
	DROP TABLE #tmp_adresses
END
GO

--new columns
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=object_id('[GenericAttribute]') and NAME='CreatedOrUpdatedDateUTC')
BEGIN
	ALTER TABLE [GenericAttribute] ADD
	CreatedOrUpdatedDateUTC datetime NULL
END
GO

 