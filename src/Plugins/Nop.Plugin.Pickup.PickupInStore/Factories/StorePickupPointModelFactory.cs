using System;
using System.Linq;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Plugin.Pickup.PickupInStore.Services;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Pickup.PickupInStore.Factories
{
    /// <summary>
    /// Represents store pickup point models factory implementation
    /// </summary>
    public class StorePickupPointModelFactory : IStorePickupPointModelFactory
    {
        #region Fields

        private readonly IStorePickupPointService _storePickupPointService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public StorePickupPointModelFactory(IStorePickupPointService storePickupPointService,
            ILocalizationService localizationService, IStoreService storeService)
        {
            _storePickupPointService = storePickupPointService;
            _localizationService = localizationService;
            _storeService = storeService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare store pickup point list model
        /// </summary>
        /// <param name="searchModel">Store pickup point search model</param>
        /// <returns>Store pickup point list model</returns>
        public StorePickupPointListModel PrepareStorePickupPointListModel(StorePickupPointSearchModel searchModel)
        {
            var pickupPoints = _storePickupPointService.GetAllStorePickupPoints(pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);
            var model = new StorePickupPointListModel().PrepareToGrid(searchModel, pickupPoints, () =>
            {
                return pickupPoints.Select(point =>
                {
                    var store = _storeService.GetStoreById(point.StoreId);
                    return new StorePickupPointModel
                    {
                        Id = point.Id,
                        Name = point.Name,
                        OpeningHours = point.OpeningHours,
                        PickupFee = point.PickupFee,
                        DisplayOrder = point.DisplayOrder,
                        StoreName = store?.Name ?? (point.StoreId == 0
                                        ? _localizationService.GetResource(
                                            "Admin.Configuration.Settings.StoreScope.AllStores")
                                        : string.Empty)
                    };
                });
            });
           
            return model;
        }

        /// <summary>
        /// Prepare store pickup point search model
        /// </summary>
        /// <param name="searchModel">Store pickup point search model</param>
        /// <returns>Store pickup point search model</returns>
        public StorePickupPointSearchModel PrepareStorePickupPointSearchModel(StorePickupPointSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion
    }
}
