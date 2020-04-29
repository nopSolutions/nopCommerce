//DPBMARK_FILE Open
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.Weixin.Open;
using Senparc.Weixin.Open.MessageHandlers;
using System.IO;
using Senparc.Weixin.MP.CommonService.Utilities;
using Senparc.Weixin.Open.Entities.Request;
using Senparc.CO2NET.Utilities;

namespace Senparc.Weixin.MP.CommonService.ThirdPartyMessageHandlers
{
    public class CustomThirdPartyMessageHandler : ThirdPartyMessageHandler
    {
        public CustomThirdPartyMessageHandler(Stream inputStream, PostModel encryptPostModel)
            : base(inputStream, encryptPostModel)
        { }

        public override string OnComponentVerifyTicketRequest(RequestMessageComponentVerifyTicket requestMessage)
        {
            var openTicketPath = ServerUtility.ContentRootMapPath("~/App_Data/OpenTicket");
            if (!Directory.Exists(openTicketPath))
            {
                Directory.CreateDirectory(openTicketPath);
            }

            //RequestDocument.Save(Path.Combine(openTicketPath, string.Format("{0}_Doc.txt", SystemTime.Now.Ticks)));

            //记录ComponentVerifyTicket（也可以存入数据库或其他可以持久化的地方）
            using (FileStream fs = new FileStream(Path.Combine(openTicketPath, string.Format("{0}.txt", RequestMessage.AppId)),FileMode.OpenOrCreate,FileAccess.ReadWrite))
            {
                using (TextWriter tw = new StreamWriter(fs))
                {
                    tw.Write(requestMessage.ComponentVerifyTicket);
                    tw.Flush();
                    //tw.Close();
                }
            }
            return base.OnComponentVerifyTicketRequest(requestMessage);
        }

        public override string OnUnauthorizedRequest(RequestMessageUnauthorized requestMessage)
        {
            //取消授权
            return base.OnUnauthorizedRequest(requestMessage);
        }
    }
}
