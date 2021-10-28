using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the queued email model factory implementation
    /// </summary>
    public partial class QueuedEmailModelFactory : IQueuedEmailModelFactory
    {
        #region Fields

        protected IDateTimeHelper DateTimeHelper { get; }
        protected IEmailAccountService EmailAccountService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IQueuedEmailService QueuedEmailService { get; }

        #endregion

        #region Ctor

        public QueuedEmailModelFactory(IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            IQueuedEmailService queuedEmailService)
        {
            DateTimeHelper = dateTimeHelper;
            EmailAccountService = emailAccountService;
            LocalizationService = localizationService;
            QueuedEmailService = queuedEmailService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a friendly email account name
        /// </summary>
        protected virtual string GetEmailAccountName(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(emailAccount.DisplayName))
                return emailAccount.Email + " (" + emailAccount.DisplayName + ")";

            return emailAccount.Email;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare queued email search model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email search model
        /// </returns>
        public virtual Task<QueuedEmailSearchModel> PrepareQueuedEmailSearchModelAsync(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare default search values
            searchModel.SearchMaxSentTries = 10;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged queued email list model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email list model
        /// </returns>
        public virtual async Task<QueuedEmailListModel> PrepareQueuedEmailListModelAsync(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.SearchStartDate.HasValue ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.SearchEndDate.HasValue ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get queued emails
            var queuedEmails = await QueuedEmailService.SearchEmailsAsync(fromEmail: searchModel.SearchFromEmail,
                toEmail: searchModel.SearchToEmail,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                loadNotSentItemsOnly: searchModel.SearchLoadNotSent,
                loadOnlyItemsToBeSent: false,
                maxSendTries: searchModel.SearchMaxSentTries,
                loadNewest: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new QueuedEmailListModel().PrepareToGridAsync(searchModel, queuedEmails, () =>
            {
                return queuedEmails.SelectAwait(async queuedEmail =>
                {
                    //fill in model values from the entity
                    var queuedEmailModel = queuedEmail.ToModel<QueuedEmailModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    queuedEmailModel.Body = string.Empty;

                    //convert dates to the user time
                    queuedEmailModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(queuedEmail.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId);
                    queuedEmailModel.EmailAccountName = GetEmailAccountName(emailAccount);
                    queuedEmailModel.PriorityName = await LocalizationService.GetLocalizedEnumAsync(queuedEmail.Priority);

                    if (queuedEmail.DontSendBeforeDateUtc.HasValue)
                    {
                        queuedEmailModel.DontSendBeforeDate = await DateTimeHelper
                            .ConvertToUserTimeAsync(queuedEmail.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    }

                    if (queuedEmail.SentOnUtc.HasValue)
                        queuedEmailModel.SentOn = await DateTimeHelper.ConvertToUserTimeAsync(queuedEmail.SentOnUtc.Value, DateTimeKind.Utc);

                    return queuedEmailModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare queued email model
        /// </summary>
        /// <param name="model">Queued email model</param>
        /// <param name="queuedEmail">Queued email</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email model
        /// </returns>
        public virtual async Task<QueuedEmailModel> PrepareQueuedEmailModelAsync(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false)
        {
            if (queuedEmail == null)
                return model;

            //fill in model values from the entity
            model ??= queuedEmail.ToModel<QueuedEmailModel>();

            model.EmailAccountName = GetEmailAccountName(await EmailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId));
            model.PriorityName = await LocalizationService.GetLocalizedEnumAsync(queuedEmail.Priority);
            model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(queuedEmail.CreatedOnUtc, DateTimeKind.Utc);

            if (queuedEmail.SentOnUtc.HasValue)
                model.SentOn = await DateTimeHelper.ConvertToUserTimeAsync(queuedEmail.SentOnUtc.Value, DateTimeKind.Utc);
            if (queuedEmail.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = await DateTimeHelper.ConvertToUserTimeAsync(queuedEmail.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            else
                model.SendImmediately = true;

            return model;
        }

        #endregion
    }
}