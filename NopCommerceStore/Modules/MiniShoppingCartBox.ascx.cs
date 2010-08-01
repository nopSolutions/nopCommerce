using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class MiniShoppingCartBoxControl : BaseNopUserControl
    {
        protected void lvCart_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = e.Item as ListViewDataItem;
                if (dataItem != null)
                {
                    var sci = dataItem.DataItem as ShoppingCartItem;
                    if (sci != null)
                    {
                        var hlProduct = dataItem.FindControl("hlProduct") as HyperLink;
                        if (hlProduct != null)
                        {
                            hlProduct.NavigateUrl = SEOHelper.GetProductUrl(sci.ProductVariant.Product);
                            hlProduct.Text = Server.HtmlEncode(sci.ProductVariant.LocalizedFullProductName);
                        }

                        var lblQty = dataItem.FindControl("lblQty") as Label;
                        if (lblQty != null)
                        {
                            lblQty.Text = string.Format("{0} x ", sci.Quantity);
                        }                        
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (SettingManager.GetSettingValueBoolean("Common.ShowMiniShoppingCart"))
            {
                var shoppingCart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                if (shoppingCart.Count == 0)
                {
                    phCheckoutInfo.Visible = false;
                    lShoppingCart.Text = GetLocaleResourceString("MiniShoppingCartBox.NoItems");

                    lvCart.Visible = false;
                }
                else
                {
                    phCheckoutInfo.Visible = true;
                    if (shoppingCart.Count == 1)
                    {
                        lShoppingCart.Text = string.Format(GetLocaleResourceString("MiniShoppingCartBox.OneItemText"), string.Format("<a href=\"{0}\" class=\"items\">{1}</a>", SEOHelper.GetShoppingCartUrl(), GetLocaleResourceString("MiniShoppingCartBox.OneItem")));
                    }
                    else
                    {
                        lShoppingCart.Text = string.Format(GetLocaleResourceString("MiniShoppingCartBox.SeveralItemsText"), string.Format("<a href=\"{0}\" class=\"items\">{1}</a>", SEOHelper.GetShoppingCartUrl(), string.Format(GetLocaleResourceString("MiniShoppingCartBox.SeveralItems"), shoppingCart.Count)));
                    }

                    lblOrderSubtotal.Text = GetLocaleResourceString("MiniShoppingCartBox.OrderSubtotal", GetOrderSubtotal(shoppingCart));

                    if (SettingManager.GetSettingValueBoolean("Display.ItemsInMiniShoppingCart", false))
                    {
                        lvCart.Visible = true;
                        lvCart.DataSource = shoppingCart;
                        lvCart.DataBind();
                    }
                    else
                    {
                        lvCart.Visible = false;
                    }

                }
            }
            else
            {
                this.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected void BtnCheckout_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(SEOHelper.GetShoppingCartUrl());
        }

        protected string GetOrderSubtotal(ShoppingCart shoppingCart)
        {
            decimal subtotalBase = decimal.Zero;
            string SubTotalError = ShoppingCartManager.GetShoppingCartSubTotal(shoppingCart,
                NopContext.Current.User, out subtotalBase);

            if (String.IsNullOrEmpty(SubTotalError))
            {
                decimal subTotal = CurrencyManager.ConvertCurrency(subtotalBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                return PriceHelper.FormatPrice(subTotal);
            }
            else
            {
                return GetLocaleResourceString("MiniShoppingCartBox.OrderSubtotal.CalculatedDuringCheckout");
            }
        }
    }
}