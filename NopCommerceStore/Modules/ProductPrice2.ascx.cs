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
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using System.Text.RegularExpressions;
using System.Globalization;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductPrice2Control : BaseNopUserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            this.BindData();

            base.OnPreRender(e);
        }

        private void BindData()
        {
            var product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                var productVariantCollection = product.ProductVariants;
                if (productVariantCollection.Count > 0)
                {
                    if (!product.HasMultipleVariants)
                    {
                        var productVariant = productVariantCollection[0];
                        if (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                            (NopContext.Current.User != null &&
                            !NopContext.Current.User.IsGuest))
                        {
                            if (productVariant.CustomerEntersPrice)
                            {
                                lblOldPrice.Visible = false;
                                lblPrice.Visible = false;
                            }
                            else
                            {
                                if (productVariant.CallForPrice)
                                {
                                    lblOldPrice.Visible = false;
                                    lblPrice.Text = GetLocaleResourceString("Products.CallForPrice");
                                }
                                else
                                {
                                    decimal taxRate = decimal.Zero;
                                    decimal oldPriceBase = TaxManager.GetPrice(productVariant, productVariant.OldPrice, out taxRate);
                                    decimal finalPriceBase = TaxManager.GetPrice(productVariant, PriceHelper.GetFinalPrice(productVariant, true), out taxRate);

                                    decimal oldPrice = CurrencyManager.ConvertCurrency(oldPriceBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                    decimal finalPrice = CurrencyManager.ConvertCurrency(finalPriceBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);

                                    if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                    {
                                        lblOldPrice.Text = PriceHelper.FormatPrice(oldPrice);
                                        lblPrice.Text = PriceHelper.FormatPrice(finalPrice);
                                    }
                                    else
                                    {
                                        lblOldPrice.Visible = false;
                                        lblPrice.Text = PriceHelper.FormatPrice(finalPrice);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lblOldPrice.Visible = false;
                            lblPrice.Visible = false;
                        }
                    }
                    else
                    {
                        var productVariant = product.MinimalPriceProductVariant;
                        if (productVariant != null)
                        {
                            if (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                                (NopContext.Current.User != null &&
                                !NopContext.Current.User.IsGuest))
                            {
                                if (productVariant.CustomerEntersPrice)
                                {
                                    lblOldPrice.Visible = false;
                                    lblPrice.Visible = false;
                                }
                                else
                                {
                                    if (productVariant.CallForPrice)
                                    {
                                        lblOldPrice.Visible = false;
                                        lblPrice.Text = GetLocaleResourceString("Products.CallForPrice");
                                    }
                                    else
                                    {
                                        decimal taxRate = decimal.Zero;
                                        decimal fromPriceBase = TaxManager.GetPrice(productVariant, PriceHelper.GetFinalPrice(productVariant, false), out taxRate);
                                        decimal fromPrice = CurrencyManager.ConvertCurrency(fromPriceBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                        lblOldPrice.Visible = false;
                                        lblPrice.Text = String.Format(GetLocaleResourceString("Products.PriceRangeFromText"), PriceHelper.FormatPrice(fromPrice));
                                    }
                                }
                            }
                            else
                            {
                                lblOldPrice.Visible = false;
                                lblPrice.Visible = false;
                            }
                        }
                    }
                }
                else
                {
                    lblOldPrice.Visible = false;
                    lblPrice.Visible = false;
                }
            }
        }

        public int ProductId
        {
            get
            {
                object obj2 = this.ViewState["ProductId"];
                if (obj2 != null)
                    return (int)obj2;
                else
                    return 0;
            }
            set
            {
                this.ViewState["ProductId"] = value;
            }
        }
    }
}