using AO.Services.Domain;
using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace AO.Services.Emails
{
    public class MessageService : WorkflowMessageService, IMessageService
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IAffiliateService _affiliateService;
        private readonly ICustomerService _customerService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITokenizer _tokenizer;
        private readonly IAOMessageTokenProvider _aoMessageTokenProvider;
        private readonly IAddressService _addressService;

        #endregion

        #region Ctor

        public MessageService(CommonSettings commonSettings,
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
            ITokenizer tokenizer,
            IAOMessageTokenProvider aoMessageTokenProvider,
            MessagesSettings messagesSettings) : base(commonSettings, emailAccountSettings, addressService, affiliateService, customerService, emailAccountService, eventPublisher, languageService, localizationService, messageTemplateService, messageTokenProvider, orderService, productService, queuedEmailService, storeContext, storeService, tokenizer, messagesSettings)
        {
            this._commonSettings = commonSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._affiliateService = affiliateService;
            this._customerService = customerService;
            this._emailAccountService = emailAccountService;
            this._eventPublisher = eventPublisher;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._messageTemplateService = messageTemplateService;
            this._messageTokenProvider = messageTokenProvider;
            this._queuedEmailService = queuedEmailService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._tokenizer = tokenizer;
            this._aoMessageTokenProvider = aoMessageTokenProvider;
            this._addressService = addressService;
        }

        #endregion

        public async Task<IList<int>> SendAdminEmail(string email, string subject, string body)
        {
            int languageId = _languageService.GetAllLanguagesAsync().Result.Where(x => x.LanguageCulture == "en-US").FirstOrDefault().Id;
            var store = _storeContext.GetCurrentStore();
            languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.AdminMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            commonTokens.Add(new Token("Admin.Body", body));

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }

        public async Task QueueAdminEmailAsync(string emailAddressList, string subject, string body)
        {
            var defaultEmailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) ?? (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

            if (defaultEmailAccount == null)
            {
                throw new ArgumentException($"No default email account found when trying to queue mail to '{emailAddressList}' with subject '{subject}'");
            }

            List<string> emailAddresses = emailAddressList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string emailAddress in emailAddresses)
            {
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    From = defaultEmailAccount.Email,
                    FromName = defaultEmailAccount.DisplayName,
                    To = emailAddress,
                    Subject = subject,
                    Body = body,
                    EmailAccountId = defaultEmailAccount.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _queuedEmailService.InsertQueuedEmailAsync(email);
            }            
        }

        public async Task QueueInvoiceEmailAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            if (invoiceItems == null)
                throw new ArgumentNullException(nameof(invoiceItems));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            string recipientEmail = "";
            string recipientName = "";

            // First try to find customer info from billing address
            if (order.BillingAddressId > 0)
            {
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                if (billingAddress != null)
                {
                    recipientEmail = billingAddress.Email;
                    recipientName = billingAddress.FirstName;
                }
            }

            // If not found, try to find customer info from shipping address
            if (string.IsNullOrEmpty(recipientEmail) && order.ShippingAddressId > 0)
            {
                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
                if (shippingAddress != null)
                {
                    recipientEmail = shippingAddress.Email;
                    if (string.IsNullOrEmpty(recipientName))
                    {
                        recipientName = shippingAddress.FirstName;
                    }
                }
            }

            // Finally use the customer entry if still not found
            if (string.IsNullOrEmpty(recipientEmail) && string.IsNullOrEmpty(customer.Email) == false)
            {
                recipientEmail = customer.Email;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                throw new ArgumentNullException($"No email found for this customer, tried from billing address, shipping address, and customer table. Order id: {order.Id}, Invoice number: {invoice.Id}");
            }

            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var languageId = await EnsureLanguageIsActiveAsync(order.CustomerLanguageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.InvoiceMessage, store.Id);
            var messageTemplate = messageTemplates.FirstOrDefault();
            if (messageTemplate == null)
            {
                throw new ArgumentNullException(nameof(messageTemplate));
            }

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

            if (order.PaidDateUtc.HasValue)
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.PaidDateUtc.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }
            else
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.CreatedOnUtc.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }

            commonTokens.Add(new Token("Invoice.InvoiceNumber", invoice.Id));
            commonTokens.Add(new Token("Invoice.InvoiceDate", invoice.InvoiceDate.ToLocalTime().ToString("dd-MM-yyyy")));
            commonTokens.Add(new Token("Invoice.PaymentDate", invoice.InvoiceIsPaid ? "Er betalt" : invoice.PaymentDate.ToLocalTime().ToString("dd-MM-yyyy")));
            commonTokens.Add(new Token("Invoice.InvoiceTotal", invoice.InvoiceTotal));
            commonTokens.Add(new Token("Invoice.InvoiceTax", invoice.InvoiceTax));
            commonTokens.Add(new Token("Invoice.InvoiceDiscount", invoice.InvoiceDiscount));
            commonTokens.Add(new Token("Invoice.InvoiceRefundedAmount", invoice.InvoiceRefundedAmount));
            commonTokens.Add(new Token("Invoice.OrderId", invoice.OrderId));
            commonTokens.Add(new Token("Invoice.TrackingNumber", invoice.TrackingNumber));
            commonTokens.Add(new Token("Invoice.Product(s)", await _aoMessageTokenProvider.ProductListToHtmlTableAsync(order, invoice, invoiceItems, languageId), true));

            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);
            
            await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
        }

        public async Task QueueInvoiceEmailAlternativeAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems, string alternativeEmailAddress)
        {
            int languageId = 2; // 2 = Danish  

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

            if (order.PaidDateUtc.HasValue)
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.PaidDateUtc.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }
            else
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.CreatedOnUtc.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }

            commonTokens.Add(new Token("Invoice.InvoiceNumber", invoice.Id));
            commonTokens.Add(new Token("Invoice.InvoiceDate", invoice.InvoiceDate.ToLocalTime().ToString("dd-MM-yyyy")));
            commonTokens.Add(new Token("Invoice.PaymentDate", invoice.InvoiceIsPaid ? "Er betalt" : invoice.PaymentDate.ToLocalTime().ToString("dd-MM-yyyy")));
            commonTokens.Add(new Token("Invoice.InvoiceTotal", invoice.InvoiceTotal));
            commonTokens.Add(new Token("Invoice.InvoiceTax", invoice.InvoiceTax));
            commonTokens.Add(new Token("Invoice.InvoiceDiscount", invoice.InvoiceDiscount));
            commonTokens.Add(new Token("Invoice.InvoiceRefundedAmount", invoice.InvoiceRefundedAmount));
            commonTokens.Add(new Token("Invoice.OrderId", invoice.OrderId));
            commonTokens.Add(new Token("Invoice.TrackingNumber", invoice.TrackingNumber));
            commonTokens.Add(new Token("Invoice.Product(s)", await _aoMessageTokenProvider.ProductListToHtmlTableAsync(order, invoice, invoiceItems, languageId), true));

            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.InvoiceMessage, store.Id);
            var messageTemplate = messageTemplates.FirstOrDefault();
            if (messageTemplate == null)
            {
                throw new ArgumentNullException(nameof(messageTemplate));
            }

            var tokens = new List<Token>(commonTokens);            
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);
                                                                   
            await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, alternativeEmailAddress, "Admin");
        }

        public async Task QueueCreditNoteEmailAlternativeAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems, string alternativeEmailAddress)
        {
            int languageId = 2; // 2 = Danish  

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

            if (order.PaidDateUtc.HasValue)
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.PaidDateUtc.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }
            else
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.CreatedOnUtc.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }

            commonTokens.Add(new Token("Invoice.InvoiceNumber", invoice.Id));
            commonTokens.Add(new Token("Invoice.InvoiceDate", invoice.InvoiceDate.ToLocalTime().ToString("dd-MM-yyyy")));
            commonTokens.Add(new Token("Invoice.PaymentDate", invoice.InvoiceIsPaid ? "Er betalt" : invoice.PaymentDate.ToLocalTime().ToString("dd-MM-yyyy")));
            commonTokens.Add(new Token("Invoice.InvoiceTotal", invoice.InvoiceTotal));
            commonTokens.Add(new Token("Invoice.InvoiceTax", invoice.InvoiceTax));
            commonTokens.Add(new Token("Invoice.InvoiceDiscount", invoice.InvoiceDiscount));
            commonTokens.Add(new Token("Invoice.InvoiceRefundedAmount", invoice.InvoiceRefundedAmount));
            commonTokens.Add(new Token("Invoice.OrderId", invoice.OrderId));
            commonTokens.Add(new Token("Invoice.TrackingNumber", invoice.TrackingNumber));
            commonTokens.Add(new Token("Invoice.Product(s)", await _aoMessageTokenProvider.ProductListToHtmlTableAsync(order, invoice, invoiceItems, languageId), true));

            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CreditNoteMessage, store.Id);
            var messageTemplate = messageTemplates.FirstOrDefault();
            if (messageTemplate == null)
            {
                throw new ArgumentNullException(nameof(messageTemplate));
            }

            var tokens = new List<Token>(commonTokens);
           
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);

            await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, alternativeEmailAddress, "Admin");
        }


        public async Task QueueCreditNoteEmailAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            if (invoiceItems == null)
                throw new ArgumentNullException(nameof(invoiceItems));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            string recipientEmail = "";
            string recipientName = "";

            // First try to find customer info from billing address
            if (order.BillingAddressId > 0)
            {
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                if (billingAddress != null)
                {
                    recipientEmail = billingAddress.Email;
                    recipientName = billingAddress.FirstName;
                }
            }

            // If not found, try to find customer info from shipping address
            if (string.IsNullOrEmpty(recipientEmail) && order.ShippingAddressId > 0)
            {
                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
                if (shippingAddress != null)
                {
                    recipientEmail = shippingAddress.Email;
                    if (string.IsNullOrEmpty(recipientName))
                    {
                        recipientName = shippingAddress.FirstName;
                    }
                }
            }

            // Finally use the customer entry if still not found
            if (string.IsNullOrEmpty(recipientEmail) && string.IsNullOrEmpty(customer.Email) == false)
            {
                recipientEmail = customer.Email;
            }

            if (string.IsNullOrEmpty(recipientEmail))
            {
                throw new ArgumentNullException($"No email found for this customer, tried from billing address, shipping address, and customer table. Order id: {order.Id}, Invoice number: {invoice.Id}");
            }

            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
            var languageId = await EnsureLanguageIsActiveAsync(order.CustomerLanguageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CreditNoteMessage, store.Id);
            var messageTemplate = messageTemplates.FirstOrDefault();
            if (messageTemplate == null)
            {
                throw new ArgumentNullException(nameof(messageTemplate));
            }

            //tokens
            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddOrderTokensAsync(commonTokens, order, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, order.CustomerId);

            if (order.PaidDateUtc.HasValue)
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.PaidDateUtc.Value.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }
            else
            {
                commonTokens.Add(new Token("Invoice.OrderPaidDate", order.CreatedOnUtc.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss")));
            }

            commonTokens.Add(new Token("Invoice.InvoiceNumber", invoice.Id));
            commonTokens.Add(new Token("Invoice.InvoiceDate", invoice.InvoiceDate));
            commonTokens.Add(new Token("Invoice.PaymentDate", invoice.PaymentDate));
            commonTokens.Add(new Token("Invoice.InvoiceTotal", invoice.InvoiceTotal));
            commonTokens.Add(new Token("Invoice.InvoiceTax", invoice.InvoiceTax));
            commonTokens.Add(new Token("Invoice.InvoiceDiscount", invoice.InvoiceDiscount));
            commonTokens.Add(new Token("Invoice.InvoiceRefundedAmount", invoice.InvoiceRefundedAmount));
            commonTokens.Add(new Token("Invoice.OrderId", invoice.OrderId));
            commonTokens.Add(new Token("Invoice.TrackingNumber", invoice.TrackingNumber));
            commonTokens.Add(new Token("Invoice.Product(s)", await _aoMessageTokenProvider.ProductListToHtmlTableAsync(order, invoice, invoiceItems, languageId), true));

            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, recipientEmail, recipientName);
        }
    }
}
