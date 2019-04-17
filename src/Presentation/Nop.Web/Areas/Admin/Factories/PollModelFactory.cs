using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Polls;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Polls;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Polls;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the poll model factory implementation
    /// </summary>
    public partial class PollModelFactory : IPollModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPollService _pollService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion

        #region Ctor

        public PollModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IPollService pollService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _pollService = pollService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare poll answer search model
        /// </summary>
        /// <param name="searchModel">Poll answer search model</param>
        /// <param name="poll">Poll</param>
        /// <returns>Poll answer search model</returns>
        protected virtual PollAnswerSearchModel PreparePollAnswerSearchModel(PollAnswerSearchModel searchModel, Poll poll)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            searchModel.PollId = poll.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PreparePollGridModel(PollSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "polls-grid",
                UrlRead = new DataUrl("List", "Poll", null),
                SearchButtonId = "search-poll",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>
            {
                new FilterParameter(nameof(searchModel.SearchStoreId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(PollModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.Name")
                },
                new ColumnProperty(nameof(PollModel.LanguageName))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.Language"),
                    Width = "100"
                },
                new ColumnProperty(nameof(PollModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.DisplayOrder"),
                    Width = "200"
                },
                new ColumnProperty(nameof(PollModel.Published))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.Published"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(PollModel.ShowOnHomepage))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.ShowOnHomepage"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(PollModel.StartDateUtc))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.StartDate"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(PollModel.EndDateUtc))
                {
                    Title = _localizationService.GetResource("Admin.ContentManagement.Polls.Fields.EndDate"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(PollModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonEdit(new DataUrl("Edit"))
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare poll search model
        /// </summary>
        /// <param name="searchModel">Poll search model</param>
        /// <returns>Poll search model</returns>
        public virtual PollSearchModel PreparePollSearchModel(PollSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PreparePollGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged poll list model
        /// </summary>
        /// <param name="searchModel">Poll search model</param>
        /// <returns>Poll list model</returns>
        public virtual PollListModel PreparePollListModel(PollSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get polls
            var polls = _pollService.GetPolls(showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new PollListModel().PrepareToGrid(searchModel, polls, () =>
            {
                return polls.Select(poll =>
                {
                    //fill in model values from the entity
                    var pollModel = poll.ToModel<PollModel>();

                    //convert dates to the user time
                    if (poll.StartDateUtc.HasValue)
                        pollModel.StartDateUtc = _dateTimeHelper.ConvertToUserTime(poll.StartDateUtc.Value, DateTimeKind.Utc);
                    if (poll.EndDateUtc.HasValue)
                        pollModel.EndDateUtc = _dateTimeHelper.ConvertToUserTime(poll.EndDateUtc.Value, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    pollModel.LanguageName = _languageService.GetLanguageById(poll.LanguageId)?.Name;

                    return pollModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare poll model
        /// </summary>
        /// <param name="model">Poll model</param>
        /// <param name="poll">Poll</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Poll model</returns>
        public virtual PollModel PreparePollModel(PollModel model, Poll poll, bool excludeProperties = false)
        {
            if (poll != null)
            {
                //fill in model values from the entity
                model = model ?? poll.ToModel<PollModel>();

                model.StartDateUtc = poll.StartDateUtc;
                model.EndDateUtc = poll.EndDateUtc;

                //prepare nested search model
                PreparePollAnswerSearchModel(model.PollAnswerSearchModel, poll);
            }

            //set default values for the new model
            if (poll == null)
            {
                model.Published = true;
                model.ShowOnHomepage = true;
            }

            //prepare available languages
            _baseAdminModelFactory.PrepareLanguages(model.AvailableLanguages, false);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, poll, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged poll answer list model
        /// </summary>
        /// <param name="searchModel">Poll answer search model</param>
        /// <param name="poll">Poll</param>
        /// <returns>Poll answer list model</returns>
        public virtual PollAnswerListModel PreparePollAnswerListModel(PollAnswerSearchModel searchModel, Poll poll)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            //get poll answers
            var pollAnswers = poll.PollAnswers.OrderBy(pollAnswer => pollAnswer.DisplayOrder).ToList().ToPagedList(searchModel);

            //prepare list model
            var model = new PollAnswerListModel
            {
                //fill in model values from the entity
                Data = pollAnswers.Select(pollAnswer => pollAnswer.ToModel<PollAnswerModel>()),
                Total = pollAnswers.TotalCount
            };

            return model;
        }

        #endregion
    }
}