CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId] ON [ProductVariant] ([ProductId])	INCLUDE ([Price],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc],[Published],[Deleted])
GO

CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [Country] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [StateProvince] ([CountryId]) INCLUDE ([DisplayOrder])
GO

CREATE NONCLUSTERED INDEX [IX_Currency_DisplayOrder] ON [Currency] ( [DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [Customer] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [Customer] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [Customer] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Order_CustomerId] ON [Order] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [BlogPost] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [BlogComment] ([BlogPostId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_News_LanguageId] ON [News] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsComment_NewsItemId] ON [NewsComment] ([NewsItemId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [PollAnswer] ([PollId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductId] ON [ProductReview] ([ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_OrderProductVariant_OrderId] ON [OrderProductVariant] ([OrderId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_OrderNote_OrderId] ON [OrderNote] ([OrderId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_TierPrice_ProductVariantId] ON [TierPrice] ([ProductVariantId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId] ON [ShoppingCartItem] ([ShoppingCartTypeId] ASC, [CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_RelatedProduct_ProductId1] ON [RelatedProduct] ([ProductId1] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_DisplayOrder] ON [ProductVariant] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariantAttributeValue_ProductVariantAttributeId] ON [ProductVariantAttributeValue] ([ProductVariantAttributeId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductAttribute_Mapping_ProductVariantId] ON [ProductVariant_ProductAttribute_Mapping] ([ProductVariantId] ASC)
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

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [Forums_Forum] ([ForumGroupId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [Forums_Topic] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [Forums_Post] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_CustomerId] ON [Forums_Post] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [Forums_Subscription] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [Forums_Subscription] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Deleted_and_Published] ON [Product] ([Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Published] ON [Product] ([Published] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Product_ShowOnHomepage] ON [Product] ([ShowOnHomepage] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId_2] ON [ProductVariant] ([ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PCM_Product_and_Category] ON [Product_Category_Mapping] ([CategoryId] ASC, [ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PMM_Product_and_Manufacturer] ON [Product_Manufacturer_Mapping] ([ManufacturerId] ASC, [ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_AllowFiltering] ON [Product_SpecificationAttribute_Mapping] ([AllowFiltering] ASC) INCLUDE ([ProductId],[SpecificationAttributeOptionId])
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_SpecificationAttributeOptionId_AllowFiltering] ON [Product_SpecificationAttribute_Mapping] ([SpecificationAttributeOptionId] ASC, [AllowFiltering] ASC) INCLUDE ([ProductId])
GO

CREATE NONCLUSTERED INDEX [IX_ProductTag_Name] ON [ProductTag] ([Name] ASC)
GO