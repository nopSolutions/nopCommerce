using System;
using System.Linq;
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

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedEmailService _queuedEmailService;

        #endregion

        #region Ctor

        public QueuedEmailModelFactory(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IQueuedEmailService queuedEmailService)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _queuedEmailService = queuedEmailService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare queued email search model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>Queued email search model</returns>
        public virtual QueuedEmailSearchModel PrepareQueuedEmailSearchModel(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare default search values
            searchModel.SearchMaxSentTries = 10;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged queued email list model
        /// </summary>
        /// <param name="searchModel">Queued email search model</param>
        /// <returns>Queued email list model</returns>
        public virtual QueuedEmailListModel PrepareQueuedEmailListModel(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.SearchStartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.SearchEndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get queued emails
            var queuedEmails = _queuedEmailService.SearchEmails(fromEmail: searchModel.SearchFromEmail,
                toEmail: searchModel.SearchToEmail,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                loadNotSentItemsOnly: searchModel.SearchLoadNotSent,
                loadOnlyItemsToBeSent: false,
                maxSendTries: searchModel.SearchMaxSentTries,
                loadNewest: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new QueuedEmailListModel().PrepareToGrid(searchModel, queuedEmails, () =>
            {
                return queuedEmails.Select(queuedEmail =>
                {
                    //fill in model values from the entity
                    var queuedEmailModel = queuedEmail.ToModel<QueuedEmailModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    queuedEmailModel.Body = string.Empty;

                    //convert dates to the user time
                    queuedEmailModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(queuedEmail.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    queuedEmailModel.EmailAccountName = queuedEmail.EmailAccount?.FriendlyName ?? string.Empty;
                    queuedEmailModel.PriorityName = _localizationService.GetLocalizedEnum(queuedEmail.Priority);
                    if (queuedEmail.DontSendBeforeDateUtc.HasValue)
                    {
                        queuedEmailModel.DontSendBeforeDate = _dateTimeHelper
                            .ConvertToUserTime(queuedEmail.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    }

                    if (queuedEmail.SentOnUtc.HasValue)
                        queuedEmailModel.SentOn = _dateTimeHelper.ConvertToUserTime(queuedEmail.SentOnUtc.Value, DateTimeKind.Utc);

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
        /// <returns>Queued email model</returns>
        public virtual QueuedEmailModel PrepareQueuedEmailModel(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false)
        {
            if (queuedEmail == null)
                return model;

            //fill in model values from the entity
            model = model ?? queuedEmail.ToModel<QueuedEmailModel>();

            model.EmailAccountName = queuedEmail.EmailAccount?.FriendlyName ?? string.Empty;
            model.PriorityName = _localizationService.GetLocalizedEnum(queuedEmail.Priority);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(queuedEmail.CreatedOnUtc, DateTimeKind.Utc);

            if (queuedEmail.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(queuedEmail.SentOnUtc.Value, DateTimeKind.Utc);
            if (queuedEmail.DontSendBeforeDateUtc.HasValue)
                model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(queuedEmail.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            else model.SendImmediately = true;

            return model;
        }

        #endregion
    }
}