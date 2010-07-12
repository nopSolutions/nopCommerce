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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class GiftCardInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            GiftCard gc = OrderManager.GetGiftCardById(this.GiftCardId);
            if (gc != null)
            {
                this.lblOrder.Text = string.Format("<a href=\"OrderDetails.aspx?OrderID={0}\">{1}</a>", gc.PurchasedOrderProductVariant.OrderId, GetLocaleResourceString("Admin.GiftCardInfo.Order.View"));
                this.lblCustomer.Text = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", gc.PurchasedOrderProductVariant.Order.CustomerId, GetLocaleResourceString("Admin.GiftCardInfo.Customer.View"));
                
                this.txtInitialValue.Value =  GiftCardHelper.GetGiftCardInitialValue(gc);
                decimal remainingAmount = GiftCardHelper.GetGiftCardRemainingAmount(gc);
                this.lblRemainingAmount.Text = PriceHelper.FormatPrice(remainingAmount, true, false);
                this.cbIsGiftCardActivated.Checked = gc.IsGiftCardActivated;
                this.txtCouponCode.Text = gc.GiftCardCouponCode;
                this.txtRecipientName.Text = gc.RecipientName;
                this.txtRecipientEmail.Text = gc.RecipientEmail;
                this.txtSenderName.Text = gc.SenderName;
                this.txtSenderEmail.Text = gc.SenderEmail;
                this.txtMessage.Text = gc.Message;
                if (gc.IsRecipientNotified)
                {
                    lblIsRecipientNotified.Text = GetLocaleResourceString("Admin.Common.Yes");
                }
                else
                {
                    lblIsRecipientNotified.Text = GetLocaleResourceString("Admin.Common.No");
                }
                this.lblPurchasedOn.Text = DateTimeHelper.ConvertToUserTime(gc.CreatedOn, DateTimeKind.Utc).ToString();

                this.pnlRecipientEmail.Visible = (GiftCardTypeEnum)gc.PurchasedOrderProductVariant.ProductVariant.GiftCardType == GiftCardTypeEnum.Virtual;
                this.pnlSenderEmail.Visible = (GiftCardTypeEnum)gc.PurchasedOrderProductVariant.ProductVariant.GiftCardType == GiftCardTypeEnum.Virtual;
                this.pnlIsRecipientNotified.Visible = (GiftCardTypeEnum)gc.PurchasedOrderProductVariant.ProductVariant.GiftCardType == GiftCardTypeEnum.Virtual;
            }
            else
            {
                Response.Redirect("PurchasedGiftCards.aspx");
            }
        }

        private void BindUsageHistory()
        {
            GiftCard gc = OrderManager.GetGiftCardById(this.GiftCardId);
            if (gc != null)
            {
                var giftCardUsageHistory = OrderManager.GetAllGiftCardUsageHistoryEntries(gc.GiftCardId, null, null);
                gvUsageHistory.DataSource = giftCardUsageHistory;
                gvUsageHistory.DataBind();
            }
        }

        public GiftCard SaveInfo()
        {
            GiftCard gc = OrderManager.GetGiftCardById(this.GiftCardId);

            decimal initialValue = txtInitialValue.Value;
            bool isGiftCardActivated = cbIsGiftCardActivated.Checked;
            string giftCardCouponCode = txtCouponCode.Text;
            string recipientName = txtRecipientName.Text.Trim();
            string recipientEmail = txtRecipientEmail.Text.Trim();
            string senderName = txtSenderName.Text.Trim();
            string senderEmail = txtSenderEmail.Text.Trim();
            string message = txtMessage.Text.Trim();

            if (gc != null)
            {
                gc = OrderManager.UpdateGiftCard(gc.GiftCardId,
                    gc.PurchasedOrderProductVariantId, initialValue, isGiftCardActivated,
                    giftCardCouponCode, recipientName, recipientEmail,
                    senderName, senderEmail, message,
                    gc.IsRecipientNotified, gc.CreatedOn);
            }
            else
            {
                Response.Redirect("PurchasedGiftCards.aspx");
            }

            return gc;
        }

        protected string GetUsedValueInfo(GiftCardUsageHistory gcuh)
        {
            string result = PriceHelper.FormatPrice(gcuh.UsedValue, true, false);
            return result;
        }

        protected string GetUsedByCustomerInfo(GiftCardUsageHistory gcuh)
        {
            string customerInfo = string.Empty;
            Customer customer = gcuh.Customer;
            if (customer != null)
            {
                if (customer.IsGuest)
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, GetLocaleResourceString("Admin.GiftCardInfo.UsageHistory.ByCustomerColumn.Guest"));
                }
                else
                {
                    customerInfo = string.Format("<a href=\"CustomerDetails.aspx?CustomerID={0}\">{1}</a>", customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
            }
            return customerInfo;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
                this.BindUsageHistory();
            }
        }

        protected void gvUsageHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gvUsageHistory.PageIndex = e.NewPageIndex;
            BindUsageHistory();
        }

        protected void btnReGenerateNewCode_Click(object sender, EventArgs e)
        {
            try
            {
                string newCode = GiftCardHelper.GenerateGiftCardCode();
                txtCouponCode.Text = newCode;
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnNotifyRecipient_Click(object sender, EventArgs e)
        {
            try
            {
                GiftCard gc = SaveInfo();
                Language customerLang = LanguageManager.GetLanguageById(gc.PurchasedOrderProductVariant.Order.CustomerLanguageId);
                if (customerLang==null)
                    customerLang = NopContext.Current.WorkingLanguage;
                int queuedEmailId = MessageManager.SendGiftCardNotification(gc, customerLang.LanguageId);
                if (queuedEmailId > 0)
                {
                    gc = OrderManager.UpdateGiftCard(gc.GiftCardId,
                        gc.PurchasedOrderProductVariantId, gc.Amount, gc.IsGiftCardActivated,
                        gc.GiftCardCouponCode, gc.RecipientName, gc.RecipientEmail,
                        gc.SenderName, gc.SenderEmail, gc.Message,
                        true, gc.CreatedOn);
                    BindData();
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }
        
        public int GiftCardId
        {
            get
            {
                return CommonHelper.QueryStringInt("GiftCardId");
            }
        }
    }
}