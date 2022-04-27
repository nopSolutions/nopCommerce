using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CountryController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryModelFactory _countryModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public CountryController(IAddressService addressService,
            ICountryModelFactory countryModelFactory,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStateProvinceService stateProvinceService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _addressService = addressService;
            _countryModelFactory = countryModelFactory;
            _countryService = countryService;
            _customerActivityService = customerActivityService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _stateProvinceService = stateProvinceService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Country country, CountryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(country,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(StateProvince stateProvince, StateProvinceModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(stateProvince,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Country country, CountryModel model)
        {
            country.LimitedToStores = model.SelectedStoreIds.Any();
            await _countryService.UpdateCountryAsync(country);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(country);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(country, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Countries

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //prepare model
            var model = await _countryModelFactory.PrepareCountrySearchModelAsync(new CountrySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CountryList(CountrySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _countryModelFactory.PrepareCountryListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //prepare model
            var model = await _countryModelFactory.PrepareCountryModelAsync(new CountryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CountryModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var country = model.ToEntity<Country>();
                await _countryService.InsertCountryAsync(country);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewCountry",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCountry"), country.Id), country);

                //locales
                await UpdateLocalesAsync(country, model);

                //Stores
                await SaveStoreMappingsAsync(country, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = country.Id });
            }

            //prepare model
            model = await _countryModelFactory.PrepareCountryModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(id);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _countryModelFactory.PrepareCountryModelAsync(null, country);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CountryModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(model.Id);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                country = model.ToEntity(country);
                await _countryService.UpdateCountryAsync(country);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditCountry",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCountry"), country.Id), country);

                //locales
                await UpdateLocalesAsync(country, model);

                //stores
                await SaveStoreMappingsAsync(country, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = country.Id });
            }

            //prepare model
            model = await _countryModelFactory.PrepareCountryModelAsync(model, country, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(id);
            if (country == null)
                return RedirectToAction("List");

            try
            {
                if (await _addressService.GetAddressTotalByCountryIdAsync(country.Id) > 0)
                    throw new NopException("The country can't be deleted. It has associated addresses");

                await _countryService.DeleteCountryAsync(country);

                //activity log
                await _customerActivityService.InsertActivityAsync("DeleteCountry",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCountry"), country.Id), country);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = country.Id });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PublishSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var countries = await _countryService.GetCountriesByIdsAsync(selectedIds.ToArray());
            foreach (var country in countries)
            {
                country.Published = true;
                await _countryService.UpdateCountryAsync(country);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var countries = await _countryService.GetCountriesByIdsAsync(selectedIds.ToArray());
            foreach (var country in countries)
            {
                country.Published = false;
                await _countryService.UpdateCountryAsync(country);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region States / provinces

        [HttpPost]
        public virtual async Task<IActionResult> States(StateProvinceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return await AccessDeniedDataTablesJson();

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(searchModel.CountryId)
                ?? throw new ArgumentException("No country found with the specified id");

            //prepare model
            var model = await _countryModelFactory.PrepareStateProvinceListModelAsync(searchModel, country);

            return Json(model);
        }

        public virtual async Task<IActionResult> StateCreatePopup(int countryId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(countryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _countryModelFactory.PrepareStateProvinceModelAsync(new StateProvinceModel(), country, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateCreatePopup(StateProvinceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(model.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity<StateProvince>();

                await _stateProvinceService.InsertStateProvinceAsync(sp);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewStateProvince",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewStateProvince"), sp.Id), sp);

                await UpdateLocalesAsync(sp, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _countryModelFactory.PrepareStateProvinceModelAsync(model, country, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> StateEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await _stateProvinceService.GetStateProvinceByIdAsync(id);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _countryModelFactory.PrepareStateProvinceModelAsync(null, country, state);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateEditPopup(StateProvinceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await _stateProvinceService.GetStateProvinceByIdAsync(model.Id);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = await _countryService.GetCountryByIdAsync(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                state = model.ToEntity(state);
                await _stateProvinceService.UpdateStateProvinceAsync(state);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditStateProvince",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditStateProvince"), state.Id), state);

                await UpdateLocalesAsync(state, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _countryModelFactory.PrepareStateProvinceModelAsync(model, country, state, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await _stateProvinceService.GetStateProvinceByIdAsync(id)
                ?? throw new ArgumentException("No state found with the specified id");

            if (await _addressService.GetAddressTotalByStateProvinceIdAsync(state.Id) > 0)
            {
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.States.CantDeleteWithAddresses"));
            }

            //int countryId = state.CountryId;
            await _stateProvinceService.DeleteStateProvinceAsync(state);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteStateProvince",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStateProvince"), state.Id), state);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var country = await _countryService.GetCountryByIdAsync(Convert.ToInt32(countryId));
            var states = country != null ? (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true)).ToList() : new List<StateProvince>();
            var result = (from s in states
                          select new { id = s.Id, name = s.Name }).ToList();
            if (addAsterisk.HasValue && addAsterisk.Value)
            {
                //asterisk
                result.Insert(0, new { id = 0, name = "*" });
            }
            else
            {
                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                        }
                    }
                }
            }

            return Json(result);
        }

        #endregion

        #region Export / import

        public virtual async Task<IActionResult> ExportCsv()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var fileName = $"states_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";

            var states = await _stateProvinceService.GetStateProvincesAsync(true);
            var result = await _exportManager.ExportStatesToTxtAsync(states);

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            try
            {
                if (importcsvfile != null && importcsvfile.Length > 0)
                {
                    var count = await _importManager.ImportStatesFromTxtAsync(importcsvfile.OpenReadStream());

                    _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.ImportSuccess"), count));

                    return RedirectToAction("List");
                }

                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}