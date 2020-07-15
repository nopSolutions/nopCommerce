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
using Nop.Core.Domain.Suppliers;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Weixin;
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
using Nop.Services.Marketing;
using Nop.Services.Suppliers;
using Nop.Services.Shipping;
using Nop.Services.Weixin;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Weixin;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Senparc.Weixin;
using Senparc.Weixin.TenPay.V2;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;


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
        private readonly IWLocationService _wLocationService;
        private readonly IWQrCodeLimitService _wQrCodeLimitService;
        private readonly IWQrCodeLimitUserService _wQrCodeLimitUserService;
        private readonly IQrCodeLimitBindingSourceService _qrCodeLimitBindingSourceService;
        private readonly IQrCodeSupplierVoucherCouponMappingService _qrCodeSupplierVoucherCouponMappingService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly IWeixinModelFactory _weixinModelFactory;
        private readonly SenparcWeixinSetting _senparcWeixinSetting;

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
            IWLocationService wLocationService,
            IWQrCodeLimitService wQrCodeLimitService,
            IWQrCodeLimitUserService wQrCodeLimitUserService,
            IQrCodeLimitBindingSourceService qrCodeLimitBindingSourceService,
            IQrCodeSupplierVoucherCouponMappingService qrCodeSupplierVoucherCouponMappingService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            IWeixinModelFactory weixinModelFactory,
            SenparcWeixinSetting senparcWeixinSetting)
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
            _wLocationService = wLocationService;
            _wQrCodeLimitService = wQrCodeLimitService;
            _wQrCodeLimitUserService = wQrCodeLimitUserService;
            _qrCodeLimitBindingSourceService = qrCodeLimitBindingSourceService;
            _qrCodeSupplierVoucherCouponMappingService = qrCodeSupplierVoucherCouponMappingService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _weixinModelFactory = weixinModelFactory;
            _senparcWeixinSetting = senparcWeixinSetting;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        #region WUser Tag

        #endregion

        #region WUser Sys Tag

        #endregion

        #region WUser list / create / edit

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

            //try to get a weixin customer with the specified id
            var user = _wUserService.GetWUserById(id);
            if (user == null || user.Deleted)
                return RedirectToAction("UserList");

            //prepare model
            var model = _weixinModelFactory.PrepareUserModel(null, user);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult UserEdit(UserModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a weixin user with the specified id
            var user = _wUserService.GetWUserById(model.Id);
            if (user == null || user.Deleted)
                return RedirectToAction("UserList");

            if (ModelState.IsValid)
            {
                try
                {
                    user.Remark = model.Remark;
                    user.SysRemark = model.SysRemark;
                    user.TagIdList = model.TagIdList;
                    user.CheckInTypeId = model.CheckInTypeId;
                    user.SubscribeSceneTypeId = model.SubscribeSceneTypeId;
                    user.RoleTypeId = model.RoleTypeId;
                    user.SceneTypeId = model.SceneTypeId;
                    user.Status = model.Status;
                    user.SupplierShopId = model.SupplierShopId;
                    user.QrScene = model.QrScene;
                    user.QrSceneStr = model.QrSceneStr;
                    user.AllowReferee = model.AllowReferee;
                    user.AllowResponse = model.AllowResponse;
                    user.AllowOrder = model.AllowOrder;
                    user.AllowNotice = model.AllowNotice;
                    user.AllowOrderNotice = model.AllowOrderNotice;

                    _wUserService.UpdateWUser(user);

                    //activity log
                    _customerActivityService.InsertActivity("EditUser",
                        string.Format(_localizationService.GetResource("ActivityLog.EditUser"), user.Id), user);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Weixin.Users.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("UserList");

                    return RedirectToAction("UserEdit", new { id = user.Id });
                }
                catch (Exception exc)
                {
                    _notificationService.ErrorNotification(exc.Message);
                }
            }

            //prepare model
            model = _weixinModelFactory.PrepareUserModel(model, user, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }


        #endregion

        #region Location list

        #endregion

        #region QrCodeLimit list

        public virtual IActionResult QrCodeLimitList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //prepare model
            var model = _weixinModelFactory.PrepareQrCodeLimitSearchModel(new QrCodeLimitSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult QrCodeLimitList(QrCodeLimitSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _weixinModelFactory.PrepareQrCodeLimitListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult QrCodeLimitCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //prepare model
            var model = _weixinModelFactory.PrepareQrCodeLimitModel(new QrCodeLimitModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult QrCodeLimitCreate(QrCodeLimitModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            if(string.IsNullOrWhiteSpace(model.SysName))
                ModelState.AddModelError(string.Empty, "System Name Not Empty");


            if (ModelState.IsValid)
            {
                //fill entity from model
                var qrCodeLimit = model.ToEntity<WQrCodeLimit>();
                qrCodeLimit.QrCodeActionType = WQrCodeActionType.QR_LIMIT_SCENE;

                _wQrCodeLimitService.InsertWQrCodeLimit(qrCodeLimit);

                //插入bingdingSource
                model.BindingSource.QrCodeLimitId = qrCodeLimit.Id;
                var qrCodeLimitBindingSource = model.BindingSource.ToEntity<QrCodeLimitBindingSource>();
                _qrCodeLimitBindingSourceService.InsertEntity(qrCodeLimitBindingSource);

                //复制备份当前ID到QrCodeId中
                qrCodeLimit.QrCodeId = qrCodeLimit.Id;

                //创建qrcode Ticket
                if(string.IsNullOrWhiteSpace(qrCodeLimit.Ticket))
                {
                    var qrcodeResult = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.Create(_senparcWeixinSetting.WeixinAppId, 0, qrCodeLimit.Id, Senparc.Weixin.MP.QrCode_ActionName.QR_LIMIT_SCENE);
                    if (qrcodeResult.errcode == ReturnCode.请求成功)
                    {
                        qrCodeLimit.Ticket = qrcodeResult.ticket;
                        qrCodeLimit.Url = qrcodeResult.url;
                    }
                }

                _wQrCodeLimitService.UpdateWQrCodeLimit(qrCodeLimit);

                //activity log
                _customerActivityService.InsertActivity("AddNewQrCodeLimit",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewQrCodeLimit"), qrCodeLimit.Id), qrCodeLimit);
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Weixin.QrCodeLimits.Added"));

                if (!continueEditing)
                    return RedirectToAction("QrCodeLimitList");

                return RedirectToAction("QrCodeLimitEdit", new { id = qrCodeLimit.Id });
            }

            //prepare model
            model = _weixinModelFactory.PrepareQrCodeLimitModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult QrCodeLimitEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a weixin customer with the specified id
            var qrcodeLimit = _wQrCodeLimitService.GetWQrCodeLimitById(id);
            if (qrcodeLimit == null)
                return RedirectToAction("QrCodeLimitList");

            //prepare model
            var model = _weixinModelFactory.PrepareQrCodeLimitModel(null, qrcodeLimit);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult QrCodeLimitEdit(QrCodeLimitModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a weixin user with the specified id
            var qrcodeLimit = _wQrCodeLimitService.GetWQrCodeLimitById(model.Id);
            if (qrcodeLimit == null)
                return RedirectToAction("QrCodeLimitList");

            if (ModelState.IsValid)
            {
                try
                {
                    qrcodeLimit.SysName = model.SysName;
                    qrcodeLimit.Description = model.Description;
                    qrcodeLimit.TagIdList = model.TagIdList;
                    qrcodeLimit.FixedUse = model.FixedUse;

                    //创建qrcode Ticket
                    if (string.IsNullOrWhiteSpace(qrcodeLimit.Ticket))
                    {
                        var qrcodeResult = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.Create(_senparcWeixinSetting.WeixinAppId, 0, qrcodeLimit.Id, Senparc.Weixin.MP.QrCode_ActionName.QR_LIMIT_SCENE);
                        if (qrcodeResult.errcode == ReturnCode.请求成功)
                        {
                            qrcodeLimit.Ticket = qrcodeResult.ticket;
                            qrcodeLimit.Url = qrcodeResult.url;
                        }
                    }

                    _wQrCodeLimitService.UpdateWQrCodeLimit(qrcodeLimit);

                    //更新binding source
                    var qrCodeLimitBindingSource = _qrCodeLimitBindingSourceService.GetEntityByQrcodeLimitId(qrcodeLimit.Id);
                    if (qrCodeLimitBindingSource == null)
                    {
                        _qrCodeLimitBindingSourceService.InsertEntity(new QrCodeLimitBindingSource()
                        {
                            QrCodeLimitId = qrcodeLimit.Id,
                            SupplierId = model.BindingSource.SupplierId,
                            SupplierShopId = model.BindingSource.SupplierShopId,
                            ProductId = model.BindingSource.ProductId,
                            Address = model.BindingSource.Address,
                            MarketingAdvertWayId = model.BindingSource.MarketingAdvertWayId,
                            UseFixUrl = model.BindingSource.UseFixUrl,
                            Url = model.BindingSource.Url,
                            MessageResponse = model.BindingSource.MessageResponse,
                            WSceneTypeId = model.BindingSource.WSceneTypeId,
                            MessageTypeId = model.BindingSource.MessageTypeId,
                            Content = model.BindingSource.Content,
                            UseBindingMessage = model.BindingSource.UseBindingMessage
                        });

                    }
                    else
                    {
                        qrCodeLimitBindingSource.SupplierId = model.BindingSource.SupplierId;
                        qrCodeLimitBindingSource.SupplierShopId = model.BindingSource.SupplierShopId;
                        qrCodeLimitBindingSource.ProductId = model.BindingSource.ProductId;
                        qrCodeLimitBindingSource.Address = model.BindingSource.Address;
                        qrCodeLimitBindingSource.MarketingAdvertWayId = model.BindingSource.MarketingAdvertWayId;
                        qrCodeLimitBindingSource.UseFixUrl = model.BindingSource.UseFixUrl;
                        qrCodeLimitBindingSource.Url = model.BindingSource.Url;
                        qrCodeLimitBindingSource.MessageResponse = model.BindingSource.MessageResponse;
                        qrCodeLimitBindingSource.WSceneTypeId = model.BindingSource.WSceneTypeId;
                        qrCodeLimitBindingSource.MessageTypeId = model.BindingSource.MessageTypeId;
                        qrCodeLimitBindingSource.Content = model.BindingSource.Content;
                        qrCodeLimitBindingSource.UseBindingMessage = model.BindingSource.UseBindingMessage;

                        _qrCodeLimitBindingSourceService.UpdateEntity(qrCodeLimitBindingSource);
                    }

                    //activity log
                    _customerActivityService.InsertActivity("EditQrCodeLimit",
                        string.Format(_localizationService.GetResource("ActivityLog.EditQrCodeLimit"), qrcodeLimit.Id), qrcodeLimit);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Weixin.QrCodeLimits.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("QrCodeLimitList");

                    return RedirectToAction("QrCodeLimitEdit", new { id = qrcodeLimit.Id });
                }
                catch (Exception exc)
                {
                    _notificationService.ErrorNotification(exc.Message);
                }
            }

            //prepare model
            model = _weixinModelFactory.PrepareQrCodeLimitModel(model, qrcodeLimit, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult QrCodeLimitUserList(QrCodeLimitUserSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var qrCodeLimit = _wQrCodeLimitService.GetWQrCodeLimitById(searchModel.QrCodeLimitId)
                ?? throw new ArgumentException("No QrCodeLimit found with the specified id");

            //prepare model
            var model = _weixinModelFactory.PrepareQrCodeLimitUserListModel(searchModel, qrCodeLimit);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult QrCodeLimitUserDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //try to get a qrCodeLimitUser
            var qrCodeLimitUser = _wQrCodeLimitUserService.GetEntityById(id)
                ?? throw new ArgumentException("No QrCodeLimitUser found with the specified id");

            _wQrCodeLimitUserService.DeleteEntity(qrCodeLimitUser);

            return new NullJsonResult();
        }

        public virtual IActionResult QrCodeLimitUserAddPopup(int qrCodeLimitId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            //prepare model
            var model = _weixinModelFactory.PrepareAddUserRelatedSearchModel(new AddUserRelatedSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult QrCodeLimitUserAddPopupList(AddUserRelatedSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _weixinModelFactory.PrepareAddUserRelatedUserListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult QrCodeLimitUserAddPopup(AddUserRelatedModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWeixin))
                return AccessDeniedView();

            var selectedUsers = _wUserService.GetWUsersByIds(model.SelectedUserIds.ToArray());

            if(selectedUsers.Any())
            {
                var existingRelatedUsers = _wQrCodeLimitUserService.GetEntitiesByQrcodeLimitId(model.RelatedId);
                foreach (var user in selectedUsers)
                {
                    if (user.Deleted || !user.AllowReferee || !user.Subscribe || existingRelatedUsers.Exists(t => t.UserId == user.Id))
                        continue;

                    _wQrCodeLimitUserService.InsertEntity(new WQrCodeLimitUserMapping
                    {
                        UserId = user.Id,
                        QrCodeLimitId = model.RelatedId,
                        UserName = user.NickName + (string.IsNullOrWhiteSpace(user.Remark) ? "" : "(" + user.Remark + ")"),
                        ExpireTime = DateTime.Now.AddDays(30),
                        Published = true
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddUserRelatedSearchModel());
        }

        #endregion

        #region Menu list

        #endregion

        #region Messages list

        #endregion

        #region AutoReply Setting

        #endregion

        #endregion
    }
}