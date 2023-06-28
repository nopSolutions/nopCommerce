using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Common;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the private message model factory
    /// </summary>
    public partial class PrivateMessagesModelFactory : IPrivateMessagesModelFactory
    {
        #region Fields

        protected readonly CustomerSettings _customerSettings;
        protected readonly ForumSettings _forumSettings;
        protected readonly ICustomerService _customerService;
        protected readonly IDateTimeHelper _dateTimeHelper;
        protected readonly IForumService _forumService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PrivateMessagesModelFactory(CustomerSettings customerSettings,
            ForumSettings forumSettings,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IForumService forumService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _customerSettings = customerSettings;
            _forumSettings = forumSettings;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _forumService = forumService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private message index model
        /// </returns>
        public virtual Task<PrivateMessageIndexModel> PreparePrivateMessageIndexModelAsync(int? page, string tab)
        {
            var inboxPage = 0;
            var sentItemsPage = 0;
            var sentItemsTabSelected = false;

            switch (tab)
            {
                case "inbox":
                    if (page.HasValue)
                    {
                        inboxPage = page.Value;
                    }

                    break;
                case "sent":
                    if (page.HasValue)
                    {
                        sentItemsPage = page.Value;
                    }

                    sentItemsTabSelected = true;

                    break;
                default:
                    break;
            }

            var model = new PrivateMessageIndexModel
            {
                InboxPage = inboxPage,
                SentItemsPage = sentItemsPage,
                SentItemsTabSelected = sentItemsTabSelected
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the inbox model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private message list model
        /// </returns>
        public virtual async Task<PrivateMessageListModel> PrepareInboxModelAsync(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var list = await _forumService.GetAllPrivateMessagesAsync(store.Id,
                0, customer.Id, null, null, false, string.Empty, page, pageSize);

            foreach (var pm in list)
                messages.Add(await PreparePrivateMessageModelAsync(pm));

            var pagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { PageNumber = page, Tab = tab }
            };

            var model = new PrivateMessageListModel
            {
                Messages = messages,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the sent model
        /// </summary>
        /// <param name="page">Number of items page</param>
        /// <param name="tab">Tab name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private message list model
        /// </returns>
        public virtual async Task<PrivateMessageListModel> PrepareSentModelAsync(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var list = await _forumService.GetAllPrivateMessagesAsync(store.Id,
                customer.Id, 0, null, false, null, string.Empty, page, pageSize);
            foreach (var pm in list)
                messages.Add(await PreparePrivateMessageModelAsync(pm));

            var pagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { PageNumber = page, Tab = tab }
            };

            var model = new PrivateMessageListModel
            {
                Messages = messages,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the send private message model
        /// </summary>
        /// <param name="customerTo">Customer, recipient of the message</param>
        /// <param name="replyToPM">Private message, pass if reply to a previous message is need</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the send private message model
        /// </returns>
        public virtual async Task<SendPrivateMessageModel> PrepareSendPrivateMessageModelAsync(Customer customerTo, PrivateMessage replyToPM)
        {
            if (customerTo == null)
                throw new ArgumentNullException(nameof(customerTo));

            var model = new SendPrivateMessageModel
            {
                ToCustomerId = customerTo.Id,
                CustomerToName = await _customerService.FormatUsernameAsync(customerTo),
                AllowViewingToProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(customerTo)
            };

            if (replyToPM == null)
                return model;

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (replyToPM.ToCustomerId == customer.Id ||
                replyToPM.FromCustomerId == customer.Id)
            {
                model.ReplyToMessageId = replyToPM.Id;
                model.Subject = $"Re: {replyToPM.Subject}";
            }

            return model;
        }

        /// <summary>
        /// Prepare the private message model
        /// </summary>
        /// <param name="pm">Private message</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the private message model
        /// </returns>
        public virtual async Task<PrivateMessageModel> PreparePrivateMessageModelAsync(PrivateMessage pm)
        {
            if (pm == null)
                throw new ArgumentNullException(nameof(pm));

            var fromCustomer = await _customerService.GetCustomerByIdAsync(pm.FromCustomerId);
            var toCustomer = await _customerService.GetCustomerByIdAsync(pm.ToCustomerId);

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromCustomerId = pm.FromCustomerId,
                CustomerFromName = await _customerService.FormatUsernameAsync(fromCustomer),
                AllowViewingFromProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(fromCustomer),
                ToCustomerId = pm.ToCustomerId,
                CustomerToName = await _customerService.FormatUsernameAsync(toCustomer),
                AllowViewingToProfile = _customerSettings.AllowViewingProfiles && !await _customerService.IsGuestAsync(toCustomer),
                Subject = pm.Subject,
                Message = _forumService.FormatPrivateMessageText(pm),
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc),
                IsRead = pm.IsRead,
            };

            return model;
        }

        #endregion

        #region Nested class

        /// <summary>
        /// record that has a slug and page for route values. Used for Private Messages pagination
        /// </summary>
        public partial record PrivateMessageRouteValues : BaseRouteValues
        {
            public string Tab { get; set; }
        }

        #endregion
    }
}