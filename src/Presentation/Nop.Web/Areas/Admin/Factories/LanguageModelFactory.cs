using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the language model factory implementation
    /// </summary>
    public partial class LanguageModelFactory : ILanguageModelFactory
    {
        #region Fields
        
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion

        #region Ctor

        public LanguageModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _languageService = languageService;
            _localizationService = localizationService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }

        #endregion

        #region Utilities

        #region Extensions by QuanNH
        protected virtual void PrepareStoresMappingModel(LanguageModel model, Language language, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var _storeMappingService = Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Stores.IStoreMappingService>();
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<Core.IWorkContext>();
            List<int> storeId = _storeMappingService.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores");

            if (!excludeProperties)
            {
                if (language != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(language).ToList();
                }
                else
                {
                    if (storeId.Count > 0) model.SelectedStoreIds = storeId;
                }

                if (storeId.Count <= 0)
                    model.LimitedToStores = false;
                else model.LimitedToStores = true;
            }
        }
        #endregion
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

            #region Extensions by QuanNH
            int storeId = 0;
            var _storeMappingService = Nop.Core.Infrastructure.EngineContext.Current.Resolve<Nop.Services.Stores.IStoreMappingService>();
            var _workContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<Core.IWorkContext>();
            int storeIds = _storeMappingService.GetStoreIdByEntityId(_workContext.CurrentCustomer.Id, "Stores").FirstOrDefault();
            if (storeIds != 0)
            {
                storeId = storeIds;
            }

            //get languages
            var languages = _languageService.GetAllLanguages(showHidden: true, storeId: storeId, loadCacheableCopy: false).ToPagedList(searchModel);

            #endregion

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
            #region Extensions by QuanNH
            PrepareStoresMappingModel(model, language, excludeProperties);
            #endregion
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
            var model = new LocaleResourceListModel().PrepareToGrid(searchModel, pagedLocaleResources, () =>
            {
                //fill in model values from the entity
                return pagedLocaleResources.Select(localeResource => new LocaleResourceModel
                {
                    LanguageId = language.Id,
                    Id = localeResource.Value.Key,
                    ResourceName = localeResource.Key,
                    ResourceValue = localeResource.Value.Value
                });
            });

            return model;
        }

        #endregion
    }
}