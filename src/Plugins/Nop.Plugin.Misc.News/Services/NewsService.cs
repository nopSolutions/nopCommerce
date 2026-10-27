using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Data;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.News.Services;

/// <summary>
/// News service
/// </summary>
public class NewsService
{
    #region Fields

    private readonly EmailAccountSettings _emailAccountSettings;
    private readonly ICustomerService _customerService;
    private readonly IEmailAccountService _emailAccountService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMessageTokenProvider _messageTokenProvider;
    private readonly IRepository<NewsComment> _newsCommentRepository;
    private readonly IRepository<NewsItem> _newsItemRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly MessagesSettings _messagesSettings;

    #endregion

    #region Ctor

    public NewsService(EmailAccountSettings emailAccountSettings,
        ICustomerService customerService,
        IEmailAccountService emailAccountService,
        IEventPublisher eventPublisher,
        IMessageTokenProvider messageTokenProvider,
        IRepository<NewsComment> newsCommentRepository,
        IRepository<NewsItem> newsItemRepository,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IWorkflowMessageService workflowMessageService,
        MessagesSettings messagesSettings)
    {
        _emailAccountSettings = emailAccountSettings;
        _customerService = customerService;
        _emailAccountService = emailAccountService;
        _eventPublisher = eventPublisher;
        _messageTokenProvider = messageTokenProvider;
        _newsCommentRepository = newsCommentRepository;
        _newsItemRepository = newsItemRepository;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _workflowMessageService = workflowMessageService;
        _messagesSettings = messagesSettings;

    }

    #endregion

    #region Utilities

    /// <summary>
    /// Add news comment tokens
    /// </summary>
    /// <param name="tokens">List of already added tokens</param>
    /// <param name="newsComment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private async Task AddNewsCommentTokensAsync(List<Token> tokens, NewsComment newsComment)
    {
        var newsItem = await GetNewsByIdAsync(newsComment.NewsItemId);

        tokens.Add(new("NewsComment.NewsTitle", newsItem.Title));

        //event notification
        await _eventPublisher.EntityTokensAddedAsync(newsComment, tokens);
    }

    /// <summary>
    /// Get email and name to send email for store owner
    /// </summary>
    /// <param name="messageTemplateEmailAccount">Message template email account</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email address and name to send email fore store owner
    /// </returns>
    private async Task<(string email, string name)> GetStoreOwnerNameAndEmailAsync(EmailAccount messageTemplateEmailAccount)
    {
        var storeOwnerEmailAccount = _messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails
            ? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
            : null;
        storeOwnerEmailAccount ??= messageTemplateEmailAccount;

        return (storeOwnerEmailAccount.Email, storeOwnerEmailAccount.DisplayName);
    }

    /// <summary>
    /// Get email and name to set ReplyTo property of email from customer 
    /// </summary>
    /// <param name="messageTemplate">Message template</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the email address and name when reply to email
    /// </returns>
    private async Task<(string email, string name)> GetCustomerReplyToNameAndEmailAsync(MessageTemplate messageTemplate, Customer customer)
    {
        if (!messageTemplate.AllowDirectReply)
            return (null, null);

        var replyToEmail = await _customerService.IsGuestAsync(customer)
            ? string.Empty
            : customer.Email;

        var replyToName = await _customerService.IsGuestAsync(customer)
            ? string.Empty
            : await _customerService.GetCustomerFullNameAsync(customer);

        return (replyToEmail, replyToName);
    }

    #endregion

    #region Methods

    #region News

    /// <summary>
    /// Deletes a news
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteNewsAsync(NewsItem newsItem)
    {
        await _newsItemRepository.DeleteAsync(newsItem);
    }

    /// <summary>
    /// Gets a news
    /// </summary>
    /// <param name="newsId">The news identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news
    /// </returns>
    public async Task<NewsItem> GetNewsByIdAsync(int newsId)
    {
        return await _newsItemRepository.GetByIdAsync(newsId, cache => default);
    }

