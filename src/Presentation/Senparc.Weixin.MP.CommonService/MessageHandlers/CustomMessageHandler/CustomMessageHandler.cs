/*----------------------------------------------------------------
    Copyright (C) 2020 Senparc

    文件名：CustomMessageHandler.cs
    文件功能描述：微信公众号自定义MessageHandler


    创建标识：Senparc - 20150312

    修改标识：Senparc - 20171027
    修改描述：v14.8.3 添加OnUnknownTypeRequest()方法Demo

    修改标识：Senparc - 20191002
    修改描述：v16.9.102 提供 MessageHandler 中间件

----------------------------------------------------------------*/

//DPBMARK_FILE MP
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Agents;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

#if NET45
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using Senparc.Weixin.MP.CommonService.Utilities;
#else
#endif

namespace Senparc.Weixin.MP.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>  /*如果不需要自定义，可以直接使用：MessageHandler<DefaultMpMessageContext> */
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */


#if !DEBUG || NETSTANDARD2_0 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_1
        string agentUrl = "http://localhost:12222/App/Weixin/4";
        string agentToken = "27C455F496044A87";
        string wiweihiKey = "CNadjJuWzyX5bz5Gn+/XoyqiqMa5DjXQ";
#else
        //下面的Url和Token可以用其他平台的消息，或者到www.weiweihi.com注册微信用户，将自动在“微信营销工具”下得到
        private string agentUrl = Config.SenparcWeixinSetting.AgentUrl;//这里使用了www.weiweihi.com微信自动托管平台
        private string agentToken = Config.SenparcWeixinSetting.AgentToken;//Token
        private string wiweihiKey = Config.SenparcWeixinSetting.SenparcWechatAgentKey;//WeiweihiKey专门用于对接www.Weiweihi.com平台，获取方式见：http://www.weiweihi.com/ApiDocuments/Item/25#51
