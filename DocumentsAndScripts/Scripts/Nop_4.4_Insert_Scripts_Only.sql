
--DBCC CHECKIDENT ('SpecificationAttributeOption', NORESEED);
--DBCC CHECKIDENT ('[SpecificationAttributeOption]', RESEED, 16);

 -------  Start: Insert Categories & Slugs -----------------------------

SET IDENTITY_INSERT [dbo].[Category] ON

INSERT [dbo].[Category] ([Id], [Name], [MetaKeywords], [MetaTitle], [PageSizeOptions], [Description], [CategoryTemplateId], [MetaDescription], [ParentCategoryId], [PictureId], [PageSize], [AllowCustomersToSelectPageSize], [ShowOnHomepage], [IncludeInTopMenu], [SubjectToAcl], [LimitedToStores], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc], [PriceRangeFiltering], [PriceFrom], [PriceTo], [ManuallyPriceRange])
VALUES (1, N'Give Support', NULL, NULL, N'6, 3, 9', NULL, 1, NULL, 0, 0, 15, 0, 0, 1, 0, 0, 1, 0, 0, GETUTCDATE(), GETUTCDATE(), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(10000.0000 AS Decimal(18, 4)), 1)
INSERT [dbo].[Category] ([Id], [Name], [MetaKeywords], [MetaTitle], [PageSizeOptions], [Description], [CategoryTemplateId], [MetaDescription], [ParentCategoryId], [PictureId], [PageSize], [AllowCustomersToSelectPageSize], [ShowOnHomepage], [IncludeInTopMenu], [SubjectToAcl], [LimitedToStores], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc], [PriceRangeFiltering], [PriceFrom], [PriceTo], [ManuallyPriceRange]) 
VALUES (2, N'Take Support', NULL, NULL, N'6, 3, 9', NULL, 1, NULL, 0, 0, 15, 0, 0, 1, 0, 0, 1, 0, 0, GETUTCDATE(), GETUTCDATE(), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(10000.0000 AS Decimal(18, 4)), 1)
INSERT [dbo].[Category] ([Id], [Name], [MetaKeywords], [MetaTitle], [PageSizeOptions], [Description], [CategoryTemplateId], [MetaDescription], [ParentCategoryId], [PictureId], [PageSize], [AllowCustomersToSelectPageSize], [ShowOnHomepage], [IncludeInTopMenu], [SubjectToAcl], [LimitedToStores], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc], [PriceRangeFiltering], [PriceFrom], [PriceTo], [ManuallyPriceRange])
VALUES (3, N'Pricing', NULL, NULL, N'6, 3, 9', NULL, 2, NULL, 0, 0, 6, 0, 0, 1, 0, 0, 1, 0, 1000, GETUTCDATE(), GETUTCDATE(), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(10000.0000 AS Decimal(18, 4)), 1)

SET IDENTITY_INSERT [dbo].[Category] OFF

-- [UrlRecord] is mandate for SEO url to create categoires otherwise categories are not clickable
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[IsActive],[LanguageId]) VALUES('Category','give-support',1,1,0)
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[IsActive],[LanguageId]) VALUES('Category','take-support',2,1,0)
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[IsActive],[LanguageId]) VALUES('Category','pricing',3,1,0)

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
	[DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [IsTelecommunicationsOrBroadcastingOrElectronicServices], [ManageInventoryMethodId],
	[ProductAvailabilityRangeId], [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity],
	[MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], 
	[OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [NotReturnable], [DisableBuyButton],
	[DisableWishlistButton], [AvailableForPreOrder], [PreOrderAvailabilityStartDateTimeUtc], [CallForPrice], [Price], [OldPrice],
	[ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [BasepriceEnabled],
	[BasepriceAmount], [BasepriceUnitId], [BasepriceBaseAmount], [BasepriceBaseUnitId], [MarkAsNew], [MarkAsNewStartDateTimeUtc], 
	[MarkAsNewEndDateTimeUtc], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc],
	[AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc])
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
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate()
)

