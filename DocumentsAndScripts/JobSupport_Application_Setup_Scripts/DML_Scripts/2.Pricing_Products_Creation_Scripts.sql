
-- use [onjobsupport47]


-------------------- Start: Pricing Products Insert Scripts ------------------------------------------------------------------------

GO
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([Id], [Name], [MetaKeywords], [MetaTitle], [Sku], [ManufacturerPartNumber], [Gtin], [RequiredProductIds],
	[AllowedQuantities], [ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [ShortDescription],
	[FullDescription], [AdminComment], [ProductTemplateId], [VendorId], [ShowOnHomepage], [MetaDescription], [AllowCustomerReviews],
	[ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores],
	[IsGiftCard], [GiftCardTypeId], [OverriddenGiftCardAmount], [RequireOtherProducts], [AutomaticallyAddRequiredProducts], [IsDownload],
	[DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload],
	[SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles],
	[IsRental], [RentalPriceLength], [RentalPricePeriodId], [IsShipEnabled], [IsFreeShipping], [ShipSeparately], [AdditionalShippingCharge],
	[DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [ManageInventoryMethodId],
	[ProductAvailabilityRangeId], [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity],
	[MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], 
	[OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [NotReturnable], [DisableBuyButton],
	[DisableWishlistButton], [AvailableForPreOrder], [PreOrderAvailabilityStartDateTimeUtc], [CallForPrice], [Price], [OldPrice],
	[ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [BasepriceEnabled],
	[BasepriceAmount], [BasepriceUnitId], [BasepriceBaseAmount], [BasepriceBaseUnitId], [MarkAsNew], [MarkAsNewStartDateTimeUtc], 
	[MarkAsNewEndDateTimeUtc], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc],
	[AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc],[DisplayAttributeCombinationImagesOnly])
VALUES (
	1, N'Free Subscription', NULL, NULL, N'Free subscription', NULL, NULL, NULL, NULL, 5, 0, 1,
	N'LifeTime validity
	Other premium member can contact you 
	Send unlimited messages/interests
	',
	N'<p>25 contacts<br />
	3 months validity<br />
	1-month profile highlighter free<br />
	Unlimited messages/interests</p>', 
	NULL, 1, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 10, NULL, 0, 0, 0, 0, NULL, 0, 100, 0, 10, 0, 1, 0, 0, 0, 0,
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate(),0
)

INSERT [dbo].[Product] ([Id], [Name], [MetaKeywords], [MetaTitle], [Sku], [ManufacturerPartNumber], [Gtin], [RequiredProductIds],
	[AllowedQuantities], [ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [ShortDescription],
	[FullDescription], [AdminComment], [ProductTemplateId], [VendorId], [ShowOnHomepage], [MetaDescription], [AllowCustomerReviews],
	[ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores],
	[IsGiftCard], [GiftCardTypeId], [OverriddenGiftCardAmount], [RequireOtherProducts], [AutomaticallyAddRequiredProducts], [IsDownload],
	[DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload],
	[SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles],
	[IsRental], [RentalPriceLength], [RentalPricePeriodId], [IsShipEnabled], [IsFreeShipping], [ShipSeparately], [AdditionalShippingCharge],
	[DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [ManageInventoryMethodId],
	[ProductAvailabilityRangeId], [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity],
	[MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], 
	[OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [NotReturnable], [DisableBuyButton],
	[DisableWishlistButton], [AvailableForPreOrder], [PreOrderAvailabilityStartDateTimeUtc], [CallForPrice], [Price], [OldPrice],
	[ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [BasepriceEnabled],
	[BasepriceAmount], [BasepriceUnitId], [BasepriceBaseAmount], [BasepriceBaseUnitId], [MarkAsNew], [MarkAsNewStartDateTimeUtc], 
	[MarkAsNewEndDateTimeUtc], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc],
	[AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc],[DisplayAttributeCombinationImagesOnly])
VALUES (
	2, N'1 Month Subscription', NULL, NULL, N'1 Month Subscription', NULL, NULL, NULL, NULL, 5, 0, 1,
	N'1 month validity
	View 20 verified  mobile numbers
	1-month profile highlighter
	Send unlimited messages/interests
	Chat online* (coming soon)', 
	N'<p>20 contacts<br />
	1 month validity<br />
	1- month profile highlighter free<br />
	Unlimited messages/interests</p>', 
	NULL, 1, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 10, NULL, 0, 0, 0, 0, NULL, 0, 100, 0, 10, 0, 1, 0, 0, 0, 0,
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(150.0000 AS Decimal(18, 4)),
	CAST(2500.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate(),0
)

INSERT [dbo].[Product] ([Id], [Name], [MetaKeywords], [MetaTitle], [Sku], [ManufacturerPartNumber], [Gtin], [RequiredProductIds],
	[AllowedQuantities], [ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [ShortDescription],
	[FullDescription], [AdminComment], [ProductTemplateId], [VendorId], [ShowOnHomepage], [MetaDescription], [AllowCustomerReviews],
	[ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores],
	[IsGiftCard], [GiftCardTypeId], [OverriddenGiftCardAmount], [RequireOtherProducts], [AutomaticallyAddRequiredProducts], [IsDownload],
	[DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload],
	[SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles],
	[IsRental], [RentalPriceLength], [RentalPricePeriodId], [IsShipEnabled], [IsFreeShipping], [ShipSeparately], [AdditionalShippingCharge],
	[DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [ManageInventoryMethodId],
	[ProductAvailabilityRangeId], [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity],
	[MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], 
	[OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [NotReturnable], [DisableBuyButton],
	[DisableWishlistButton], [AvailableForPreOrder], [PreOrderAvailabilityStartDateTimeUtc], [CallForPrice], [Price], [OldPrice],
	[ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [BasepriceEnabled],
	[BasepriceAmount], [BasepriceUnitId], [BasepriceBaseAmount], [BasepriceBaseUnitId], [MarkAsNew], [MarkAsNewStartDateTimeUtc], 
	[MarkAsNewEndDateTimeUtc], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc],
	[AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc],[DisplayAttributeCombinationImagesOnly])
VALUES (
	3, N'3 months subscription', NULL, NULL, N'3 months subscription', NULL, NULL, NULL, NULL, 5, 0, 1,
	N'3 months validity
	View 50 verified mobile numbers
	3-month profile highlighter
	Send unlimited messages/interests
	Chat online* (coming soon)', 
	N'<p>50 contacts<br />
	3 months validity<br />
	3-month profile highlighter free<br />
	Unlimited messages/interests</p>', 
	NULL, 1, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 10, NULL, 0, 0, 0, 0, NULL, 0, 100, 0, 10, 0, 1, 0, 0, 0, 0,
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(200.0000 AS Decimal(18, 4)),
	CAST(4000.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate(),0
)

SET IDENTITY_INSERT [dbo].[Product] OFF

-- [UrlRecord] is mandate for SEO url to create products otherwise products are not clickable
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[LanguageId],[IsActive]) VALUES('Product','free-subscription',1,0,1)
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[LanguageId],[IsActive]) VALUES('Product','1-month-subscription',2,0,1)
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[LanguageId],[IsActive]) VALUES('Product','3-month-subscription',3,0,1)

-------------------- Start:[Product_Category_Mapping] table Insert Scripts ------------------------------------------------------------------------

INSERT INTO [dbo].[Product_Category_Mapping] ([CategoryId],[ProductId],[IsFeaturedProduct],[DisplayOrder]) VALUES(3,1,1,1)
INSERT INTO [dbo].[Product_Category_Mapping] ([CategoryId],[ProductId],[IsFeaturedProduct],[DisplayOrder]) VALUES(3,2,1,2)
INSERT INTO [dbo].[Product_Category_Mapping] ([CategoryId],[ProductId],[IsFeaturedProduct],[DisplayOrder]) VALUES(3,3,1,3)

 -------  Start: Pricing Products Category Template  ---------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM [CategoryTemplate] WHERE [Name]='Pricing Category Template')
   BEGIN
		INSERT INTO [dbo].[CategoryTemplate]([Name],[ViewPath],[DisplayOrder]) 
        VALUES ('Pricing Category Template','CategoryTemplate.Pricing',3)
   END


IF NOT EXISTS (SELECT * FROM [ProductTemplate] WHERE [Name]='Product Price')
   BEGIN
		INSERT INTO [dbo].[ProductTemplate]([Name],[ViewPath],[DisplayOrder]) 
        VALUES('Product Price','_ProductBox.Price',2)
   END


-------------------- End: Pricing Products Insert Scripts ------------------------------------------------------------------------