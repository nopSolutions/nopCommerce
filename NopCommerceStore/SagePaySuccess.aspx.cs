using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPoint;
using NopSolutions.NopCommerce.Payment.Methods.SagePay;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class SagePaySuccessPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if (!Page.IsPostBack)
            {
                string crypt = Request.Params["Crypt"];
                SagePayPaymentProcessor proc = new SagePayPaymentProcessor();
                string decrypted = proc.Decrypt(crypt);
                var decryptedDict = proc.Parse(decrypted);

                string status = decryptedDict["Status"];
                if (string.IsNullOrEmpty(status))
                    status = string.Empty;

                if (status != "OK")
                {
                    // this is a very serious error - it means that the SagePay message is corrupt
                    string message = "Error in response from SagePay - status is " + status;
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, message, decrypted);
                    throw new NopException("Response Incorrect");
                }

                string TxAuthNo = decryptedDict["TxAuthNo"];
                if (string.IsNullOrEmpty(TxAuthNo))
                    TxAuthNo = string.Empty;

                string VendorTxCode = decryptedDict["VendorTxCode"];
                if (string.IsNullOrEmpty(VendorTxCode))
                    VendorTxCode = string.Empty;

                if (string.IsNullOrEmpty(TxAuthNo))
                {
                    // this is a very serious error - it means that the SagePay message is corrupt :(
                    string message = "Error in TXAuthNo response from SagePay - status is " + status;
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, message, decrypted);
                    throw new NopException("Response Incorrect");
                }

                if (string.IsNullOrEmpty(VendorTxCode))
                {
                    // this is a very serious error - it means that the SagePay message is corrupt :(
                    string message = "Error in VendorTxCode response from SagePay - status is " + status;
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.OrderError, message, decrypted);
                    throw new NopException("Response Incorrect");
                }

                // we've got here and we have a valid VendorTxCode and a valid order id
                Order order = IoC.Resolve<IOrderService>().GetOrderById((int)Convert.ToDouble(VendorTxCode));
                if (order == null)
                    throw new NopException(string.Format("The order ID {0} doesn't exists", VendorTxCode));

                // would like to add more secure code here - this flow doesn't feel entirely safe/secure!
                // would like to store the TxAuthNo returned.

                if (IoC.Resolve<IOrderService>().CanMarkOrderAsPaid(order))
                {
                    IoC.Resolve<IOrderService>().MarkOrderAsPaid(order.OrderId);
                }
                Response.Redirect("~/checkoutcompleted.aspx");
            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page is tracked by 'Online Customers' module
        /// </summary>
        public override bool TrackedByOnlineCustomersModule
        {
            get
            {
                return false;
            }
        }
    }
}
