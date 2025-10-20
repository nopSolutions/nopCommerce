using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.RFQ.Services;

/// <summary>
/// Represents RFQ message service
/// </summary>
public class RfqMessageService : WorkflowMessageService
{
    #region Fields

    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IWebHelper _webHelper;
    private readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public RfqMessageService(CommonSettings commonSettings,
        EmailAccountSettings emailAccountSettings,
        INopUrlHelper nopUrlHelper,
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
        IWebHelper webHelper,
        LocalizationSettings localizationSettings,
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
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    private async Task AddAdminRequestQuoteTokensAsync(IList<Token> tokens, RequestQuote requestQuote, Language language)
    {
        tokens.Add(new Token("RequestQuote.Id", requestQuote.Id));
        tokens.Add(new Token("RequestQuote.CreatedOn", requestQuote.CreatedOnUtc.ToString("D", new CultureInfo(language.LanguageCulture))));
        tokens.Add(new Token("RequestQuote.URL", $"{_webHelper.GetStoreLocation()}Admin/RfqAdmin/AdminRequest/{requestQuote.Id}"));

        //event notification
        await _eventPublisher.EntityTokensAddedAsync(requestQuote, tokens);
    }

    private async Task AddCustomerQuoteTokensAsync(IList<Token> tokens, Quote quote, Language language)
    {
        tokens.Add(new Token("Quote.Id", quote.Id));
        tokens.Add(new Token("Quote.CreatedOn", quote.CreatedOnUtc.ToString("D", new CultureInfo(language.LanguageCulture))));
        var expirationDate = quote.ExpirationDateUtc?.ToString("D", new CultureInfo(language.LanguageCulture));
        tokens.Add(new Token("Quote.ExpirationOn", expirationDate));
        tokens.Add(new Token("Quote.ExpirationOnIsSet", !string.IsNullOrWhiteSpace(expirationDate)));

        var url = _nopUrlHelper.RouteUrl(RfqDefaults.CustomerQuoteRouteName, new { quoteId = quote.Id }, _webHelper.GetCurrentRequestProtocol());
        tokens.Add(new Token("Quote.URL", url));

        //event notification
        await _eventPublisher.EntityTokensAddedAsync(quote, tokens);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends "Customer sent new request quote" message 
    /// </summary>
    /// <param name="requestQuote">Request quote</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public async Task<IList<int>> CustomerSentNewRequestQuoteAsync(RequestQuote requestQuote)
    {
        ArgumentNullException.ThrowIfNull(requestQuote);

        var store = await _storeContext.GetCurrentStoreAsync();

        var messageTemplates = await GetActiveMessageTemplatesAsync(RfqDefaults.CUSTOMER_SENT_NEW_REQUEST_QUOTE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        var languageId = await EnsureLanguageIsActiveAsync(_localizationSettings.DefaultAdminLanguageId, store.Id);
        var language = await _languageService.GetLanguageByIdAsync(languageId);
        var customer = await _customerService.GetCustomerByIdAsync(requestQuote.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(tokens, requestQuote.CustomerId);
            await AddAdminRequestQuoteTokensAsync(tokens, requestQuote, language);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);

            var replyToEmail = messageTemplate.AllowDirectReply ? customer.Email : "";
            var replyToName = messageTemplate.AllowDirectReply ? $"{customer.FirstName} {customer.LastName}" : "";

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName, replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
    }

    /// <summary>
    /// Sends "Administrator sent new quote" message 
    /// </summary>
    /// <param name="quote">The quote</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public async Task<IList<int>> AdminSentNewQuoteAsync(Quote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        var store = await _storeContext.GetCurrentStoreAsync();

        var messageTemplates = await GetActiveMessageTemplatesAsync(RfqDefaults.ADMIN_SENT_NEW_QUOTE, store.Id);
        if (!messageTemplates.Any())
            return new List<int>();

        var customer = await _customerService.GetCustomerByIdAsync(quote.CustomerId);
        var languageId = await EnsureLanguageIsActiveAsync(customer.LanguageId ?? 0, store.Id);
        var language = await _languageService.GetLanguageByIdAsync(languageId);


        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            //tokens
            var tokens = new List<Token>();
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);
            await _messageTokenProvider.AddCustomerTokensAsync(tokens, quote.CustomerId);
            await AddCustomerQuoteTokensAsync(tokens, quote, language);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, customer.Email, $"{customer.FirstName} {customer.LastName}");
        }).ToListAsync();
    }

    #endregion
}
