using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPoint;
using NopSolutions.NopCommerce.Payment.Methods.SagePay;

namespace NopSolutions.NopCommerce.Web
{
    public partial class SagePayFailurePage : BaseNopPage
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

                if (status == "OK")
                {
                    // this is a very serious error - it means that the SagePay message is corrupt
                    string message = "Error in response from SagePay - expected failure, but saw OK - status is " + status;
                    LogManager.InsertLog(LogTypeEnum.OrderError, message, decrypted);
                    throw new NopException("Response Incorrect");
                }

                string statusDetail = decryptedDict["StatusDetail"];
                if (string.IsNullOrEmpty(statusDetail))
                    statusDetail = string.Empty;

                lblMessageTitle.Text = string.Format("Payment failed - status {0}", status);
                lblMessage.Text = string.Format("Payment failed - error detail is {0}", statusDetail);

                string VendorTxCode = decryptedDict["VendorTxCode"];
                if (string.IsNullOrEmpty(VendorTxCode))
                    VendorTxCode = string.Empty;

                if (string.IsNullOrEmpty(VendorTxCode))
                {
                    // this is a very serious error - it means that the SagePay message is corrupt :(
                    string message = "Error in VendorTxCode response from SagePay - status is " + status;
                    LogManager.InsertLog(LogTypeEnum.OrderError, message, decrypted);
                    throw new NopException("Response Incorrect");
                }

                // we've got here and we have a valid VendorTxCode and a valid order id
                Order order = OrderManager.GetOrderById((int)Convert.ToDouble(VendorTxCode));
                if (order == null)
                    throw new NopException(string.Format("The order ID {0} doesn't exists", VendorTxCode));

                // would be nice to let the user have another go here - e.g. with a different card

                // cancel the order and let the customer know
                if (OrderManager.CanCancelOrder(order))
                {
                    OrderManager.CancelOrder(order.OrderId, true);
                }
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
