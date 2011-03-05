using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Catalog;
using Nop.Core.Html;

namespace Nop.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ITokenizer _tokenizer;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly INewsLetterSubscriptionService _newLetterSubscriptionService;

        private readonly StoreInformationSettings _storeSettings;
        private readonly MessageTemplatesSettings _templatesSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messageTemplateService">MessageTemplateService</param>
        /// <param name="queuedEmailService">QueuedEmailService</param>
        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, ILanguageService languageService,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            ITokenizer tokenizer, IEmailAccountService emailAccountService,
            IPriceFormatter priceFormatter, INewsLetterSubscriptionService newsLetterSubscriptionService,
            StoreInformationSettings storeSettings, MessageTemplatesSettings templatesSettings)
        {
            _messageTemplateService = messageTemplateService;
            _queuedEmailService = queuedEmailService;
            _languageService = languageService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _tokenizer = tokenizer;
            _emailAccountService = emailAccountService;
            _priceFormatter = priceFormatter;
            _newLetterSubscriptionService = newsLetterSubscriptionService;

            _storeSettings = storeSettings;
            _templatesSettings = templatesSettings;
        }

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var messageTemplate = GetLocalizedActiveMessageTemplate(9, languageId); //string templateName = "OrderPlaced.StoreOwnerNotification";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            return SendNotification(messageTemplate, languageId, orderTokens);

        }

        /// <summary>
        /// Sends an order placed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderPlacedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var messageTemplate = GetLocalizedActiveMessageTemplate(18, languageId); //string templateName = "OrderPlaced.CustomerNotification";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            return SendNotification(messageTemplate, languageId, orderTokens, "Not implemented", "Not Implemented");

        }

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderCompletedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var messageTemplate = GetLocalizedActiveMessageTemplate(19, languageId); //string templateName = "OrderCompleted.CustomerNotification";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(order, languageId);

            return SendNotification(messageTemplate, languageId, orderTokens, "Not implemented", "Not Implemented");
        }

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            var messageTemplate = GetLocalizedActiveMessageTemplate(29, languageId); //string templateName = "GiftCard.Notification";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(giftCard);

            return SendNotification(messageTemplate, languageId, orderTokens, giftCard.RecipientEmail, giftCard.RecipientName);
        }

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var messageTemplate = GetLocalizedActiveMessageTemplate(27, languageId); //string templateName = "NewsLetterSubscription.ActivationMessage";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

            return SendNotification(messageTemplate, languageId, orderTokens, subscription.Email, String.Empty);
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription,
            int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            var messageTemplate = GetLocalizedActiveMessageTemplate(28, languageId); //string templateName = "NewsLetterSubscription.DeactivationMessage";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(subscription);

            return SendNotification(messageTemplate, languageId, orderTokens, subscription.Email, String.Empty);
        }

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendProductReviewNotificationMessage(ProductReview productReview,
            int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            var messageTemplate = GetLocalizedActiveMessageTemplate(26, languageId); //string templateName = "Product.ProductReview";
            if (messageTemplate == null)
                return 0;

            var orderTokens = GenerateTokens(productReview);

            return SendNotification(messageTemplate, languageId, orderTokens);

        }


        private int SendNotification(MessageTemplate messageTemplate, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress = null, string toName = null)
        {
            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized((mt) => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized((mt) => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized((mt) => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens);
            var bodyReplaced = _tokenizer.Replace(body, tokens);

            var emailAccount = GetEmalAccountOfMessageTemplate(messageTemplate, languageId);

            var email = new QueuedEmail()
            {
                Priority = 5,
                From = emailAccount.Email,
                FromName = emailAccount.DisplayName,
                To = toEmailAddress ?? emailAccount.Email,
                ToName = toName ?? emailAccount.DisplayName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                CreatedOn = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;

        }


        private void AddStoreTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("Store.Name", _storeSettings.StoreName));
            tokens.Add(new Token("Store.URL", _storeSettings.StoreUrl));
            tokens.Add(new Token("Store.Email", "Not implemented"));
        }

        private void AddOrderTokens(IList<Token> tokens, Order order, int languageId)
        {
            tokens.Add(new Token("Order.OrderNumber", order.Id.ToString()));

            tokens.Add(new Token("Order.Product(s)", ProductListToHtmlTable(order, languageId)));
            tokens.Add(new Token("Order.CreatedOn", FormatUTCDateTimeForCustomer(order.CreatedOnUtc, order.Customer, languageId)));

            tokens.Add(new Token("Order.OrderURLForCustomer", string.Format("{0}orderdetails.aspx?orderid={1}", _storeSettings.StoreUrl, order.Id)));

        }

        private void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard)
        {
            tokens.Add(new Token("GiftCard.SenderName", giftCard.SenderName));
            tokens.Add(new Token("GiftCard.SenderEmail", giftCard.SenderEmail));
            tokens.Add(new Token("GiftCard.RecipientName", giftCard.RecipientName));
            tokens.Add(new Token("GiftCard.RecipientEmail", giftCard.RecipientEmail));
            tokens.Add(new Token("GiftCard.Amount", _priceFormatter.FormatPrice(giftCard.Amount, true, false)));
            tokens.Add(new Token("GiftCard.CouponCode", giftCard.GiftCardCouponCode));

            var giftCardMesage = String.IsNullOrWhiteSpace(giftCard.Message) ? giftCard.Message
                : HtmlHelper.FormatText(giftCard.Message, false, true, false, false, false, false);

            tokens.Add(new Token("GiftCard.Message", giftCardMesage));
        }

        private void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", "Not implemented"));
            tokens.Add(new Token("Customer.Username", "Not implemented"));
            tokens.Add(new Token("Customer.FullName", "Not implemented"));
            tokens.Add(new Token("Customer.VatNumber", customer.VatNumber));
            tokens.Add(new Token("Customer.VatNumberStatus", customer.VatNumberStatus.ToString()));
            tokens.Add(new Token("Customer.PasswordRecoveryURL", "Not implemented"));
            tokens.Add(new Token("Customer.AccountActivationURL", "Not implemented"));
        }

        private void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));

            const string urlFormat = "{0}newslettersubscriptionactivation.aspx?t={1}&active={2}";

            var activationUrl = String.Format(urlFormat, _storeSettings.StoreUrl, subscription.NewsLetterSubscriptionGuid, 1);
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl));

            var deActivationUrl = String.Format("urlFormat", _storeSettings.StoreUrl, subscription.NewsLetterSubscriptionGuid, 0);
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deActivationUrl));
        }

        private void AddTokens(IList<Token> tokens, ProductReview productReview)
        {
            tokens.Add(new Token("ProductReview.ProductName", productReview.Product.Name));
        }

        private IEnumerable<Token> GenerateTokens(Order order, int languageId)
        {
            var tokens = new List<Token>();
            AddStoreTokens(tokens);

            AddOrderTokens(tokens, order, languageId);

            return tokens;
        }

        private IEnumerable<Token> GenerateTokens(GiftCard giftCard)
        {
            var tokens = new List<Token>();

            AddStoreTokens(tokens);
            AddGiftCardTokens(tokens, giftCard);

            return tokens;
        }

        private IEnumerable<Token> GenerateTokens(NewsLetterSubscription subscription)
        {
            var tokens = new List<Token>();

            AddStoreTokens(tokens);
            AddNewsLetterSubscriptionTokens(tokens, subscription);

            var customer = _newLetterSubscriptionService.GetNewsLetterSubscriptionCustomer(subscription);
            if (customer != null)
            {
                AddCustomerTokens(tokens, customer);
            }

            return tokens;
        }

        private IEnumerable<Token> GenerateTokens(ProductReview productReview)
        {
            var tokens = new List<Token>();

            AddStoreTokens(tokens);
            AddTokens(tokens, productReview);

            return tokens;
        }

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>HTML table of products</returns>
        private string ProductListToHtmlTable(Order order, int languageId)
        {
            var language = GetCustomOrContextLanguage(languageId);
            languageId = language.Id;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine(string.Format("<tr style=\"background-color:{0};text-align:center;\">", _templatesSettings.Color1));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Order.ProductsGrid.Name", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Order.ProductsGrid.Price", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Order.ProductsGrid.Quantity", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Order.ProductsGrid.Total", languageId)));
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;

        }
        

        private string FormatUTCDateTimeForCustomer(DateTime dt, Customer customer, int languageId)
        {
            var language = _languageService.GetLanguageById(languageId);
            if (language != null && !String.IsNullOrEmpty(language.LanguageCulture))
            {
                DateTime custTimeZoneDt = _dateTimeHelper.ConvertToUserTime(dt, TimeZoneInfo.Utc, _dateTimeHelper.GetCustomerTimeZone(customer));
                return custTimeZoneDt.ToString("D", new CultureInfo(language.LanguageCulture));
            }
            else
            {
                return dt.ToString("D");
            }

        }

        private MessageTemplate GetLocalizedActiveMessageTemplate(int messageTemplateId, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate == null)
                return null;

            var isActive = messageTemplate.GetLocalized((mt) => mt.IsActive, languageId);
            if (!isActive)
                return null;

            return messageTemplate;
        }

        private EmailAccount GetEmalAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            return _emailAccountService.GetEmailAccountById(emailAccounId);

        }

        private Language GetCustomOrContextLanguage(int languageId)
        {
            return _languageService.GetLanguageById(languageId) ?? EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage;
        }


        public int SendQuantityBelowStoreOwnerNotification(ProductVariant productVariant, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderShippedCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderDeliveredCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewCustomerNotificationMessage(Customer customer, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendNewVATSubmittedStoreOwnerNotification(Customer customer, string vatName, string vatAddress, int languageId)
        {
            throw new NotImplementedException();
        }

        public int SendProductEmailAFriendMessage(Customer customer, int languageId, Product product, string friendsEmail, string personalMessage)
        {
            throw new NotImplementedException();
        }
    }
}
