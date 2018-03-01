using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Factories
{
    /// <summary>
    /// Represents base model factory that implements a most common factory interfaces
    /// </summary>
    public abstract partial class BaseModelFactory : ILocalizedModelFactory
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public BaseModelFactory(ILanguageService languageService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            this._languageService = languageService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare localized model for localizable entities
        /// </summary>
        /// <typeparam name="T">Localized model type</typeparam>
        /// <param name="configure">Model configuration action</param>
        /// <returns>List of localized model</returns>
        public virtual IList<T> PrepareLocalizedModels<T>(Action<T, int> configure = null) where T : ILocalizedLocaleModel
        {
            //get all available languages
            var availableLanguages = _languageService.GetAllLanguages(showHidden: true);

            //prepare models
            var localizedModels = availableLanguages.Select(language =>
            {
                //create localized model
                var localizedModel = Activator.CreateInstance<T>();

                //set language
                localizedModel.LanguageId = language.Id;

                //invoke the model configuration action
                if (configure != null)
                    configure.Invoke(localizedModel, localizedModel.LanguageId);

                return localizedModel;
            }).ToList();

            return localizedModels;
        }

        /// <summary>
        /// Prepare selected and all available stores for the passed model
        /// </summary>
        /// <typeparam name="TModel">Store mapping supported model type</typeparam>
        /// <typeparam name="TEntity">Store mapping supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="ignoreStoreMappings">Whether to ignore existing store mappings</param>
        public virtual void PrepareModelStores<TModel, TEntity>(TModel model, TEntity entity, bool ignoreStoreMappings)
            where TModel : IStoreMappingSupportedModel where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //try to get store identifiers with granted access
            if (!ignoreStoreMappings && entity != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(entity).ToList();

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            model.AvailableStores = availableStores.Select(store => new SelectListItem
            {
                Text = store.Name,
                Value = store.Id.ToString(),
                Selected = model.SelectedStoreIds.Contains(store.Id)
            }).ToList();
        }

        #endregion
    }
}