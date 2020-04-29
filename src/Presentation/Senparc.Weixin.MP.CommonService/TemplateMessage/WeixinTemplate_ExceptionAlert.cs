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
    /*
       {{first.DATA}}
        Time：{{keyword1.DATA}}
        Host：{{keyword2.DATA}}
        Service：{{keyword3.DATA}}
        Status：{{keyword4.DATA}}
        Message：{{keyword5.DATA}}
        {{remark.DATA}}
      */

    public class WeixinTemplate_ExceptionAlert : TemplateMessageBase
    {
        const string TEMPLATE_ID = "ur6TqESOo-32FEUk4qJxeWZZVt4KEOPjqbAFDGWw6gg";//每个公众号都不同，需要根据实际情况修改

        public TemplateDataItem first { get; set; }
        /// <summary>
        /// Time
        /// </summary>
        public TemplateDataItem keyword1 { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        public TemplateDataItem keyword2 { get; set; }
        /// <summary>
        /// Service
        /// </summary>
        public TemplateDataItem keyword3 { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public TemplateDataItem keyword4 { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public TemplateDataItem keyword5 { get; set; }

        public TemplateDataItem remark { get; set; }

        public WeixinTemplate_ExceptionAlert(string _first, string host, string service, string status, string message,
            string _remark, string url = null, string templateId = TEMPLATE_ID)
            : base(templateId, url, "系统异常告警通知")
        {
            first = new TemplateDataItem(_first);
            keyword1 = new TemplateDataItem(SystemTime.Now.LocalDateTime.ToString());
            keyword2 = new TemplateDataItem(host);
            keyword3 = new TemplateDataItem(service);
            keyword4 = new TemplateDataItem(status);
            keyword5 = new TemplateDataItem(message);
            remark = new TemplateDataItem(_remark);
        }
    }
}
