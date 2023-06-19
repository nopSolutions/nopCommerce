using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.Brevo.Services
{
    /// <summary>
    /// Represents overridden workflow message service
    /// </summary>
    public class BrevoMessageService : WorkflowMessageService
    {
        #region Fields

        protected readonly BrevoManager _brevoEmailManager;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public BrevoMessageService(BrevoManager brevoEmailManager,
            CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IOrderService orderService,
            IProductService productService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IQueuedEmailService queuedEmailService,
            ITokenizer tokenizer,
            MessagesSettings messagesSettings)
            : base(commonSettings,
                emailAccountSettings,
                addressService,
                affiliateService,
                customerService,
                emailAccountService,
                eventPublisher,
                languageService,
                localizationService,
                messageTemplateService,
                messageTokenProvider,
                orderService,
                productService,
                queuedEmailService,
                storeContext,
                storeService,
                tokenizer,
                messagesSettings)
        {
            _brevoEmailManager = brevoEmailManager;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Send SMS notification by message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task SendSmsNotificationAsync(MessageTemplate messageTemplate, IEnumerable<Token> tokens)
        {
            //get plugin settings
            var storeId = (int?)tokens.FirstOrDefault(token => token.Key == "Store.Id")?.Value;
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId ?? 0);

            //ensure SMS notifications enabled
            if (!brevoSettings.UseSmsNotifications)
                return;

            //whether to send SMS by the passed message template
            var sendSmsForThisMessageTemplate = await _genericAttributeService
                .GetAttributeAsync<bool>(messageTemplate, BrevoDefaults.UseSmsAttribute);
            if (!sendSmsForThisMessageTemplate)
                return;

            //get text with replaced tokens
            var text = await _genericAttributeService.GetAttributeAsync<string>(messageTemplate, BrevoDefaults.SmsTextAttribute);
            if (!string.IsNullOrEmpty(text))
                text = _tokenizer.Replace(text, tokens, false);

            //get phone number send to
            var phoneNumberTo = string.Empty;
            var phoneType = await _genericAttributeService.GetAttributeAsync<int>(messageTemplate, BrevoDefaults.PhoneTypeAttribute);
            switch (phoneType)
            {
                case 0:
                    //merchant phone
                    phoneNumberTo = brevoSettings.StoreOwnerPhoneNumber;
                    break;
                case 1:
                    //customer phone
                    phoneNumberTo = tokens.FirstOrDefault(token => token.Key == "Customer.PhoneNumber")?.Value?.ToString();
                    break;
                case 2:
                    //order billing address phone
                    phoneNumberTo = tokens.FirstOrDefault(token => token.Key == "Order.BillingPhoneNumber")?.Value?.ToString();
                    break;
            }

            //try to send SMS
            await _brevoEmailManager.SendSMSAsync(phoneNumberTo, brevoSettings.SmsSenderName, text);
        }

        /// <summary>
        /// Send email notification by message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        protected async Task<int?> SendEmailNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            //get plugin settings
            var storeId = (int?)tokens.FirstOrDefault(token => token.Key == "Store.Id")?.Value;
            var brevoSettings = await _settingService.LoadSettingAsync<BrevoSettings>(storeId ?? 0);

            //ensure email notifications enabled
            if (!brevoSettings.UseSmtp)
                return null;

            //whether to send email by the passed message template
            var templateId = await _genericAttributeService.GetAttributeAsync<int?>(messageTemplate, BrevoDefaults.TemplateIdAttribute);
            var sendEmailForThisMessageTemplate = templateId.HasValue;
            if (!sendEmailForThisMessageTemplate)
                return null;

            //get the specified email account from settings
            emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(brevoSettings.EmailAccountId) ?? emailAccount;

            //get an email from the template
            var email = await _brevoEmailManager.GetQueuedEmailFromTemplateAsync(templateId.Value)
                ?? throw new NopException($"There is no template with id {templateId}");

            //replace body and subject tokens
            if (string.IsNullOrEmpty(subject))
                subject = email.Subject;
            if (!string.IsNullOrEmpty(subject))
                email.Subject = _tokenizer.Replace(subject, tokens, false);
            if (!string.IsNullOrEmpty(email.Body))
                email.Body = _tokenizer.Replace(email.Body, tokens, true);

            //set email parameters
            email.Priority = QueuedEmailPriority.High;
            email.From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : email.From;
            email.FromName = !string.IsNullOrEmpty(fromName) ? fromName : email.FromName;
            email.To = toEmailAddress;
            email.ToName = CommonHelper.EnsureMaximumLength(toName, 300);
            email.ReplyTo = replyToEmailAddress;
            email.ReplyToName = replyToName;
            email.CC = string.Empty;
            email.Bcc = messageTemplate.BccEmailAddresses;
            email.AttachmentFilePath = attachmentFilePath;
            email.AttachmentFileName = attachmentFileName;
            email.AttachedDownloadId = messageTemplate.AttachedDownloadId;
            email.CreatedOnUtc = DateTime.UtcNow;
            email.EmailAccountId = emailAccount.Id;
            email.DontSendBeforeDateUtc = messageTemplate.DelayBeforeSend.HasValue
                ? (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
                : null;

            //queue email
            await _queuedEmailService.InsertQueuedEmailAsync(email);
            return email.Id;
        }

        #endregion

        #region Methods

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public override async Task<int> SendNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName, string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null, string fromEmail = null, string fromName = null,
            string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            //try to send SMS notification
            await SendSmsNotificationAsync(messageTemplate, tokens);

            //try to send email notification
            var emailId = await SendEmailNotificationAsync(messageTemplate, emailAccount, tokens,
                toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                replyToEmailAddress, replyToName, fromEmail, fromName, subject);
            if (emailId.HasValue)
                return emailId.Value;

            //send base notification
            return await base.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens,
                toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        }

        #endregion
    }
}