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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class StateProvinceInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            StateProvince stateProvince = this.StateProvinceService.GetStateProvinceById(this.StateProvinceId);
            if (stateProvince != null)
            {
                CommonHelper.SelectListItem(this.ddlCountry, stateProvince.CountryId);
                this.txtName.Text = stateProvince.Name;
                this.txtAbbreviation.Text = stateProvince.Abbreviation;
                this.txtDisplayOrder.Value = stateProvince.DisplayOrder;
            }
        }

        private void FillDropDowns()
        {
            this.ddlCountry.Items.Clear();
            var countryCollection = this.CountryService.GetAllCountries();
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

        public StateProvince SaveInfo()
        {
            StateProvince stateProvince = this.StateProvinceService.GetStateProvinceById(this.StateProvinceId);
            if (stateProvince != null)
            {
                stateProvince.CountryId = int.Parse(this.ddlCountry.SelectedItem.Value);
                stateProvince.Name = txtName.Text;
                stateProvince.Abbreviation = txtAbbreviation.Text;
                stateProvince.DisplayOrder = txtDisplayOrder.Value;
                this.StateProvinceService.UpdateStateProvince(stateProvince);
            }
            else
            {
                stateProvince = new StateProvince()
                {
                    CountryId = int.Parse(this.ddlCountry.SelectedItem.Value),
                    Name = txtName.Text,
                    Abbreviation = txtAbbreviation.Text,
                    DisplayOrder = txtDisplayOrder.Value
                };
                this.StateProvinceService.InsertStateProvince(stateProvince);
            }
            return stateProvince;
        }

        public int StateProvinceId
        {
            get
            {
                return CommonHelper.QueryStringInt("StateProvinceId");
            }
        }
    }
}