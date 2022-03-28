-- 1
-- Inserts pictures into DB
-- Uses AltAttribute as the old ID, meant to allow for tracking based on source DB
INSERT INTO NOPCommerce_Stage_440.dbo.Picture (MimeType, SeoFilename, AltAttribute, TitleAttribute, IsNew)
SELECT MimeType, SeoFilename, Id, TitleAttribute, IsNew
FROM DB_4215_mickey2.dbo.Picture

INSERT INTO NOPCommerce_Stage_440.dbo.PictureBinary (PictureId, BinaryData)
SELECT tp.Id, sp.PictureBinary
FROM DB_4215_mickey2.dbo.Picture sp
JOIN NOPCommerce_Stage_440.dbo.Picture tp on sp.Id = tp.AltAttribute

------------------------------------------------------------------------------------------
-- 2
-- Inserts UrlRecords
INSERT INTO NOPCommerce_Stage_440.dbo.UrlRecord (EntityId, EntityName, Slug, IsActive, LanguageId)
SELECT EntityId, EntityName, Slug, IsActive, LanguageId
FROM DB_4215_mickey2.dbo.UrlRecord

------------------------------------------------------------------------------------------
-- 3
-- Merges categories, need to
-- update parentCategoryId and pictures after
MERGE NOPCommerce_Stage_440.dbo.Category as Target
USING DB_4215_mickey2.dbo.Category as Source
ON Source.Name = Target.Name

WHEN NOT MATCHED BY Target THEN
	INSERT (
		Name,
		Description,
		CategoryTemplateId,
		ParentCategoryId,
		PictureId,
		PageSize,
		AllowCustomersToSelectPageSize,
		PageSizeOptions,
		ShowOnHomePage,
		IncludeInTopMenu,
		SubjectToAcl,
		LimitedToStores,
		Published,
		Deleted,
		DisplayOrder,
		CreatedOnUtc,
		UpdatedOnUtc,
		-- 4.4.0 fields
		PriceRangeFiltering,
		PriceFrom,
		PriceTo,
		ManuallyPriceRange)
	VALUES (
		Source.Name,
		Source.Description,
		99, -- marked to indicate this is an imported value
		Source.ParentCategoryId,
		Source.PictureId,
		Source.PageSize,
		Source.AllowCustomersToSelectPageSize,
		Source.PageSizeOptions,
		Source.ShowOnHomePage,
		Source.IncludeInTopMenu,
		Source.SubjectToAcl,
		Source.LimitedToStores,
		Source.Published,
		Source.Deleted,
		Source.DisplayOrder,
		Source.CreatedOnUtc,
		Source.UpdatedOnUtc,
		1,
		0,
		10000,
		0);

-- update ParentCategoryId
UPDATE ac
SET ParentCategoryId = ac.Id
FROM NOPCommerce_Stage_440.dbo.Category ac
INNER JOIN DB_4215_mickey2.dbo.Category mc ON mc.Name = ac.Name
WHERE ac.CategoryTemplateId = 99

-- update pictureId
UPDATE ac
SET PictureId = p.Id
FROM NOPCommerce_Stage_440.dbo.Category ac
INNER JOIN NOPCommerce_Stage_440.dbo.Picture p ON p.AltAttribute = ac.PictureId
WHERE ac.CategoryTemplateId = 99

-- create store mappings
INSERT INTO NOPCommerce_Stage_440.dbo.StoreMapping
SELECT Id, 'Category', (SELECT Id from NOPCommerce_Stage_440.dbo.Store where Name = 'Mickey Shorr')
FROM NOPCommerce_Stage_440.dbo.category
WHERE CategoryTemplateId = 99

-- update URL records
UPDATE ur
SET EntityId = ac.Id
FROM NOPCommerce_Stage_440.dbo.UrlRecord ur
INNER JOIN DB_4215_mickey2.dbo.Category mc ON mc.Id = ur.EntityId
INNER JOIN NOPCommerce_Stage_440.dbo.Category ac ON mc.Name = ac.Name
WHERE ac.CategoryTemplateId = 99

-- wrap-up
UPDATE NOPCommerce_Stage_440.dbo.Category
SET CategoryTemplateId = 1
WHERE CategoryTemplateId = 99

------------------------------------------------------------------------------------------
-- 4
-- Merges manufacturers

MERGE NOPCommerce_Stage_440.dbo.Manufacturer as Target
USING DB_4215_mickey2.dbo.Manufacturer as Source
ON Source.Name = Target.Name

