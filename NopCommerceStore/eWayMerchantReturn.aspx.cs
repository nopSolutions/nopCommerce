//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web
{
    public partial class eWayMerchantReturnPage : BaseNopPage
    {
        public string _AuthCode = string.Empty;
        public string _ResponseCode = string.Empty;
        public string _ReturnAmount = string.Empty;
        public string _TrxnNumber = string.Empty;
        public string _TrxnStatus = string.Empty;
        public string _MerchnatOption1 = string.Empty;
        public string _MerchnatOption2 = string.Empty;
        public string _MerchnatOption3 = string.Empty;
        public string _ReferenceNumber = string.Empty;
        public string _ReferenceInvoice = string.Empty;
        public string _TrxnResponseMessage = string.Empty;
        public string _ErrorMessage = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            string AccessPaymentCode = string.Empty;

            if (!Page.IsPostBack)
            {
                if (Request.Form["AccessPaymentCode"] != null)
                    AccessPaymentCode = Request.Form["AccessPaymentCode"].ToString();

                //get the result of the transaction based on the unique payment code
                if (CheckAccessCode(AccessPaymentCode))
                {
                    if (_TrxnStatus.ToLower().Equals("true"))
                    {
                        divAuthorised.Style["display"] = "block";
                    }
                    else
                    {
                        divFailed.Style["display"] = "block";
                        Response.Redirect(CommonHelper.GetStoreLocation());
                    }

                    //display the saved values
                    lblStatus.Text = _TrxnStatus;
                    lblAuthCode.Text = _AuthCode;
                    lblResponseCode.Text = _ResponseCode;
                    lblReturnAmount.Text = _ReturnAmount;
                    lblMerchantInvoice.Text = _ReferenceInvoice;
                    lblTrxnNumber.Text = _TrxnNumber;
                    lblMerchantOption1.Text = _MerchnatOption1;
                    lblMerchantOption2.Text = _MerchnatOption2;
                    lblMerchantOption3.Text = _MerchnatOption3;
                    lblMerchantReference.Text = _ReferenceNumber;
                    lblTrxnResponseMessage.Text = _TrxnResponseMessage;
                    lblErrorMessage.Text = _ErrorMessage;


                    int orderId = Convert.ToInt32(_MerchnatOption1);
                    Order order = OrderManager.GetOrderById(orderId);
                    if (String.IsNullOrEmpty(_ErrorMessage) && order != null)
                    {
                        if (OrderManager.CanMarkOrderAsPaid(order))
                        {
                            OrderManager.MarkOrderAsPaid(order.OrderId);
                        }
                        Response.Redirect("~/checkoutcompleted.aspx");
                    }
                    else
                    {
                        Response.Redirect(CommonHelper.GetStoreLocation());
                    }

                }
                else
                {
                    // unable to check the transaction amount
                    divFraud.Style["display"] = "block";
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
            }
        }

        /// <summary>
        /// Procedure to check the 64 character access payment code
        /// for security
        /// </summary>
        /// <param name="AccessPaymentCode">64 char code</param>
        /// <returns>true if found; false if not found</returns>
        protected bool CheckAccessCode(string AccessPaymentCode)
        {
            //POST to Payment gateway the access code returned
            string strPost = "CustomerID=" + SettingManager.GetSettingValue("PaymentMethod.eWayUK.CustomerId");
            strPost += Format("AccessPaymentCode", AccessPaymentCode);
            strPost += Format("UserName", SettingManager.GetSettingValue("PaymentMethod.eWayUK.Username"));

            string url = SettingManager.GetSettingValue("PaymentMethod.eWayUK.PaymentPage") + "Result?" + strPost;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = WebRequestMethods.Http.Get;
            string resultXML = string.Empty;

            try
            {
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

                //get the response from the transaction generate page
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    resultXML = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            //parse the results save the values
            ParseAndSaveXMLResult(resultXML);

            return true;
        }

        /// <summary>
        /// Parse the XML Returned and save all the values to be displayed to user
        /// </summary>
        /// <param name="resultXML">XML of the transaction result</param>
        private void ParseAndSaveXMLResult(string resultXML)
        {
            string _currentNode;
            StringReader _sr = new StringReader(resultXML);
            XmlTextReader _xtr = new XmlTextReader(_sr);
            _xtr.XmlResolver = null;
            _xtr.WhitespaceHandling = WhitespaceHandling.None;

            // get the root node
            _xtr.Read();

            if ((_xtr.NodeType == XmlNodeType.Element) && (_xtr.Name == "TransactionResponse"))
            {
                while (_xtr.Read())
                {
                    if ((_xtr.NodeType == XmlNodeType.Element) && (!_xtr.IsEmptyElement))
                    {
                        _currentNode = _xtr.Name;
                        _xtr.Read();
                        if (_xtr.NodeType == XmlNodeType.Text)
                        {
                            switch (_currentNode)
                            {

                                case "AuthCode":
                                    _AuthCode = _xtr.Value;
                                    break;
                                case "ResponseCode":
                                    _ResponseCode = _xtr.Value;
                                    break;
                                case "ReturnAmount":
                                    _ReturnAmount = _xtr.Value;
                                    break;
                                case "TrxnStatus":
                                    _TrxnStatus = _xtr.Value;
                                    break;
                                case "TrxnNumber":
                                    _TrxnNumber = _xtr.Value;
                                    break;
                                case "MerchantOption1":
                                    _MerchnatOption1 = _xtr.Value;
                                    break;
                                case "MerchantOption2":
                                    _MerchnatOption2 = _xtr.Value;
                                    break;
                                case "MerchantOption3":
                                    _MerchnatOption3 = _xtr.Value;
                                    break;
                                case "MerchantInvoice":
                                    _ReferenceInvoice = _xtr.Value;
                                    break;
                                case "MerchantReference":
                                    _ReferenceNumber = _xtr.Value;
                                    break;

                                case "TrxnResponseMessage":
                                    _TrxnResponseMessage = _xtr.Value;
                                    break;
                                case "ErrorMessage":
                                    _ErrorMessage = _xtr.Value;
                                    break;

                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Format the string needed to post to the Generate page
        /// </summary>
        /// <param name="fieldName">eWAY Parameter Name</param>
        /// <param name="value">Value of Parameter</param>
        /// <returns>Formated value for the URL</returns>
        private string Format(string fieldName, string value)
        {
            if (!string.IsNullOrEmpty(value))
                return "&" + fieldName + "=" + value;
            else
                return "";
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