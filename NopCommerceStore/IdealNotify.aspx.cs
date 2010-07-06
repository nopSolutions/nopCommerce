using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.Moneris;

namespace NopSolutions.NopCommerce.Web
{
    public partial class IdealNotify : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //comment this line to process return
            Response.Redirect(CommonHelper.GetStoreLocation());

            try
            {
                //// Create XML document based on the request
                XmlDocument doc = new XmlDocument();
                doc.Load(Request.InputStream);
                //// Try to find the order the notification is posted about
                Order o = OrderManager.GetOrderById(Convert.ToInt32(doc.ChildNodes[1]["purchaseID"].InnerText));
                if (o == null)
                {
                    throw new NullReferenceException("No order");
                }
                //// Find the status iDeal gave to the order
                string status = doc.ChildNodes[1]["status"].InnerText;
                switch (status.ToLower())
                {
                    case "success":
                        if (OrderManager.CanMarkOrderAsPaid(o))
                        {
                            OrderManager.MarkOrderAsPaid(o.OrderId);
                        }
                        break;
                    case "Expired":
                    case "Cancelled":
                    case "Failure":
                        if (OrderManager.CanCancelOrder(o))
                        {
                            OrderManager.CancelOrder(o.OrderId, true);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                LogManager.InsertLog(LogTypeEnum.OrderError, "iDeal payment error" + exc.Message, exc);
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
