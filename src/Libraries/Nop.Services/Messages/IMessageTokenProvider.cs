using System.Collections.Generic;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddStoreTokens(IList<Token> tokens);

        void AddOrderTokens(IList<Token> tokens, Order order, int languageId);

        void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, OrderProductVariant opv);

        void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard);

        void AddCustomerTokens(IList<Token> tokens, Customer customer);

        void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription);

        void AddProductReviewTokens(IList<Token> tokens, ProductReview productReview);

        void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment);

        void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment);

        void AddProductTokens(IList<Token> tokens, Product product);

        void AddProductVariantTokens(IList<Token> tokens, ProductVariant productVariant);

        void AddForumTokens(IList<Token> tokens, Forum forum);

        void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic);

        void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost);

        void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage);

        string[] GetListOfCampaignAllowedTokens();

        string[] GetListOfAllowedTokens();
    }
}
