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
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.ShippingByWeightAndCountryConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                gvShippingByWeightAndCountry.Columns[2].HeaderText = string.Format("From [{0}]", MeasureManager.BaseWeightIn.Name);
                gvShippingByWeightAndCountry.Columns[3].HeaderText = string.Format("To [{0}]", MeasureManager.BaseWeightIn.Name);
                gvShippingByWeightAndCountry.Columns[6].HeaderText = "Charge amount";
                if (ShippingByWeightAndCountryManager.CalculatePerWeightUnit)
                {
                    gvShippingByWeightAndCountry.Columns[6].HeaderText += string.Format(" per {0}", MeasureManager.BaseWeightIn.Name);
                }
                FillDropDowns();
                BindSettings();
                BindData();
            }
        }

        private void FillDropDowns()
        {
            ddlShippingMethod.Items.Clear();
            var shippingMethodCollection = ShippingMethodManager.GetAllShippingMethods();
            foreach (ShippingMethod shippingMethod in shippingMethodCollection)
            {
                ListItem item = new ListItem(shippingMethod.Name, shippingMethod.ShippingMethodId.ToString());
                ddlShippingMethod.Items.Add(item);
            }

            ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountries();
            foreach (Country country in countryCollection)
            {
                ListItem item = new ListItem(country.Name, country.CountryId.ToString());
                ddlCountry.Items.Add(item);
            }
        }

        private void BindData()
        {
            var shippingByWeightAndCountryCollection = ShippingByWeightAndCountryManager.GetAll();
            gvShippingByWeightAndCountry.DataSource = shippingByWeightAndCountryCollection;
            gvShippingByWeightAndCountry.DataBind();
        }

        private void BindSettings()
        {
            cbLimitMethodsToCreated.Checked = SettingManager.GetSettingValueBoolean("ShippingByWeightAndCountry.LimitMethodsToCreated");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int shippingMethodId = int.Parse(this.ddlShippingMethod.SelectedItem.Value);
                int countryId = int.Parse(this.ddlCountry.SelectedItem.Value);
                ShippingByWeightAndCountry shippingByWeightAndCountry = ShippingByWeightAndCountryManager.InsertShippingByWeightAndCountry(shippingMethodId,
                    countryId, txtFrom.Value, txtTo.Value, cbUsePercentage.Checked,
                    txtShippingChargePercentage.Value, txtShippingChargeAmount.Value);

                BindData();
            }
            catch (Exception exc)
            {
                processAjaxError(exc);
            }
        }

        protected void gvShippingByWeightAndCountry_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateShippingByWeightAndCountry")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvShippingByWeightAndCountry.Rows[index];

                HiddenField hfShippingByWeightAndCountryId = row.FindControl("hfShippingByWeightAndCountryId") as HiddenField;
                DropDownList ddlShippingMethod = row.FindControl("ddlShippingMethod") as DropDownList;
                DropDownList ddlCountry = row.FindControl("ddlCountry") as DropDownList;
                DecimalTextBox txtFrom = row.FindControl("txtFrom") as DecimalTextBox;
                DecimalTextBox txtTo = row.FindControl("txtTo") as DecimalTextBox;
                CheckBox cbUsePercentage = row.FindControl("cbUsePercentage") as CheckBox;
                DecimalTextBox txtShippingChargePercentage = row.FindControl("txtShippingChargePercentage") as DecimalTextBox;
                DecimalTextBox txtShippingChargeAmount = row.FindControl("txtShippingChargeAmount") as DecimalTextBox;

                int shippingByWeightAndCountryId = int.Parse(hfShippingByWeightAndCountryId.Value);
                int shippingMethodId = int.Parse(ddlShippingMethod.SelectedItem.Value);
                int countryId = int.Parse(ddlCountry.SelectedItem.Value);
                ShippingByWeightAndCountry shippingByWeightAndCountry = ShippingByWeightAndCountryManager.GetById(shippingByWeightAndCountryId);

                if (shippingByWeightAndCountry != null)
                    ShippingByWeightAndCountryManager.UpdateShippingByWeightAndCountry(shippingByWeightAndCountry.ShippingByWeightAndCountryId,
                      shippingMethodId, countryId, txtFrom.Value, txtTo.Value, cbUsePercentage.Checked,
                      txtShippingChargePercentage.Value, txtShippingChargeAmount.Value);

                BindData();
            }
        }

        protected void gvShippingByWeightAndCountry_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ShippingByWeightAndCountry shippingByWeightAndCountry = (ShippingByWeightAndCountry)e.Row.DataItem;

                Button btnUpdate = e.Row.FindControl("btnUpdate") as Button;
                if (btnUpdate != null)
                    btnUpdate.CommandArgument = e.Row.RowIndex.ToString();

                DropDownList ddlShippingMethod = e.Row.FindControl("ddlShippingMethod") as DropDownList;
                ddlShippingMethod.Items.Clear();
                var shippingMethodCollection = ShippingMethodManager.GetAllShippingMethods();
                foreach (ShippingMethod shippingMethod in shippingMethodCollection)
                {
                    ListItem item = new ListItem(shippingMethod.Name, shippingMethod.ShippingMethodId.ToString());
                    ddlShippingMethod.Items.Add(item);
                    if (shippingByWeightAndCountry.ShippingMethodId == shippingMethod.ShippingMethodId)
                        item.Selected = true;
                }


                DropDownList ddlCountry = e.Row.FindControl("ddlCountry") as DropDownList;
                ddlCountry.Items.Clear();
                var countryCollection = CountryManager.GetAllCountries();
                foreach (Country country in countryCollection)
                {
                    ListItem item = new ListItem(country.Name, country.CountryId.ToString());
                    ddlCountry.Items.Add(item);
                    if (shippingByWeightAndCountry.CountryId == country.CountryId)
                        item.Selected = true;
                }
            }
        }

        protected void gvShippingByWeightAndCountry_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int shippingByWeightAndCountryId = (int)gvShippingByWeightAndCountry.DataKeys[e.RowIndex]["ShippingByWeightAndCountryId"];
            ShippingByWeightAndCountry shippingByWeightAndCountry = ShippingByWeightAndCountryManager.GetById(shippingByWeightAndCountryId);
            if (shippingByWeightAndCountry != null)
            {
                ShippingByWeightAndCountryManager.DeleteShippingByWeightAndCountry(shippingByWeightAndCountry.ShippingByWeightAndCountryId);
                BindData();
            }
        }
        
        protected void processAjaxError(Exception exc)
        {
            ProcessException(exc, false);
            pnlError.Visible = true;
            lErrorTitle.Text = exc.Message;
        }
        
        public void Save()
        {
            SettingManager.SetParam("ShippingByWeightAndCountry.LimitMethodsToCreated", cbLimitMethodsToCreated.Checked.ToString());
        }
    }
}
