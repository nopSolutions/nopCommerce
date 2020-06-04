using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Nop.Core.Infrastructure;
using Nop.Data;

using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.NeuChar.MessageHandlers;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.CommonService;
using Senparc.Weixin.MP.CommonService.CustomMessageHandler;
using Senparc.Weixin.MP.CommonService.Mvc.Extension.Results;

namespace Senparc.Weixin.MP.CommonService.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class WeixinController : Controller
    {
        #region Fields

        private readonly INopFileProvider _fileProvider;
        private readonly SenparcWeixinSetting _senparcWeixinSetting;

        private readonly Func<string> _getRandomFileName = () => SystemTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        #endregion

        #region Ctor

        public WeixinController(INopFileProvider fileProvider,
            SenparcWeixinSetting senparcWeixinSetting)
        {
            _fileProvider = fileProvider;
            _senparcWeixinSetting = senparcWeixinSetting;
        }

        #endregion

        /// <summary>
        /// 微信后台验证地址
        /// </summary>
        [HttpGet]
        public IActionResult Index(PostModel postModel, string echostr)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return Content("System not installed.");

            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, _senparcWeixinSetting.Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("Failed.");
                //return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, _senparcWeixinSetting.Token) + "。");
                    //"如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 最简化的处理流程
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> Index(PostModel postModel)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return Content("System not installed.");

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, _senparcWeixinSetting.Token))
            {
                return Content("Error.");
            }

            postModel.Token = _senparcWeixinSetting.Token;
            postModel.EncodingAESKey =_senparcWeixinSetting.EncodingAESKey; //根据自己后台的设置保持一致
            postModel.AppId =_senparcWeixinSetting.WeixinAppId; //根据自己后台的设置保持一致

            var cancellationToken = new CancellationToken();//给异步方法使用

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制（实际最大限制 99999）
            //注意：如果使用分布式缓存，不建议此值设置过大，如果需要储存历史信息，请使用数据库储存
            var maxRecordCount = 10;

            var messageHandler = new CustomMessageHandler.CustomMessageHandler(Request.GetRequestMemoryStream(), postModel, maxRecordCount);

            #region 没有重写的异步方法将默认尝试调用同步方法中的代码（为了偷懒）

            /* 使用 SelfSynicMethod 的好处是可以让异步、同步方法共享同一套（同步）代码，无需写两次，
             * 当然，这并不一定适用于所有场景，所以是否选用需要根据实际情况而定，这里只是演示，并不盲目推荐。*/
            messageHandler.DefaultMessageHandlerAsyncEvent = DefaultMessageHandlerAsyncEvent.SelfSynicMethod;

            #endregion

            #region 设置消息去重 设置

            /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
             * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
            messageHandler.OmitRepeatedMessage = true;//默认已经开启，此处仅作为演示，也可以设置为false在本次请求中停用此功能

            #endregion

            try
            {
                messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）
                await messageHandler.ExecuteAsync(cancellationToken); //执行微信处理过程（关键）
                messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）
                return new FixWeixinBugWeixinResult(messageHandler);
            }
            catch(Exception ex)
            {
                #region 异常处理
                WeixinTrace.Log("MessageHandler错误：{0}", ex.Message);

                using (TextWriter tw = new StreamWriter(_fileProvider.MapPath("/App_Data/Error_" + _getRandomFileName() + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
                #endregion
            }

        }

        /// <summary>
        /// 【异步方法】用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        public IActionResult Post(PostModel postModel)
        {
            return Content("");


            /* 异步请求请见 WeixinAsyncController（推荐） */

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, _senparcWeixinSetting.Token))
            {
                return Content("参数错误！");
            }

            #region 打包 PostModel 信息

            postModel.Token = _senparcWeixinSetting.Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = _senparcWeixinSetting.EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId =_senparcWeixinSetting.WeixinAppId;//根据自己后台的设置保持一致（必须提供）

            #endregion

            //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制（实际最大限制 99999）
            //注意：如果使用分布式缓存，不建议此值设置过大，如果需要储存历史信息，请使用数据库储存
            var maxRecordCount = 10;

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler.CustomMessageHandler(Request.GetRequestMemoryStream(), postModel, maxRecordCount);

            #region 设置消息去重设置

            /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
             * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的 RequestMessage */
            messageHandler.OmitRepeatedMessage = true;//默认已经是开启状态，此处仅作为演示，也可以设置为 false 在本次请求中停用此功能

            #endregion

            try
            {
                messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）
                messageHandler.Execute();//执行微信处理过程（关键）
                messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）

                //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
                //return new WeixinResult(messageHandler);//v0.8+
                return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
            }
            catch (Exception ex)
            {
                #region 异常处理
                WeixinTrace.Log("MessageHandler错误：{0}", ex.Message);

                using (TextWriter tw = new StreamWriter(_fileProvider.MapPath("~/App_Data/Error_" + _getRandomFileName() + ".txt")))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
                #endregion
            }
        }
    }
}