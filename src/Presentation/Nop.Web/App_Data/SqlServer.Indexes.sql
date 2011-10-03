CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [dbo].[LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId] ON [dbo].[ProductVariant] ([ProductId])	INCLUDE ([Price],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc],[Published],[Deleted])
GO

CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [dbo].[Country] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [dbo].[StateProvince] ([CountryId]) INCLUDE ([DisplayOrder])
GO

CREATE NONCLUSTERED INDEX [IX_Currency_DisplayOrder] ON [dbo].[Currency] ( [DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [dbo].[Log] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [dbo].[Customer] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [dbo].[Customer] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [dbo].[Customer] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [dbo].[QueuedEmail] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Order_CustomerId] ON [dbo].[Order] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [dbo].[Language] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_CustomerAttribute_CustomerId] ON [dbo].[CustomerAttribute] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [dbo].[BlogPost] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [dbo].[BlogComment] ([BlogPostId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_News_LanguageId] ON [dbo].[News] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsComment_NewsItemId] ON [dbo].[NewsComment] ([NewsItemId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [dbo].[PollAnswer] ([PollId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductId] ON [dbo].[ProductReview] ([ProductId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_OrderProductVariant_OrderId] ON [dbo].[OrderProductVariant] ([OrderId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_OrderNote_OrderId] ON [dbo].[OrderNote] ([OrderId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_TierPrice_ProductVariantId] ON [dbo].[TierPrice] ([ProductVariantId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId] ON [dbo].[ShoppingCartItem] ([ShoppingCartTypeId] ASC, [CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_RelatedProduct_ProductId1] ON [dbo].[RelatedProduct] ([ProductId1] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_DisplayOrder] ON [dbo].[ProductVariant] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariantAttributeValue_ProductVariantAttributeId] ON [dbo].[ProductVariantAttributeValue] ([ProductVariantAttributeId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductAttribute_Mapping_ProductVariantId] ON [dbo].[ProductVariant_ProductAttribute_Mapping] ([ProductVariantId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Manufacturer_DisplayOrder] ON [dbo].[Manufacturer] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [dbo].[Category] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [dbo].[Category] ([ParentCategoryId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [dbo].[Forums_Group] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [dbo].[Forums_Forum] ([DisplayOrder] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [dbo].[Forums_Forum] ([ForumGroupId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [dbo].[Forums_Topic] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [dbo].[Forums_Post] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_CustomerId] ON [dbo].[Forums_Post] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [dbo].[Forums_Subscription] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [dbo].[Forums_Subscription] ([TopicId] ASC)
GO
