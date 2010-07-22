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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class EstimateShippingControl : BaseNopUserControl
    {
        protected ShoppingCart cart = null;

        private void FillCountryDropDownsForShipping()
        {
            this.ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountriesForShipping();
            foreach (var country in countryCollection)
            {
                var ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlCountry.Items.Add(ddlCountryItem2);
            }
        }
        
        private void FillStateProvinceDropDowns()
        {
            this.ddlStateProvince.Items.Clear();
            int countryId = 0;
            if (this.ddlCountry.SelectedItem != null)
                countryId = int.Parse(this.ddlCountry.SelectedItem.Value);

            var stateProvinceCollection = StateProvinceManager.GetStateProvincesByCountryId(countryId);
            foreach (var stateProvince in stateProvinceCollection)
            {
                var ddlStateProviceItem2 = new ListItem(stateProvince.Name, stateProvince.StateProvinceId.ToString());
                this.ddlStateProvince.Items.Add(ddlStateProviceItem2);
            }
            if (stateProvinceCollection.Count == 0)
            {
                var ddlStateProvinceItem = new ListItem(GetLocaleResourceString("EstimateShipping.StateProvinceNonUS"), "0");
                this.ddlStateProvince.Items.Add(ddlStateProvinceItem);
            }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillStateProvinceDropDowns();
        }

        protected string FormatShippingOption(ShippingOption shippingOption)
        {
            //calculate discounted and taxed rate
            Discount appliedDiscount = null;
            decimal shippingTotalWithoutDiscount = shippingOption.Rate;
            decimal discountAmount = ShippingManager.GetShippingDiscount(NopContext.Current.User, 
                shippingTotalWithoutDiscount, out appliedDiscount);
            decimal shippingTotalWithDiscount = shippingTotalWithoutDiscount - discountAmount;
            if (shippingTotalWithDiscount < decimal.Zero)
                shippingTotalWithDiscount = decimal.Zero;
            shippingTotalWithDiscount = Math.Round(shippingTotalWithDiscount, 2);

            decimal rateBase = TaxManager.GetShippingPrice(shippingTotalWithDiscount, NopContext.Current.User);
            decimal rate = CurrencyManager.ConvertCurrency(rateBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
            string rateStr = PriceHelper.FormatShippingPrice(rate, true);
            return string.Format("({0})", rateStr);
        }

        protected Address GetEnteredAddress()
        {
            Address address = new Address();
            if (this.ddlCountry.SelectedItem == null)
                throw new NopException("Countries are not populated");
            address.CountryId = int.Parse(this.ddlCountry.SelectedItem.Value);

            if (this.ddlStateProvince.SelectedItem == null)
                throw new NopException("State/Provinces are not populated");
            var stateProvince = StateProvinceManager.GetStateProvinceById(int.Parse(this.ddlStateProvince.SelectedItem.Value));
            if (stateProvince != null && stateProvince.CountryId == address.CountryId)
                address.StateProvinceId = stateProvince.StateProvinceId;

            address.ZipPostalCode = txtZipPostalCode.Text.Trim();

            return address;
        }

        protected void BindData()
        {
            bool shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(Cart);
            if (!shoppingCartRequiresShipping || 
                this.Cart.Count == 0 ||
                !SettingManager.GetSettingValueBoolean("Shipping.EstimateShipping.Enabled"))
            {
                this.Visible = false;
            }
            else
            {
                this.Visible = true;
                this.phShippingMethods.Visible = true;
            }
        }

        protected void BindMethods()
        {
            bool shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(Cart);
            if (shoppingCartRequiresShipping)
            {
                string error = string.Empty;
                Address address = GetEnteredAddress();
                var shippingOptions = ShippingManager.GetShippingOptions(Cart, NopContext.Current.User, address, ref error);
                if (!String.IsNullOrEmpty(error))
                {
                    LogManager.InsertLog(LogTypeEnum.ShippingError, error, error);
                    phShippingMethods.Visible = false;

                    pnlWarnings.Visible = true;
                    lblWarning.Visible = true;
                    lblWarning.Text = Server.HtmlEncode(error);
                }
                else
                {
                    if (shippingOptions.Count > 0)
                    {
                        phShippingMethods.Visible = true;

                        pnlWarnings.Visible = false;
                        lblWarning.Visible = false;

                        dlShippingOptions.DataSource = shippingOptions;
                        dlShippingOptions.DataBind();
                    }
                    else
                    {
                        phShippingMethods.Visible = false;

                        pnlWarnings.Visible = true;
                        lblWarning.Visible = true;
                        lblWarning.Text = GetLocaleResourceString("Checkout.ShippingIsNotAllowed");
                    }
                }
            }
        }

        protected void btnGetQuote_Click(object sender, EventArgs e)
        {
            BindMethods();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillCountryDropDownsForShipping();
                this.FillStateProvinceDropDowns();
                this.BindData();
            }
        }

        public ShoppingCart Cart
        {
            get
            {
                if (cart == null)
                {
                    cart = ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                }
                return cart;
            }
        }
    }
}