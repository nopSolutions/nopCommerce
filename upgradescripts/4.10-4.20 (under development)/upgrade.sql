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
