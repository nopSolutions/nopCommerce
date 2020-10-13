using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IAddressService _addressService;
        private readonly IAffiliateService _affiliateService;
        private readonly ICustomerService _customerService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITokenizer _tokenizer;

        #endregion

        #region Ctor

        public WorkflowMessageService(CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IOrderService orderService,
            IProductService productService,
            IQueuedEmailService queuedEmailService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITokenizer tokenizer)
        {
            _commonSettings = commonSettings;
            _emailAccountSettings = emailAccountSettings;
            _addressService = addressService;
            _affiliateService = affiliateService;
            _customerService = customerService;
            _emailAccountService = emailAccountService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _orderService = orderService;
            _productService = productService;
            _queuedEmailService = queuedEmailService;
            _storeContext = storeContext;
            _storeService = storeService;
            _tokenizer = tokenizer;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get active message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>List of message templates</returns>
        protected virtual async Task<IList<MessageTemplate>> GetActiveMessageTemplates(string messageTemplateName, int storeId)
        {
            //get message templates by the name
            var messageTemplates = await _messageTemplateService.GetMessageTemplatesByName(messageTemplateName, storeId);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<MessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        /// <summary>
        /// Get EmailAccount to use with a message templates
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>EmailAccount</returns>
        protected virtual async Task<EmailAccount> GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = await _localizationService.GetLocalized(messageTemplate, mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = (await _emailAccountService.GetEmailAccountById(emailAccountId) ?? await _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)) ??
                               (await _emailAccountService.GetAllEmailAccounts()).FirstOrDefault();
            return emailAccount;
        }

        /// <summary>
        /// Ensure language is active
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Return a value language identifier</returns>
        protected virtual async Task<int> EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = await _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = (await _languageService.GetAllLanguages(storeId: storeId)).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = (await _languageService.GetAllLanguages()).FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }

        #endregion

        #region Methods

        #region Customer workflow

        /// <summary>
        /// Sends 'New customer' notification message to a store owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendCustomerRegisteredNotificationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerRegisteredNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerWelcomeMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an email validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerEmailValidationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an email re-validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendCustomerEmailRevalidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerEmailRevalidationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                //email to re-validate
                var toEmail = customer.EmailToRevalidate;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.CustomerPasswordRecoveryMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        #endregion

        #region Order workflow

        /// <summary>
        /// Sends an order placed notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPlacedVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedVendorNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId, vendor.Id);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order placed notification to an affiliate
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPlacedAffiliateNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var affiliate = await _affiliateService.GetAffiliateById(order.AffiliateId);

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedAffiliateNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var affiliateAddress = _addressService.GetAddressById(affiliate.AddressId).Result;
                var toEmail = affiliateAddress.Email;
                var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPaidStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to an affiliate
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPaidAffiliateNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var affiliate = await _affiliateService.GetAffiliateById(order.AffiliateId);

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidAffiliateNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var affiliateAddress = _addressService.GetAddressById(affiliate.AddressId).Result;
                var toEmail = affiliateAddress.Email;
                var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPaidCustomerNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    attachmentFilePath, attachmentFileName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPaidVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidVendorNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId, vendor.Id);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order placed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderPlacedCustomerNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    attachmentFilePath, attachmentFileName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a shipment sent notification to a customer
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendShipmentSentCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = await _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ShipmentSentCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddShipmentTokens(commonTokens, shipment, languageId);
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a shipment delivered notification to a customer
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendShipmentDeliveredCustomerNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = await _orderService.GetOrderById(shipment.OrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ShipmentDeliveredCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddShipmentTokens(commonTokens, shipment, languageId);
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderCompletedCustomerNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderCompletedCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    attachmentFilePath, attachmentFileName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order cancelled notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderCancelledCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order refunded notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderRefundedStoreOwnerNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddOrderRefundedTokens(commonTokens, order, refundedAmount);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends an order refunded notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendOrderRefundedCustomerNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.OrderRefundedCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddOrderRefundedTokens(commonTokens, order, refundedAmount);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a new order note added notification to a customer
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewOrderNoteAddedCustomerNotification(OrderNote orderNote, int languageId)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            var order = await _orderService.GetOrderById(orderNote.OrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewOrderNoteAddedCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderNoteTokens(commonTokens, orderNote);
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a store owner
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendRecurringPaymentCancelledStoreOwnerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var order = await _orderService.GetOrderById(recurringPayment.InitialOrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);
            await _messageTokenProvider.AddRecurringPaymentTokens(commonTokens, recurringPayment);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a customer
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendRecurringPaymentCancelledCustomerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var order = await _orderService.GetOrderById(recurringPayment.InitialOrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.RecurringPaymentCancelledCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);
            await _messageTokenProvider.AddRecurringPaymentTokens(commonTokens, recurringPayment);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a "Recurring payment failed" notification to a customer
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendRecurringPaymentFailedCustomerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var order = await _orderService.GetOrderById(recurringPayment.InitialOrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.RecurringPaymentFailedCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, order.CustomerId);
            await _messageTokenProvider.AddRecurringPaymentTokens(commonTokens, recurringPayment);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddNewsLetterSubscriptionTokens(commonTokens, subscription);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddNewsLetterSubscriptionTokens(commonTokens, subscription);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty).Result;
            }).ToList();
        }

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="customerEmail">Customer's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendProductEmailAFriendMessage(Customer customer, int languageId,
            Product product, string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.EmailAFriendMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            await _messageTokenProvider.AddProductTokens(commonTokens, product, languageId);
            commonTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
            commonTokens.Add(new Token("EmailAFriend.Email", customerEmail));

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="customerEmail">Customer's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendWishlistEmailAFriendMessage(Customer customer, int languageId,
             string customerEmail, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.WishlistToFriendMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            commonTokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            commonTokens.Add(new Token("Wishlist.Email", customerEmail));

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty).Result;
            }).ToList();
        }

        #endregion

        #region Return requests

        /// <summary>
        /// Sends 'New Return Request' message to a store owner
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, OrderItem orderItem, Order order, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, returnRequest.CustomerId);
            await _messageTokenProvider.AddReturnRequestTokens(commonTokens, returnRequest, orderItem);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends 'New Return Request' message to a customer
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewReturnRequestCustomerNotification(ReturnRequest returnRequest, OrderItem orderItem, Order order)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            var languageId = await EnsureLanguageIsActive(order.CustomerLanguageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewReturnRequestCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var customer = await _customerService.GetCustomerById(returnRequest.CustomerId);

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            await _messageTokenProvider.AddReturnRequestTokens(commonTokens, returnRequest, orderItem);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = _customerService.IsGuest(customer).Result ?
                    billingAddress.Email :
                    customer.Email;
                var toName = _customerService.IsGuest(customer).Result ?
                    billingAddress.FirstName :
                    _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends 'Return Request status changed' message to a customer
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendReturnRequestStatusChangedCustomerNotification(ReturnRequest returnRequest, OrderItem orderItem, Order order)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore();
            var languageId = await EnsureLanguageIsActive(order.CustomerLanguageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ReturnRequestStatusChangedCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var customer = await _customerService.GetCustomerById(returnRequest.CustomerId);

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            await _messageTokenProvider.AddReturnRequestTokens(commonTokens, returnRequest, orderItem);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId).Result;

                var toEmail = _customerService.IsGuest(customer).Result ?
                    billingAddress.Email :
                    customer.Email;
                var toName = _customerService.IsGuest(customer).Result ?
                    billingAddress.FirstName :
                    _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewForumTopicMessage(Customer customer, ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewForumTopicMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddForumTopicTokens(commonTokens, forumTopic);
            await _messageTokenProvider.AddForumTokens(commonTokens, forum);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewForumPostMessage(Customer customer, ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewForumPostMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddForumPostTokens(commonTokens, forumPost);
            await _messageTokenProvider.AddForumTopicTokens(commonTokens, forumTopic, friendlyForumTopicPageIndex, forumPost.Id);
            await _messageTokenProvider.AddForumTokens(commonTokens, forum);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendPrivateMessageNotification(PrivateMessage privateMessage, int languageId)
        {
            if (privateMessage == null)
                throw new ArgumentNullException(nameof(privateMessage));

            var store = await _storeService.GetStoreById(privateMessage.StoreId) ?? await _storeContext.GetCurrentStore();

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.PrivateMessageNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddPrivateMessageTokens(commonTokens, privateMessage);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, privateMessage.ToCustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var customer = _customerService.GetCustomerById(privateMessage.ToCustomerId).Result;
                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        #endregion

        #region Misc

        /// <summary>
        /// Sends 'New vendor account submitted' message to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewVendorAccountApplyStoreOwnerNotification(Customer customer, Vendor vendor, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            await _messageTokenProvider.AddVendorTokens(commonTokens, vendor);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends 'Vendor information changed' message to a store owner
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendVendorInformationChangeNotification(Vendor vendor, int languageId)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.VendorInformationChangeNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddVendorTokens(commonTokens, vendor);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            var order = await _orderService.GetOrderByOrderItem(giftCard.PurchasedWithOrderItemId ?? 0);

            var store = order != null ? await _storeService.GetStoreById(order.StoreId) ?? await _storeContext.GetCurrentStore() : await _storeContext.GetCurrentStore();

            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.GiftCardNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddGiftCardTokens(commonTokens, giftCard);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = giftCard.RecipientEmail;
                var toName = giftCard.RecipientName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendProductReviewNotificationMessage(ProductReview productReview, int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ProductReviewStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddProductReviewTokens(commonTokens, productReview);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, productReview.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a product review reply notification message to a customer
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendProductReviewReplyCustomerNotificationMessage(ProductReview productReview, int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            var store = await _storeService.GetStoreById(productReview.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ProductReviewReplyCustomerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var customer = await _customerService.GetCustomerById(productReview.CustomerId);

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddProductReviewTokens(commonTokens, productReview);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendQuantityBelowStoreOwnerNotification(Product product, int languageId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddProductTokens(commonTokens, product, languageId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="combination">Attribute combination</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendQuantityBelowStoreOwnerNotification(ProductAttributeCombination combination, int languageId)
        {
            if (combination == null)
                throw new ArgumentNullException(nameof(combination));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var commonTokens = new List<Token>();
            var product = await _productService.GetProductById(combination.ProductId);

            await _messageTokenProvider.AddProductTokens(commonTokens, product, languageId);
            await _messageTokenProvider.AddAttributeCombinationTokens(commonTokens, combination, languageId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a "new VAT submitted" notification to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewVatSubmittedStoreOwnerNotification(Customer customer,
            string vatName, string vatAddress, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            commonTokens.Add(new Token("VatValidationResult.Name", vatName));
            commonTokens.Add(new Token("VatValidationResult.Address", vatAddress));

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>List of queued email identifiers</returns>
        public virtual async Task<IList<int>> SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.BlogCommentNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddBlogCommentTokens(commonTokens, blogComment);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, blogComment.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        {
            if (newsComment == null)
                throw new ArgumentNullException(nameof(newsComment));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.NewsCommentNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddNewsCommentTokens(commonTokens, newsComment);
            await _messageTokenProvider.AddCustomerTokens(commonTokens, newsComment.CustomerId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a 'Back in stock' notification message to a customer
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendBackInStockNotification(BackInStockSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var customer = await _customerService.GetCustomerById(subscription.CustomerId);

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //ensure that customer is registered (simple and fast way)
            if (!CommonHelper.IsValidEmail(customer.Email))
                return new List<int>();

            var store = await _storeService.GetStoreById(subscription.StoreId) ?? await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.BackInStockNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomerTokens(commonTokens, customer);
            await _messageTokenProvider.AddBackInStockTokens(commonTokens, subscription);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = customer.Email;
                var toName = _customerService.GetCustomerFullName(customer).Result;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendContactUsMessage(int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ContactUsMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName),
                new Token("ContactUs.Body", body, true)
            };

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                string fromEmail;
                string fromName;
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    fromEmail = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
                }
                else
                {
                    fromEmail = senderEmail;
                    fromName = senderName;
                }

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends "contact vendor" message
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<IList<int>> SendContactVendorMessage(Vendor vendor, int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = await _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplates(MessageTemplateSystemNames.ContactVendorMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName),
                new Token("ContactUs.Body", body, true)
            };

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId).Result;

                string fromEmail;
                string fromName;
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    fromEmail = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
                }
                else
                {
                    fromEmail = senderEmail;
                    fromName = senderName;
                }

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount).Wait();

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName).Result;
            }).ToList();
        }

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<int> SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            var messageTemplate = await _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate == null)
                throw new ArgumentException("Template cannot be loaded");

            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens).Wait();

            return await SendNotification(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null);
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual async Task<int> SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            //retrieve localized message template data
            var bcc = await _localizationService.GetLocalized(messageTemplate, mt => mt.BccEmailAddresses, languageId);
            if (string.IsNullOrEmpty(subject))
                subject = await _localizationService.GetLocalized(messageTemplate, mt => mt.Subject, languageId);
            var body = await _localizationService.GetLocalized(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            await _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        #endregion

        #endregion
    }
}