WHEN NOT MATCHED BY Target THEN
	INSERT (
		Name,
		Description,
		ManufacturerTemplateId,
		PictureId,
		PageSize,
		AllowCustomersToSelectPageSize,
		PageSizeOptions,
		SubjectToAcl,
		LimitedToStores,
		Published,
		Deleted,
		DisplayOrder,
		CreatedOnUtc,
		UpdatedOnUtc,
		-- 4.4.0 fields
		PriceRangeFiltering,
		PriceFrom,
		PriceTo,
		ManuallyPriceRange)
	VALUES (
		Source.Name,
		Source.Description,
		99, -- marked to indicate this is an imported value
		Source.PictureId,
		Source.PageSize,
		Source.AllowCustomersToSelectPageSize,
		Source.PageSizeOptions,
		Source.SubjectToAcl,
		Source.LimitedToStores,
		Source.Published,
		Source.Deleted,
		Source.DisplayOrder,
		Source.CreatedOnUtc,
		Source.UpdatedOnUtc,
		1,
		0,
		10000,
		0);

-- update pictureId
UPDATE ac
SET PictureId = p.Id
FROM NOPCommerce_Stage_440.dbo.Manufacturer ac
INNER JOIN NOPCommerce_Stage_440.dbo.Picture p ON p.AltAttribute = ac.PictureId
WHERE ac.ManufacturerTemplateId = 99

-- create store mappings
INSERT INTO NOPCommerce_Stage_440.dbo.StoreMapping
SELECT Id, 'Manufacturer', (SELECT Id from NOPCommerce_Stage_440.dbo.Store where Name = 'Mickey Shorr')
FROM NOPCommerce_Stage_440.dbo.Manufacturer
WHERE ManufacturerTemplateId = 99

-- update URL records
UPDATE ur
SET EntityId = ac.Id
FROM NOPCommerce_Stage_440.dbo.UrlRecord ur
INNER JOIN DB_4215_mickey2.dbo.Manufacturer mc ON mc.Id = ur.EntityId
INNER JOIN NOPCommerce_Stage_440.dbo.Manufacturer ac ON mc.Name = ac.Name
WHERE ac.ManufacturerTemplateId = 99

-- wrap-up
UPDATE NOPCommerce_Stage_440.dbo.Manufacturer
SET ManufacturerTemplateId = 1
WHERE ManufacturerTemplateId = 99

------------------------------------------------------------------------------------------
-- 5
-- Merges products

