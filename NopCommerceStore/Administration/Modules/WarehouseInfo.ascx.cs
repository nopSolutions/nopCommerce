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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class WarehouseInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Warehouse warehouse = WarehouseManager.GetWarehouseById(this.WarehouseId);
            if (warehouse != null)
            {
                this.txtName.Text = warehouse.Name;
                this.txtPhoneNumber.Text = warehouse.PhoneNumber;
                this.txtEmail.Text = warehouse.Email;
                this.txtFaxNumber.Text = warehouse.FaxNumber;
                this.txtAddress1.Text = warehouse.Address1;
                this.txtAddress2.Text = warehouse.Address2;
                this.txtCity.Text = warehouse.City;
                this.txtStateProvince.Text = warehouse.StateProvince;
                this.txtZipPostalCode.Text = warehouse.ZipPostalCode;
                CommonHelper.SelectListItem(this.ddlCountry, warehouse.CountryId);
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(warehouse.CreatedOn, DateTimeKind.Utc).ToString();
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(warehouse.UpdatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        private void FillDropDowns()
        {
            this.ddlCountry.Items.Clear();
            var countryCollection = CountryManager.GetAllCountries();
            foreach (Country country in countryCollection)
            {
                ListItem ddlCountryItem2 = new ListItem(country.Name, country.CountryId.ToString());
                this.ddlCountry.Items.Add(ddlCountryItem2);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public Warehouse SaveInfo()
        {
            Warehouse warehouse = WarehouseManager.GetWarehouseById(this.WarehouseId);

            if (warehouse != null)
            {
                warehouse = WarehouseManager.UpdateWarehouse(warehouse.WarehouseId, txtName.Text,
                    txtPhoneNumber.Text, txtEmail.Text, txtFaxNumber.Text,
                    txtAddress1.Text, txtAddress2.Text, txtCity.Text, txtStateProvince.Text, txtZipPostalCode.Text,
                    int.Parse(this.ddlCountry.SelectedItem.Value), warehouse.Deleted, warehouse.CreatedOn, DateTime.UtcNow);

            }
            else
            {
                DateTime now = DateTime.UtcNow;
                warehouse = WarehouseManager.InsertWarehouse(txtName.Text,
                   txtPhoneNumber.Text, txtEmail.Text, txtFaxNumber.Text,
                   txtAddress1.Text, txtAddress2.Text, txtCity.Text, txtStateProvince.Text, txtZipPostalCode.Text,
                   int.Parse(this.ddlCountry.SelectedItem.Value), false, now, now);

            }

            return warehouse;
        }

        public int WarehouseId
        {
            get
            {
                return CommonHelper.QueryStringInt("WarehouseId");
            }
        }
    }
}