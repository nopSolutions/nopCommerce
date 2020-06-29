using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.Entities;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Weixin;
using Nop.Services.Weixin;
using Senparc.Weixin.MP.CommonService.Utilities;
using StackExchange.Profiling.Internal;
using Nop.Services.Suppliers;

namespace Senparc.Weixin.MP.CommonService.Controllers
{
    public partial class OAuth2Controller : Controller
    {
        #region Fields

        private readonly INopFileProvider _fileProvider;
        private readonly SenparcWeixinSetting _senparcWeixinSetting;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWUserService _wUserService;
        private readonly ISupplierUserAuthorityMappingService _supplierUserAuthorityMappingService;

        #endregion

        #region Ctor

        public OAuth2Controller(INopFileProvider fileProvider,
            IWUserService wUserService,
            IWebHelper webHelper,
            IWorkContext workContext,
            ISupplierUserAuthorityMappingService supplierUserAuthorityMappingService,
            SenparcWeixinSetting senparcWeixinSetting)
        {
            _fileProvider = fileProvider;
            _wUserService = wUserService;
            _webHelper = webHelper;
            _workContext = workContext;
            _supplierUserAuthorityMappingService = supplierUserAuthorityMappingService;
            _senparcWeixinSetting = senparcWeixinSetting;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">用户尝试进入的需要登录的页面</param>
        /// <returns></returns>
        public IActionResult Index(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            var stateSession = HttpContext.Session.GetString(NopWeixinDefaults.WeixinOauthStateString);

            if (string.IsNullOrEmpty(stateSession) || state != stateSession)
            {
                return Content("验证失败！");
            }

            //判断是否使用的userinfo的Oauth授权
            var isUserInfoOauthType = state.StartsWith("userinfo", StringComparison.InvariantCultureIgnoreCase);

            //为了安全清除session
            HttpContext.Session.Remove(NopWeixinDefaults.WeixinOauthStateString);

            OAuthAccessTokenResult accessTokenResult = null;

            //通过，用code换取access_token
            try
            {
                accessTokenResult = OAuthApi.GetAccessToken(_senparcWeixinSetting.WeixinAppId, _senparcWeixinSetting.WeixinAppSecret, code);
                if (accessTokenResult == null || accessTokenResult.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + accessTokenResult.errmsg);
                }
            }
            catch (Exception ex)
            {
                WeixinTrace.SendCustomLog("Oauth2Callback发生错误 GetAccessToken：", ex.ToString());
                return Content(ex.Message);
            }

            //初始化基础Session信息
            var oauthSession = new OauthSession
            {
                OpenId = accessTokenResult.openid,
                AccessToken = accessTokenResult.access_token,
                RefreshToken = accessTokenResult.refresh_token,
                CreatTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)
            };

            //保存基础session信息
            HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

            //初始化用户数据库基础信息
            #region 初始化用户数据库基础信息
            var insertCurrentUser = false;
            var needUpdateCurrentUser = false;
            var currentUser = _wUserService.GetWUserByOpenId(accessTokenResult.openid);
            if (currentUser == null)
            {
                insertCurrentUser = true; //需要插入
                needUpdateCurrentUser = true;// 需要更新（插入即更新）
                currentUser = new WUser
                {
                    OpenId = accessTokenResult.openid,
                    RefereeId = 0, //从State参数中分离查找
                    WConfigId = 0,
                    OpenIdHash = CommonHelper.StringToLong(accessTokenResult.openid),
                    CheckInType = WCheckInType.Oauth,  //每个渠道不同
                    LanguageType = WLanguageType.ZH_CN,
                    Sex = 0,
                    RoleType = WRoleType.Visitor,
                    SceneType = WSceneType.None,
                    Status = 0,
                    SupplierShopId = 0,  //需要参数传入重新赋值
                    QrScene = 0,
                    Subscribe = false,
                    AllowReferee = true,
                    AllowResponse = true,
                    AllowOrder = true,
                    AllowNotice = false,
                    AllowOrderNotice = false,
                    InBlackList = false,
                    Deleted = false,
                    SubscribeTime = 0,
                    UnSubscribeTime = 0,
                    UpdateTime = 0,
                    CreatTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)
                };

                //获取推荐人Id
                var stateParams = state.Split("_", StringSplitOptions.RemoveEmptyEntries);
                if (stateParams.Length == 2 && !string.IsNullOrEmpty(stateParams[1]))
                {
                    long.TryParse(stateParams[1], out var refereeOpenIdHash);
                    if (refereeOpenIdHash > 0)
                    {
                        var refereeUser = _wUserService.GetWUserByOpenIdHash(refereeOpenIdHash);
                        if (refereeUser != null)
                            currentUser.RefereeId = refereeUser.Id;
                    }
                    else
                    {
                        var refereeUser = _wUserService.GetWUserByOpenId(stateParams[1]);
                        if (refereeUser != null)
                            currentUser.RefereeId = refereeUser.Id;
                    }
                }

