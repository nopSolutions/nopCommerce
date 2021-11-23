using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
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

        protected CustomerSettings CustomerSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IForumService ForumService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IStoreContext StoreContext { get; }
        protected IWorkContext WorkContext { get; }

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
            CustomerSettings = customerSettings;
            ForumSettings = forumSettings;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            ForumService = forumService;
            LocalizationService = localizationService;
            StoreContext = storeContext;
            WorkContext = workContext;
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

            var pageSize = ForumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();
            var store = await StoreContext.GetCurrentStoreAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var list = await ForumService.GetAllPrivateMessagesAsync(store.Id,
                0, customer.Id, null, null, false, string.Empty, page, pageSize);
            
            foreach (var pm in list)
                messages.Add(await PreparePrivateMessageModelAsync(pm));

            var pagerModel = new PagerModel(LocalizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { pageNumber = page, tab = tab }
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

            var pageSize = ForumSettings.PrivateMessagesPageSize;

            var messages = new List<PrivateMessageModel>();
            var store = await StoreContext.GetCurrentStoreAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var list = await ForumService.GetAllPrivateMessagesAsync(store.Id,
                customer.Id, 0, null, false, null, string.Empty, page, pageSize);
            foreach (var pm in list)
                messages.Add(await PreparePrivateMessageModelAsync(pm));

            var pagerModel = new PagerModel(LocalizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { pageNumber = page, tab = tab }
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
                CustomerToName = await CustomerService.FormatUsernameAsync(customerTo),
                AllowViewingToProfile = CustomerSettings.AllowViewingProfiles && !await CustomerService.IsGuestAsync(customerTo)
            };

            if (replyToPM == null)
                return model;

            var customer = await WorkContext.GetCurrentCustomerAsync();
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

            var fromCustomer = await CustomerService.GetCustomerByIdAsync(pm.FromCustomerId);
            var toCustomer = await CustomerService.GetCustomerByIdAsync(pm.ToCustomerId);

            var model = new PrivateMessageModel
            {
                Id = pm.Id,
                FromCustomerId = pm.FromCustomerId,
                CustomerFromName = await CustomerService.FormatUsernameAsync(fromCustomer),
                AllowViewingFromProfile = CustomerSettings.AllowViewingProfiles && !await CustomerService.IsGuestAsync(fromCustomer),
                ToCustomerId = pm.ToCustomerId,
                CustomerToName = await CustomerService.FormatUsernameAsync(toCustomer),
                AllowViewingToProfile = CustomerSettings.AllowViewingProfiles && !await CustomerService.IsGuestAsync(toCustomer),
                Subject = pm.Subject,
                Message = ForumService.FormatPrivateMessageText(pm),
                CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(pm.CreatedOnUtc, DateTimeKind.Utc),
                IsRead = pm.IsRead,
            };

            return model;
        }

        #endregion
    }
}