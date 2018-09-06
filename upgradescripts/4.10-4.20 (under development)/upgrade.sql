--upgrade scripts from nopCommerce 4.10 to 4.20

--new locale resources
declare @resources xml
--a resource will be deleted if its value is empty
set @resources='
<Language> 
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