                //获取SupplierShopId
                var supplierUserAuthorityMapping = _supplierUserAuthorityMappingService.GetEntityByUserId(currentUser.RefereeId);
                if (supplierUserAuthorityMapping != null)
                    currentUser.SupplierShopId = supplierUserAuthorityMapping.SupplierShopId ?? 0;
            }

            #endregion

            try
            {
                //使用UserApi获取用户基础信息
                #region 使用UserApi获取用户基础信息
                var userInfoGetSuccess = false; //UserApi获取是否成功
                if (currentUser.UpdateTime + 36000 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now))
                {
                    var userInfo = AdvancedAPIs.UserApi.Info(_senparcWeixinSetting.WeixinAppId, accessTokenResult.openid);
                    if (userInfo != null && userInfo.errcode == ReturnCode.请求成功)
                    {
                        needUpdateCurrentUser = true;// 需要更新
                        if (userInfo.subscribe == 1)
                        {
                            userInfoGetSuccess = true;  //UserApi获取成功

                            currentUser.Subscribe = true;
                            currentUser.NickName = userInfo.nickname;
                            currentUser.Sex = Convert.ToByte(userInfo.sex);
                            currentUser.LanguageType = Enum.TryParse(typeof(WLanguageType), userInfo.language, true, out var outLanguage) ? (WLanguageType)outLanguage : WLanguageType.ZH_CN;
                            currentUser.City = userInfo.city;
                            currentUser.Province = userInfo.province;
                            currentUser.Country = userInfo.country;
                            currentUser.HeadImgUrl = Utilities.HeadImageUrlHelper.GetHeadImageUrlKey(userInfo.headimgurl);
                            currentUser.SubscribeTime = (int)userInfo.subscribe_time;
                            currentUser.UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                            currentUser.UnionId = userInfo.unionid;
                            currentUser.Remark = userInfo.remark;
                            currentUser.GroupId = userInfo.groupid.ToString();
                            currentUser.TagIdList = string.Join(",", userInfo.tagid_list) + (userInfo.tagid_list.Length > 0 ? "," : "");
                            currentUser.SubscribeSceneType = Enum.TryParse(typeof(WSubscribeSceneType), userInfo.subscribe_scene, true, out var wSubscribeSceneType) ? (WSubscribeSceneType)wSubscribeSceneType : WSubscribeSceneType.ADD_SCENE_OTHERS;
                        }
                        else
                        {
                            currentUser.Subscribe = false;
                        }
                    }
                }

                #endregion

                //使用SnapUserInfo获取用户基础信息
                #region 使用SnapUserInfo获取用户基础信息
                if (!userInfoGetSuccess && isUserInfoOauthType)
                {
                    var userBaseInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                    if (userBaseInfo != null && !string.IsNullOrEmpty(userBaseInfo.nickname))
                    {
                        needUpdateCurrentUser = true;// 需要更新

                        currentUser.NickName = userBaseInfo.nickname;
                        currentUser.Sex = Convert.ToByte(userBaseInfo.sex);
                        currentUser.City = userBaseInfo.city;
                        currentUser.Province = userBaseInfo.province;
                        currentUser.Country = userBaseInfo.country;
                        currentUser.HeadImgUrl = Utilities.HeadImageUrlHelper.GetHeadImageUrlKey(userBaseInfo.headimgurl);
                        currentUser.UnionId = userBaseInfo.unionid;
                        currentUser.UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                    }
                }
                #endregion

            }
            catch
            {
                //do nothing 进入该步骤表示授权成功，能否获取用户基本信息不重要，这里不能影响跳转操作
            }

            //更新oauthSession信息
            #region 更新oauthSession信息

            oauthSession.UserBaseInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrl(currentUser.HeadImgUrl);
            oauthSession.UserBaseInfo.NickName = currentUser.NickName;
            oauthSession.UserBaseInfo.OpenId = currentUser.OpenId;
            oauthSession.UserBaseInfo.Subscribe = currentUser.Subscribe;
            oauthSession.UserBaseInfo.SubscribeTime = currentUser.SubscribeTime;
            oauthSession.UserBaseInfo.UnSubscribeTime = currentUser.UnSubscribeTime;
            oauthSession.UserBaseInfo.UnionId = currentUser.UnionId;
            //保存更新
            HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

            #endregion

            //用户基础信息插入/更新
            #region 用户基础信息插入/更新
            if (insertCurrentUser)
                _wUserService.InsertWUser(currentUser);
            else if (needUpdateCurrentUser)
                _wUserService.UpdateWUser(currentUser);

            #endregion


            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            //默认消息
            return Content("SUCCESS.");
        }
    }
}