    /// <summary>
    /// Get news by identifiers
    /// </summary>
    /// <param name="newsIds">News identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news
    /// </returns>
    public async Task<IList<NewsItem>> GetNewsByIdsAsync(int[] newsIds)
    {
        return await _newsItemRepository.GetByIdsAsync(newsIds);
    }

    /// <summary>
    /// Gets all news
    /// </summary>
    /// <param name="languageId">Language identifier; 0 if you want to get all records</param>
    /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="title">Filter by news item title</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news items
    /// </returns>
    public async Task<IPagedList<NewsItem>> GetAllNewsAsync(int languageId = 0, int storeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, string title = null)
    {
        var news = await _newsItemRepository.GetAllPagedAsync(async query =>
        {
            if (languageId > 0)
                query = query.Where(n => languageId == n.LanguageId);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(n => n.Title.Contains(title));

            if (!showHidden || storeId > 0)
            {
                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);
            }

            if (!showHidden)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(n => n.Published);
                query = query.Where(n => !n.StartDateUtc.HasValue || n.StartDateUtc <= utcNow);
                query = query.Where(n => !n.EndDateUtc.HasValue || n.EndDateUtc >= utcNow);
            }

            return query.OrderByDescending(n => n.StartDateUtc ?? n.CreatedOnUtc);
        }, pageIndex, pageSize);

        return news;
    }

    /// <summary>
    /// Inserts a news item
    /// </summary>
    /// <param name="news">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertNewsAsync(NewsItem news)
    {
        await _newsItemRepository.InsertAsync(news);
    }

    /// <summary>
    /// Updates the news item
    /// </summary>
    /// <param name="news">News item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateNewsAsync(NewsItem news)
    {
        await _newsItemRepository.UpdateAsync(news);
    }

    /// <summary>
    /// Get a value indicating whether a news item is available now (availability dates)
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <param name="dateTime">Datetime to check; pass null to use current date</param>
    /// <returns>Result</returns>
    public bool IsNewsAvailable(NewsItem newsItem, DateTime? dateTime = null)
    {
        ArgumentNullException.ThrowIfNull(newsItem);

        if (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= dateTime)
            return false;

        if (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= dateTime)
            return false;

        return true;
    }

    #endregion

    #region News comments

    /// <summary>
    /// Gets all comments
    /// </summary>
    /// <param name="customerId">Customer identifier; 0 to load all records</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="newsItemId">News item ID; 0 or null to load all records</param>
    /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
    /// <param name="fromUtc">Item creation from; null to load all records</param>
    /// <param name="toUtc">Item creation to; null to load all records</param>
    /// <param name="commentText">Search comment text; null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the comments
    /// </returns>
    public async Task<IList<NewsComment>> GetAllCommentsAsync(int customerId = 0, int storeId = 0, int? newsItemId = null,
        bool? approved = null, DateTime? fromUtc = null, DateTime? toUtc = null, string commentText = null)
    {
        return await _newsCommentRepository.GetAllAsync(query =>
        {
            if (approved.HasValue)
                query = query.Where(comment => comment.IsApproved == approved);

            if (newsItemId > 0)
                query = query.Where(comment => comment.NewsItemId == newsItemId);

            if (customerId > 0)
                query = query.Where(comment => comment.CustomerId == customerId);

            if (storeId > 0)
                query = query.Where(comment => comment.StoreId == storeId);

            if (fromUtc.HasValue)
                query = query.Where(comment => fromUtc.Value <= comment.CreatedOnUtc);

            if (toUtc.HasValue)
                query = query.Where(comment => toUtc.Value >= comment.CreatedOnUtc);

            if (!string.IsNullOrEmpty(commentText))
                query = query.Where(c => c.CommentText.Contains(commentText) || c.CommentTitle.Contains(commentText));

            query = query.OrderBy(nc => nc.CreatedOnUtc);

            return query;
        });
    }

    /// <summary>
    /// Gets a news comment
    /// </summary>
    /// <param name="newsCommentId">News comment identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment
    /// </returns>
    public async Task<NewsComment> GetNewsCommentByIdAsync(int newsCommentId)
    {
        return await _newsCommentRepository.GetByIdAsync(newsCommentId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Get news comments by identifiers
    /// </summary>
    /// <param name="commentIds">News comment identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comments
    /// </returns>
    public async Task<IList<NewsComment>> GetNewsCommentsByIdsAsync(int[] commentIds)
    {
        return await _newsCommentRepository.GetByIdsAsync(commentIds);
    }

    /// <summary>
    /// Get the count of news comments
    /// </summary>
    /// <param name="newsItem">News item</param>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="isApproved">A value indicating whether to count only approved or not approved comments; pass null to get number of all comments</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of news comments
    /// </returns>
    public async Task<int> GetNewsCommentsCountAsync(NewsItem newsItem, int storeId = 0, bool? isApproved = null)
    {
        var query = _newsCommentRepository.Table.Where(comment => comment.NewsItemId == newsItem.Id);

        if (storeId > 0)
            query = query.Where(comment => comment.StoreId == storeId);

        if (isApproved.HasValue)
            query = query.Where(comment => comment.IsApproved == isApproved.Value);

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NewsDefaults.NewsCommentsNumberCacheKey, newsItem, storeId, isApproved);

        return await _staticCacheManager.GetAsync(cacheKey, async () => await query.CountAsync());
    }

    /// <summary>
    /// Deletes a news comment
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteNewsCommentAsync(NewsComment newsComment)
    {
        await _newsCommentRepository.DeleteAsync(newsComment);
    }

    /// <summary>
    /// Deletes a news comments
    /// </summary>
    /// <param name="newsComments">News comments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteNewsCommentsAsync(IList<NewsComment> newsComments)
    {
        ArgumentNullException.ThrowIfNull(newsComments);

        foreach (var newsComment in newsComments)
            await DeleteNewsCommentAsync(newsComment);
    }

    /// <summary>
    /// Inserts a news comment
    /// </summary>
    /// <param name="comment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertNewsCommentAsync(NewsComment comment)
    {
        await _newsCommentRepository.InsertAsync(comment);
    }

    /// <summary>
    /// Update a news comment
    /// </summary>
    /// <param name="comment">News comment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateNewsCommentAsync(NewsComment comment)
    {
        await _newsCommentRepository.UpdateAsync(comment);
    }

    #endregion

    #region Messages

    /// <summary>
    /// Sends a news comment notification message to a store owner
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <param name="languageId">Message language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the queued email identifier
    /// </returns>
    public async Task<IList<int>> SendNewsCommentStoreOwnerNotificationMessageAsync(NewsComment newsComment, int languageId)
    {
        ArgumentNullException.ThrowIfNull(newsComment);

        var store = await _storeContext.GetCurrentStoreAsync();
        languageId = await _workflowMessageService.EnsureLanguageIsActiveAsync(languageId, store.Id);

        var messageTemplates = await _workflowMessageService.GetActiveMessageTemplatesAsync(NewsDefaults.NewsCommentStoreOwnerNotification, store.Id);
        if (!messageTemplates.Any())
            return [];

        var customer = await _customerService.GetCustomerByIdAsync(newsComment.CustomerId);

        //tokens
        var commonTokens = new List<Token>();
        await AddNewsCommentTokensAsync(commonTokens, newsComment);
        await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, newsComment.CustomerId);

        return await messageTemplates.SelectAwait(async messageTemplate =>
        {
            //email account
            var emailAccount = await _workflowMessageService.GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            var tokens = new List<Token>(commonTokens);
            await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount, languageId);

            //event notification
            await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var (toEmail, toName) = await GetStoreOwnerNameAndEmailAsync(emailAccount);
            var (replyToEmail, replyToName) = await GetCustomerReplyToNameAndEmailAsync(messageTemplate, customer);

            return await _workflowMessageService.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                replyToEmailAddress: replyToEmail, replyToName: replyToName);
        }).ToListAsync();
    }

    #endregion

    #endregion
}