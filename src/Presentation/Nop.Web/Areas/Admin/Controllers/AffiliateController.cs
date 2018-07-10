using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Services.Affiliates;
using Nop.Services.Localization;
using Nop.Services.Logging;
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

        private readonly IAffiliateModelFactory _affiliateModelFactory;
        private readonly IAffiliateService _affiliateService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AffiliateController(IAffiliateModelFactory affiliateModelFactory,
            IAffiliateService affiliateService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._affiliateModelFactory = affiliateModelFactory;
            this._affiliateService = affiliateService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
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
                return AccessDeniedKendoGridJson();

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
                var affiliate = model.ToEntity<Affiliate>();

                //validate friendly URL name
                var friendlyUrlName = _affiliateService.ValidateFriendlyUrlName(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;

                affiliate.Address = model.Address.ToEntity<Address>();
                affiliate.Address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (affiliate.Address.CountryId == 0)
                    affiliate.Address.CountryId = null;
                if (affiliate.Address.StateProvinceId == 0)
                    affiliate.Address.StateProvinceId = null;

                _affiliateService.InsertAffiliate(affiliate);

                //activity log
                _customerActivityService.InsertActivity("AddNewAffiliate",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

                SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Added"));

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
                affiliate = model.ToEntity(affiliate);

                //validate friendly URL name
                var friendlyUrlName = _affiliateService.ValidateFriendlyUrlName(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;

                affiliate.Address = model.Address.ToEntity(affiliate.Address);

                //some validation
                if (affiliate.Address.CountryId == 0)
                    affiliate.Address.CountryId = null;
                if (affiliate.Address.StateProvinceId == 0)
                    affiliate.Address.StateProvinceId = null;

                _affiliateService.UpdateAffiliate(affiliate);

                //activity log
                _customerActivityService.InsertActivity("EditAffiliate",
                    string.Format(_localizationService.GetResource("ActivityLog.EditAffiliate"), affiliate.Id), affiliate);

                SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

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

            SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult AffiliatedOrderListGrid(AffiliatedOrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedKendoGridJson();

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
                return AccessDeniedKendoGridJson();

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