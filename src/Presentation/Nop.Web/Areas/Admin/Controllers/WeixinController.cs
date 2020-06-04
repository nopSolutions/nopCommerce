using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Weixin;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Weixin;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class WeixinController : BaseAdminController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IWUserService _wUserService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly IWeixinModelFactory _weixinModelFactory;

        #endregion

        #region Ctor

        public WeixinController(
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IWUserService wUserService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            IWeixinModelFactory weixinModelFactory)
        {
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _wUserService = wUserService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _weixinModelFactory = weixinModelFactory;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        #region WUser list / create / edit / delete


        public virtual IActionResult UserList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //prepare model
            var model = _weixinModelFactory.PrepareUserSearchModel(new UserSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult UserList(UserSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _weixinModelFactory.PrepareUserListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult UserEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _wUserService.GetWUserById(id);
            if (user == null)
                return RedirectToAction("UserList");

            //prepare model
            var model = _weixinModelFactory.PrepareUserModel(null, user);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult UserEdit(UserModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a product with the specified id
            var user = _wUserService.GetWUserById(model.Id);
            if (user == null)
                return RedirectToAction("UserList");

            if (ModelState.IsValid)
            {

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Weixin.Users.Updated"));

                if (!continueEditing)
                    return RedirectToAction("UserList");

                return RedirectToAction("UserEdit", new { id = user.Id });
            }

            //prepare model
            model = _weixinModelFactory.PrepareUserModel(model, user, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a user with the specified id
            var user = _wUserService.GetWUserById(id);
            if (user == null)
                return RedirectToAction("UserList");


            _wUserService.DeleteWUser(user);

            //activity log
            _customerActivityService.InsertActivity("DeleteWUser",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteWUser"), user.NickName), user);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Weixin.Users.Deleted"));

            return RedirectToAction("UserList");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _wUserService.DeleteWUsers(_wUserService.GetWUsersByIds(selectedIds.ToArray()));
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}