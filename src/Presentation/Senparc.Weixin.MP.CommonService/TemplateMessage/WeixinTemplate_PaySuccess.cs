//DPBMARK_FILE MP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace Senparc.Weixin.MP.CommonService.TemplateMessage
{
    /// <summary>
    /// 支付成功模板消息（购买成功通知）
    /// </summary>
    public class WeixinTemplate_PaySuccess : TemplateMessageBase
    {
        const string TEMPLATE_ID = "66Gf81swxfWt_P_HkH0Bapvj1nlpiWGmEkXDeCvWcVo";//每个公众号都不同，需要根据实际情况修改


        public TemplateDataItem name { get; set; }
        /// <summary>
        /// Time
        /// </summary>
        public TemplateDataItem remark { get; set; }


        public WeixinTemplate_PaySuccess(string url,string productName,string notice) 
            : base(TEMPLATE_ID, url, "购买成功通知")
        {
            name = new TemplateDataItem(productName);
            remark = new TemplateDataItem(notice);
        }
    }
}
