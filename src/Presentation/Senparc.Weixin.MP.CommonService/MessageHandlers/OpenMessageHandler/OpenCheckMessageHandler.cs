//DPBMARK_FILE Open
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;


#if NET45
using System.Web.Configuration;
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Senparc.Weixin.MP.CommonService.OpenTicket;
using Senparc.Weixin.MP.CommonService.Utilities;
#else
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Senparc.Weixin.MP.CommonService.OpenTicket;
using Senparc.Weixin.MP.CommonService.Utilities;
#endif

namespace Senparc.Weixin.MP.CommonService.MessageHandlers.OpenMessageHandler
{
    /// <summary>
    /// 开放平台全网发布之前需要做的验证
    /// </summary>
    public class OpenCheckMessageHandler : MessageHandler<CustomMessageContext>
    {
        /*
           https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419318611&lang=zh_CN
            自动化测试的专用测试公众号的信息如下：
            （1）appid： wx570bc396a51b8ff8
            （2）Username： gh_3c884a361561
        */

        //private string testAppId = "wx570bc396a51b8ff8";

#if NET45
        private string componentAppId = Config.SenparcWeixinSetting.Component_Appid;
        private string componentSecret = Config.SenparcWeixinSetting.Component_Secret;
#else
        private string componentAppId = "ComponentAppId";
        private string componentSecret = "Component_Secret";
#endif



        public OpenCheckMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {

        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            if (requestMessage.Content == "TESTCOMPONENT_MSG_TYPE_TEXT")
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = requestMessage.Content + "_callback";//固定为TESTCOMPONENT_MSG_TYPE_TEXT_callback
                return responseMessage;
            }

            if (requestMessage.Content.StartsWith("QUERY_AUTH_CODE:"))
            {
                string openTicket = OpenTicketHelper.GetOpenTicket(componentAppId);
                var query_auth_code = requestMessage.Content.Replace("QUERY_AUTH_CODE:", "");
                try
                {
                    var component_access_token = Open.ComponentAPIs.ComponentApi.GetComponentAccessToken(componentAppId, componentSecret, openTicket).component_access_token;
                    var oauthResult = Open.ComponentAPIs.ComponentApi.QueryAuth(component_access_token, componentAppId, query_auth_code);

                    //调用客服接口
                    var content = query_auth_code + "_from_api";
                    var sendResult = AdvancedAPIs.CustomApi.SendText(oauthResult.authorization_info.authorizer_access_token,
                          requestMessage.FromUserName, content);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            return null;
        }

        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = requestMessage.Event + "from_callback";
            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "默认消息";
            return responseMessage;
        }
    }
}
