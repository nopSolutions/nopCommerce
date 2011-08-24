using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Transactions;

namespace Nop.Data
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Add SQL Server indexes for performance optimization
        /// </summary>
        /// <param name="context">Context</param>
        public static void InstallNopCommerceIndexes(this DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [dbo].[LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductId] ON [dbo].[ProductVariant] ([ProductId])	INCLUDE ([Price],[AvailableStartDateTimeUtc],[AvailableEndDateTimeUtc],[Published],[Deleted])");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [dbo].[Country] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [dbo].[StateProvince] ([CountryId]) INCLUDE ([DisplayOrder])");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Currency_DisplayOrder] ON [dbo].[Currency] ( [DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [dbo].[Log] ([CreatedOnUtc] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [dbo].[Customer] ([Email] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [dbo].[Customer] ([Username] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [dbo].[Customer] ([CustomerGuid] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [dbo].[QueuedEmail] ([CreatedOnUtc] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Order_CustomerId] ON [dbo].[Order] ([CustomerId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [dbo].[Language] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_CustomerAttribute_CustomerId] ON [dbo].[CustomerAttribute] ([CustomerId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [dbo].[BlogPost] ([LanguageId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [dbo].[BlogComment] ([BlogPostId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_News_LanguageId] ON [dbo].[News] ([LanguageId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_NewsComment_NewsItemId] ON [dbo].[NewsComment] ([NewsItemId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [dbo].[PollAnswer] ([PollId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ProductReview_ProductId] ON [dbo].[ProductReview] ([ProductId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_OrderProductVariant_OrderId] ON [dbo].[OrderProductVariant] ([OrderId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_OrderNote_OrderId] ON [dbo].[OrderNote] ([OrderId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_TierPrice_ProductVariantId] ON [dbo].[TierPrice] ([ProductVariantId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId] ON [dbo].[ShoppingCartItem] ([ShoppingCartTypeId] ASC, [CustomerId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_RelatedProduct_ProductId1] ON [dbo].[RelatedProduct] ([ProductId1] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ProductVariant_DisplayOrder] ON [dbo].[ProductVariant] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ProductVariantAttributeValue_ProductVariantAttributeId] ON [dbo].[ProductVariantAttributeValue] ([ProductVariantAttributeId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_ProductVariant_ProductAttribute_Mapping_ProductVariantId] ON [dbo].[ProductVariant_ProductAttribute_Mapping] ([ProductVariantId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Manufacturer_DisplayOrder] ON [dbo].[Manufacturer] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Category_DisplayOrder] ON [dbo].[Category] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [dbo].[Category] ([ParentCategoryId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [dbo].[Forums_Group] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [dbo].[Forums_Forum] ([DisplayOrder] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [dbo].[Forums_Forum] ([ForumGroupId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [dbo].[Forums_Topic] ([ForumId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [dbo].[Forums_Post] ([TopicId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Post_CustomerId] ON [dbo].[Forums_Post] ([CustomerId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [dbo].[Forums_Subscription] ([ForumId] ASC)");
            context.Database.ExecuteSqlCommand("CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [dbo].[Forums_Subscription] ([TopicId] ASC)");
        }
    }
}
