
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
	1, N'3 Months Subscription', NULL, NULL, N'3 months subscription', NULL, NULL, NULL, NULL, 5, 0, 1,
	N'3 months validity
	View 25 verified  mobile numbers 
	1-month profile highlighter
	Send unlimited messages/interests
	Chat online* (coming soon)',
	N'<p>25 contacts<br />
	3 months validity<br />
	1-month profile highlighter free<br />
	Unlimited messages/interests</p>', 
	NULL, 1, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 10, NULL, 0, 0, 0, 0, NULL, 0, 100, 0, 10, 0, 1, 0, 0, 0, 0,
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(100.0000 AS Decimal(18, 4)),
	CAST(150.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(1000.0000 AS Decimal(18, 4)), 0,
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
	2, N'6 Month Subscription', NULL, NULL, N'6 Month Subscription', NULL, NULL, NULL, NULL, 5, 0, 1,
	N'6 months validity
	View 50 verified  mobile numbers
	3-months profile highlighter
	Send unlimited messages/interests
	Chat online* (coming soon)', 
	N'<p>50 contacts<br />
	6 months validity<br />
	3 - months profile highlighter free<br />
	Unlimited messages/interests</p>', 
	NULL, 1, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 10, NULL, 0, 0, 0, 0, NULL, 0, 100, 0, 10, 0, 1, 0, 0, 0, 0,
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(150.0000 AS Decimal(18, 4)),
	CAST(175.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(1000.0000 AS Decimal(18, 4)), 0,
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
	3, N'1 Year subscription', NULL, NULL, N'1 Year subscription', NULL, NULL, NULL, NULL, 5, 0, 1,
	N'12 months validity
	View 100 verified mobile numbers
	6-month profile highlighter
	Send unlimited messages/interests
	Chat online* (coming soon)', 
	N'<p>100 contacts<br />
	12 months validity<br />
	6-month profile highlighter free<br />
	Unlimited messages/interests</p>', 
	NULL, 1, 0, 0, NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, 0, 0, 0, 0, 1, 10, NULL, 0, 0, 0, 0, NULL, 0, 100, 0, 10, 0, 1, 0, 0, 0, 0,
	CAST(0.0000 AS Decimal(18, 4)), 0, 1, 0, 0, 0, 0, 0, 0, 10000, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, NULL, 0, CAST(200.0000 AS Decimal(18, 4)),
	CAST(250.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), 0, CAST(0.0000 AS Decimal(18, 4)), CAST(1000.0000 AS Decimal(18, 4)), 0,
	CAST(0.0000 AS Decimal(18, 4)), 1, CAST(0.0000 AS Decimal(18, 4)), 1, 0, NULL, NULL, 0, 0, CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)),
	CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), NULL, NULL, 0, 1, 0, Getutcdate(), Getutcdate()
)

SET IDENTITY_INSERT [dbo].[Product] OFF

-- [UrlRecord] is mandate for SEO url to create products otherwise products are not clickable
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[LanguageId],[IsActive]) VALUES('Product','3-month-subscription',1,0,1)
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[LanguageId],[IsActive]) VALUES('Product','6-month-subscription',2,0,1)
INSERT INTO [dbo].[UrlRecord]([EntityName],[Slug],[EntityId],[LanguageId],[IsActive]) VALUES('Product','1-year-subscription',3,0,1)

-------------------- Start:[Product_Category_Mapping] table Insert Scripts ------------------------------------------------------------------------

INSERT INTO [dbo].[Product_Category_Mapping] ([CategoryId],[ProductId],[IsFeaturedProduct],[DisplayOrder]) VALUES(3,1,1,1)
INSERT INTO [dbo].[Product_Category_Mapping] ([CategoryId],[ProductId],[IsFeaturedProduct],[DisplayOrder]) VALUES(3,2,1,2)
INSERT INTO [dbo].[Product_Category_Mapping] ([CategoryId],[ProductId],[IsFeaturedProduct],[DisplayOrder]) VALUES(3,3,1,3)

 -------  Start: Pricing Products Category Template  ---------------------------------------------------------------------------------------

