using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Weixin;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Weixin;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Nop.Services.Weixin;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the weixin model factory implementation
    /// </summary>
    public partial class WeixinModelFactory : IWeixinModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWUserService _wUserService;
        private readonly IWUserTagService _wUserTagService;
        private readonly IWQrCodeLimitService _wQrCodeLimitService;
        private readonly IWQrCodeLimitUserService _wQrCodeLimitUserService;
        private readonly IQrCodeLimitBindingSourceService _qrCodeLimitBindingSourceService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public WeixinModelFactory(
            CurrencySettings currencySettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            IOrderService orderService,
            IPictureService pictureService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            ISettingModelFactory settingModelFactory,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWUserService wUserService,
            IWUserTagService wUserTagService,
            IWQrCodeLimitService wQrCodeLimitService,
            IWQrCodeLimitUserService wQrCodeLimitUserService,
            IQrCodeLimitBindingSourceService qrCodeLimitBindingSourceService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _currencySettings = currencySettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _orderService = orderService;
            _pictureService = pictureService;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _productTagService = productTagService;
            _productTemplateService = productTemplateService;
            _settingModelFactory = settingModelFactory;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _wUserService = wUserService;
            _wUserTagService = wUserTagService;
            _wQrCodeLimitService = wQrCodeLimitService;
            _wQrCodeLimitUserService = wQrCodeLimitUserService;
            _qrCodeLimitBindingSourceService = qrCodeLimitBindingSourceService;
            _workContext = workContext;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods


        #region UserModel

        /// <summary>
        /// Prepare User model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="product">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product model</returns>
        public virtual UserModel PrepareUserModel(UserModel model, WUser user, bool excludeProperties = false)
        {
            if (user != null)
            {
                //fill in model values from the entity
                model ??= new UserModel();

                model.Id = user.Id;

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.OpenId = user.OpenId;
                    model.RefereeId = user.RefereeId;
                    model.WConfigId = user.WConfigId;
                    model.OpenIdHash = user.OpenIdHash;
                    model.UnionId = user.UnionId;
                    model.NickName = user.NickName;
                    model.Province = user.Province;
                    model.City = user.City;
                    model.Country = user.Country;
                    model.HeadImgUrl = Senparc.Weixin.MP.CommonService.Utilities.HeadImageUrlHelper.GetHeadImageUrl(user.HeadImgUrl);
                    model.Remark = user.Remark;
                    model.SysRemark = user.SysRemark;
                    model.GroupId = user.GroupId;
                    model.TagIdList = user.TagIdList;
                    model.Sex = user.Sex;
                    model.CheckInTypeId = user.CheckInTypeId;
                    model.LanguageTypeId = user.LanguageTypeId;
                    model.SubscribeSceneTypeId = user.SubscribeSceneTypeId;
                    model.RoleTypeId = user.RoleTypeId;
                    model.SceneTypeId = user.SceneTypeId;
                    model.Status = user.Status;
                    model.SupplierShopId = user.SupplierShopId;
                    model.QrScene = user.QrScene;
                    model.QrSceneStr = user.QrSceneStr;
                    model.Subscribe = user.Subscribe;
                    model.AllowReferee = user.AllowReferee;
                    model.AllowResponse = user.AllowResponse;
                    model.AllowOrder = user.AllowOrder;
                    model.AllowNotice = user.AllowNotice;
                    model.AllowOrderNotice = user.AllowOrderNotice;
                    model.InBlackList = user.InBlackList;
                    model.Deleted = user.Deleted;

                    if (user.SubscribeTime == 0)
                        model.SubscribeTime = null;
                    else
                        model.SubscribeTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.SubscribeTime);

                    if (user.UnSubscribeTime == 0)
                        model.UnSubscribeTime = null;
                    else
                        model.UnSubscribeTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.UnSubscribeTime);

                    if (user.UpdateTime == 0)
                        model.UpdateTime = null;
                    else
                        model.UpdateTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.UpdateTime);

                    model.CreatTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.CreatTime);
                }

            }
            else
            {
                
            }

            //set default values for the new model
            if (user == null)
            {
                
            }

            return model;
        }

        /// <summary>
        /// Prepare User search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User search model</returns>
        public virtual UserSearchModel PrepareUserSearchModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged User list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>User list model</returns>
        public virtual UserListModel PrepareUserListModel(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get users
            var users = _wUserService.GetAllUsers(
                nickName: searchModel.SearchUserNickName,
                remark: searchModel.SearchUserRemark,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize) ;

            //prepare list model
            var model = new UserListModel().PrepareToGrid(searchModel, users, () =>
            {
                return users.Select(user =>
                {
                    //fill in model values from the entity
                    var userModel = user.ToModel<UserModel>();

                    if (!string.IsNullOrEmpty(user.HeadImgUrl))
                        userModel.HeadImgUrl = Senparc.Weixin.MP.CommonService.Utilities.HeadImageUrlHelper.GetHeadImageUrl(user.HeadImgUrl);

                    //convert dates to the user time
                    if(user.SubscribeTime>0)
                        userModel.SubscribeTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.SubscribeTime);
                    if (user.UnSubscribeTime > 0)
                        userModel.UnSubscribeTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.UnSubscribeTime);
                    if (user.UpdateTime > 0)
                        userModel.UpdateTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.UpdateTime);
                    if (user.CreatTime > 0)
                        userModel.CreatTime = Nop.Core.Weixin.Helpers.DateTimeHelper.GetDateTimeFromXml(user.CreatTime);

                    return userModel;
                });
            });

            return model;
        }

        #endregion

        #region QrCodeLimitModel

        /// <summary>
        /// Prepare QrCodeLimit model
        /// </summary>
        /// <param name="model">QrCodeLimit model</param>
        /// <param name="QrCodeLimit">QrCodeLimit</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product model</returns>
        public virtual QrCodeLimitModel PrepareQrCodeLimitModel(QrCodeLimitModel model, WQrCodeLimit qrCodeLimit, bool excludeProperties = false)
        {
            if (qrCodeLimit != null)
            {
                //fill in model values from the entity
                model ??= new QrCodeLimitModel();

                model.Id = qrCodeLimit.Id;

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.QrCodeId = qrCodeLimit.QrCodeId;
                    model.WConfigId = qrCodeLimit.WConfigId ?? 0;
                    model.WQrCodeCategoryId = qrCodeLimit.WQrCodeCategoryId ?? 0;
                    model.WQrCodeChannelId = qrCodeLimit.WQrCodeChannelId ?? 0;
                    model.QrCodeActionTypeId = qrCodeLimit.QrCodeActionTypeId;
                    model.SysName = qrCodeLimit.SysName;
                    model.Description = qrCodeLimit.Description;
                    model.Ticket = qrCodeLimit.Ticket;
                    model.Url = qrCodeLimit.Url;
                    model.SceneStr = qrCodeLimit.SceneStr;
                    model.TagIdList = qrCodeLimit.TagIdList;
                    model.FixedUse = qrCodeLimit.FixedUse;

                    var qrCodeLimitBindingSource = _qrCodeLimitBindingSourceService.GetEntityByQrcodeLimitId(qrCodeLimit.Id);
                    if (qrCodeLimitBindingSource != null)
                    {
                        var qrCodeLimitBindingSourceModel = qrCodeLimitBindingSource.ToModel<QrCodeLimitBindingSourceModel>();
                        model.BindingSource = qrCodeLimitBindingSourceModel;
                    }
                }
            }
            else
            {

            }

            //set default values for the new model
            if (qrCodeLimit == null)
            {
                model.FixedUse = false;
            }

            return model;
        }

        /// <summary>
        /// Prepare QrCodeLimit search model
        /// </summary>
        /// <param name="searchModel">QrCodeLimit search model</param>
        /// <returns>QrCodeLimit search model</returns>
        public virtual QrCodeLimitSearchModel PrepareQrCodeLimitSearchModel(QrCodeLimitSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged QrCodeLimit list model
        /// </summary>
        /// <param name="searchModel">QrCodeLimit search model</param>
        /// <returns>QrCodeLimit list model</returns>
        public virtual QrCodeLimitListModel PrepareQrCodeLimitListModel(QrCodeLimitSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get users
            var qrCodeLimits =  _wQrCodeLimitService.GetWQrCodeLimits(
                wConfigId: searchModel.WConfigId,
                wQrCodeCategoryId:searchModel.WQrCodeCategoryId,
                wQrCodeChannelId:searchModel.WQrCodeChannelId,
                fixedUse:searchModel.SearchFixedUse,
                hasCreated:searchModel.SearchHasCreated,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new QrCodeLimitListModel().PrepareToGrid(searchModel, qrCodeLimits, () =>
            {
                return qrCodeLimits.Select(qrCodeLimit =>
                {
                    //fill in model values from the entity
                    var qrCodeLimitModel = qrCodeLimit.ToModel<QrCodeLimitModel>();

                    return qrCodeLimitModel;
                });
            });

            return model;
        }

        public virtual QrCodeLimitUserListModel PrepareQrCodeLimitUserListModel(QrCodeLimitUserSearchModel searchModel, WQrCodeLimit qrCodeLimit)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (qrCodeLimit == null)
                throw new ArgumentNullException(nameof(qrCodeLimit));

            var qrCodeLimitUsers = _wQrCodeLimitUserService.GetEntities(
                userId: searchModel.UserId,
                qrCodeLimitId: qrCodeLimit.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new QrCodeLimitUserListModel().PrepareToGrid(searchModel, qrCodeLimitUsers, () =>
            {
                return qrCodeLimitUsers.Select(qrCodeLimitUser =>
                {
                    //fill in model values from the entity
                    var qrCodeLimitUserModel = qrCodeLimitUser.ToModel<QrCodeLimitUserModel>();

                    var user = _wUserService.GetWUserById(qrCodeLimitUser.UserId);
                    if (user != null)
                    {
                        if (string.IsNullOrWhiteSpace(qrCodeLimitUserModel.UserName))
                            qrCodeLimitUserModel.UserName = user.NickName + (string.IsNullOrWhiteSpace(user.Remark) ? "" : " (" + user.Remark + ")");

                        qrCodeLimitUserModel.HeadImageUrl = Senparc.Weixin.MP.CommonService.Utilities.HeadImageUrlHelper.GetHeadImageUrl(user.HeadImgUrl, 64);
                    }

                    return qrCodeLimitUserModel;
                });
            });

            return model;
        }

        public virtual AddUserRelatedSearchModel PrepareAddUserRelatedSearchModel(AddUserRelatedSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged related product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>Related product list model to add to the product</returns>
        public virtual AddUserRelatedUserListModel PrepareAddUserRelatedUserListModel(AddUserRelatedSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get users
            var users = _wUserService.GetAllUsers(
                nickName: searchModel.SearchUserNickName,
                remark: searchModel.SearchUserRemark,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddUserRelatedUserListModel().PrepareToGrid(searchModel, users, () =>
            {
                return users.Select(user =>
                {
                    var userModel = user.ToModel<AddUserRelatedUserModel>();

                    userModel.NickName = user.NickName + (string.IsNullOrWhiteSpace(user.Remark) ? "" : " (" + user.Remark + ")");
                    userModel.HeadImgUrl = Senparc.Weixin.MP.CommonService.Utilities.HeadImageUrlHelper.GetHeadImageUrl(user.HeadImgUrl, 64);
                    return userModel;
                });
            });

            return model;
        }



        #endregion


        #endregion
    }
}