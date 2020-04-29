//DPBMARK_FILE MiniProgram
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace Senparc.Weixin.MP.CommonService.TemplateMessage.WxOpen
{
    public class WxOpenTemplateMessage_PaySuccessNotice : TemplateMessageBase
    {

        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
        public TemplateDataItem keyword5 { get; set; }
        public TemplateDataItem keyword6 { get; set; }

        /// <summary>
        /// “购买成功通知”模板消息数据
        /// </summary>
        /// <param name="payAddress">购买地点</param>
        /// <param name="payTime">购买时间</param>
        /// <param name="productName">物品名称</param>
        /// <param name="orderNumber">交易单号</param>
        /// <param name="orderPrice">购买价格</param>
        /// <param name="hotLine">售后电话</param>
        /// <param name="url"></param>
        /// <param name="templateId"></param>
        public WxOpenTemplateMessage_PaySuccessNotice(
            string payAddress, DateTimeOffset payTime, string productName,
            string orderNumber, decimal orderPrice, string hotLine,
            string url,
            //根据实际的“模板ID”进行修改
            string templateId = "Ap1S3tRvsB8BXsWkiILLz93nhe7S8IgAipZDfygy9Bg")
            : base(templateId, url, "购买成功通知")
        {
            /* 
                关键词
                购买地点 {{keyword1.DATA}}
                购买时间 {{keyword2.DATA}}
                物品名称 {{keyword3.DATA}}
                交易单号 {{keyword4.DATA}}
                购买价格 {{keyword5.DATA}}
                售后电话 {{keyword6.DATA}}
                */

            keyword1 = new TemplateDataItem(payAddress);
            keyword2 = new TemplateDataItem(payTime.LocalDateTime.ToString());
            keyword3 = new TemplateDataItem(productName);
            keyword4 = new TemplateDataItem(orderNumber);
            keyword5 = new TemplateDataItem(orderPrice.ToString("C"));
            keyword6 = new TemplateDataItem(hotLine);
        }
    }

}