INSERT INTO NOPCommerce_Stage_440.dbo.Product (
		[ProductTypeId],
		[ParentGroupedProductId],
		[VisibleIndividually],
		[Name],
		[ShortDescription],
		[FullDescription],
		[AdminComment],
		[ProductTemplateId],
		[VendorId],
		[ShowOnHomepage],
		[MetaKeywords],
		[MetaDescription],
		[MetaTitle],
		[AllowCustomerReviews],
		[ApprovedRatingSum],
		[NotApprovedRatingSum],
		[ApprovedTotalReviews],
		[NotApprovedTotalReviews],
		[SubjectToAcl],
		[LimitedToStores],
		[Sku],
		[ManufacturerPartNumber],
		[Gtin],
		[IsGiftCard],
		[GiftCardTypeId],
		[OverriddenGiftCardAmount],
		[RequireOtherProducts],
		[RequiredProductIds],
		[AutomaticallyAddRequiredProducts],
		[IsDownload],
		[DownloadId],
		[UnlimitedDownloads],
		[MaxNumberOfDownloads],
		[DownloadExpirationDays],
		[DownloadActivationTypeId],
		[HasSampleDownload],
		[SampleDownloadId],
		[HasUserAgreement],
		[UserAgreementText],
		[IsRecurring],
		[RecurringCycleLength],
		[RecurringCyclePeriodId],
		[RecurringTotalCycles],
		[IsRental],
		[RentalPriceLength],
		[RentalPricePeriodId],
		[IsShipEnabled],
		[IsFreeShipping],
		[ShipSeparately],
		[AdditionalShippingCharge],
		[DeliveryDateId],
		[IsTaxExempt],
		[TaxCategoryId],
		[IsTelecommunicationsOrBroadcastingOrElectronicServices],
		[ManageInventoryMethodId],
		[UseMultipleWarehouses],
		[WarehouseId],
		[StockQuantity],
		[DisplayStockAvailability],
		[DisplayStockQuantity],
		[MinStockQuantity],
		[LowStockActivityId],
		[NotifyAdminForQuantityBelow],
		[BackorderModeId],
		[AllowBackInStockSubscriptions],
		[OrderMinimumQuantity],
		[OrderMaximumQuantity],
		[AllowedQuantities],
		[AllowAddingOnlyExistingAttributeCombinations],
		[DisableBuyButton],
		[DisableWishlistButton],
		[AvailableForPreOrder],
		[PreOrderAvailabilityStartDateTimeUtc],
		[CallForPrice],
		[Price],
		[OldPrice],
		[ProductCost],
		[CustomerEntersPrice],
		[MinimumCustomerEnteredPrice],
		[MaximumCustomerEnteredPrice],
		[BasepriceEnabled],
		[BasepriceAmount],
		[BasepriceUnitId],
		[BasepriceBaseAmount],
		[BasepriceBaseUnitId],
		[MarkAsNew],
		[MarkAsNewStartDateTimeUtc],
		[MarkAsNewEndDateTimeUtc],
		[HasTierPrices],
		[HasDiscountsApplied],
		[Weight],
		[Length],
		[Width],
		[Height],
		[AvailableStartDateTimeUtc],
		[AvailableEndDateTimeUtc],
		[DisplayOrder],
		[Published],
		[Deleted],
		[CreatedOnUtc],
		[UpdatedOnUtc],
		[NotReturnable],
		[CartPrice],
		[ProductAvailabilityRangeId])
	SELECT 
		mp.[ProductTypeId],
		mp.[ParentGroupedProductId],
		mp.[VisibleIndividually],
		mp.[Name],
		mp.[ShortDescription],
		mp.[FullDescription],
		mp.[AdminComment],
		99, -- use as indicator
		mp.[VendorId],
		mp.[ShowOnHomepage],
		mp.[MetaKeywords],
		mp.[MetaDescription],
		mp.[MetaTitle],
		mp.[AllowCustomerReviews],
		mp.[ApprovedRatingSum],
		mp.[NotApprovedRatingSum],
		mp.[ApprovedTotalReviews],
		mp.[NotApprovedTotalReviews],
		mp.[SubjectToAcl],
		mp.[LimitedToStores],
		mp.[Sku],
		mp.[ManufacturerPartNumber],
		mp.[Gtin],
		mp.[IsGiftCard],
		mp.[GiftCardTypeId],
		mp.[OverriddenGiftCardAmount],
		mp.[RequireOtherProducts],
		mp.[RequiredProductIds],
		mp.[AutomaticallyAddRequiredProducts],
		mp.[IsDownload],
		mp.[DownloadId],
		mp.[UnlimitedDownloads],
		mp.[MaxNumberOfDownloads],
		mp.[DownloadExpirationDays],
		mp.[DownloadActivationTypeId],
		mp.[HasSampleDownload],
		mp.[SampleDownloadId],
		mp.[HasUserAgreement],
		mp.[UserAgreementText],
		mp.[IsRecurring],
		mp.[RecurringCycleLength],
		mp.[RecurringCyclePeriodId],
		mp.[RecurringTotalCycles],
		mp.[IsRental],
		mp.[RentalPriceLength],
		mp.[RentalPricePeriodId],
		mp.[IsShipEnabled],
		mp.[IsFreeShipping],
		mp.[ShipSeparately],
		mp.[AdditionalShippingCharge],
		mp.[DeliveryDateId],
		mp.[IsTaxExempt],
		mp.[TaxCategoryId],
		mp.[IsTelecommunicationsOrBroadcastingOrElectronicServices],
		mp.[ManageInventoryMethodId],
		mp.[UseMultipleWarehouses],
		mp.[WarehouseId],
		mp.[StockQuantity],
		mp.[DisplayStockAvailability],
		mp.[DisplayStockQuantity],
		mp.[MinStockQuantity],
		mp.[LowStockActivityId],
		mp.[NotifyAdminForQuantityBelow],
		mp.[BackorderModeId],
		mp.[AllowBackInStockSubscriptions],
		mp.[OrderMinimumQuantity],
		mp.[OrderMaximumQuantity],
		mp.[AllowedQuantities],
		mp.[AllowAddingOnlyExistingAttributeCombinations],
		mp.[DisableBuyButton],
		mp.[DisableWishlistButton],
		mp.[AvailableForPreOrder],
		mp.[PreOrderAvailabilityStartDateTimeUtc],
		mp.[CallForPrice],
		mp.[Price],
		mp.[OldPrice],
		mp.[ProductCost],
		mp.[CustomerEntersPrice],
		mp.[MinimumCustomerEnteredPrice],
		mp.[MaximumCustomerEnteredPrice],
		mp.[BasepriceEnabled],
		mp.[BasepriceAmount],
		mp.[BasepriceUnitId],
		mp.[BasepriceBaseAmount],
		mp.[BasepriceBaseUnitId],
		mp.[MarkAsNew],
		mp.[MarkAsNewStartDateTimeUtc],
		mp.[MarkAsNewEndDateTimeUtc],
		mp.[HasTierPrices],
		mp.[HasDiscountsApplied],
		mp.[Weight],
		mp.[Length],
		mp.[Width],
		mp.[Height],
		mp.[AvailableStartDateTimeUtc],
		mp.[AvailableEndDateTimeUtc],
		mp.[DisplayOrder],
		mp.[Published],
		mp.[Deleted],
		mp.[CreatedOnUtc],
		mp.[UpdatedOnUtc],
		mp.[NotReturnable],
		null,
		mp.[ProductAvailabilityRangeId]
	FROM DB_4215_mickey2.dbo.Product mp
	WHERE mp.Sku COLLATE SQL_Latin1_General_CP1_CI_AS NOT IN
		(select Sku COLLATE SQL_Latin1_General_CP1_CI_AS from NOPCommerce_Stage_440.dbo.Product)
	AND mp.Published = 1 and mp.Deleted = 0

