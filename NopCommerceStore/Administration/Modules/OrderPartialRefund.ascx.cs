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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class OrderPartialRefundControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            var order = OrderManager.GetOrderById(this.OrderId);
            if (order != null || order.Deleted)
            {
                try
                {
                    lOrderInfo.Text = string.Format(GetLocaleResourceString("Admin.OrderPartialRefund.OrderInfo"), order.OrderId);
                    decimal maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
                    lblMaxAmountToRefund.Text = string.Format(GetLocaleResourceString("Admin.OrderPartialRefund.MaxRefund"), maxAmountToRefund.ToString("G29"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode);
                    if (this.Online)
                    {
                        if (!OrderManager.CanPartiallyRefund(order, decimal.Zero))
                            throw new NopException("Can not do partial refund for this order");
                    }
                    else
                    {
                        if (!OrderManager.CanPartiallyRefundOffline(order, decimal.Zero))
                            throw new NopException("Can not do partial refund for this order");
                    }
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
            else
            {
                lOrderInfo.Text = "Order is not found";
            }
            
        }

        protected void btnRefund_Click(object sender, EventArgs e)
        {
            try
            {
                var order = OrderManager.GetOrderById(this.OrderId);
                if (order != null)
                {
                    string error = string.Empty;
                    decimal amountToRefund = txtAmountToRefund.Value;
                    if (amountToRefund <= decimal.Zero)
                        throw new NopException("Enter amount to refund");

                    if (this.Online)
                    {
                        order = OrderManager.PartiallyRefund(this.OrderId, amountToRefund, ref error);
                        if (!String.IsNullOrEmpty(error))
                        {
                            throw new NopException(error);
                        }
                    }
                    else
                    {
                        order = OrderManager.PartiallyRefundOffline(this.OrderId, amountToRefund);
                    }
                }
                else
                {
                    throw new NopException("Order is not found");
                }

                this.Page.ClientScript.RegisterStartupScript(typeof(RelatedProductAddControl), "closerefresh", "<script language=javascript>try {window.opener.document.forms[0]." + this.BtnId + ".click();}catch (e){} window.close();</script>");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        private string BtnId
        {
            get
            {
                return CommonHelper.QueryString("BtnId");
            }
        }

        public int OrderId
        {
            get
            {
                return CommonHelper.QueryStringInt("oid");
            }
        }

        public bool Online
        {
            get
            {
                return CommonHelper.QueryStringBool("online");
            }
        }
    }
}