INSERT INTO [dbo].[ProductTemplate]([Name],[ViewPath],[DisplayOrder]) VALUES('Product Price','_ProductBox.Price',2)
INSERT INTO [dbo].[CategoryTemplate]([Name],[ViewPath],[DisplayOrder]) VALUES ('Pricing Category Template','CategoryTemplate.Pricing',3)

-------------------   [CustomerAttribute]  -----------------------------

SELECT * FROM [CustomerAttribute]

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

-- Make all feilds mandatory
UPDATE [CustomerAttribute] SET IsRequired=1

------------------------  [SpecificationAttribute]  -----------------------------------------

-- Delete  FROM  SpecificationAttribute 
SELECT * FROM SpecificationAttribute

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

SELECT * FROM [SpecificationAttribute] 
SELECT * FROM [SpecificationAttributeOption]

-- Delete  * from  [SpecificationAttributeOption]


SET IDENTITY_INSERT [SpecificationAttributeOption] ON

-- Profile Type
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (1,1,'Give Support',0);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (2,1,'Take Support',1);

-- Current Avalibility
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (3,2,'Available',2);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (4,2,'UnAvailable',3);

-- Relavent Experiance
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (5,3,'1-3 Years',4);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (6,3,'3-6 Years',5);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (7,3,'6-10 Years',6);
INSERT INTO [SpecificationAttributeOption] ([Id],[SpecificationAttributeId] ,[Name], [DisplayOrder]) VALUES (8,3,'10 + Years',7);

-- Mother Tounge
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(9,'English', 4, 8);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(10,'Bengali', 4, 9);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(11,'Gujarati', 4, 10);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(12,'Hindi', 4, 11);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(13,'Kannada', 4, 12);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(14,'Malayalam', 4,13);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(15,'Marathi', 4, 14);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(16,'Oriya', 4, 15);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(17,'Punjabi', 4, 16);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(18,'Tamil', 4, 17);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(19,'Telugu', 4, 18);

-- Gender
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(20,'Male', 9, 19);
INSERT INTO [SpecificationAttributeOption] ([Id],[Name],[SpecificationAttributeId],[DisplayOrder]) VALUES(21,'Fe Male', 9, 20);

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
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('C#', 6, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('MVC', 6, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('JAVA', 6, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('AWS', 6, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('TeraData', 6, 0);
INSERT INTO [SpecificationAttributeOption] ( [Name], [SpecificationAttributeId], [DisplayOrder]) VALUES ('Azure', 6, 0);

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
-------------------- Start: [EmailAccount] ------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM [EmailAccount] WHERE [Email]='no-reply@Onjobsupport.in')
   BEGIN
	 INSERT INTO [EmailAccount]([DisplayName],[Email],[Host],[Username],[Password],[Port])
     VALUES ('On Job Suport','no-reply@Onjobsupport.in','smtp-relay.sendinblue.com','umsateesh@gmail.com','TkUZDCRvhxnF8Era','587')
   END

------------------------- Shopping cart settings -----------------

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.threemonthsubscriptionproductid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.threemonthsubscriptionproductid','16',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.sixmonthsubscriptionproductid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.sixmonthsubscriptionproductid','17',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.oneyearsubscriptionproductid')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.oneyearsubscriptionproductid','18',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.threemonthsubscriptionallottedcount')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.threemonthsubscriptionallottedcount','25',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.sixmonthsubscriptionallottedcount')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.sixmonthsubscriptionallottedcount','50',0)
   END

IF NOT EXISTS (SELECT * FROM [Setting] WHERE [Name]='shoppingCartSettings.oneyearsubscriptionallottedcount')
   BEGIN
	 INSERT INTO [Setting] ([Name],[Value],[StoreId]) VALUES ('shoppingCartSettings.oneyearsubscriptionallottedcount','100',0)
   END


--UPDATE [Setting] SET [Value]=1  WHERE [Name]='shoppingCartSettings.threemonthsubscriptionproductid'
--UPDATE [Setting] SET [Value]=2  WHERE [Name]='shoppingCartSettings.sixmonthsubscriptionproductid'
--UPDATE [Setting] SET [Value]=3  WHERE [Name]='shoppingCartSettings.oneyearsubscriptionproductid'

------------------------- Customer settings -----------------

SELECT * FROM [Setting] WHERE [Name] like 'customersettings.gender%'

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