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
using System.ComponentModel;
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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class OrderDetailsControl : BaseNopUserControl
    {
        #region Fields
        private Order order = null;
        #endregion

        #region Utilities

        protected void BindData()
        {
            this.lnkPrint.NavigateUrl = Page.ResolveUrl("~/PrintOrderDetails.aspx?OrderID=" + this.OrderId).ToLowerInvariant();
            this.lblOrderId.Text = order.OrderId.ToString();
            this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(order.CreatedOn, DateTimeKind.Utc).ToString("D");
            this.lblOrderStatus.Text = OrderManager.GetOrderStatusName(order.OrderStatusId);
            this.btnReOrder.Visible = OrderManager.IsReOrderAllowed;
            this.phReturnRequest.Visible = OrderManager.IsReturnRequestAllowed(order);

            //shipping info
            if (order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired)
            {
                this.pnlShipping.Visible = true;
                this.lShippingFirstName.Text = Server.HtmlEncode(order.ShippingFirstName);
                this.lShippingLastName.Text = Server.HtmlEncode(order.ShippingLastName);
                this.lShippingPhoneNumber.Text = Server.HtmlEncode(order.ShippingPhoneNumber);
                this.lShippingEmail.Text = Server.HtmlEncode(order.ShippingEmail);
                this.lShippingFaxNumber.Text = Server.HtmlEncode(order.ShippingFaxNumber);
                if (!String.IsNullOrEmpty(order.ShippingCompany))
                    this.lShippingCompany.Text = Server.HtmlEncode(order.ShippingCompany);
                else
                    pnlShippingCompany.Visible = false;
                this.lShippingAddress1.Text = Server.HtmlEncode(order.ShippingAddress1);
                if (!String.IsNullOrEmpty(order.ShippingAddress2))
                    this.lShippingAddress2.Text = Server.HtmlEncode(order.ShippingAddress2);
                else
                    pnlShippingAddress2.Visible = false;
                this.lShippingCity.Text = Server.HtmlEncode(order.ShippingCity);
                this.lShippingStateProvince.Text = Server.HtmlEncode(order.ShippingStateProvince);
                this.lShippingZipPostalCode.Text = Server.HtmlEncode(order.ShippingZipPostalCode);
                if (!String.IsNullOrEmpty(order.ShippingCountry))
                    this.lShippingCountry.Text = Server.HtmlEncode(order.ShippingCountry);
                else
                    pnlShippingCountry.Visible = false;

                this.lblShippingMethod.Text = Server.HtmlEncode(order.ShippingMethod);
                this.lblOrderWeight.Text = string.Format("{0:F2} [{1}]", order.OrderWeight, MeasureManager.BaseWeightIn.Name);

                if (order.ShippedDate.HasValue)
                    this.lblShippedDate.Text = DateTimeHelper.ConvertToUserTime(order.ShippedDate.Value, DateTimeKind.Utc).ToString("D");
                else
                    this.lblShippedDate.Text = GetLocaleResourceString("Order.NotYetShipped");

                if (order.DeliveryDate.HasValue)
                    this.lblDeliveredOn.Text = DateTimeHelper.ConvertToUserTime(order.DeliveryDate.Value, DateTimeKind.Utc).ToString("D");
                else
                    this.lblDeliveredOn.Text = GetLocaleResourceString("Order.Order.NotYetDelivered");

                if (!string.IsNullOrEmpty(order.TrackingNumber))
                {
                    lblTrackingNumber.Text = order.TrackingNumber;
                    pnlTrackingNumber.Visible = true;
                }
                else
                    pnlTrackingNumber.Visible = false;

                this.pnlShippingTotal.Visible = true;
            }
            else
            {
                this.pnlShipping.Visible = false;
                this.pnlShippingTotal.Visible = false;
            }

            //billing info
            this.lBillingFirstName.Text = Server.HtmlEncode(order.BillingFirstName);
            this.lBillingLastName.Text = Server.HtmlEncode(order.BillingLastName);
            this.lBillingPhoneNumber.Text = Server.HtmlEncode(order.BillingPhoneNumber);
            this.lBillingEmail.Text = Server.HtmlEncode(order.BillingEmail);
            this.lBillingFaxNumber.Text = Server.HtmlEncode(order.BillingFaxNumber);
            if (!String.IsNullOrEmpty(order.BillingCompany))
                this.lBillingCompany.Text = Server.HtmlEncode(order.BillingCompany);
            else
                pnlBillingCompany.Visible = false;
            this.lBillingAddress1.Text = Server.HtmlEncode(order.BillingAddress1);
            if (!String.IsNullOrEmpty(order.BillingAddress2))
                this.lBillingAddress2.Text = Server.HtmlEncode(order.BillingAddress2);
            else
                pnlBillingAddress2.Visible = false;
            this.lBillingCity.Text = Server.HtmlEncode(order.BillingCity);
            this.lBillingStateProvince.Text = Server.HtmlEncode(order.BillingStateProvince);
            this.lBillingZipPostalCode.Text = Server.HtmlEncode(order.BillingZipPostalCode);
            if (!String.IsNullOrEmpty(order.BillingCountry))
                this.lBillingCountry.Text = Server.HtmlEncode(order.BillingCountry);
            else
                pnlBillingCountry.Visible = false;

            //VAT number
            if (!String.IsNullOrEmpty(order.VatNumber))
                this.lVatNumber.Text = Server.HtmlEncode(order.VatNumber);
            else
                phVatNumber.Visible = false;

            //payment method
            var paymentMethod = PaymentMethodManager.GetPaymentMethodById(order.PaymentMethodId);
            if (paymentMethod != null)
                this.lPaymentMethod.Text = paymentMethod.VisibleName;
            else
                this.lPaymentMethod.Text = order.PaymentMethodName;

            //totals
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    {
                        this.lblOrderSubtotal.Text = PriceHelper.FormatPrice(order.OrderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, false);
                        this.lblOrderShipping.Text = PriceHelper.FormatShippingPrice(order.OrderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, false);
                        this.lblPaymentMethodAdditionalFee.Text = PriceHelper.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, false);
                    }
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    {
                        this.lblOrderSubtotal.Text = PriceHelper.FormatPrice(order.OrderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, true);
                        this.lblOrderShipping.Text = PriceHelper.FormatShippingPrice(order.OrderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, true);
                        this.lblPaymentMethodAdditionalFee.Text = PriceHelper.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, true);
                    }
                    break;
            }

            bool displayPaymentMethodFee = true;
            if (order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency == decimal.Zero)
            {
                displayPaymentMethodFee = false;
            }
            phPaymentMethodAdditionalFee.Visible = displayPaymentMethodFee;
            
            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (TaxManager.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayTypeEnum.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && TaxManager.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates = order.TaxRatesDictionaryInCustomerCurrency;

                    displayTaxRates = TaxManager.DisplayTaxRates && taxRates.Count > 0;
                    displayTax = !displayTaxRates;

                    string taxStr = PriceHelper.FormatPrice(order.OrderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                    this.lblOrderTax.Text = taxStr;
                    rptrTaxRates.DataSource = taxRates;
                    rptrTaxRates.DataBind();
                }
            }
            rptrTaxRates.Visible = displayTaxRates;
            phTaxTotal.Visible = displayTax;

            //discount
            if (order.OrderDiscountInCustomerCurrency > decimal.Zero)
            {
                phDiscount.Visible = true;
                string discountStr = PriceHelper.FormatPrice(-order.OrderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                this.lblDiscount.Text = discountStr;
            }
            else
            {
                phDiscount.Visible = false;
            }

            //gift cards
            var gcuhC = OrderManager.GetAllGiftCardUsageHistoryEntries(null, null, order.OrderId);
            if (gcuhC.Count > 0)
            {
                rptrGiftCards.Visible = true;
                rptrGiftCards.DataSource = gcuhC;
                rptrGiftCards.DataBind();
            }
            else
            {
                rptrGiftCards.Visible = false;
            }

            //reward points           
            if (order.RedeemedRewardPoints!=null)
            { 
                phRewardPoints.Visible = true;
                lRewardPointsTitle.Text = string.Format(GetLocaleResourceString("Order.Totals.RewardPoints"), -order.RedeemedRewardPoints.Points);
                lblRewardPointsAmount.Text = PriceHelper.FormatPrice(-order.RedeemedRewardPoints.UsedAmountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
            }
            else
            {
                phRewardPoints.Visible = false;
            }

            //total
            string orderTotalStr = PriceHelper.FormatPrice(order.OrderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);
            this.lblOrderTotal.Text = orderTotalStr;
            this.lblOrderTotal2.Text = orderTotalStr;

            //purchased products
            var orderProductVariants = order.OrderProductVariants;
            bool hasDownloadableItems = false;
            foreach (var orderProductVariant in orderProductVariants)
            {
                var productVariant = orderProductVariant.ProductVariant;
                if (productVariant != null && productVariant.IsDownload)
                {
                    hasDownloadableItems = true;
                    break;
                }
            }

            //sku column
            gvOrderProductVariants.Columns[0].Visible = SettingManager.GetSettingValueBoolean("Display.Products.ShowSKU");
            //downloads column
            gvOrderProductVariants.Columns[2].Visible = hasDownloadableItems && !this.IsInvoice;

            //bind data
            gvOrderProductVariants.DataSource = orderProductVariants;
            gvOrderProductVariants.DataBind();

            //checkout attributes
            lCheckoutAttributes.Text = order.CheckoutAttributeDescription;

            var orderNoteCollection = order.OrderNotes;
            if(orderNoteCollection.Count > 0)
            {
                gvOrderNotes.DataSource = order.OrderNotes;
                gvOrderNotes.DataBind();
            }
            else
            {
                pnlOrderNotesTitle.Visible = false;
                pnlOrderNotes.Visible = false;
            }
        }

        #endregion

        #region Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }
            order = OrderManager.GetOrderById(this.OrderId);
            if (order == null || order.Deleted || NopContext.Current.User.CustomerId != order.CustomerId)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            if (!Page.IsPostBack)
            {
                this.BindData();
            }

            //buttons
            lbPDFInvoice.Visible = SettingManager.GetSettingValueBoolean("Features.SupportPDF");
        }

        protected void BtnReOrder_OnClick(object sender, EventArgs e)
        {
            try
            {
                OrderManager.ReOrder(this.OrderId);
                Response.Redirect(SEOHelper.GetShoppingCartUrl());
            }
            catch(Exception)
            {
            }
        }

        protected void btnReturnItems_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/returnitems.aspx?orderid={0}", this.OrderId));
        }
        
        protected void lbPDFInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = string.Format("order_{0}_{1}.pdf", order.OrderGuid, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                string filePath = string.Format("{0}files\\ExportImport\\{1}", HttpContext.Current.Request.PhysicalApplicationPath, fileName);

                PDFHelper.PrintOrderToPdf(order, NopContext.Current.WorkingLanguage.LanguageId, filePath);
                CommonHelper.WriteResponsePdf(filePath, fileName);
            }
            catch(Exception ex)
            {
                LogManager.InsertLog(LogTypeEnum.CustomerError, "Error generating PDF", ex);
            }
        }

        protected void rptrTaxRates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var item = (KeyValuePair<decimal, decimal>)e.Item.DataItem;

                var lTaxRateTitle = e.Item.FindControl("lTaxRateTitle") as Literal;
                lTaxRateTitle.Text = String.Format(GetLocaleResourceString("Order.Totals.TaxRate"), TaxManager.FormatTaxRate(item.Key));

                var lTaxRateValue = e.Item.FindControl("lTaxRateValue") as Literal;
                lTaxRateValue.Text = PriceHelper.FormatPrice(item.Value, true, false);
            }
        }

        protected void rptrGiftCards_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var giftCardUsageHistory = e.Item.DataItem as GiftCardUsageHistory;

                var lGiftCard = e.Item.FindControl("lGiftCard") as Literal;
                lGiftCard.Text = String.Format(GetLocaleResourceString("Order.GiftCardInfo"), Server.HtmlEncode(giftCardUsageHistory.GiftCard.GiftCardCouponCode));

                var lblGiftCardAmount = e.Item.FindControl("lblGiftCardAmount") as Label;
                lblGiftCardAmount.Text = PriceHelper.FormatPrice(-giftCardUsageHistory.UsedValueInCustomerCurrency, true, order.CustomerCurrencyCode, false);
            }
        }

        #endregion

        #region Methods
        public string GetProductVariantName(int productVariantId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant != null)
                return productVariant.LocalizedFullProductName;
            return "Not available. ID=" + productVariantId.ToString();
        }
        
        public string GetAttributeDescription(OrderProductVariant opv)
        {
            string result = opv.AttributeDescription;
            if (!String.IsNullOrEmpty(result))
                result = "<br />" + result;
            return result;
        }

        public string GetProductUrl(int productVariantId)
        {
            var productVariant = ProductManager.GetProductVariantById(productVariantId);
            if (productVariant != null)
                return SEOHelper.GetProductUrl(productVariant.ProductId);
            return string.Empty;
        }

        public string GetDownloadUrl(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            if (OrderManager.IsDownloadAllowed(orderProductVariant))
            {
                result = string.Format("<a class=\"link\" href=\"{0}\" >{1}</a>", DownloadManager.GetDownloadUrl(orderProductVariant), GetLocaleResourceString("Order.Download"));
            }
            else
            {
                result = GetLocaleResourceString("Order.ProductsGrid.Download.na");
            }
            return result;
        }

        public string GetLicenseDownloadUrl(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            if (OrderManager.IsLicenseDownloadAllowed(orderProductVariant))
            {
                result = string.Format("<a class=\"link\" href=\"{0}\" >{1}</a>", DownloadManager.GetLicenseDownloadUrl(orderProductVariant), GetLocaleResourceString("Order.DownloadLicense"));
            }
            return result;
        }

        public string GetProductVariantUnitPrice(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    result = PriceHelper.FormatPrice(orderProductVariant.UnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, false);
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    result = PriceHelper.FormatPrice(orderProductVariant.UnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, true);
                    break;
            }

            return result;
        }

        public string GetProductVariantSubTotal(OrderProductVariant orderProductVariant)
        {
            string result = string.Empty;
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    result = PriceHelper.FormatPrice(orderProductVariant.PriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, false);
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    result = PriceHelper.FormatPrice(orderProductVariant.PriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, NopContext.Current.WorkingLanguage, true);
                    break;
            }
            
           return result;
        }
        
        #endregion

        #region Properties

        public int OrderId
        {
            get
            {
                return CommonHelper.QueryStringInt("OrderId");
            }
        }

        public bool IsInvoice
        {
            get
            {
                if (ViewState["IsInvoice"] == null)
                    return false;
                else
                    return (bool)ViewState["IsInvoice"];
            }
            set
            {
                ViewState["IsInvoice"] = value; 
            }
        }

        #endregion
    }
}