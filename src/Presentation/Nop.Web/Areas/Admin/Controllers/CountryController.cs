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

        protected IAddressService AddressService { get; }
        protected ICountryModelFactory CountryModelFactory { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IExportManager ExportManager { get; }
        protected IImportManager ImportManager { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }

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
            AddressService = addressService;
            CountryModelFactory = countryModelFactory;
            CountryService = countryService;
            CustomerActivityService = customerActivityService;
            ExportManager = exportManager;
            ImportManager = importManager;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            StateProvinceService = stateProvinceService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Country country, CountryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(country,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(StateProvince stateProvince, StateProvinceModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(stateProvince,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Country country, CountryModel model)
        {
            country.LimitedToStores = model.SelectedStoreIds.Any();
            await CountryService.UpdateCountryAsync(country);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(country);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await StoreMappingService.InsertStoreMappingAsync(country, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //prepare model
            var model = await CountryModelFactory.PrepareCountrySearchModelAsync(new CountrySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CountryList(CountrySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CountryModelFactory.PrepareCountryListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //prepare model
            var model = await CountryModelFactory.PrepareCountryModelAsync(new CountryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CountryModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var country = model.ToEntity<Country>();
                await CountryService.InsertCountryAsync(country);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCountry",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCountry"), country.Id), country);

                //locales
                await UpdateLocalesAsync(country, model);

                //Stores
                await SaveStoreMappingsAsync(country, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Countries.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = country.Id });
            }

            //prepare model
            model = await CountryModelFactory.PrepareCountryModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(id);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CountryModelFactory.PrepareCountryModelAsync(null, country);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CountryModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(model.Id);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                country = model.ToEntity(country);
                await CountryService.UpdateCountryAsync(country);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditCountry",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCountry"), country.Id), country);

                //locales
                await UpdateLocalesAsync(country, model);

                //stores
                await SaveStoreMappingsAsync(country, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Countries.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = country.Id });
            }

            //prepare model
            model = await CountryModelFactory.PrepareCountryModelAsync(model, country, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(id);
            if (country == null)
                return RedirectToAction("List");

            try
            {
                if (await AddressService.GetAddressTotalByCountryIdAsync(country.Id) > 0)
                    throw new NopException("The country can't be deleted. It has associated addresses");

                await CountryService.DeleteCountryAsync(country);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteCountry",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCountry"), country.Id), country);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Countries.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = country.Id });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PublishSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var countries = await CountryService.GetCountriesByIdsAsync(selectedIds.ToArray());
            foreach (var country in countries)
            {
                country.Published = true;
                await CountryService.UpdateCountryAsync(country);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var countries = await CountryService.GetCountriesByIdsAsync(selectedIds.ToArray());
            foreach (var country in countries)
            {
                country.Published = false;
                await CountryService.UpdateCountryAsync(country);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region States / provinces

        [HttpPost]
        public virtual async Task<IActionResult> States(StateProvinceSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return await AccessDeniedDataTablesJson();

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(searchModel.CountryId)
                ?? throw new ArgumentException("No country found with the specified id");

            //prepare model
            var model = await CountryModelFactory.PrepareStateProvinceListModelAsync(searchModel, country);

            return Json(model);
        }

        public virtual async Task<IActionResult> StateCreatePopup(int countryId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(countryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CountryModelFactory.PrepareStateProvinceModelAsync(new StateProvinceModel(), country, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateCreatePopup(StateProvinceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(model.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity<StateProvince>();

                await StateProvinceService.InsertStateProvinceAsync(sp);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewStateProvince",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewStateProvince"), sp.Id), sp);

                await UpdateLocalesAsync(sp, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await CountryModelFactory.PrepareStateProvinceModelAsync(model, country, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> StateEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await StateProvinceService.GetStateProvinceByIdAsync(id);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CountryModelFactory.PrepareStateProvinceModelAsync(null, country, state);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateEditPopup(StateProvinceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await StateProvinceService.GetStateProvinceByIdAsync(model.Id);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = await CountryService.GetCountryByIdAsync(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                state = model.ToEntity(state);
                await StateProvinceService.UpdateStateProvinceAsync(state);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditStateProvince",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditStateProvince"), state.Id), state);

                await UpdateLocalesAsync(state, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await CountryModelFactory.PrepareStateProvinceModelAsync(model, country, state, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> StateDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = await StateProvinceService.GetStateProvinceByIdAsync(id)
                ?? throw new ArgumentException("No state found with the specified id");

            if (await AddressService.GetAddressTotalByStateProvinceIdAsync(state.Id) > 0)
            {
                return ErrorJson(await LocalizationService.GetResourceAsync("Admin.Configuration.Countries.States.CantDeleteWithAddresses"));
            }

            //int countryId = state.CountryId;
            await StateProvinceService.DeleteStateProvinceAsync(state);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteStateProvince",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteStateProvince"), state.Id), state);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var country = await CountryService.GetCountryByIdAsync(Convert.ToInt32(countryId));
            var states = country != null ? (await StateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true)).ToList() : new List<StateProvince>();
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
                        result.Insert(0, new { id = 0, name = await LocalizationService.GetResourceAsync("Admin.Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = await LocalizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = await LocalizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = await LocalizationService.GetResourceAsync("Admin.Address.SelectState") });
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var fileName = $"states_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";

            var states = await StateProvinceService.GetStateProvincesAsync(true);
            var result = await ExportManager.ExportStatesToTxtAsync(states);

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            try
            {
                if (importcsvfile != null && importcsvfile.Length > 0)
                {
                    var count = await ImportManager.ImportStatesFromTxtAsync(importcsvfile.OpenReadStream());

                    NotificationService.SuccessNotification(string.Format(await LocalizationService.GetResourceAsync("Admin.Configuration.Countries.ImportSuccess"), count));

                    return RedirectToAction("List");
                }

                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}