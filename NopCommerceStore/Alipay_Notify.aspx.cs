using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.Alipay;

namespace NopSolutions.NopCommerce.Web
{
    public partial class Alipay_NotifyPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            string alipayNotifyURL = "https://www.alipay.com/cooperate/gateway.do?service=notify_verify";
            string partner = SettingManager.GetSettingValue("PaymentMethod.Alipay.Partner");
            if (string.IsNullOrEmpty(partner))
                throw new Exception("Partner is not set");
            string key = SettingManager.GetSettingValue("PaymentMethod.Alipay.Key");
            if (string.IsNullOrEmpty(key))
                throw new Exception("Partner is not set");
            string _input_charset = "utf-8";

            alipayNotifyURL = alipayNotifyURL + "&partner=" + partner + "&notify_id=" + Request.Form["notify_id"];
            string responseTxt = AlipayPaymentProcessor.Get_Http(alipayNotifyURL, 120000);

            int i;
            NameValueCollection coll;
            coll = Request.Form;
            String[] requestarr = coll.AllKeys;
            string[] Sortedstr = AlipayPaymentProcessor.BubbleSort(requestarr);

            StringBuilder prestr = new StringBuilder();
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (Request.Form[Sortedstr[i]] != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i] + "=" + Request.Form[Sortedstr[i]]);
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "=" + Request.Form[Sortedstr[i]] + "&");

                    }
                }
            }

            prestr.Append(key);

            string mysign = AlipayPaymentProcessor.GetMD5(prestr.ToString(), _input_charset);

            string sign = Request.Form["sign"];

            if (mysign == sign && responseTxt == "true")
            {
                if (Request.Form["trade_status"] == "WAIT_BUYER_PAY")
                {
                    string strOrderNO = Request.Form["out_trade_no"];
                    string strPrice = Request.Form["total_fee"];
                }
                else if (Request.Form["trade_status"] == "TRADE_FINISHED" || Request.Form["trade_status"] == "TRADE_SUCCESS")
                {
                    string strOrderNO = Request.Form["out_trade_no"];
                    string strPrice = Request.Form["total_fee"];

                    int orderId = 0;
                    if (Int32.TryParse(strOrderNO, out orderId))
                    {
                        Order order = OrderManager.GetOrderById(orderId);
                        if (order != null && OrderManager.CanMarkOrderAsPaid(order))
                        {
                            OrderManager.MarkOrderAsPaid(order.OrderId);
                        }
                    }
                }
                else
                {
                }

                Response.Write("success");
            }
            else
            {
                Response.Write("fail");
                string logStr = "MD5:mysign=" + mysign + ",sign=" + sign + ",responseTxt=" + responseTxt;
                LogManager.InsertLog(LogTypeEnum.OrderError, logStr, logStr);
            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        } 
    }
}
