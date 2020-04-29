/*----------------------------------------------------------------
    Copyright (C) 2020 Senparc

    文件名：CustomMessageHandler.cs
    文件功能描述：微信公众号自定义MessageHandler


    创建标识：Senparc - 20150312
----------------------------------------------------------------*/

//DPBMARK_FILE MiniProgram
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Entities.Request;
using Senparc.Weixin.WxOpen.MessageHandlers;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if NET45
using System.Web.Configuration;
#else

#endif

namespace Senparc.Weixin.MP.CommonService.WxOpenMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomWxOpenMessageHandler : WxOpenMessageHandler<CustomWxOpenMessageContext>
    {
        private string appId = Config.SenparcWeixinSetting.WxOpenAppId;
        private string appSecret = Config.SenparcWeixinSetting.WxOpenAppSecret;

        /// <summary>
        /// 为中间件提供生成当前类的委托
        /// </summary>
        public static Func<Stream, PostModel, int, CustomWxOpenMessageHandler> GenerateMessageHandler = (stream, postModel, maxRecordCount) => new CustomWxOpenMessageHandler(stream, postModel, maxRecordCount);


        public CustomWxOpenMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalGlobalMessageContext.ExpireMinutes = 3。
            GlobalMessageContext.ExpireMinutes = 3;

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


        public override async Task OnExecutingAsync(CancellationToken cancellationToken)
        {
            //测试MessageContext.StorageData
            var currentMessageContext = await base.GetCurrentMessageContext();
            if (currentMessageContext.StorageData == null || (currentMessageContext.StorageData is int))
            {
                currentMessageContext.StorageData = 0;
            }
            await base.OnExecutingAsync(cancellationToken);
        }

        public override async Task OnExecutedAsync(CancellationToken cancellationToken)
        {
            await base.OnExecutedAsync(cancellationToken);
            var currentMessageContext = await base.GetCurrentMessageContext();
            currentMessageContext.StorageData = ((int)currentMessageContext.StorageData) + 1;
        }


        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            //TODO:这里的逻辑可以交给Service处理具体信息，参考OnLocationRequest方法或/Service/LocationSercice.cs

            //这里可以进行数据库记录或处理

            //发送一条客服消息回复用户

            var contentUpper = requestMessage.Content.ToUpper();
            if (contentUpper == "LINK")
            {
                //发送客服消息
                Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendLink(appId, OpenId, "欢迎使用 Senparc.Weixin SDK", "感谢大家的支持！\r\n\r\n盛派永远在你身边！",
                    "https://weixin.senparc.com", "https://sdk.weixin.senparc.com/images/book-cover-front-small-3d-transparent.png");
            }
            else if (contentUpper == "CARD")
            {
                //上传封面临时素材
                var uploadResult = MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(appId, UploadMediaFileType.image, ServerUtility.ContentRootMapPath("~/Images/Logo.thumb.jpg"));

                //发送客服消息
                Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendMiniProgramPage(appId, OpenId, "欢迎使用 Senparc.Weixin SDK", "pages/websocket/websocket",
                 uploadResult.media_id);
            }
            else if (contentUpper == "客服")
            {
                Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendText(appId, OpenId, "您即将进入客服");
                var responseMessage = base.CreateResponseMessage<ResponseMessageTransfer_Customer_Service>();
                return responseMessage;
            }
            else
            {

                var result = new StringBuilder();
                result.AppendFormat("您刚才发送了文字信息：{0}\r\n\r\n", requestMessage.Content);

                var messageContext = GetCurrentMessageContext().ConfigureAwait(false).GetAwaiter().GetResult();
                if (messageContext.RequestMessages.Count > 1)
                {
                    result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n", messageContext.RequestMessages.Count,
                        messageContext.StorageData);
                    for (int i = messageContext.RequestMessages.Count - 2; i >= 0; i--)
                    {
                        var historyMessage = messageContext.RequestMessages[i];
                        string content = null;
                        if (historyMessage is RequestMessageText)
                        {
                            content = (historyMessage as RequestMessageText).Content;
                        }
                        else if (historyMessage is RequestMessageEvent_UserEnterTempSession)
                        {
                            content = "[进入客服]";
                        }
                        else
                        {
                            content = string.Format("[非文字信息:{0}]", historyMessage.GetType().Name);
                        }

                        result.AppendFormat("{0} 【{1}】{2}\r\n",
                            historyMessage.CreateTime.ToString("HH:mm:ss"),
                            historyMessage.MsgType.ToString(),
                            content
                            );
                    }
                    result.AppendLine("\r\n");
                }

                //处理微信换行符识别问题
                var msg = result.ToString().Replace("\r\n", "\n");

                //发送客服消息
                Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendText(appId, OpenId, msg);

                //也可以使用微信公众号的接口，完美兼容：
                //Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendText(appId, WeixinOpenId, msg);
            }

            return new SuccessResponseMessage();

            //和公众号一样回复XML是无效的：
            //            return new SuccessResponseMessage()
            //            {
            //                ReturnText = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xml>
            //    <ToUserName><![CDATA[{0}]]></ToUserName>
            //    <FromUserName><![CDATA[{1}]]></FromUserName>
            //    <CreateTime>1357986928</CreateTime>
            //    <MsgType><![CDATA[text]]></MsgType>
            //    <Content><![CDATA[TNT2]]></Content>
            //</xml>",requestMessage.FromUserName,requestMessage.ToUserName)
            //            };
        }

        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            //发来图片，进行处理
            Task.Factory.StartNew(async () =>
            {
                await Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendTextAsync(appId, OpenId, "刚才您发送了这张图片：");
                await Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendImageAsync(appId, OpenId, requestMessage.MediaId);
            });
            return DefaultResponseMessage(requestMessage);
        }

        public override IResponseMessageBase OnEvent_UserEnterTempSessionRequest(RequestMessageEvent_UserEnterTempSession requestMessage)
        {
            //进入客服
            var msg = @"欢迎您！这条消息来自 Senparc.Weixin 进入客服事件。

您可以进行以下测试：
1、发送任意文字，返回上下文消息记录
2、发送图片，返回同样的图片
3、发送文字“link”,返回图文链接
4、发送文字“card”，发送小程序卡片";
            Senparc.Weixin.WxOpen.AdvancedAPIs.CustomApi.SendText(appId, OpenId, msg);

            return DefaultResponseMessage(requestMessage);
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //所有没有被处理的消息会默认返回这里的结果

            return new SuccessResponseMessage();

            //return new SuccessResponseMessage();等效于：
            //base.TextResponseMessage = "success";
            //return null;
        }
    }
}