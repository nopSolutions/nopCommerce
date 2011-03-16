using System;
using System.Collections.Generic;
using System.Linq;

using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data;

namespace Nop.Services.Messages
{
    public partial class NewsLetterSubscriptionService: INewsLetterSubscriptionService
    {
        private readonly IRepository<NewsLetterSubscription> _newsLetterSubscriptionRepository;
        private readonly IRepository<Customer> _customersRepository;
   
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="newsLetterSubscriptionRepository">NewsLetter subscription repository</param>
        /// <param name="customersRepository">Customer repository</param>
        public NewsLetterSubscriptionService(IRepository<NewsLetterSubscription> newsLetterSubscriptionRepository,
            IRepository<Customer> customersRepository)
        {
            this._newsLetterSubscriptionRepository = newsLetterSubscriptionRepository;
            this._customersRepository = customersRepository;
        }

        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        public void InsertNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription)
        {
            if (newsLetterSubscription == null)
                throw new ArgumentNullException("newsLetterSubscription");

            if (!CommonHelper.IsValidEmail(newsLetterSubscription.Email))
            {
                throw new NopException("Email is not valid.");
            }

            newsLetterSubscription.Email = CommonHelper.EnsureNotNull(newsLetterSubscription.Email);
            newsLetterSubscription.Email = newsLetterSubscription.Email.Trim();
            newsLetterSubscription.Email = CommonHelper.EnsureMaximumLength(newsLetterSubscription.Email, 255);

            _newsLetterSubscriptionRepository.Insert(newsLetterSubscription);
        }

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        public void UpdateNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription)
        {
            if (newsLetterSubscription == null)
                throw new ArgumentNullException("newsLetterSubscription");

            newsLetterSubscription.Email = CommonHelper.EnsureNotNull(newsLetterSubscription.Email);
            newsLetterSubscription.Email = newsLetterSubscription.Email.Trim();
            newsLetterSubscription.Email = CommonHelper.EnsureMaximumLength(newsLetterSubscription.Email, 255);

            if (!CommonHelper.IsValidEmail(newsLetterSubscription.Email))
            {
                throw new NopException("Email is not valid.");
            }

            _newsLetterSubscriptionRepository.Update(newsLetterSubscription);

        }

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        public void DeleteNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription)
        {
            if (newsLetterSubscription == null)
                return;

            _newsLetterSubscriptionRepository.Delete(newsLetterSubscription);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public NewsLetterSubscription GetNewsLetterSubscriptionById(int newsLetterSubscriptionId)
        {
            if (newsLetterSubscriptionId == 0)
                return null;

            var queuedEmail = _newsLetterSubscriptionRepository.GetById(newsLetterSubscriptionId);
            return queuedEmail;
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetter subscription</returns>
        public NewsLetterSubscription GetNewsLetterSubscriptionByGuid(Guid newsLetterSubscriptionGuid)
        {
            if (newsLetterSubscriptionGuid == Guid.Empty)
                return null;

            var newsLetterSubscriptions = from nls in _newsLetterSubscriptionRepository.Table
                                          where nls.NewsLetterSubscriptionGuid == newsLetterSubscriptionGuid
                                          orderby nls.Id
                                          select nls;

            return newsLetterSubscriptions.FirstOrDefault();
        }

        /// <summary>
        /// Gets a newsletter subscription by email
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <returns>NewsLetter subscription</returns>
        public NewsLetterSubscription GetNewsLetterSubscriptionByEmail(string email)
        {
            if (!CommonHelper.IsValidEmail(email))
                return null;

            email = email.Trim();

            var newsLetterSubscriptions = from nls in _newsLetterSubscriptionRepository.Table
                                          where nls.Email == email
                                          orderby nls.Id
                                          select nls;

            return newsLetterSubscriptions.FirstOrDefault();

        }

        /// <summary>
        /// Gets the newsletter subscription list
        /// </summary>
        /// <param name="email">Email to search or string. Empty to load all records.</param>
        /// <param name="showHidden">A value indicating whether the not active subscriptions should be loaded</param>
        /// <returns>NewsLetterSubscription entity list</returns>
        public IList<NewsLetterSubscription> GetAllNewsLetterSubscriptions(string email, bool showHidden = false)
        {
            //var query1 = from nls in _newsLetterSubscriptionRepository.Table
            //             from c in _customersRepository.Table
            //             .Where(c => c.Email == nls.Email)
            //             .DefaultIfEmpty()
            //             where
            //             (showHidden || nls.Active) &&
            //             (c == null || c.Id == 0 || (c.Active && !c.Deleted)) &&
            //             (String.IsNullOrEmpty(email) || nls.Email.Contains(email))
            //             select nls.Id;

            //var query2 = from nls in _newsLetterSubscriptionRepository.Table
            //             where query1.Contains(nls.Id)
            //             orderby nls.Email
            //             select nls;

            //var newsletterSubscriptions = query2.ToList();
            //return newsletterSubscriptions;


            var query = _newsLetterSubscriptionRepository.Table;
            if (!String.IsNullOrEmpty(email))
                query = query.Where(nls => nls.Email.Contains(email));
            query = query.Where(nls => showHidden || nls.Active);
            query = query.OrderBy(nls => nls.Email);

            var newsletterSubscriptions = query.ToList();
            return newsletterSubscriptions;
        }

        /// <summary>
        /// Gets a customer subscribed to newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">The newsLetter subscription</param>
        /// <returns>Customer</returns>
        public Customer GetNewsLetterSubscriptionCustomer(NewsLetterSubscription newsLetterSubscription)
        {
            //TODO implement get customer by email functionality(customer entity doesn't have Email property anymore

            throw new NotImplementedException();
        }

    }
}