INSERT [dbo].[Product] ([Id], [Name], [MetaKeywords], [MetaTitle], [Sku], [ManufacturerPartNumber], [Gtin], [RequiredProductIds],
	[AllowedQuantities], [ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [ShortDescription],
	[FullDescription], [AdminComment], [ProductTemplateId], [VendorId], [ShowOnHomepage], [MetaDescription], [AllowCustomerReviews],
	[ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores],
	[IsGiftCard], [GiftCardTypeId], [OverriddenGiftCardAmount], [RequireOtherProducts], [AutomaticallyAddRequiredProducts], [IsDownload],
	[DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload],
	[SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles],
	[IsRental], [RentalPriceLength], [RentalPricePeriodId], [IsShipEnabled], [IsFreeShipping], [ShipSeparately], [AdditionalShippingCharge],
	[DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [IsTelecommunicationsOrBroadcastingOrElectronicServices], [ManageInventoryMethodId],
	[ProductAvailabilityRangeId], [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity],
	[MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], 
	[OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [NotReturnable], [DisableBuyButton],
	[DisableWishlistButton], [AvailableForPreOrder], [PreOrderAvailabilityStartDateTimeUtc], [CallForPrice], [Price], [OldPrice],
	[ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [BasepriceEnabled],
	[BasepriceAmount], [BasepriceUnitId], [BasepriceBaseAmount], [BasepriceBaseUnitId], [MarkAsNew], [MarkAsNewStartDateTimeUtc], 
	[MarkAsNewEndDateTimeUtc], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc],
	[AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc])
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
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(150.0000 AS Decimal(18, 4)),
	CAST(2500.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate()
)

INSERT [dbo].[Product] ([Id], [Name], [MetaKeywords], [MetaTitle], [Sku], [ManufacturerPartNumber], [Gtin], [RequiredProductIds],
	[AllowedQuantities], [ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [ShortDescription],
	[FullDescription], [AdminComment], [ProductTemplateId], [VendorId], [ShowOnHomepage], [MetaDescription], [AllowCustomerReviews],
	[ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores],
	[IsGiftCard], [GiftCardTypeId], [OverriddenGiftCardAmount], [RequireOtherProducts], [AutomaticallyAddRequiredProducts], [IsDownload],
	[DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload],
	[SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles],
	[IsRental], [RentalPriceLength], [RentalPricePeriodId], [IsShipEnabled], [IsFreeShipping], [ShipSeparately], [AdditionalShippingCharge],
	[DeliveryDateId], [IsTaxExempt], [TaxCategoryId], [IsTelecommunicationsOrBroadcastingOrElectronicServices], [ManageInventoryMethodId],
	[ProductAvailabilityRangeId], [UseMultipleWarehouses], [WarehouseId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity],
	[MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], 
	[OrderMinimumQuantity], [OrderMaximumQuantity], [AllowAddingOnlyExistingAttributeCombinations], [NotReturnable], [DisableBuyButton],
	[DisableWishlistButton], [AvailableForPreOrder], [PreOrderAvailabilityStartDateTimeUtc], [CallForPrice], [Price], [OldPrice],
	[ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [BasepriceEnabled],
	[BasepriceAmount], [BasepriceUnitId], [BasepriceBaseAmount], [BasepriceBaseUnitId], [MarkAsNew], [MarkAsNewStartDateTimeUtc], 
	[MarkAsNewEndDateTimeUtc], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc],
	[AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc])
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
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(200.0000 AS Decimal(18, 4)),
	CAST(4000.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate()
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
-------------------   [CustomerAttribute]  -----------------------------

-- SELECT * FROM [CustomerAttribute]

SET IDENTITY_INSERT [dbo].[CustomerAttribute] ON 

INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (1,'ProfileType', 0, 2,0);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (2,'Current Avalibility', 0, 1,4);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (3,'Relavent Experiance', 0, 1,3);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (4,'Mother Tongue', 0, 1,5);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (5,'Short Description', 0, 10,6);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (6,'Full Description', 0, 10,7);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (7,'Primary Technology', 0, 51,1);
INSERT INTO [CustomerAttribute] ([Id], [Name], [IsRequired], [AttributeControlTypeId],[DisplayOrder]) VALUES (8,'Secondary Technology', 0, 51,2);

SET IDENTITY_INSERT [dbo].[CustomerAttribute] OFF

-- Make all feilds mandatory except secondary technology which is not used presently
UPDATE [CustomerAttribute] SET IsRequired=1 WHERE Id Not In (8)

------------------------  [SpecificationAttribute]  -----------------------------------------

-- Delete  FROM  SpecificationAttribute 
 -- SELECT * FROM SpecificationAttribute

SET IDENTITY_INSERT [dbo].[SpecificationAttribute] ON 

INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (1, N'ProfileType', 0)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (2, N'Current Avalibility', 4)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (3, N'Relavent Experiance', 3)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (4, N'Mother Tongue', 5)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (5, N'Short Description', 6)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (6, N'Full Description', 7)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (7, N'Primary Technology', 1)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (8, N'Secondary Technology', 2)
INSERT [dbo].[SpecificationAttribute] ([Id], [Name], [DisplayOrder]) VALUES (9, N'Gender', 8)

SET IDENTITY_INSERT [dbo].[SpecificationAttribute] OFF

--------------------   [SpecificationAttributeOption] ------------------------------------------------------------------------

-- SELECT * FROM [SpecificationAttribute] 
-- SELECT * FROM [SpecificationAttributeOption]

-- Delete  * from  [SpecificationAttributeOption]


SET IDENTITY_INSERT [SpecificationAttributeOption] ON

-- Profile Type
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (1,1,'Give Support',0);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (2,1,'Take Support',1);

-- Current Avalibility
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (3,2,'Available',2);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (4,2,'UnAvailable',3);

-- Relavent Experiance
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (5,3,'0 ',4);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (6,3,'1+',5);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (7,3,'2+ ',6);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (8,3,'3+',7);

INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (9,3,'4+',8);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (10,3,'5+',9);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (11,3,'6+',10);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (12,3,'7+',11);

INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (13,3,'8+',12);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (14,3,'9+',13);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (15,3,'10+',14);

-- Mother Tounge
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(16,'English', 4, 8);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(17,'Bengali', 4, 9);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(18,'Gujarati', 4, 10);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(19,'Hindi', 4, 11);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(20,'Kannada', 4, 12);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(21,'Malayalam', 4,13);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(22,'Marathi', 4, 14);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(23,'Oriya', 4, 15);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(24,'Punjabi', 4, 16);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(25,'Tamil', 4, 17);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(26,'Telugu', 4, 18);

-- Gender
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(27,'Male', 9, 19);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(28,'Fe Male', 9, 20);

SET IDENTITY_INSERT [SpecificationAttributeOption] OFF

-- Primary Technology
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('C#', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('ASP.NET MVC', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('JAVA', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('TeraData', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Azure', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Science', 7, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Angular', 7, 0);

-- Secondary Technology
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('C#', 6, 0);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('MVC', 6, 0);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('JAVA', 6, 0);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS', 6, 0);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('TeraData', 6, 0);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Azure', 6, 0);

--------------------   ------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM [LocaleStringResource] WHERE [ResourceName]='Orders.UpgradeSubscription.Message')
   BEGIN
		INSERT INTO [dbo].[LocaleStringResource]([ResourceName],[ResourceValue],[LanguageId]) 
		VALUES('Orders.UpgradeSubscription.Message','Please upgrade to Subscription to View Mobile Number ,Send the messages.',1)
   END

-- SELECT * FROM [LocaleStringResource] WHERE [ResourceName]='Orders.UpgradeSubscription.Message'
-- UPDATE [LocaleStringResource] SET [ResourceValue]='Please upgrade to Subscription to View Mobile Number ,Send the messages.' WHERE [ResourceName]='Orders.UpgradeSubscription.Message'

-------------------- Start: [ActivityLogType] ------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM [ActivityLogType] WHERE SystemKeyword='PublicStore.ViewContactDetail')
   BEGIN
		INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled]) 
		VALUES('PublicStore.ViewContactDetail','PublicStore.ViewContactDetail',1)
   END

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.EditCustomerAvailabilityToTrue')
BEGIN
    INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
    VALUES ('PublicStore.EditCustomerAvailabilityToTrue','PublicStore.EditCustomerAvailabilityToTrue',1)
END
GO

IF NOT EXISTS (SELECT 1 FROM [ActivityLogType] WHERE [SystemKeyword] = N'PublicStore.CustomerSubscriptionInfo')
BEGIN
    INSERT INTO [dbo].[ActivityLogType]([SystemKeyword],[Name],[Enabled])
    VALUES ('PublicStore.CustomerSubscriptionInfo','PublicStore.CustomerSubscriptionInfo',1)
END
GO

-------------------- Start: [EmailAccount] ------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM [EmailAccount] WHERE [Email]='no-reply@Onjobsupport.in')
   BEGIN
	 INSERT INTO [EmailAccount]([DisplayName],[Email],[Host],[Username],[Password],[Port])
     VALUES ('On Job Suport','no-reply@Onjobsupport.in','smtp-relay.sendinblue.com','umsateesh@gmail.com','TkUZDCRvhxnF8Era','587')
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


--UPDATE [Setting] SET [Value]=1  WHERE [Name]='shoppingCartSettings.threemonthsubscriptionproductid'
--UPDATE [Setting] SET [Value]=2  WHERE [Name]='shoppingCartSettings.sixmonthsubscriptionproductid'
--UPDATE [Setting] SET [Value]=3  WHERE [Name]='shoppingCartSettings.oneyearsubscriptionproductid'

------------------------- Customer settings -----------------

-- SELECT * FROM [Setting] WHERE [Name] like 'customersettings.gender%'

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

--UPDATE [Setting] SET [Value]=8  WHERE [Name]='customersettings.genderspecificationattributeid'
--UPDATE [Setting] SET [Value]=28  WHERE [Name]='customersettings.gendermalespecificationattributeoptionid'
--UPDATE [Setting] SET [Value]=29  WHERE [Name]='customersettings.genderfemalespecificationattributeoptionid'

---------------- Customer Roles  GiveSupport & TakeSupport -----------------
-- These roles are used for showing different topic content to different customers
-- Guest cusotmers see one topic , Give support customers see another topic and same with take support role 
SET IDENTITY_INSERT [dbo].[CustomerRole] ON

INSERT [dbo].[CustomerRole] ([Id], [Name], [SystemName], [FreeShipping], [TaxExempt], [Active], [IsSystemRole], [EnablePasswordLifetime], [OverrideTaxDisplayType], [DefaultTaxDisplayTypeId], [PurchasedWithProductId])
VALUES (6, N'Give Support', N'GiveSupport', 1, 1, 1, 0, 0, 0, 0, 0)
INSERT [dbo].[CustomerRole] ([Id], [Name], [SystemName], [FreeShipping], [TaxExempt], [Active], [IsSystemRole], [EnablePasswordLifetime], [OverrideTaxDisplayType], [DefaultTaxDisplayTypeId], [PurchasedWithProductId]) 
VALUES (7, N'Take Support', N'TakeSupport', 1, 1, 1, 0, 0, 1, 0, 0)

SET IDENTITY_INSERT [dbo].[CustomerRole] OFF

---------------------- Topic Texts --------------------

INSERT INTO [dbo].[Topic]
           ([SystemName],[IncludeInSitemap],[IncludeInTopMenu],[IncludeInFooterColumn1]
		   ,[IncludeInFooterColumn2],[IncludeInFooterColumn3],[DisplayOrder],[AccessibleWhenStoreClosed]
           ,[IsPasswordProtected],[Title],[Body],[Published],[TopicTemplateId],[SubjectToAcl],[LimitedToStores])
     VALUES('LoginPageContent',0,0,0,0,0,1,0,0,'',
			'<div class="information-boxes-wrapper"> <div class="information-boxes-block"> <div class="information-box" href="abv.bg"> <div class="image-wrapper"> <div class="image-holder"> <img alt="" src="https://venture2.nop-templates.com/images/thumbs/0000442.png"> </div></div><div class="information-wrapper"> <div class="title">Why Choose Us</div></div><div class="information-wrapper"> <div class="description"> Lorem ipsum dolor sit amet, et mel quis habeo patrioque,eu eripuit menandri. Lorem ipsum dolor sit amet, et mel quis habeo patrioque,eu eripuit menandri. </div></div></div><div class="information-box"> <div class="image-wrapper"> <div class="image-holder"> <img alt="" src="https://venture2.nop-templates.com/images/thumbs/0000443.png"> </div></div><div class="information-wrapper"> <div class="title">100% Satisfaction Guaranteed</div></div><div class="information-wrapper"> <div class="description">Lorem ipsum dolor sit amet, et mel quis habeo patrioque.</div></div></div><div class="information-box"> <div class="image-wrapper"> <div class="image-holder"> <img alt="" src="https://venture2.nop-templates.com/images/thumbs/0000444.png"> </div></div><div class="information-wrapper"> <div class="title">100% Satisfaction Guaranteed</div></div><div class="information-wrapper"> <div class="description">Lorem ipsum dolor sit amet, et mel quis habeo patrioque.</div></div></div></div></div>'
			,1,1,0,0)

---------------------- Topic Texts --------------------

IF NOT EXISTS (SELECT * FROM [CustomerRole] WHERE [Name]='GiveSupport-Paid')
   BEGIN
    INSERT INTO [dbo].[CustomerRole]([Name],[SystemName],[FreeShipping],[TaxExempt],[Active],[IsSystemRole],[EnablePasswordLifetime],[OverrideTaxDisplayType],[DefaultTaxDisplayTypeId],[PurchasedWithProductId])
    VALUES ('GiveSupport-Paid','GiveSupport-Paid',1,0,1,0,0,0,0,0)
  END

IF NOT EXISTS (SELECT * FROM [CustomerRole] WHERE [Name]='PaidCustomer')
   BEGIN
    INSERT INTO [dbo].[CustomerRole]([Name],[SystemName],[FreeShipping],[TaxExempt],[Active],[IsSystemRole],[EnablePasswordLifetime],[OverrideTaxDisplayType],[DefaultTaxDisplayTypeId],[PurchasedWithProductId])
    VALUES ('PaidCustomer','PaidCustomer',1,0,1,0,0,0,0,0)
 END

 ----------------------  --------------------

 -- primary technology insert queries

--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AI', 7, 0);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Android', 7, 1);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Angular', 7, 2);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Appian Developer', 7, 3);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS', 7, 4);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Big Data', 7, 5);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Blockchain', 7, 6);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Cyber Ark', 7, 7);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Cyber Security', 7, 8);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Science', 7, 9);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Dell Boomi', 7, 10);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DevOps', 7, 11);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DOT NET', 7, 12);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('GIS', 7, 13);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Guidewire', 7, 14);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('MS sql server', 7, 15);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Office 365 Admin', 7, 16);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('OKTA', 7, 17);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('ORACLE', 7, 18);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Pega', 7, 19);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Performance Testing', 7, 20);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Power BI', 7, 21);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Python', 7, 22);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('React js', 7, 23);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Salesforce', 7, 24);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SAP Hybris', 7, 25);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SAP Modules', 7, 26);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SAS', 7, 27);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SCCM', 7, 28);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SCRUM Master', 7, 29);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Splunk', 7, 30);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Talend', 7, 31);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('TOSCA', 7, 32);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('UI/UX Developer', 7, 33);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Workday', 7, 34);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AutoML', 7, 35);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('BigQuery', 7, 36);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DataFusion ', 7, 37);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DataProc', 7, 38);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Microstrategy', 7, 39);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('PostgreSql', 7, 40);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Powershell Scripting', 7, 41);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Servicenow', 7, 42);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SnowFlake', 7, 43);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Vmware Adminstartion', 7, 44);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Abinitio', 7, 45);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Agile', 7, 46);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AirFlow', 7, 47);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Asp.Net Core', 7, 48);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Athena', 7, 49);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS Cloud Engineer', 7, 50);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS,', 7, 51);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS-Athena', 7, 52);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS-EC2', 7, 53);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS-Glue', 7, 54);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS-IAM', 7, 55);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS-RDS', 7, 56);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS-S3', 7, 57);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Azure', 7, 58);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Azure Data Factory', 7, 59);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Azure Devops', 7, 60);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Bigdata', 7, 61);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Bit Bucket', 7, 62);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Blue prism ', 7, 63);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Bootstrap', 7, 64);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('C#', 7, 65);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('C++', 7, 66);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Check Point', 7, 67);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Citrix', 7, 68);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Cloud Data Store', 7, 69);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('CloudERA', 7, 70);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Cognos Bi', 7, 71);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Csharp', 7, 72);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Css', 7, 73);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Cybersecurity', 7, 74);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Analysis', 7, 75);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Analyst', 7, 76);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Analytics', 7, 77);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Engineering', 7, 78);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Governance', 7, 79);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Data Store', 7, 80);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Datascience', 7, 81);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DataStage', 7, 82);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Devops', 7, 83);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DevOps + AWS', 7, 84);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Django', 7, 85);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Docker', 7, 86);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Dot Net Core', 7, 87);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('DOT NET', 7, 88);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Drupal', 7, 89);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Flask', 7, 90);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('GCP', 7, 91);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('GIT', 7, 92);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Golang', 7, 93);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Hadoop', 7, 94);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('HTML', 7, 95);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Informatica', 7, 96);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Informatica Power Center', 7, 97);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('IOS', 7, 98);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('IOT', 7, 99);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Java Full Stack', 7, 100);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Jenkins', 7, 101);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Jira admin', 7, 102);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Juniper', 7, 103);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Kafka', 7, 104);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Kubernetes - Admin', 7, 105);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Kubernetes - Developer', 7, 106);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Linux', 7, 107);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Maven', 7, 108);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('MicroStratogy', 7, 109);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('MSBI', 7, 110);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Mulesoft', 7, 111);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Networking', 7, 112);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('NumPy', 7, 113);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('OBI EE', 7, 114);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle', 7, 115);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle Cloud', 7, 116);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle DBA', 7, 117);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle Dba', 7, 118);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle Ebs', 7, 119);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle Pl/Sql', 7, 120);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Oracle', 7, 121);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Palo Alto', 7, 122);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Pandas', 7, 123);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Pega Prpc', 7, 124);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Pega', 7, 125);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('PeopleSoft', 7, 126);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('PHP', 7, 127);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Power Automate', 7, 128);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Power BI', 7, 129);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Powerpoint', 7, 130);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Powershell', 7, 131);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Pyspark', 7, 132);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Python', 7, 133);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Python Django', 7, 134);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('QlikView', 7, 135);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('React Native', 7, 136);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('RPA- UiPath', 7, 137);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('RPA-Blueprism', 7, 138);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Sailpoint', 7, 139);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Salesforce Developer', 7, 140);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Salesforce  Admin', 7, 141);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SAP', 7, 142);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SAS', 7, 143);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Selenium', 7, 144);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('ServiceNow Admin', 7, 145);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('ServiceNow Developer', 7, 146);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Share Point', 7, 147);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Snowflake', 7, 148);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Spark', 7, 149);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Splunk', 7, 150);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Spring Boot', 7, 151);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('SQL', 7, 152);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Sql dba', 7, 153);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('System Administrator', 7, 154);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Tableau', 7, 155);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Teradata', 7, 156);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Terraform', 7, 157);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Tibco', 7, 158);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('TOGAF', 7, 159);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('UI/UX', 7, 160);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('WebLogic', 7, 161);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Workday', 7, 162);
--INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Workday HCM', 7, 163);
