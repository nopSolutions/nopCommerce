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
using NopSolutions.NopCommerce.Payment.Methods.QuickPay;

namespace NopSolutions.NopCommerce.Web
{
    public partial class QuickPayReturnPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                /*  Documentation.
                 * 
                 *  Response data fields
                 * 
                 *  msgtype  	/^[a-z]$/  	Defines which action was performed - Each message type is described in detail later
                    ordernumber 	/^[a-zA-Z0-9]{4,20}$/ 	A value specified by merchant in the initial request.
                    amount 	/^[0-9]{1,10}$/ 	The amount defined in the request in its smallest unit. In example, 1 EUR is written 100.
                    currency 	/^[A-Z]{3}$/ 	The transaction currency as the 3-letter ISO 4217 alphabetical code.
                    time 	/^[0-9]{12}$/ 	The time of which the message was handled. Format is YYMMDDHHIISS.
                    state 	/^[1-9]{1,2}$/ 	The current state of the transaction. See Appendix D.
                    qpstat 	/^[0-9]{3}$/ 	Return code from QuickPay. See Appendix A.
                    qpstatmsg 	/^[\w -.]{1,}$/ 	A message detailing errors and warnings if any.
                    chstat 	/^[0-9]{3}$/ 	Return code from the clearing house. Please refer to the clearing house documentation.
                    chstatmsg 	/^[\w -.]{1,}$/ 	A message from the clearing house detailing errors and warnings if any.
                    merchant 	/^[\w -.]{1,100}$/ 	The QuickPay merchant name
                    merchantemail 	/^[\w_-.\@]{6,}$/ 	The QuickPay merchant email/username
                    transaction 	/^[0-9]{1,32}$/ 	The id assigned to the current transaction.
                    cardtype 	/^[\w-]{1,32}$/ 	The card type used to authorize the transaction.
                    cardnumber 	/^[\w\s]{,32}$/ 	A truncated version of the card number - eg. 'XXXX XXXX XXXX 1234'. Note: This field will be empty for other message types than 'authorize' and 'subscribe'.
                    md5check 	/^[a-z0-9]{32}$/ 	A MD5 checksum to ensure data integrity. See Section 3.1 for more information.
                 
                 *  MD5 checksum calculation for a Form callback

                    The field values must be concatenated in the following order:

                    cstr = concatenate(
                        'msgtype',
                        'ordernumber',
                        'amount',
                        'currency',
                        'time',
                        'state',
                        'qpstat',
                        'qpstatmsg',
                        'chstat',
                        'chstatmsg',
                        'merchant',
                        'merchantemail',
                        'transaction',
                        'cardtype',
                        'cardnumber',
                        'secret'
                    )

                    md5check = md5(cstr)

                 *  TESTNUMBERS
                 *  I testmode kan man fremprovokere fejlrespons ved, at sende kortoplysninger der indeholder et bogstav, f.eks:

                 *  Cart that WILL FAIL
                    Korntnr: 4571123412341234, Udløbsdato: 09/12 og cvd: 12a.

                    Så bliver kortet afvist, selv om der køres i testmode.

                 *  Cart that WILL SUCEED
                    En succesrespons kan opnåes ved at bruge f.eks.:

                    Kortnr: 4571123412341234, Udløbsdato: 09/12 og cvd: 123.

                 * 
                 * 
                 * * */

                string msgtype = CommonHelper.GetFormString("msgtype");
                string ordernumber = CommonHelper.GetFormString("ordernumber");
                string amount = CommonHelper.GetFormString("amount");
                string currency = CommonHelper.GetFormString("currency");
                string time = CommonHelper.GetFormString("time");
                string state = CommonHelper.GetFormString("state");
                string qpstat = CommonHelper.GetFormString("qpstat");
                string qpstatmsg = CommonHelper.GetFormString("qpstatmsg");
                string chstat = CommonHelper.GetFormString("chstat");
                string chstatmsg = CommonHelper.GetFormString("chstatmsg");
                string merchant = CommonHelper.GetFormString("merchant");
                string merchantemail = CommonHelper.GetFormString("merchantemail");
                string transaction = CommonHelper.GetFormString("transaction");
                string cardtype = CommonHelper.GetFormString("cardtype");
                string cardnumber = CommonHelper.GetFormString("cardnumber");
                string responseMD5check = CommonHelper.GetFormString("md5check");
                string md5secret = SettingManager.GetSettingValue(QuickPayConstants.SETTING_MD5SECRET);


                var processor = new QuickPayPaymentProcessor();
                string serverMD5check = processor.GetMD5(
                                                        string.Concat(msgtype, ordernumber, amount, currency, time, state, qpstat,
                                                        qpstatmsg, chstat, chstatmsg, merchant, merchantemail, transaction,
                                                        cardtype, cardnumber, md5secret)
                                                        );
                if (String.IsNullOrEmpty(serverMD5check))
                    throw new Exception("serverMD5check could not be empty");

                if (responseMD5check != serverMD5check)
                    throw new NopException("MD5 Check doesn't match. This may just be an error in the setting or it COULD be a hacker trying to fake a completed order");
                /*
                 *  Possible status codes
                    Code 	Description
                    000 	Approved.
                    001 	Rejected by clearing house. See field 'chstat' and 'chstatmsg' for further explanation.
                    002 	Communication error.
                    003 	Card expired.
                    004 	Transition is not allowed for transaction current state.
                    005 	Authorization is expired.
                    006 	Error reported by clearing house.
                    007 	Error reported by QuickPay.
                    008 	Error in request data.
                 * */
                if (qpstat != "000")
                    throw new NopException("The order was NOT approved, stat is: " + qpstat);

                if (string.IsNullOrEmpty(ordernumber))
                    throw new NopException("Order is was empty");

                if (string.IsNullOrEmpty(merchant))
                    throw new NopException("Quickpay merchant is not set");

                Order order = OrderManager.GetOrderById(Convert.ToInt32(ordernumber));

                if (order == null)
                    throw new NopException(string.Format("The order ID {0} doesn't exists", ordernumber));
                
                if (OrderManager.CanMarkOrderAsPaid(order))
                {
                    OrderManager.MarkOrderAsPaid(order.OrderId);
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
