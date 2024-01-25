
-- use [onjobsupport47]

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