#endif

        private string appId = Config.SenparcWeixinSetting.WeixinAppId;
        private string appSecret = Config.SenparcWeixinSetting.WeixinAppSecret;

        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// 注意：这里只做测试，只适用于单服务器
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, CustomMessageHandler> GenerateMessageHandler = (stream, postModel, maxRecordCount)
                        => new CustomMessageHandler(stream, postModel, maxRecordCount, false /* 是否只允许处理加密消息，以提高安全性 */);

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false)
            : base(inputStream, postModel, maxRecordCount, onlyAllowEncryptMessage)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalGlobalMessageContext.ExpireMinutes = 3。
            GlobalMessageContext.ExpireMinutes = 3;
            
            OnlyAllowEncryptMessage = true;//是否只允许接收加密消息，默认为 false

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }


        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <param name="requestMessage">请求消息</param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {
            //说明：实际项目中这里的逻辑可以交给Service处理具体信息，参考OnLocationRequest方法或/Service/LocationSercice.cs

            #region 书中例子 
            //if (requestMessage.Content == "你好")
            //{
            //    var responseMessage = base.CreateResponseMessage<ResponseMessageNews>();
            //    var title = "Title";
            //    var description = "Description";
            //    var picUrl = "PicUrl";
            //    var url = "Url";
            //    responseMessage.Articles.Add(new Article()
            //    {
            //        Title = title,
            //        Description = description,
            //        PicUrl = picUrl,
            //        Url = url
            //    });
            //    return responseMessage;
            //}
            //else if (requestMessage.Content == "Senparc")
            //{
            //    //相似处理逻辑
            //}
            //else
            //{
            //    //...
            //}

            #endregion

            #region 历史方法

            //方法一（v0.1），此方法调用太过繁琐，已过时（但仍是所有方法的核心基础），建议使用方法二到四
            //var responseMessage =
            //    ResponseMessageBase.CreateFromRequestMessage(RequestMessage, ResponseMsgType.Text) as
            //    ResponseMessageText;

            //方法二（v0.4）
            //var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(RequestMessage);

            //方法三（v0.4），扩展方法，需要using Senparc.Weixin.MP.Helpers;
            //var responseMessage = RequestMessage.CreateResponseMessage<ResponseMessageText>();

            //方法四（v0.6+），仅适合在HandlerMessage内部使用，本质上是对方法三的封装
            //注意：下面泛型ResponseMessageText即返回给客户端的类型，可以根据自己的需要填写ResponseMessageNews等不同类型。

            #endregion

            var defaultResponseMessage = base.CreateResponseMessage<ResponseMessageText>();

            var requestHandler = await requestMessage.StartHandler()
                //关键字不区分大小写，按照顺序匹配成功后将不再运行下面的逻辑
                .Keyword("约束", () =>
                {
                    defaultResponseMessage.Content =
                    @"您正在进行微信内置浏览器约束判断测试。您可以：
<a href=""https://sdk.weixin.senparc.com/FilterTest/"">点击这里</a>进行客户端约束测试（地址：https://sdk.weixin.senparc.com/FilterTest/），如果在微信外打开将直接返回文字。
或：
<a href=""https://sdk.weixin.senparc.com/FilterTest/Redirect"">点击这里</a>进行客户端约束测试（地址：https://sdk.weixin.senparc.com/FilterTest/Redirect），如果在微信外打开将重定向一次URL。";
                    return defaultResponseMessage;
                }).
                //匹配任一关键字
                Keywords(new[] { "托管", "代理" }, () =>
                {
                    //开始用代理托管，把请求转到其他服务器上去，然后拿回结果
                    //甚至也可以将所有请求在DefaultResponseMessage()中托管到外部。

                    var dt1 = SystemTime.Now; //计时开始

                    var agentXml = RequestDocument.ToString();

                    #region 暂时转发到SDK线上Demo

                    agentUrl = "https://sdk.weixin.senparc.com/weixin";
                    //agentToken = WebConfigurationManager.AppSettings["WeixinToken"];//Token

                    //修改内容，防止死循环
                    var agentDoc = XDocument.Parse(agentXml);
                    agentDoc.Root.Element("Content").SetValue("代理转发文字：" + requestMessage.Content);
                    agentDoc.Root.Element("CreateTime").SetValue(DateTimeHelper.GetUnixDateTime(SystemTime.Now));//修改时间，防止去重
                    agentDoc.Root.Element("MsgId").SetValue("123");//防止去重
                    agentXml = agentDoc.ToString();

                    #endregion

                    var responseXml = MessageAgent.RequestXml(this, agentUrl, agentToken, agentXml);
                    //获取返回的XML
                    //上面的方法也可以使用扩展方法：this.RequestResponseMessage(this,agentUrl, agentToken, RequestDocument.ToString());

                    /* 如果有WeiweihiKey，可以直接使用下面的这个MessageAgent.RequestWeiweihiXml()方法。
                    * WeiweihiKey专门用于对接www.weiweihi.com平台，获取方式见：https://www.weiweihi.com/ApiDocuments/Item/25#51
                    */
                    //var responseXml = MessageAgent.RequestWeiweihiXml(weiweihiKey, RequestDocument.ToString());//获取Weiweihi返回的XML

                    var dt2 = SystemTime.Now; //计时结束

                    //转成实体。
                    /* 如果要写成一行，可以直接用：
                    * responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
                    * 或
                    *
                    */
                    var msg = string.Format("\r\n\r\n代理过程总耗时：{0}毫秒", (dt2 - dt1).Milliseconds);
                    var agentResponseMessage = responseXml.CreateResponseMessage(this.MessageEntityEnlightener);
                    if (agentResponseMessage is ResponseMessageText)
                    {
                        (agentResponseMessage as ResponseMessageText).Content += msg;
                    }
                    else if (agentResponseMessage is ResponseMessageNews)
                    {
                        (agentResponseMessage as ResponseMessageNews).Articles[0].Description += msg;
                    }
                    return agentResponseMessage;//可能出现多种类型，直接在这里返回
                })
                .Keywords(new[] { "测试", "退出" }, () =>
                {
                    /*
                     * 这是一个特殊的过程，此请求通常来自于微微嗨（http://www.weiweihi.com）的“盛派网络小助手”应用请求（https://www.weiweihi.com/User/App/Detail/1），
                     * 用于演示微微嗨应用商店的处理过程，由于微微嗨的应用内部可以单独设置对话过期时间，所以这里通常不需要考虑对话状态，只要做最简单的响应。
                     */
                    if (defaultResponseMessage.Content == "测试")
                    {
                        //进入APP测试
                        defaultResponseMessage.Content = "您已经进入【盛派网络小助手】的测试程序，请发送任意信息进行测试。发送文字【退出】退出测试对话。10分钟内无任何交互将自动退出应用对话状态。";
                    }
                    else
                    {
                        //退出APP测试
                        defaultResponseMessage.Content = "您已经退出【盛派网络小助手】的测试程序。";
                    }
                    return defaultResponseMessage;
                })
                .Keyword("AsyncTest", () =>
                {
                    //异步并发测试（提供给单元测试使用）
#if NET45
                    var begin = SystemTime.Now;
                    int t1, t2, t3;
                    System.Threading.ThreadPool.GetAvailableThreads(out t1, out t3);
                    System.Threading.ThreadPool.GetMaxThreads(out t2, out t3);
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
                    var end = SystemTime.Now;
                    var thread = System.Threading.Thread.CurrentThread;
                    defaultResponseMessage.Content = string.Format("TId:{0}\tApp:{1}\tBegin:{2:mm:ss,ffff}\tEnd:{3:mm:ss,ffff}\tTPool：{4}",
                            thread.ManagedThreadId,
                            HttpContext.Current != null ? HttpContext.Current.ApplicationInstance.GetHashCode() : -1,
                            begin,
                            end,
                            t2 - t1
                            );
#endif

                    return defaultResponseMessage;
                })
                .Keyword("OPEN", () =>
                {
                    var openResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageNews>();
                    openResponseMessage.Articles.Add(new Article()
                    {
                        Title = "开放平台微信授权测试！",
                        Description = @"点击进入Open授权页面。

授权之后，您的微信所收到的消息将转发到第三方（盛派网络小助手）的服务器上，并获得对应的回复。

测试完成后，您可以登陆公众号后台取消授权。",
                        Url = "https://sdk.weixin.senparc.com/OpenOAuth/JumpToMpOAuth"
                    });
                    return openResponseMessage;
                })
                .Keyword("错误", () =>
                {
                    var errorResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                    //因为没有设置errorResponseMessage.Content，所以这小消息将无法正确返回。
                    return errorResponseMessage;
                })
                .Keyword("容错", () =>
                {
                    Thread.Sleep(4900);//故意延时1.5秒，让微信多次发送消息过来，观察返回结果
                    var faultTolerantResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                    faultTolerantResponseMessage.Content = string.Format("测试容错，MsgId：{0}，Ticks：{1}", requestMessage.MsgId,
                        SystemTime.Now.Ticks);
                    return faultTolerantResponseMessage;
                })
                .Keyword("TM", () =>
                {
                    var openId = requestMessage.FromUserName;
                    var checkCode = Guid.NewGuid().ToString("n").Substring(0, 3);//为了防止openId泄露造成骚扰，这里启用验证码
                    TemplateMessageCollection[checkCode] = openId;
                    defaultResponseMessage.Content = string.Format(@"新的验证码为：{0}，请在网页上输入。网址：https://sdk.weixin.senparc.com/AsyncMethods", checkCode);
                    return defaultResponseMessage;
                })
                .Keyword("OPENID", () =>
                {
                    var openId = requestMessage.FromUserName;//获取OpenId
                    var userInfo = AdvancedAPIs.UserApi.Info(appId, openId, Language.zh_CN);

                    defaultResponseMessage.Content = string.Format(
                        "您的OpenID为：{0}\r\n昵称：{1}\r\n性别：{2}\r\n地区（国家/省/市）：{3}/{4}/{5}\r\n关注时间：{6}\r\n关注状态：{7}",
                        requestMessage.FromUserName, userInfo.nickname, (WeixinSex)userInfo.sex, userInfo.country, userInfo.province, userInfo.city, DateTimeHelper.GetDateTimeFromXml(userInfo.subscribe_time), userInfo.subscribe);
                    return defaultResponseMessage;
                })
                .Keyword("EX", () =>
                {
                    var ex = new WeixinException("openid:" + requestMessage.FromUserName + ":这是一条测试异常信息");//回调过程在global的ConfigWeixinTraceLog()方法中
                    defaultResponseMessage.Content = "请等待异步模板消息发送到此界面上（自动延时数秒）。\r\n当前时间：" + SystemTime.Now.ToString();
                    return defaultResponseMessage;
                })
                .Keyword("MUTE", () => //不回复任何消息
                {
                    //方案一：
                    return new SuccessResponseMessage();

                    //方案二：
                    var muteResponseMessage = base.CreateResponseMessage<ResponseMessageNoResponse>();
                    return muteResponseMessage;

                    //方案三：
                    base.TextResponseMessage = "success";
                    return null;

                    //方案四：
                    return null;//在 Action 中结合使用 return new FixWeixinBugWeixinResult(messageHandler);
                })
                .Keyword("JSSDK", () =>
                {
                    defaultResponseMessage.Content = "点击打开：https://sdk.weixin.senparc.com/WeixinJsSdk";
                    return defaultResponseMessage;
                })


                //选择菜单，关键字：101（微信服务器端最终格式：id="s:101",content="满意"）
                .SelectMenuKeyword("101", () =>
                {
                    defaultResponseMessage.Content = $"感谢您的评价（{requestMessage.Content}）！我们会一如既往为提高企业和开发者生产力而努力！";
                    return defaultResponseMessage;
                })
                //选择菜单，关键字：102（微信服务器端最终格式：id="s:102",content="一般"）
                .SelectMenuKeyword("102", () =>
                {
                    defaultResponseMessage.Content = $"感谢您的评价（{requestMessage.Content}）！希望我们的服务能让您越来越满意！";
                    return defaultResponseMessage;
                })
                //选择菜单，关键字：103（微信服务器端最终格式：id="s:103",content="不满意"）
                .SelectMenuKeyword("103", () =>
                {
                    defaultResponseMessage.Content = $"感谢您的评价（{requestMessage.Content}）！我们需要您的意见或建议，欢迎向我们反馈！ <a href=\"https://github.com/JeffreySu/WeiXinMPSDK/issues/new\">点击这里</a>";
                    return defaultResponseMessage;
                })
                .SelectMenuKeywords(new[] { "110", "111" }, () =>
                {
                    defaultResponseMessage.Content = $"这里只是演示，可以同时支持多个选择菜单";
                    return defaultResponseMessage;
                })


                //“一次订阅消息”接口测试
                .Keyword("订阅", () =>
                {
                    defaultResponseMessage.Content = "点击打开：https://sdk.weixin.senparc.com/SubscribeMsg";
                    return defaultResponseMessage;
                })
                //正则表达式
                .Regex(@"^\d+#\d+$", () =>
                {
                    defaultResponseMessage.Content = string.Format("您输入了：{0}，符合正则表达式：^\\d+#\\d+$", requestMessage.Content);
                    return defaultResponseMessage;
                })

                //当 Default 使用异步方法时，需要写在最后一个，且 requestMessage.StartHandler() 前需要使用 await 等待异步方法执行；
                //当 Default 使用同步方法，不一定要在最后一个,并且不需要使用 await
                .Default(async () =>
                {
                    var result = new StringBuilder();
                    result.AppendFormat("您刚才发送了文字信息：{0}\r\n\r\n", requestMessage.Content);

                    var currentMessageContext = await base.GetCurrentMessageContext();
                    if (currentMessageContext.RequestMessages.Count > 1)
                    {
                        result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n", currentMessageContext.RequestMessages.Count,
                            currentMessageContext.StorageData);
                        for (int i = currentMessageContext.RequestMessages.Count - 2; i >= 0; i--)
                        {
                            var historyMessage = currentMessageContext.RequestMessages[i];
                            result.AppendFormat("{0} 【{1}】{2}\r\n",
                                historyMessage.CreateTime.ToString("HH:mm:ss"),
                                historyMessage.MsgType.ToString(),
                                (historyMessage is RequestMessageText)
                                    ? (historyMessage as RequestMessageText).Content
                                    : "[非文字类型]"
                                );
                        }
                        result.AppendLine("\r\n");
                    }

                    result.AppendFormat("如果您在{0}分钟内连续发送消息，记录将被自动保留（当前设置：最多记录{1}条）。过期后记录将会自动清除。\r\n",
                        GlobalMessageContext.ExpireMinutes, GlobalMessageContext.MaxRecordCount);
                    result.AppendLine("\r\n");
                    result.AppendLine(
                        "您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com");

                    defaultResponseMessage.Content = result.ToString();

                    return defaultResponseMessage;
                });

            return requestHandler.GetResponseMessage() as IResponseMessageBase;
        }

        /// <summary>
        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnLocationRequestAsync(RequestMessageLocation requestMessage)
        {
            var locationService = new LocationService();
            var responseMessage = locationService.GetResponseMessage(requestMessage as RequestMessageLocation);
            return responseMessage;
        }

        public override async Task<IResponseMessageBase> OnShortVideoRequestAsync(RequestMessageShortVideo requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您刚才发送的是小视频";
            return responseMessage;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnImageRequestAsync(RequestMessageImage requestMessage)
        {
            //一隔一返回News或Image格式
            if (base.GlobalMessageContext.GetMessageContext(requestMessage).RequestMessages.Count() % 2 == 0)
            {
                var responseMessage = CreateResponseMessage<ResponseMessageNews>();

                responseMessage.Articles.Add(new Article()
                {
                    Title = "您刚才发送了图片信息",
                    Description = "您发送的图片将会显示在边上",
                    PicUrl = requestMessage.PicUrl,
                    Url = "https://sdk.weixin.senparc.com"
                });
                responseMessage.Articles.Add(new Article()
                {
                    Title = "第二条",
                    Description = "第二条带连接的内容",
                    PicUrl = requestMessage.PicUrl,
                    Url = "https://sdk.weixin.senparc.com"
                });

                return responseMessage;
            }
            else
            {
                var responseMessage = CreateResponseMessage<ResponseMessageImage>();
                responseMessage.Image.MediaId = requestMessage.MediaId;
                return responseMessage;
            }
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnVoiceRequestAsync(RequestMessageVoice requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            //上传缩略图
            //var accessToken = Containers.AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            var uploadResult = AdvancedAPIs.MediaApi.UploadTemporaryMedia(appId, UploadMediaFileType.image,
                                                         ServerUtility.ContentRootMapPath("~/Images/Logo.jpg"));

            //设置音乐信息
            responseMessage.Music.Title = "天籁之音";
            responseMessage.Music.Description = "播放您上传的语音";
            responseMessage.Music.MusicUrl = "https://sdk.weixin.senparc.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            responseMessage.Music.HQMusicUrl = "https://sdk.weixin.senparc.com/Media/GetVoice?mediaId=" + requestMessage.MediaId;
            responseMessage.Music.ThumbMediaId = uploadResult.media_id;

            //推送一条客服消息
            try
            {
                CustomApi.SendText(appId, OpenId, "本次上传的音频MediaId：" + requestMessage.MediaId);

            }
            catch
            {
            }

            return responseMessage;
        }
        /// <summary>
        /// 处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnVideoRequestAsync(RequestMessageVideo requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了一条视频信息，ID：" + requestMessage.MediaId;

            #region 上传素材并推送到客户端

            Task.Factory.StartNew(async () =>
             {
                 //上传素材
                 var dir = ServerUtility.ContentRootMapPath("~/App_Data/TempVideo/");
                 var file = await MediaApi.GetAsync(appId, requestMessage.MediaId, dir);
                 var uploadResult = await MediaApi.UploadTemporaryMediaAsync(appId, UploadMediaFileType.video, file, 50000);
                 await CustomApi.SendVideoAsync(appId, base.WeixinOpenId, uploadResult.media_id, "这是您刚才发送的视频", "这是一条视频消息");
             }).ContinueWith(async task =>
             {
                 if (task.Exception != null)
                 {
                     WeixinTrace.Log("OnVideoRequest()储存Video过程发生错误：", task.Exception.Message);

                     var msg = string.Format("上传素材出错：{0}\r\n{1}",
                                task.Exception.Message,
                                task.Exception.InnerException != null
                                    ? task.Exception.InnerException.Message
                                    : null);
                     await CustomApi.SendTextAsync(appId, base.WeixinOpenId, msg);
                 }
             });

            #endregion

            return responseMessage;
        }


        /// <summary>
        /// 处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnLinkRequestAsync(RequestMessageLink requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = string.Format(@"您发送了一条连接信息：
