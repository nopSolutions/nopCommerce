#region Extensions by QuanNH
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Customers;
using Nop.Core;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Core.Domain.Stores;
using Nop.Services.Messages;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class StoreMappingController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreModelFactory _storeModelFactory;
        private readonly INotificationService _notificationService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly CatalogSettings _catalogSettings;

        public StoreMappingController(ICustomerService customerService, 
            IStoreService storeService, 
            IStoreMappingService storeMappingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IStoreModelFactory storeModelFactory,
            INotificationService notificationService,
            IBaseAdminModelFactory baseAdminModelFactory,
            CatalogSettings catalogSettings
            )
        {
            this._customerService = customerService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._storeModelFactory = storeModelFactory;
            this._notificationService = notificationService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._catalogSettings = catalogSettings;
        }

        #region Utilities

        public virtual void PrepareModelStores(StoreMappingModel model)
        {
            //stores
            var allStores = _storeService.GetAllStoresByEntityName(_workContext.CurrentCustomer.Id, "Stores");
            if (allStores.Count <= 0)
            {
                allStores = _storeService.GetAllStores();
                model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            }
            foreach (var s in allStores)
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

            //Customer
            int[] searchCustomerRoleIds = new int[] { 3 };

            foreach (var s in _customerService.GetAllCustomers(customerRoleIds: searchCustomerRoleIds))
                model.AvailableCustomers.Add(new SelectListItem() { Text = s.Email, Value = s.Id.ToString() });

        }

        /// <summary>
        /// Prepare StoreMapping search model
        /// </summary>
        /// <param name="searchModel">StoreMapping search model</param>
        /// <returns>StoreMapping search model</returns>
        public virtual StoreMappingSearchModel PrepareStoreMappingSearchModel(StoreMappingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged StoreMapping list model
        /// </summary>
        /// <param name="searchModel">StoreMapping search model</param>
        /// <returns>StoreMapping list model</returns>
        public virtual StoreMappingListModel PrepareStoreMappingListModel(StoreMappingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get StoreMappings
            var storeMappings = _storeMappingService.GetAllStoreMappings(storeId: searchModel.SearchStoreId, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            
            //prepare list model
            var model = new StoreMappingListModel().PrepareToGrid(searchModel, storeMappings, () =>
            {
                return storeMappings.Select(storeMapping =>
                {
                    //fill in model values from the entity
                    var _storeMapping = storeMapping.ToModel<StoreMappingModel>();
                    //fill in additional values (not existing in the entity)
                    _storeMapping.UserName = _customerService.GetCustomerById(storeMapping.EntityId) == null ? "This user has been deleted" : _customerService.GetCustomerById(storeMapping.EntityId).Email;
                    _storeMapping.StoreName = _storeService.GetStoreById(storeMapping.StoreId) == null ? "This store has been deleted" : _storeService.GetStoreById(storeMapping.StoreId).Name;
                    _storeMapping.StoreUrl = _storeService.GetStoreById(storeMapping.StoreId) == null ? "This store has been deleted" : _storeService.GetStoreById(storeMapping.StoreId).Url;

                    return _storeMapping;
                });
            });

            return model;
        }
        #endregion
        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = PrepareStoreMappingSearchModel(new StoreMappingSearchModel());

            return View(model);
        }

        [HttpPost]
        public IActionResult List(StoreMappingSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = PrepareStoreMappingListModel(searchModel);

            return Json(model);
        }

        public IActionResult Create()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = new StoreMappingModel();
            PrepareModelStores(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(StoreMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(model.EntityName))
                ModelState.AddModelError(string.Empty, "EntityName is required");

            if (model.EntityName != "Stores" && model.EntityName != "Admin")
                ModelState.AddModelError(string.Empty, "EntityName is Stores or Admin");

            if (model.StoreId == 0)
                ModelState.AddModelError(string.Empty, "Store is required");

            var currentStoreId = _storeMappingService.GetStoreIdByEntityId(model.EntityId, "Stores").FirstOrDefault();
            var adminStoreId = _storeMappingService.GetStoreIdByEntityId(model.EntityId, "Admin").FirstOrDefault();

            if (currentStoreId > 0 || adminStoreId>0)
            {
                ModelState.AddModelError(string.Empty, "User already mapped to an existing store");
                //ModelState.AddModelError(string.Empty, "CurrentStoreID:" + currentStoreId.ToString());
                //ModelState.AddModelError(string.Empty, "AdminStoreID:" + adminStoreId.ToString());

            }

            if (ModelState.IsValid)
            {
                var storeMapping = model.ToEntity<StoreMapping>();

                _storeMappingService.Insert_Store_Mapping(storeMapping);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = storeMapping.Id }) : RedirectToAction("List");
            }
            PrepareModelStores(model);
            //If we got this far, something failed, redisplay form
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeMapping = _storeMappingService.GetStoreMappingById(id);
            if (storeMapping == null)
                //No store found with the specified id
                return RedirectToAction("List");

            var model = storeMapping.ToModel<StoreMappingModel>();
            PrepareModelStores(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public IActionResult Edit(StoreMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeMapping = _storeMappingService.GetStoreMappingById(model.Id);
            if (storeMapping == null)
                //No store found with the specified id
                return RedirectToAction("List");


            if (string.IsNullOrEmpty(model.EntityName))
                ModelState.AddModelError(string.Empty, "EntityName is required");

            if (model.EntityName != "Stores" && model.EntityName != "Admin")
                ModelState.AddModelError(string.Empty, "Set EntityName to Stores or Admin");

            if (model.StoreId == 0)
                ModelState.AddModelError(string.Empty, "Store is required");

            if (ModelState.IsValid)
            {
                storeMapping = model.ToEntity(storeMapping);
                _storeMappingService.UpdateStoreMapping(storeMapping);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = storeMapping.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeMapping = _storeMappingService.GetStoreMappingById(id);
            if (storeMapping == null)
                //No store found with the specified id
                return RedirectToAction("List");

            try
            {
                _storeMappingService.DeleteStoreMapping(storeMapping);
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.SuccessNotification(exc.Message);
                return RedirectToAction("Edit", new { id = storeMapping.Id });
            }
        }
    }
}
#endregion;