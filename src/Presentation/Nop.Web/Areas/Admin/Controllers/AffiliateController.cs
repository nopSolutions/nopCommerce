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

        protected IAddressService AddressService { get; }
        protected IAffiliateModelFactory AffiliateModelFactory { get; }
        protected IAffiliateService AffiliateService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }

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
            AddressService = addressService;
            AffiliateModelFactory = affiliateModelFactory;
            AffiliateService = affiliateService;
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //prepare model
            var model =await AffiliateModelFactory.PrepareAffiliateSearchModelAsync(new AffiliateSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AffiliateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await AffiliateModelFactory.PrepareAffiliateListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //prepare model
            var model = await AffiliateModelFactory.PrepareAffiliateModelAsync(new AffiliateModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(AffiliateModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
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

                await AddressService.InsertAddressAsync(address);

                var affiliate = model.ToEntity<Affiliate>();

                //validate friendly URL name
                var friendlyUrlName = await AffiliateService.ValidateFriendlyUrlNameAsync(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.AddressId = address.Id;

                await AffiliateService.InsertAffiliateAsync(affiliate);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewAffiliate",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Affiliates.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = affiliate.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await AffiliateModelFactory.PrepareAffiliateModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = await AffiliateService.GetAffiliateByIdAsync(id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await AffiliateModelFactory.PrepareAffiliateModelAsync(null, affiliate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(AffiliateModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = await AffiliateService.GetAffiliateByIdAsync(model.Id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var address = await AddressService.GetAddressByIdAsync(affiliate.AddressId);
                address = model.Address.ToEntity(address);

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await AddressService.UpdateAddressAsync(address);

                affiliate = model.ToEntity(affiliate);

                //validate friendly URL name
                var friendlyUrlName = await AffiliateService.ValidateFriendlyUrlNameAsync(affiliate, model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.AddressId = address.Id;

                await AffiliateService.UpdateAffiliateAsync(affiliate);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditAffiliate",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditAffiliate"), affiliate.Id), affiliate);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Affiliates.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = affiliate.Id });
            }

            //prepare model
            model = await AffiliateModelFactory.PrepareAffiliateModelAsync(model, affiliate, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            //try to get an affiliate with the specified id
            var affiliate = await AffiliateService.GetAffiliateByIdAsync(id);
            if (affiliate == null)
                return RedirectToAction("List");

            await AffiliateService.DeleteAffiliateAsync(affiliate);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteAffiliate",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteAffiliate"), affiliate.Id), affiliate);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Affiliates.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> AffiliatedOrderListGrid(AffiliatedOrderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return await AccessDeniedDataTablesJson();

            //try to get an affiliate with the specified id
            var affiliate = await AffiliateService.GetAffiliateByIdAsync(searchModel.AffliateId)
                ?? throw new ArgumentException("No affiliate found with the specified id");

            //prepare model
            var model = await AffiliateModelFactory.PrepareAffiliatedOrderListModelAsync(searchModel, affiliate);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AffiliatedCustomerList(AffiliatedCustomerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAffiliates))
                return await AccessDeniedDataTablesJson();

            //try to get an affiliate with the specified id
            var affiliate = await AffiliateService.GetAffiliateByIdAsync(searchModel.AffliateId)
                ?? throw new ArgumentException("No affiliate found with the specified id");

            //prepare model
            var model = await AffiliateModelFactory.PrepareAffiliatedCustomerListModelAsync(searchModel, affiliate);

            return Json(model);
        }

        #endregion
    }
}