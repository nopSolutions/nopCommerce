using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Senparc.Weixin.Entities;
using Senparc.NeuChar.MessageHandlers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Senparc.Weixin.MP.CommonService.Mvc.Extension.Results
{
    public class FixWeixinBugWeixinResult : ContentResult
    {
        protected IMessageHandlerDocument _messageHandlerDocument;

        /// <summary>
        /// 这个类型只用于特殊阶段：目前IOS版本微信有换行的bug，\r\n会识别为2行
        /// </summary>
        public FixWeixinBugWeixinResult(IMessageHandlerDocument messageHandlerDocument)
        {
            _messageHandlerDocument = messageHandlerDocument;
        }

        public FixWeixinBugWeixinResult(string content)
        {
            //_content = content;
            base.Content = content;
        }


        public new string Content
        {
            get
            {
                if (!string.IsNullOrEmpty(base.Content))
                {
                    return base.Content;
                }

                if (_messageHandlerDocument != null)
                {
                    if (!string.IsNullOrEmpty(_messageHandlerDocument.TextResponseMessage))
                    {
                        return _messageHandlerDocument.TextResponseMessage.Replace("\r\n", "\n");
                    }
                }
                return null;
            }

            set => base.Content = value;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var content = this.Content;

            if (content == null)
            {
                if (_messageHandlerDocument == null)
                {
                    throw new Senparc.Weixin.Exceptions.WeixinException("执行WeixinResult时提供的MessageHandler不能为Null！", null);
                }
                var finalResponseDocument = _messageHandlerDocument.FinalResponseDocument;

                if (finalResponseDocument == null)
                {
                    //throw new Senparc.Weixin.MP.WeixinException("FinalResponseDocument不能为Null！", null);
                }
                else
                {
                    content = finalResponseDocument.ToString();
                }
            }

            context.HttpContext.Response.ContentType = "text/xml";
            content = (content ?? "").Replace("\r\n", "\n");

            var bytes = Encoding.UTF8.GetBytes(content);
            //context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            await context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);

            // return base.ExecuteResultAsync(context);
        }

    }
}