Title：{0}
Description:{1}
Url:{2}", requestMessage.Title, requestMessage.Description, requestMessage.Url);
            return responseMessage;
        }

        public override async Task<IResponseMessageBase> OnFileRequestAsync(RequestMessageFile requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format(@"您发送了一个文件：
文件名：{0}
说明:{1}
大小：{2}
MD5:{3}", requestMessage.Title, requestMessage.Description, requestMessage.FileTotalLen, requestMessage.FileMd5);
            return responseMessage;
        }

        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override async Task<IResponseMessageBase> OnEventRequestAsync(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = await base.OnEventRequestAsync(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
                                                                                      //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }


        public override async Task<IResponseMessageBase> OnUnknownTypeRequestAsync(RequestMessageUnknownType requestMessage)
        {
            /*
             * 此方法用于应急处理SDK没有提供的消息类型，
             * 原始XML可以通过requestMessage.RequestDocument（或this.RequestDocument）获取到。
             * 如果不重写此方法，遇到未知的请求类型将会抛出异常（v14.8.3 之前的版本就是这么做的）
             */
            var msgType = Senparc.NeuChar.Helpers.MsgTypeHelper.GetRequestMsgTypeString(requestMessage.RequestDocument);
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "未知消息类型：" + msgType;

            WeixinTrace.SendCustomLog("未知请求消息类型", requestMessage.RequestDocument.ToString());//记录到日志中

            return responseMessage;
        }
    }
}