-- update URL records
UPDATE ur
SET EntityId = ap.Id
FROM NOPCommerce_Stage_440.dbo.UrlRecord ur
INNER JOIN DB_4215_mickey2.dbo.Product mp ON mp.Id = ur.EntityId
INNER JOIN NOPCommerce_Stage_440.dbo.Product ap ON mp.Name = ap.Name
WHERE ap.ProductTemplateId = 99 and EntityName = 'Product'

-- add product-category mappings
INSERT INTO NOPCommerce_Stage_440.dbo.Product_Category_Mapping (ProductId, CategoryId, IsFeaturedProduct, DisplayOrder)
SELECT ap.Id, ac.Id, 0, 0
FROM DB_4215_mickey2.dbo.Product_Category_Mapping mpcm
INNER JOIN DB_4215_mickey2.dbo.Product mp on mpcm.ProductId = mp.Id
INNER JOIN NOPCommerce_Stage_440.dbo.Product ap on ap.Sku = mp.Sku
INNER JOIN DB_4215_mickey2.dbo.Category mc on mpcm.CategoryId = mc.Id
INNER JOIN NOPCommerce_Stage_440.dbo.Category ac on ac.Name = mc.Name

-- create store mappings
INSERT INTO NOPCommerce_Stage_440.dbo.StoreMapping
SELECT Id, 'Product', (SELECT Id from NOPCommerce_Stage_440.dbo.Store where Name = 'Mickey Shorr')
FROM NOPCommerce_Stage_440.dbo.Product
WHERE ProductTemplateId = 99

-- create store mappings for categories
INSERT INTO NOPCommerce_Stage_440.dbo.StoreMapping
SELECT p.Id, 'Product', (SELECT Id from NOPCommerce_Stage_440.dbo.Store where Name = 'Mickey Shorr')
FROM NOPCommerce_Stage_440.dbo.Product p
INNER JOIN NOPCommerce_Stage_440.dbo.Product_Category_Mapping pcm on pcm.ProductId = p.Id
INNER JOIN NOPCommerce_Stage_440.dbo.Category c on pcm.CategoryId = c.Id 
WHERE c.Name IN ('Mobile Audio & Video', 'Car Starters & Alarms', 'Marine Audio', 'Motorcycle Audio', 'Motorsport / ATV Audio')


-- wrap-up
UPDATE NOPCommerce_Stage_440.dbo.Product
SET ProductTemplateId = 1
WHERE ProductTemplateId = 99

------------------------------------------------------------------------------------------
-- 6
-- Adds stores

INSERT INTO NOPCommerce_Stage_440.dbo.SS_SL_Shop (IsVisible, Name, Latitude, Longitude, ShortDescription, FullDescription, ShowOnHomePage,Tags, DisplayOrder, LimitedToStores)
SELECT IsVisible, Name, Latitude, Longitude, ShortDescription, FullDescription, ShowOnHomePage,Tags, DisplayOrder, LimitedToStores FROM DB_4215_mickey2.dbo.SS_SL_Shop