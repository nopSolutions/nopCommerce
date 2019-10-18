using System;
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

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //prepare model
            var model = _affiliateModelFactory.PrepareAffiliateSearchModel(new AffiliateSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(AffiliateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _affiliateModelFactory.PrepareAffiliateListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //prepare model
            var model = _affiliateModelFactory.PrepareAffiliateModel(new AffiliateModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Create(AffiliateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
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

                _addressService.InsertAddress(address);

                var affiliate = model.ToEntity<Affiliate>();

                //validate friendly URL name
                var friendlyUrlName = _affiliateService.ValidateFriendlyUrlName(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.AddressId = address.Id;

                _affiliateService.InsertAffiliate(affiliate);

                //activity log
                _customerActivityService.InsertActivity("AddNewAffiliate",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = affiliate.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _affiliateModelFactory.PrepareAffiliateModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = _affiliateService.GetAffiliateById(id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _affiliateModelFactory.PrepareAffiliateModel(null, affiliate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(AffiliateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = _affiliateService.GetAffiliateById(model.Id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var address = _addressService.GetAddressById(affiliate.AddressId);
                address = model.Address.ToEntity(address);

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                _addressService.UpdateAddress(address);

                affiliate = model.ToEntity(affiliate);

                //validate friendly URL name
                var friendlyUrlName = _affiliateService.ValidateFriendlyUrlName(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.AddressId = address.Id;

                _affiliateService.UpdateAffiliate(affiliate);

                //activity log
                _customerActivityService.InsertActivity("EditAffiliate",
                    string.Format(_localizationService.GetResource("ActivityLog.EditAffiliate"), affiliate.Id), affiliate);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = affiliate.Id });
            }

            //prepare model
            model = _affiliateModelFactory.PrepareAffiliateModel(model, affiliate, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = _affiliateService.GetAffiliateById(id);
            if (affiliate == null)
                return RedirectToAction("List");

            _affiliateService.DeleteAffiliate(affiliate);

            //activity log
            _customerActivityService.InsertActivity("DeleteAffiliate",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteAffiliate"), affiliate.Id), affiliate);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult AffiliatedOrderListGrid(AffiliatedOrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedDataTablesJson();

            //try to get an affiliate with the specified id
            var affiliate = _affiliateService.GetAffiliateById(searchModel.AffliateId)
                ?? throw new ArgumentException("No affiliate found with the specified id");

            //prepare model
            var model = _affiliateModelFactory.PrepareAffiliatedOrderListModel(searchModel, affiliate);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult AffiliatedCustomerList(AffiliatedCustomerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedDataTablesJson();

            //try to get an affiliate with the specified id
            var affiliate = _affiliateService.GetAffiliateById(searchModel.AffliateId)
                ?? throw new ArgumentException("No affiliate found with the specified id");

            //prepare model
            var model = _affiliateModelFactory.PrepareAffiliatedCustomerListModel(searchModel, affiliate);

            return Json(model);
        }

        #endregion
    }
}