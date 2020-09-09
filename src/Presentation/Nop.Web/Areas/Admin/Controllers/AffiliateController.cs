using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class AffiliateController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IAffiliateModelFactory _affiliateModelFactory;
        private readonly IAffiliateService _affiliateService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AffiliateController(IAddressService addressService,
            IAffiliateModelFactory affiliateModelFactory,
            IAffiliateService affiliateService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            _addressService = addressService;
            _affiliateModelFactory = affiliateModelFactory;
            _affiliateService = affiliateService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //prepare model
            var model =await _affiliateModelFactory.PrepareAffiliateSearchModel(new AffiliateSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AffiliateSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _affiliateModelFactory.PrepareAffiliateListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //prepare model
            var model = await _affiliateModelFactory.PrepareAffiliateModel(new AffiliateModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(AffiliateModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity<Address>();

                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddress(address);

                var affiliate = model.ToEntity<Affiliate>();

                //validate friendly URL name
                var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlName(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.AddressId = address.Id;

                await _affiliateService.InsertAffiliate(affiliate);

                //activity log
                await _customerActivityService.InsertActivity("AddNewAffiliate",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Affiliates.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = affiliate.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _affiliateModelFactory.PrepareAffiliateModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = await _affiliateService.GetAffiliateById(id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _affiliateModelFactory.PrepareAffiliateModel(null, affiliate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(AffiliateModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = await _affiliateService.GetAffiliateById(model.Id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var address = await _addressService.GetAddressById(affiliate.AddressId);
                address = model.Address.ToEntity(address);

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.UpdateAddress(address);

                affiliate = model.ToEntity(affiliate);

                //validate friendly URL name
                var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlName(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.AddressId = address.Id;

                await _affiliateService.UpdateAffiliate(affiliate);

                //activity log
                await _customerActivityService.InsertActivity("EditAffiliate",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditAffiliate"), affiliate.Id), affiliate);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Affiliates.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = affiliate.Id });
            }

            //prepare model
            model = await _affiliateModelFactory.PrepareAffiliateModel(model, affiliate, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = await _affiliateService.GetAffiliateById(id);
            if (affiliate == null)
                return RedirectToAction("List");

            await _affiliateService.DeleteAffiliate(affiliate);

            //activity log
            await _customerActivityService.InsertActivity("DeleteAffiliate",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteAffiliate"), affiliate.Id), affiliate);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Affiliates.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> AffiliatedOrderListGrid(AffiliatedOrderSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedDataTablesJson();

            //try to get an affiliate with the specified id
            var affiliate = await _affiliateService.GetAffiliateById(searchModel.AffliateId)
                ?? throw new ArgumentException("No affiliate found with the specified id");

            //prepare model
            var model = await _affiliateModelFactory.PrepareAffiliatedOrderListModel(searchModel, affiliate);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AffiliatedCustomerList(AffiliatedCustomerSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedDataTablesJson();

            //try to get an affiliate with the specified id
            var affiliate = await _affiliateService.GetAffiliateById(searchModel.AffliateId)
                ?? throw new ArgumentException("No affiliate found with the specified id");

            //prepare model
            var model = await _affiliateModelFactory.PrepareAffiliatedCustomerListModel(searchModel, affiliate);

            return Json(model);
        }

        #endregion
    }
}