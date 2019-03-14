CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_PriceDatesEtc] ON [Product]  ([Price] ASC, [AvailableStartDateTimeUtc] ASC, [AvailableEndDateTimeUtc] ASC, [Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [Country] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Currency_DisplayOrder] ON [Currency] ( [DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [Customer] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [Customer] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [Customer] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_SystemName] ON [Customer] ([SystemName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CreatedOnUtc] ON [Customer] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_Order_CreatedOnUtc] ON [Order] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsletterSubscription_Email_StoreId] ON [NewsLetterSubscription] ([Email] ASC, [StoreId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId] ON [ShoppingCartItem] ([ShoppingCartTypeId] ASC, [CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_RelatedProduct_ProductId1] ON [RelatedProduct] ([ProductId1] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductAttributeValue_ProductAttributeMappingId_DisplayOrder] ON [ProductAttributeValue] ([ProductAttributeMappingId] ASC, [DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_ProductAttribute_Mapping_ProductId_DisplayOrder] ON [Product_ProductAttribute_Mapping] ([ProductId] ASC, [DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Manufacturer_DisplayOrder] ON [Manufacturer] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [Category] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [Category] ([ParentCategoryId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [Forums_Group] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [Forums_Forum] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [Forums_Subscription] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [Forums_Subscription] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Deleted_and_Published] ON [Product] ([Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Published] ON [Product] ([Published] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_ShowOnHomepage] ON [Product] ([ShowOnHomePage] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_ParentGroupedProductId] ON [Product] ([ParentGroupedProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_VisibleIndividually] ON [Product] ([VisibleIndividually] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PCM_Product_and_Category] ON [Product_Category_Mapping] ([CategoryId] ASC, [ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PCM_ProductId_Extended] ON [Product_Category_Mapping] ([ProductId] ASC, [IsFeaturedProduct] ASC) INCLUDE ([CategoryId])
GO

CREATE NONCLUSTERED INDEX [IX_PMM_Product_and_Manufacturer] ON [Product_Manufacturer_Mapping] ([ManufacturerId] ASC, [ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PMM_ProductId_Extended] ON [Product_Manufacturer_Mapping] ([ProductId] ASC, [IsFeaturedProduct] ASC) INCLUDE ([ManufacturerId])
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_AllowFiltering] ON [Product_SpecificationAttribute_Mapping] ([AllowFiltering] ASC) INCLUDE ([ProductId],[SpecificationAttributeOptionId])
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_SpecificationAttributeOptionId_AllowFiltering] ON [Product_SpecificationAttribute_Mapping] ([SpecificationAttributeOptionId] ASC, [AllowFiltering] ASC) INCLUDE ([ProductId])
GO

CREATE NONCLUSTERED INDEX [IX_ProductTag_Name] ON [ProductTag] ([Name] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] DESC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Custom_1] ON [UrlRecord] ([EntityId] ASC, [EntityName] ASC, [LanguageId] ASC, [IsActive] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_AclRecord_EntityId_EntityName] ON [AclRecord] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StoreMapping_EntityId_EntityName] ON [StoreMapping] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_LimitedToStores] ON [Category] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Manufacturer_LimitedToStores] ON [Manufacturer] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_LimitedToStores] ON [Product] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_SubjectToAcl] ON [Category] ([SubjectToAcl] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Manufacturer_SubjectToAcl] ON [Manufacturer] ([SubjectToAcl] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_SubjectToAcl] ON [Product] ([SubjectToAcl] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Category_Mapping_IsFeaturedProduct] ON [Product_Category_Mapping] (IsFeaturedProduct ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Manufacturer_Mapping_IsFeaturedProduct] ON [Product_Manufacturer_Mapping] (IsFeaturedProduct ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerRole_Mapping_Customer_Id] ON [Customer_CustomerRole_Mapping] (Customer_Id ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Delete_Id] ON [Product] (Deleted ASC, Id ASC)
GO

CREATE NONCLUSTERED INDEX [IX_GetLowStockProducts] ON [Product] (Deleted ASC, VendorId ASC, ProductTypeId ASC, ManageInventoryMethodId ASC, MinStockQuantity ASC, UseMultipleWarehouses ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_SentOnUtc_DontSendBeforeDateUtc_Extended] ON [QueuedEmail] ([SentOnUtc], [DontSendBeforeDateUtc]) INCLUDE ([SentTries])
GO

CREATE NONCLUSTERED INDEX [IX_Product_VisibleIndividually_Published_Deleted_Extended] ON [Product] ([VisibleIndividually],[Published],[Deleted]) INCLUDE ([Id],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc])
GO

CREATE NONCLUSTERED INDEX [IX_Category_Deleted_Extended] ON [Category] ([Deleted]) INCLUDE ([Id],[Name],[SubjectToAcl],[LimitedToStores],[Published])
GO