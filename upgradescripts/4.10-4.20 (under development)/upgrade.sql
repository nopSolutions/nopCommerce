--upgrade scripts from nopCommerce 4.10 to 4.20

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language>
  <LocaleResource Name="Admin.ContentManagement.Topics.Fields.Title.Required">
    <Value>Title is required</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Settings.Order.DisableBillingAddressCheckoutStep.Hint">
    <Value>Check to disable "Billing address" step during checkout. Billing address will be pre-filled and saved using the default registration data (this option cannot be used with guest checkout enabled). Also ensure that appropriate address fields that cannot be pre-filled are not required (or disabled). If a customer doesn''t have a billing address, then the billing address step will be displayed.</Value>
  </LocaleResource>
  <LocaleResource Name="Common.RelativeDateTime.Past">
    <Value>{0} ago</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore">
    <Value>Ignore additional shipping charge for pick up in store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickUpInStore.Hint">
    <Value>Check if you want ignore additional shipping charge for pick up in store.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseResponseCompression">
    <Value>Use response compression</Value>
  </LocaleResource>  
   <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.UseResponseCompression.Hint">
    <Value>Enable to compress response (gzip by default). You can disable it if you have an active IIS Dynamic Compression Module configured at the server level.</Value>
  </LocaleResource>   
  <LocaleResource Name="Admin.System.Warnings.URL.Reserved">
    <Value>The entered text will be replaced by ''{0}'', since it is already used as a SEO-friendly name for another page or contains invalid characters</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.PayPalStandard.Instructions">
    <Value>
		<![CDATA[
		<p>
			<b>If you''re using this gateway ensure that your primary store currency is supported by PayPal.</b>
			<br />
			<br />To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal. Follow these steps to configure your account for PDT:<br />
			<br />1. Log in to your PayPal account (click <a href="https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8" target="_blank">here</a> to create your account).
			<br />2. Click the Profile button.
			<br />3. Click the Profile and Settings button.
			<br />4. Select the My selling tools item on left panel.
			<br />5. Click Website Preferences Update in the Selling online section.
			<br />6. Under Auto Return for Website Payments, click the On radio button.
			<br />7. For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment ({0}).
			<br />8. Under Payment Data Transfer, click the On radio button and get your PDT identity token.
			<br />9. Click Save.
			<br />
		</p>
		]]>
	</Value>
  </LocaleResource>  
  <LocaleResource Name="Plugins.ExternalAuth.Facebook.Instructions">
    <Value><![CDATA[<p>To configure authentication with Facebook, please follow these steps:<br/><br/><ol><li>Navigate to the <a href="https://developers.facebook.com/apps" target ="_blank" > Facebook for Developers</a> page and sign in. If you don''t already have a Facebook account, use the <b>Sign up for Facebook</b> link on the login page to create one.</li><li>Tap the <b>+ Add a New App button</b> in the upper right corner to create a new App ID. (If this is your first app with Facebook, the text of the button will be <b>Create a New App</b>.)</li><li>Fill out the form and tap the <b>Create App ID button</b>.</li><li>The <b>Product Setup</b> page is displayed, letting you select the features for your new app. Click <b>Get Started</b> on <b>Facebook Login</b>.</li><li>Click the <b>Settings</b> link in the menu at the left, you are presented with the <b>Client OAuth Settings</b> page with some defaults already set.</li><li>Enter "{0:s}signin-facebook" into the <b>Valid OAuth Redirect URIs</b> field.</li><li>Click <b>Save Changes</b>.</li><li>Click the <b>Dashboard</b> link in the left navigation.</li><li>Copy your App ID and App secret below.</li></ol><br/><br/></p>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Filtering.SpecificationFilter.Separator">
    <Value>or</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert">
    <Value>Information</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Common.Ok">
    <Value>Ok</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.States.Failed">
    <Value>Failed to retrieve states.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.FailedRetrieving">
    <Value>Failed to retrieve specification options.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Alert.FailedGetDiscountRequirements">
    <Value>Failed to load requirements info. Please refresh the page.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.RewardPoints.Alert.HistoryAdd">
    <Value>Failed to add reward points.</Value>
  </LocaleResource>
    <LocaleResource Name="Admin.Promotions.Discounts.Requirements.Alert.FailedToSave">
    <Value>Failed to save requirement</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.GiftCards.Fields.GiftCardCouponCode.Alert.FailedGenerate">
    <Value>Failed to generate code.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Reports.Customers.CustomerStatistics.Alert.FailedLoad">
    <Value>Failed to load statistics.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.SalesReport.OrderStatistics.Alert.FailedLoad">
    <Value>Failed to load statistics.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Dimensions.MarkAsPrimaryDimension.Alert.FailedToUpdate">
    <Value>Failed to update dimension.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Shipping.Measures.Weights.MarkAsPrimaryWeight.Alert.FailedToUpdate">
    <Value>Failed to update weight.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Orders.OrderNotes.Alert.Add">
    <Value>Failed to add order note.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Alert.AddNew">
    <Value>Upload picture first.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Pictures.Alert.PictureAdd">
    <Value>Failed to add product picture.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedGenerate">
    <Value>Error while generating attribute combinations.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Download.SaveDownloadURL.Alert.FailedSave">
    <Value>Failed to save download object.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Vendors.VendorNotes.AddTitle.Alert.FailedAddNote">
    <Value>Failed to add vendor note.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.FailedAdd">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Alert.Error">
    <Value>Failed to update currency.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.SelectOption">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.NoAttributeOptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.FailedToSave">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.SelectOption">
    <Value></Value>
  </LocaleResource> 
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.NoAttributeOptions">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Promotions.Discounts.Requirements.FailedToSave">
    <Value>Failed to save discount requirements.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.Save.Error">
    <Value>Error while saving.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.Save.Ok">
    <Value>Successfully saved.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Common.Alert.Add.Error">
    <Value>Failed to add record.</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Activated">
    <Value>Coupon code ({0}) is activated! The discount will be applied to your order.</Value>
  </LocaleResource>  
  <LocaleResource Name="ShoppingCart.DiscountCouponCode.Invalid">
    <Value>This coupon code ({0}) is invalid or no longer available.</Value>
  </LocaleResource>
   <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireDigit">
    <Value>Password must have at least one digit</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireDigit.Hint">
    <Value>Specify that passwords must have at least one digit.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireLowercase">
    <Value>Password must have at least one lowercase</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireLowercase.Hint">
    <Value>Specify that password must have at least one lowercase.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireNonAlphanumeric">
    <Value>Password must have at least one non alphanumeric character</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireNonAlphanumeric.Hint">
    <Value>Specify that password must have at least one non alphanumeric character.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireUppercase">
    <Value>Password must have at least one uppercase</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.CustomerUser.PasswordRequireUppercase.Hint">
    <Value>Specify that passwords must have at least one uppercase.</Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.IsNotEmpty">
    <Value>Password is required.</Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireDigit">
    <Value><![CDATA[<li>must have at least one digit</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireLowercase">
    <Value><![CDATA[<li>must have at least one lowercase</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireNonAlphanumeric">
    <Value><![CDATA[<li>must have at least one special character (e.g. #?!@$%^&*-)</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.RequireUppercase">
    <Value><![CDATA[<li>must have at least one uppercase</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.LengthValidation">
    <Value><![CDATA[<li>must have at least {0} characters</li>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Validation.Password.Rule">
    <Value><![CDATA[<p>Password must meet the following rules: </p><ul>{0}{1}{2}{3}{4}</ul>]]></Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Fields.NewPassword.LengthValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.ChangePassword.Fields.NewPassword.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Password.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.Fields.Password.LengthValidation">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Account.PasswordRecovery.NewPassword.Required">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Account.PasswordRecovery.NewPassword.LengthValidation">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="FormattedAttributes.PriceAdjustment">
    <Value> [{0}{1}{2}]</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Customers.Customers.Impersonate.Inactive">
    <Value>This customer is inactive</Value>
  </LocaleResource>
  <LocaleResource Name="ShoppingCart.Discount.CannotBeUsed">
    <Value>You cannot use this discount coupon because the validation conditions are not met</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductUseLimitedToStores">
    <Value>Export/Import products with "limited to stores"</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.Catalog.ExportImportProductUseLimitedToStores.Hint">
    <Value>Check if products should be exported/imported with "limited to stores" property.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Import.StoresDontExist">
    <Value>Stores with the following names and/or IDs don''t exist: {0}</Value>
  </LocaleResource>
  <LocaleResource Name="Plugins.Payments.Square.Fields.Location.Select">
    <Value>Select location</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.GroupTierPricesForDistinctShoppingCartItems">
    <Value>Group tier prices for distinct shopping cart items</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ShoppingCart.GroupTierPricesForDistinctShoppingCartItems.Hint">
    <Value>Allows to offer special prices when customers buy bigger amounts of a particular product. For example, when a customer could have two shopping cart items for the same products (different product attributes).</Value>
  </LocaleResource>
  <LocaleResource Name="Checkout.Addresses.Invalid">
    <Value>You have {0} invalid address(es)</Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Catalog.Products.Fields.LowStockActivity.Hint">
    <Value>Action to be taken when your current stock quantity falls below (reaches) the ''Minimum stock quantity''. Activation of the action will occur only after an order is placed.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Categories.Mappings">
    <Value>Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Manufacturers.Mappings">
    <Value>Mappings</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Plugins">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Configuration.Tax">
    <Value></Value>
  </LocaleResource>  
  <LocaleResource Name="Admin.Vendors.Display">
    <Value>Display</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Import.DatabaseNotContainCategory">
    <Value>Database doesn''t contain the ''{0}'' category</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.BlogPostId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.Blog.Comments.List.BlogPostId.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.NewsItemId">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ContentManagement.News.Comments.List.NewsItemId.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.AssociatedProducts.TryToAddSelfGroupedProduct">
    <Value>You cannot add current group product to related ones. This group product was ignored while adding.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForgotPasswordPage">
    <Value>Show on forgot password page</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnForgotPasswordPage.Hint">
    <Value>Check to show CAPTCHA on forgot password page when restore password.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.OneColumnProductPage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.OneColumnProductPage">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.CommonInfo">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Mappings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Security">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.RequireOtherProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CreatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.CreatedOn.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UpdatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.UpdatedOn.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.CreatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.UpdatedOn">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ID">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Fields.ID.Hint">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.Id">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Mappings">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Prices">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.Price">
    <Value>Price</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.LinkedProducts">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.AdvancedProductTypes">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Settings.ProductEditor.BlockTitle.Security">
    <Value></Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.EndDate">
    <Value>End date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.EndDate.Hint">
    <Value>The end date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.StartDate">
    <Value>Start date</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.StartDate.Hint">
    <Value>The start date for the search.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Product">
    <Value>Product</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Product.Hint">
    <Value>Search by a specific product.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.BillingCountry">
    <Value>Billing country</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.BillingCountry.Hint">
    <Value>Filter by billing country.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Store">
    <Value>Store</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.ShoppingCartType.Store.Hint">
    <Value>Search by a specific store.</Value>
  </LocaleResource>
</Language>'

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

UPDATE [Topic] 
SET [IncludeInFooterColumn1] = 0
WHERE [SystemName] = 'VendorTermsOfService'
GO

UPDATE [Topic]
SET [Title] = ISNULL([SystemName], '')
WHERE [Title] IS NULL OR [Title] = ''
GO

ALTER TABLE [Topic] ALTER COLUMN [Title] nvarchar(max) NOT NULL
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
	@AllowedCustomerRoleIds	nvarchar(MAX) = null,	--a list of customer role IDs (comma-separated list) for which a product should be shown (if a subjet to ACL)
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


		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
	

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[ShortDescription], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[ShortDescription]) > 0 '


			--product full description
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE '
			IF @UseFullTextSearch = 1
				SET @sql = @sql + 'CONTAINS(p.[FullDescription], @Keywords) '
			ELSE
				SET @sql = @sql + 'PATINDEX(@Keywords, p.[FullDescription]) > 0 '



			--localized product short description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
				

			--localized product full description
			SET @sql = @sql + '
			UNION
			SELECT lp.EntityId
			FROM LocalizedProperty lp with (NOLOCK)
			WHERE
				lp.LocaleKeyGroup = N''Product''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0 '
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + '
			UNION
			SELECT p.Id
			FROM Product p with (NOLOCK)
			WHERE p.[Sku] = @OriginalKeywords '
		END

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

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'shippingsettings.ignoreadditionalshippingchargeforpickupinstore')
BEGIN
	INSERT [Setting] ([Name], [Value], [StoreId])
	VALUES (N'shippingsettings.ignoreadditionalshippingchargeforpickupinstore', N'true', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [name] = N'adminareasettings.usericheditorforcustomeremails')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'adminareasettings.usericheditorforcustomeremails', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'messagessettings.usepopupnotifications')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'messagessettings.usepopupnotifications', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequirelowercase')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequirelowercase', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequireuppercase')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequireuppercase', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequirenonalphanumeric')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequirenonalphanumeric', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'customersettings.passwordrequiredigit')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'customersettings.passwordrequiredigit', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'catalogsettings.exportimportproductuselimitedtostores')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.exportimportproductuselimitedtostores', N'False', 0)
END
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'catalogsettings.useajaxloadmenu')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'catalogsettings.useajaxloadmenu', N'False', 0)
END
GO

--updating of indexes in the Picture table for reduced table size after upgrade nopCommerce from 4.00 to 4.10 version
ALTER INDEX ALL ON [Picture] REBUILD
GO

--new activity log type
IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [Name] = N'Upload a favicon and app icons archive')
BEGIN
	INSERT [ActivityLogType] ( [SystemKeyword], [Name], [Enabled]) VALUES ( N'UploadIconsArchive', N'Upload a favicon and app icons archive', 1)
END
GO

--new ground shipping description
UPDATE [ShippingMethod] 
SET [Description] = 'Shipping by land transport'
WHERE [Name] = 'Ground'
GO


--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.onecolumnproductpage'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.createdon'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.updatedon'
GO

--delete setting
DELETE FROM [Setting]
WHERE [Name] = N'producteditorsettings.id'
GO

--new setting
IF NOT EXISTS (SELECT 1 FROM [Setting] WHERE [Name] = N'mediasettings.useabsoluteimagepath')
BEGIN
    INSERT [Setting] ([Name], [Value], [StoreId])
    VALUES (N'mediasettings.useabsoluteimagepath', N'True', 0)
END
GO

