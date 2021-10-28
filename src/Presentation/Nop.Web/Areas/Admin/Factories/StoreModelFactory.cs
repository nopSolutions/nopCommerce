using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the store model factory implementation
    /// </summary>
    public partial class StoreModelFactory : IStoreModelFactory
    {
        #region Fields

        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedModelFactory LocalizedModelFactory { get; }
        protected IStoreService StoreService { get; }

        #endregion

        #region Ctor

        public StoreModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IStoreService storeService)
        {
            BaseAdminModelFactory = baseAdminModelFactory;
            LocalizationService = localizationService;
            LocalizedModelFactory = localizedModelFactory;
            StoreService = storeService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare store search model
        /// </summary>
        /// <param name="searchModel">Store search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store search model
        /// </returns>
        public virtual Task<StoreSearchModel> PrepareStoreSearchModelAsync(StoreSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged store list model
        /// </summary>
        /// <param name="searchModel">Store search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store list model
        /// </returns>
        public virtual async Task<StoreListModel> PrepareStoreListModelAsync(StoreSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get stores
            var stores = (await StoreService.GetAllStoresAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = new StoreListModel().PrepareToGrid(searchModel, stores, () =>
            {
                //fill in model values from the entity
                return stores.Select(store => store.ToModel<StoreModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare store model
        /// </summary>
        /// <param name="model">Store model</param>
        /// <param name="store">Store</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the store model
        /// </returns>
        public virtual async Task<StoreModel> PrepareStoreModelAsync(StoreModel model, Store store, bool excludeProperties = false)
        {
            Action<StoreLocalizedModel, int> localizedModelConfiguration = null;

            if (store != null)
            {
                //fill in model values from the entity
                model ??= store.ToModel<StoreModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await LocalizationService.GetLocalizedAsync(store, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare available languages
            await BaseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, 
                defaultItemText: await LocalizationService.GetResourceAsync("Admin.Common.EmptyItemText"));

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await LocalizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}