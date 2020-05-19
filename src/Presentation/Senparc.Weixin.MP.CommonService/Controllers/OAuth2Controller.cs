using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.Exceptions;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.CommonAPIs;
using System.Text;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using Nop.Core;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Weixin;
using Nop.Services.Weixin;
using Senparc.Weixin.MP.CommonService.Utilities;

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

        #endregion

        #region Ctor

        public OAuth2Controller(INopFileProvider fileProvider,
            IWUserService wUserService,
            IWebHelper webHelper,
            IWorkContext workContext,
            SenparcWeixinSetting senparcWeixinSetting)
        {
            _fileProvider = fileProvider;
            _wUserService = wUserService;
            _webHelper = webHelper;
            _workContext = workContext;
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

            //判断是否userinfo授权
            var useUserinfo = stateSession.StartsWith("userinfo", StringComparison.InvariantCultureIgnoreCase);

            //为了安全清除session
            HttpContext.Session.Remove(NopWeixinDefaults.WeixinOauthStateString);

            OAuthAccessTokenResult accessTokenResult = null;

            //通过，用code换取access_token
            try
            {
                accessTokenResult = OAuthApi.GetAccessToken(_senparcWeixinSetting.WeixinAppId, _senparcWeixinSetting.Token, code);

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

            //初始化Session信息
            var oauthSession = new OauthSession
            {
                OpenId = accessTokenResult.openid,
                AccessToken = accessTokenResult.access_token,
                RefreshToken = accessTokenResult.refresh_token,
                CreatTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)
            };

            //保存session
            HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

            //不确定是否已经关注，这里都进行一次用户信息获取尝试
            try
            {
                var localWUserInfo = _wUserService.GetWUserByOpenId(accessTokenResult.openid);
                if (localWUserInfo != null)
                {
                    //本地已经保存，使用本地信息
                    //超过10小时未更新，尝试获取最新信息，并更新
                    if (localWUserInfo.UpdateTime + 36000 < Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now))
                    {
                        var userInfo = AdvancedAPIs.UserApi.Info(_senparcWeixinSetting.WeixinAppId, accessTokenResult.openid);
                        if (userInfo != null && userInfo.errcode != ReturnCode.请求成功)
                        {
                            //已经关注
                            if (userInfo.subscribe == 1)
                            {
                                #region 本地已保存/需更新/请求成功/已关注

                                //Session赋值
                                oauthSession.UserBaseInfo.HeadImgUrl = userInfo.headimgurl;
                                oauthSession.UserBaseInfo.NickName = userInfo.nickname;
                                oauthSession.UserBaseInfo.OpenId = userInfo.openid;
                                oauthSession.UserBaseInfo.Subscribe = true;
                                oauthSession.UserBaseInfo.SubscribeTime = (int)userInfo.subscribe_time;
                                oauthSession.UserBaseInfo.UnionId = userInfo.unionid;
                                //保存session
                                HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                //WUser更新
                                localWUserInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userInfo.headimgurl);
                                localWUserInfo.NickName = userInfo.nickname;
                                localWUserInfo.Subscribe = true;
                                localWUserInfo.SubscribeTime = (int)userInfo.subscribe_time;
                                localWUserInfo.UnionId = userInfo.unionid;
                                localWUserInfo.City = userInfo.city;
                                localWUserInfo.Country = userInfo.country;
                                localWUserInfo.Province = userInfo.province;
                                localWUserInfo.Sex = Convert.ToByte(userInfo.sex);
                                localWUserInfo.UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                                //update
                                _wUserService.UpdateWUser(localWUserInfo);

                                #endregion
                            }
                            else
                            {
                                #region 本地已保存/需更新/请求成功/未关注

                                //又取消关注了，先使用本地已经保存信息，再尝试使用OauthApi获取信息
                                //Session赋值
                                oauthSession.UserBaseInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrl(localWUserInfo.HeadImgUrl);
                                oauthSession.UserBaseInfo.NickName = localWUserInfo.NickName;
                                oauthSession.UserBaseInfo.OpenId = localWUserInfo.OpenId;
                                oauthSession.UserBaseInfo.Subscribe = false;
                                oauthSession.UserBaseInfo.UnSubscribeTime = localWUserInfo.UnSubscribeTime;
                                oauthSession.UserBaseInfo.UnionId = localWUserInfo.UnionId;
                                //保存session
                                HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                //先更新关注状态，保持数据库同步
                                localWUserInfo.Subscribe = false;
                                _wUserService.UpdateWUser(localWUserInfo);

                                //开始尝试使用OauthApi获取
                                if (useUserinfo)
                                {
                                    var userBaseInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                                    if (userBaseInfo != null)
                                    {
                                        //Session赋值
                                        oauthSession.UserBaseInfo.HeadImgUrl = userBaseInfo.headimgurl;
                                        oauthSession.UserBaseInfo.NickName = userBaseInfo.nickname;
                                        oauthSession.UserBaseInfo.OpenId = userBaseInfo.openid;
                                        oauthSession.UserBaseInfo.Subscribe = false;
                                        oauthSession.UserBaseInfo.UnionId = userBaseInfo.unionid;
                                        //保存session
                                        HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                        //WUser更新
                                        localWUserInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userBaseInfo.headimgurl);
                                        localWUserInfo.NickName = userBaseInfo.nickname;
                                        localWUserInfo.Subscribe = false;
                                        localWUserInfo.UnionId = userBaseInfo.unionid;
                                        localWUserInfo.City = userBaseInfo.city;
                                        localWUserInfo.Country = userBaseInfo.country;
                                        localWUserInfo.Province = userBaseInfo.province;
                                        localWUserInfo.Sex = Convert.ToByte(userBaseInfo.sex);
                                        localWUserInfo.UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                                        //update
                                        _wUserService.UpdateWUser(localWUserInfo);
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            #region 本地已保存/需更新/请求失败/未知关注
                            //userInfo 获取失败，先使用默认
                            //Session赋值
                            oauthSession.UserBaseInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrl(localWUserInfo.HeadImgUrl);
                            oauthSession.UserBaseInfo.NickName = localWUserInfo.NickName;
                            oauthSession.UserBaseInfo.OpenId = localWUserInfo.OpenId;
                            oauthSession.UserBaseInfo.Subscribe = localWUserInfo.Subscribe;
                            oauthSession.UserBaseInfo.SubscribeTime = localWUserInfo.SubscribeTime;
                            oauthSession.UserBaseInfo.UnSubscribeTime = localWUserInfo.UnSubscribeTime;
                            oauthSession.UserBaseInfo.UnionId = localWUserInfo.UnionId;
                            //保存session
                            HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                            //开始尝试使用OauthApi获取
                            if (useUserinfo)
                            {
                                var userBaseInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                                if (userBaseInfo != null)
                                {
                                    //Session赋值
                                    oauthSession.UserBaseInfo.HeadImgUrl = userBaseInfo.headimgurl;
                                    oauthSession.UserBaseInfo.NickName = userBaseInfo.nickname;
                                    oauthSession.UserBaseInfo.OpenId = userBaseInfo.openid;
                                    oauthSession.UserBaseInfo.UnionId = userBaseInfo.unionid;

                                    //保存session
                                    HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                    //WUser更新
                                    localWUserInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userBaseInfo.headimgurl);
                                    localWUserInfo.NickName = userBaseInfo.nickname;
                                    localWUserInfo.UnionId = userBaseInfo.unionid;
                                    localWUserInfo.City = userBaseInfo.city;
                                    localWUserInfo.Country = userBaseInfo.country;
                                    localWUserInfo.Province = userBaseInfo.province;
                                    localWUserInfo.Sex = Convert.ToByte(userBaseInfo.sex);
                                    localWUserInfo.UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                                    //update
                                    _wUserService.UpdateWUser(localWUserInfo);
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        #region 本地已保存/不更新/不请求/未知关注

                        //本地已保存，不需要更新
                        //Session赋值
                        oauthSession.UserBaseInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrl(localWUserInfo.HeadImgUrl);
                        oauthSession.UserBaseInfo.NickName = localWUserInfo.NickName;
                        oauthSession.UserBaseInfo.OpenId = localWUserInfo.OpenId;
                        oauthSession.UserBaseInfo.Subscribe = localWUserInfo.Subscribe;
                        oauthSession.UserBaseInfo.SubscribeTime = localWUserInfo.SubscribeTime;
                        oauthSession.UserBaseInfo.UnSubscribeTime = localWUserInfo.UnSubscribeTime;
                        oauthSession.UserBaseInfo.UnionId = localWUserInfo.UnionId;
                        //保存session
                        HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                        //开始尝试使用OauthApi获取
                        if (useUserinfo)
                        {
                            var userBaseInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                            if (userBaseInfo != null)
                            {
                                //Session赋值
                                oauthSession.UserBaseInfo.HeadImgUrl = userBaseInfo.headimgurl;
                                oauthSession.UserBaseInfo.NickName = userBaseInfo.nickname;
                                oauthSession.UserBaseInfo.OpenId = userBaseInfo.openid;
                                oauthSession.UserBaseInfo.UnionId = userBaseInfo.unionid;

                                //保存session
                                HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                //WUser更新
                                localWUserInfo.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userBaseInfo.headimgurl);
                                localWUserInfo.NickName = userBaseInfo.nickname;
                                localWUserInfo.UnionId = userBaseInfo.unionid;
                                localWUserInfo.City = userBaseInfo.city;
                                localWUserInfo.Country = userBaseInfo.country;
                                localWUserInfo.Province = userBaseInfo.province;
                                localWUserInfo.Sex = Convert.ToByte(userBaseInfo.sex);
                                localWUserInfo.UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now);
                                //update
                                _wUserService.UpdateWUser(localWUserInfo);
                            }
                        }

                        #endregion
                    }
                }
                else
                {

                    //本地未保存，插入
                    var initUser = new WUser
                    {
                        OpenId = accessTokenResult.openid,
                        RefereeId = 0,
                        WConfigId = 0,
                        OpenIdHash = CommonHelper.StringToLong(accessTokenResult.openid),
                        CheckInType = WCheckInType.Oauth,
                        LanguageType = WLanguageType.ZH_CN,
                        Sex = 0,
                        RoleType = WRoleType.Visitor,
                        SceneType = WSceneType.None,
                        Status = 0,
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
                        UpdateTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now),
                        CreatTime = (int)Nop.Core.Weixin.Helpers.DateTimeHelper.GetUnixDateTime(DateTime.Now)
                    };

                    var userInfo = AdvancedAPIs.UserApi.Info(_senparcWeixinSetting.WeixinAppId, accessTokenResult.openid);
                    if (userInfo != null && userInfo.errcode == ReturnCode.请求成功)
                    {
                        //userInfo请求成功
                        if (userInfo.subscribe == 1)
                        {
                            #region 本地未保存/请求成功/已关注

                            //已经关注
                            initUser.UnionId = userInfo.unionid;
                            initUser.NickName = userInfo.nickname;
                            initUser.Province = userInfo.province;
                            initUser.City = userInfo.city;
                            initUser.Country = userInfo.country;
                            initUser.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userInfo.headimgurl);
                            initUser.Remark = userInfo.remark;
                            initUser.GroupId = userInfo.groupid.ToString();
                            initUser.TagIdList = string.Join(",", userInfo.tagid_list) + (userInfo.tagid_list.Length > 0 ? "," : "");
                            initUser.Sex = Convert.ToByte(userInfo.sex);
                            initUser.LanguageType = Enum.TryParse(typeof(WLanguageType), userInfo.language, true, out var wLanguageType) ? (WLanguageType)wLanguageType : WLanguageType.ZH_CN;
                            initUser.SubscribeSceneType = Enum.TryParse(typeof(WSubscribeSceneType), userInfo.subscribe_scene, true, out var wSubscribeSceneType) ? (WSubscribeSceneType)wSubscribeSceneType : WSubscribeSceneType.ADD_SCENE_OTHERS;
                            initUser.UpdateTime = (int)userInfo.subscribe_time;
                            initUser.SubscribeTime = (int)userInfo.subscribe_time;
                            initUser.Subscribe = true;
                            //insert
                            _wUserService.InsertWUser(initUser);

                            //Session赋值
                            oauthSession.UserBaseInfo.HeadImgUrl = userInfo.headimgurl;
                            oauthSession.UserBaseInfo.NickName = userInfo.nickname;
                            oauthSession.UserBaseInfo.OpenId = userInfo.openid;
                            oauthSession.UserBaseInfo.UnionId = userInfo.unionid;
                            oauthSession.UserBaseInfo.Subscribe = true;
                            oauthSession.UserBaseInfo.SubscribeTime = (int)userInfo.subscribe_time;

                            //保存session
                            HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                            #endregion
                        }
                        else
                        {
                            #region 本地未保存/请求成功/未关注

                            //未关注
                            //开始尝试使用OauthApi获取
                            if (useUserinfo)
                            {
                                var userBaseInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                                if (userBaseInfo != null)
                                {
                                    //Session赋值
                                    oauthSession.UserBaseInfo.HeadImgUrl = userBaseInfo.headimgurl;
                                    oauthSession.UserBaseInfo.NickName = userBaseInfo.nickname;
                                    oauthSession.UserBaseInfo.OpenId = userBaseInfo.openid;
                                    oauthSession.UserBaseInfo.UnionId = userBaseInfo.unionid;
                                    oauthSession.UserBaseInfo.Subscribe = false;

                                    //保存session
                                    HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                    //initUser更新
                                    initUser.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userBaseInfo.headimgurl);
                                    initUser.NickName = userBaseInfo.nickname;
                                    initUser.UnionId = userBaseInfo.unionid;
                                    initUser.City = userBaseInfo.city;
                                    initUser.Country = userBaseInfo.country;
                                    initUser.Province = userBaseInfo.province;
                                    initUser.Sex = Convert.ToByte(userBaseInfo.sex);
                                    //insert
                                    _wUserService.InsertWUser(initUser);
                                }
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        #region 本地未保存/请求失败/未知关注

                        //userInfo请求失败
                        //开始尝试使用OauthApi获取
                        if (useUserinfo)
                        {
                            var userBaseInfo = OAuthApi.GetUserInfo(accessTokenResult.access_token, accessTokenResult.openid);
                            if (userBaseInfo != null)
                            {
                                //Session赋值
                                oauthSession.UserBaseInfo.HeadImgUrl = userBaseInfo.headimgurl;
                                oauthSession.UserBaseInfo.NickName = userBaseInfo.nickname;
                                oauthSession.UserBaseInfo.OpenId = userBaseInfo.openid;
                                oauthSession.UserBaseInfo.UnionId = userBaseInfo.unionid;
                                oauthSession.UserBaseInfo.Subscribe = false;

                                //保存session
                                HttpContext.Session.Set(NopWeixinDefaults.WeixinOauthSession, oauthSession);

                                //initUser更新
                                initUser.HeadImgUrl = HeadImageUrlHelper.GetHeadImageUrlKey(userBaseInfo.headimgurl);
                                initUser.NickName = userBaseInfo.nickname;
                                initUser.UnionId = userBaseInfo.unionid;
                                initUser.City = userBaseInfo.city;
                                initUser.Country = userBaseInfo.country;
                                initUser.Province = userBaseInfo.province;
                                initUser.Sex = Convert.ToByte(userBaseInfo.sex);
                                //insert
                                _wUserService.InsertWUser(initUser);
                            }
                        }

                        #endregion
                    }
                }
            }
            catch
            {
                //do nothing 不做任何操作，获取用户基本信息，这里不能影响跳转操作
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            //默认消息
            return Content("SUCCESS.");
        }
    }
}
