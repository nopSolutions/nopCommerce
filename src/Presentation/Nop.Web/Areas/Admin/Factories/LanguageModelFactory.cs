using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the language model factory implementation
    /// </summary>
    public partial class LanguageModelFactory : ILanguageModelFactory
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Ctor

        public LanguageModelFactory(IActionContextAccessor actionContextAccessor,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlHelperFactory urlHelperFactory)
        {
            _actionContextAccessor = actionContextAccessor;
            _baseAdminModelFactory = baseAdminModelFactory;
            _languageService = languageService;
            _localizationService = localizationService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _urlHelperFactory = urlHelperFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare locale resource search model
        /// </summary>
        /// <param name="searchModel">Locale resource search model</param>
        /// <param name="language">Language</param>
        /// <returns>Locale resource search model</returns>
        protected virtual LocaleResourceSearchModel PrepareLocaleResourceSearchModel(LocaleResourceSearchModel searchModel, Language language)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (language == null)
                throw new ArgumentNullException(nameof(language));

            searchModel.LanguageId = language.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare language datatables model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language datatables model</returns>
        protected virtual DataTablesModel PrepareLanguageGridModel(LanguageSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "languages-grid",
                UrlRead = new DataUrl("List", "Language", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(LanguageModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.Languages.Fields.Name")
                },
                new ColumnProperty(nameof(LanguageModel.FlagImageFileName))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.Languages.Fields.FlagImage"),
                    Width = "100",
                    Render = new RenderPicture(urlHelper.Content("~/images/flags/"))
                },
                new ColumnProperty(nameof(LanguageModel.LanguageCulture))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.Languages.Fields.LanguageCulture"),
                    Width = "200"
                },
                new ColumnProperty(nameof(LanguageModel.DisplayOrder))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.Languages.Fields.DisplayOrder"),
                    Width = "150"
                },
                new ColumnProperty(nameof(LanguageModel.Published))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.Languages.Fields.Published"),
                    Width = "150",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(LanguageModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    Render = new RenderButtonEdit(new DataUrl("Edit"))
                }
            };

            //prepare column definitions
            model.ColumnDefinitions = new List<ColumnDefinition>
            {
                new ColumnDefinition()
                {
                    Targets = "1",
                    ClassName =  StyleColumn.CenterBody
                },
                new ColumnDefinition()
                {
                    Targets = "[4,5]",
                    ClassName =  StyleColumn.CenterAll
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare language search model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language search model</returns>
        public virtual LanguageSearchModel PrepareLanguageSearchModel(LanguageSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareLanguageGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged language list model
        /// </summary>
        /// <param name="searchModel">Language search model</param>
        /// <returns>Language list model</returns>
        public virtual LanguageListModel PrepareLanguageListModel(LanguageSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get languages
            var languages = _languageService.GetAllLanguages(showHidden: true, loadCacheableCopy: false).ToPagedList(searchModel);

            //prepare list model
            var model = new LanguageListModel().PrepareToGrid(searchModel, languages, () =>
            {
                return languages.Select(language =>
                {
                    return language.ToModel<LanguageModel>();
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare language model
        /// </summary>
        /// <param name="model">Language model</param>
        /// <param name="language">Language</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Language model</returns>
        public virtual LanguageModel PrepareLanguageModel(LanguageModel model, Language language, bool excludeProperties = false)
        {
            if (language != null)
            {
                //fill in model values from the entity
                model = model ?? language.ToModel<LanguageModel>();

                //prepare nested search model
                PrepareLocaleResourceSearchModel(model.LocaleResourceSearchModel, language);
            }

            //set default values for the new model
            if (language == null)
            {
                model.DisplayOrder = _languageService.GetAllLanguages().Max(l => l.DisplayOrder) + 1;
                model.Published = true;
            }

            //prepare available currencies
            //TODO: add locale resource for "---"
            _baseAdminModelFactory.PrepareCurrencies(model.AvailableCurrencies, defaultItemText: "---");

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, language, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged locale resource list model
        /// </summary>
        /// <param name="searchModel">Locale resource search model</param>
        /// <param name="language">Language</param>
        /// <returns>Locale resource list model</returns>
        public virtual LocaleResourceListModel PrepareLocaleResourceListModel(LocaleResourceSearchModel searchModel, Language language)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (language == null)
                throw new ArgumentNullException(nameof(language));

            //get locale resources
            var localeResources = _localizationService.GetAllResourceValues(language.Id, loadPublicLocales: null)
                .OrderBy(localeResource => localeResource.Key).AsQueryable();

            //filter locale resources
            //TODO: move filter to language service
            if (!string.IsNullOrEmpty(searchModel.SearchResourceName))
                localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(searchModel.SearchResourceName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchResourceValue))
                localeResources = localeResources.Where(l => l.Value.Value.ToLowerInvariant().Contains(searchModel.SearchResourceValue.ToLowerInvariant()));

            var pagedLocaleResources = new PagedList<KeyValuePair<string, KeyValuePair<int, string>>>(localeResources,
                searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new LocaleResourceListModel
            {
                //fill in model values from the entity
                Data = pagedLocaleResources.Select(localeResource => new LocaleResourceModel
                {
                    LanguageId = language.Id,
                    Id = localeResource.Value.Key,
                    ResourceName = localeResource.Key,
                    ResourceValue = localeResource.Value.Value
                }),
                Total = pagedLocaleResources.TotalCount
            };

            return model;
        }

        #endregion
    }
}