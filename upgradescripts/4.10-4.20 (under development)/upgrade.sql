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
    <Value>Failed to add specification attribute.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Configuration.Currencies.Alert.Error">
    <Value>Failed to update currency.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.SelectOption">
    <Value>Select specification attribute option.</Value>
  </LocaleResource>
  <LocaleResource Name="Admin.Catalog.Products.SpecificationAttributes.Alert.NoAttributeOptions">
    <Value>First, please create at least one specification attribute option</Value>
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
