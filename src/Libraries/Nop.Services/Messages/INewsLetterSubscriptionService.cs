using System;
using System.Collections.Generic;

using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Messages
{
    public partial interface INewsLetterSubscriptionService
    {
        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        void InsertNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription);

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        void UpdateNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription);

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        void DeleteNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription);

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        /// <returns>NewsLetter subscription</returns>
        NewsLetterSubscription GetNewsLetterSubscriptionById(int newsLetterSubscriptionId);

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetter subscription</returns>
        NewsLetterSubscription GetNewsLetterSubscriptionByGuid(Guid newsLetterSubscriptionGuid);

        /// <summary>
        /// Gets a newsletter subscription by email
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <returns>NewsLetter subscription</returns>
        NewsLetterSubscription GetNewsLetterSubscriptionByEmail(string email);

        /// <summary>
        /// Gets the newsletter subscription list
        /// </summary>
        /// <param name="email">Email to search or string. Empty to load all records.</param>
        /// <param name="showHidden">A value indicating whether the not active subscriptions should be loaded</param>
        /// <returns>NewsLetterSubscription entity list</returns>
        IList<NewsLetterSubscription> GetAllNewsLetterSubscriptions(string email, bool showHidden = false);
    }
}
