using System;
using System.Collections.Generic;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages.SMS
{
    /// <summary>
    /// Represents the SMS service
    /// </summary>
    public partial class SMSService : ISMSService
    {
        #region Constants
        private const string SMSPROVIDERS_BY_ID_KEY = "Nop.smsprovider.id-{0}";
        private const string SMSPROVIDERS_PATTERN_KEY = "Nop.smsprovider.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public SMSService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a SMS provider
        /// </summary>
        /// <param name="smsProviderId">SMS provider identifier</param>
        public void DeleteSMSProvider(int smsProviderId)
        {
            var smsProvider = GetSMSProviderById(smsProviderId);
            if (smsProvider == null)
                return;

            
            if (!_context.IsAttached(smsProvider))
                _context.SMSProviders.Attach(smsProvider);
            _context.DeleteObject(smsProvider);
            _context.SaveChanges();

            if (CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SMSPROVIDERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a SMS provider
        /// </summary>
        /// <param name="smsProviderId">SMS provider identifier</param>
        /// <returns>SMS provider</returns>
        public SMSProvider GetSMSProviderById(int smsProviderId)
        {
            if (smsProviderId == 0)
                return null;

            string key = string.Format(SMSPROVIDERS_BY_ID_KEY, smsProviderId);
            object obj2 = _cacheManager.Get(key);
            if (CacheEnabled && (obj2 != null))
            {
                return (SMSProvider)obj2;
            }

            
            var query = from p in _context.SMSProviders
                        where p.SMSProviderId == smsProviderId
                        select p;
            var smsProvider = query.SingleOrDefault();

            if (CacheEnabled)
            {
                _cacheManager.Add(key, smsProvider);
            }
            return smsProvider;
        }

        /// <summary>
        /// Gets a SMS provider
        /// </summary>
        /// <param name="systemKeyword">SMS provider system keyword</param>
        /// <returns>SMS provider</returns>
        public SMSProvider GetSMSProviderBySystemKeyword(string systemKeyword)
        {
            
            var query = from p in _context.SMSProviders
                        where p.SystemKeyword == systemKeyword
                        select p;
            var smsProvider = query.FirstOrDefault();

            return smsProvider;
        }

        /// <summary>
        /// Gets all SMS providers
        /// </summary>
        /// <returns>SMS provider collection</returns>
        public List<SMSProvider> GetAllSMSProviders()
        {
            return GetAllSMSProviders();
        }

        /// <summary>
        /// Gets all SMS providers
        /// </summary>
        /// <param name="showHidden">A value indicating whether the not active SMS providers should be load</param>
        /// <returns>SMS provider collection</returns>
        public List<SMSProvider> GetAllSMSProviders(bool showHidden)
        {
            
            var query = from p in _context.SMSProviders
                        where showHidden || p.IsActive
                        orderby p.Name
                        select p;
            return query.ToList();
        }

        /// <summary>
        /// Inserts a SMS provider
        /// </summary>
        /// <param name="smsProvider">SMS provider</param>
        public void InsertSMSProvider(SMSProvider smsProvider)
        {
            if (smsProvider == null)
                throw new ArgumentNullException("smsProvider");
            
            smsProvider.Name = CommonHelper.EnsureNotNull(smsProvider.Name);
            smsProvider.Name = CommonHelper.EnsureMaximumLength(smsProvider.Name, 100);
            smsProvider.ClassName = CommonHelper.EnsureNotNull(smsProvider.ClassName);
            smsProvider.ClassName = CommonHelper.EnsureMaximumLength(smsProvider.ClassName, 500);
            smsProvider.SystemKeyword = CommonHelper.EnsureNotNull(smsProvider.SystemKeyword);
            smsProvider.SystemKeyword = CommonHelper.EnsureMaximumLength(smsProvider.SystemKeyword, 500);

            
            
            _context.SMSProviders.AddObject(smsProvider);
            _context.SaveChanges();

            if (CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SMSPROVIDERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the SMS provider
        /// </summary>
        /// <param name="smsProvider">SMS provider</param>
        public void UpdateSMSProvider(SMSProvider smsProvider)
        {
            if (smsProvider == null)
                throw new ArgumentNullException("smsProvider");

            smsProvider.Name = CommonHelper.EnsureNotNull(smsProvider.Name);
            smsProvider.Name = CommonHelper.EnsureMaximumLength(smsProvider.Name, 100);
            smsProvider.ClassName = CommonHelper.EnsureNotNull(smsProvider.ClassName);
            smsProvider.ClassName = CommonHelper.EnsureMaximumLength(smsProvider.ClassName, 500);
            smsProvider.SystemKeyword = CommonHelper.EnsureNotNull(smsProvider.SystemKeyword);
            smsProvider.SystemKeyword = CommonHelper.EnsureMaximumLength(smsProvider.SystemKeyword, 500);

            
            if (!_context.IsAttached(smsProvider))
                _context.SMSProviders.Attach(smsProvider);

            _context.SaveChanges();

            if (CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SMSPROVIDERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        public int SendSMS(string text)
        {
            int i = 0;

            foreach (SMSProvider smsProvider in GetAllSMSProviders(false))
            {
                var iSMSProvider = smsProvider.Instance;
                if (iSMSProvider.SendSMS(text))
                {
                    i++;
                }
            }
            return i;
        }

        /// <summary>
        /// Sends SMS notification about placed order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>true if message was sent successfully; otherwise false.</returns>
        public void SendOrderPlacedNotification(Order order)
        {
            if (order != null)
            {
                if (SendSMS(String.Format("New order(#{0}) has been placed.", order.OrderId)) > 0)
                {
                    IoCFactory.Resolve<IOrderService>().InsertOrderNote(order.OrderId, "\"Order placed\" SMS alert (to store owner) has been sent", false, DateTime.UtcNow);
                }
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.SMSManager.CacheEnabled");
            }
        }
        #endregion

    }
}
