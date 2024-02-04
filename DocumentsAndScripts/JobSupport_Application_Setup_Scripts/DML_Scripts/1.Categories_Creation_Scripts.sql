
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

-- update category description for give support & Take support
  UPDATE [dbo].[Category]
  SET Description='<table style="border-collapse: collapse; width: 99.9%; height: 136px; background-color: #f9f9f9; border-color: #ffa500; border-style: solid;" border="1" cellspacing="5" cellpadding="5"><caption>&nbsp;</caption>  <tbody>  <tr>  <td style="width: 100%;">  <ul>  <li><span style="font-size: 10pt;">Below Profiles are interested to take support from you.</span></li>  <li><span style="font-size: 10pt;">You can filter further by using left filter to match your skillset.</span></li>  <li><span style="font-size: 10pt;">You can shortlist the profiles that you like. Shortlisted profiles will appear in My Account page for easy access in future.</span></li>  <li><span style="font-size: 10pt;">You can send interest to the profiles that you like. If they also interested about your profile they may contact you.</span></li>  </ul>  </td>  </tr>  </tbody>  </table>'
  Where Id=2

  UPDATE [dbo].[Category]
  SET Description='<table style="border-collapse: collapse; width: 99.9%; height: 136px; background-color: #f9f9f9; border-color: #ffa500; border-style: solid;" border="1" cellspacing="5" cellpadding="5"><caption>&nbsp;</caption>  <tbody>  <tr>  <td style="width: 100%;">  <ul>  <li><span style="font-size: 10pt;">Below Profiles are interested to provide support.</span></li>  <li><span style="font-size: 10pt;">You can filter further by using left filter to match your skillset.</span></li>  <li><span style="font-size: 10pt;">You can shortlist the profiles that you like. Shortlisted profiles will appear in My Account page for easy access in future.</span></li>  <li><span style="font-size: 10pt;">You can send interest to the profiles that you like. If they also interested about your profile they may contact you.</span></li>  </ul>  </td>  </tr>  </tbody>  </table>'
  Where Id=1
