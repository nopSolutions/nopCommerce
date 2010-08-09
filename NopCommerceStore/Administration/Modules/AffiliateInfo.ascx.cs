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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common.Utils;
using System.Collections.Generic;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class AffiliateInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Affiliate affiliate = AffiliateManager.GetAffiliateById(this.AffiliateId);

            if (affiliate != null)
            {
                this.lblAffiliateId.Text = affiliate.AffiliateId.ToString();
                this.hlAffiliateUrl.NavigateUrl = this.hlAffiliateUrl.Text = CommonHelper.ModifyQueryString(CommonHelper.GetStoreLocation() + "default.aspx", "AffiliateID=" + affiliate.AffiliateId.ToString(), null);
                
                this.txtFirstName.Text = affiliate.FirstName;
                this.txtLastName.Text = affiliate.LastName;
                this.txtMiddleName.Text = affiliate.MiddleName;
                this.txtPhoneNumber.Text = affiliate.PhoneNumber;
                this.txtEmail.Text = affiliate.Email;
                this.txtFaxNumber.Text = affiliate.FaxNumber;
                this.txtCompany.Text = affiliate.Company;
                this.txtAddress1.Text = affiliate.Address1;
                this.txtAddress2.Text = affiliate.Address2;
                this.txtCity.Text = affiliate.City;
                this.txtStateProvince.Text = affiliate.StateProvince;
                this.txtZipPostalCode.Text = affiliate.ZipPostalCode;
                this.pnlAffiliateId.Visible = true;
                CommonHelper.SelectListItem(this.ddlCountry, affiliate.CountryId);
                this.cbActive.Checked = affiliate.Active;
            }
            else
            {
                this.pnlAffiliateId.Visible = false;
                this.pnlAffiliateUrl.Visible = false;
            }
        }

        private void FillDropDowns()
        {
            this.ddlCountry.Items.Clear();
            List<Country> countryCollection = CountryManager.GetAllCountriesForRegistration();
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

        public Affiliate SaveInfo()
        {
            Affiliate affiliate = AffiliateManager.GetAffiliateById(this.AffiliateId);

            if (affiliate != null)
            {
                affiliate = AffiliateManager.UpdateAffiliate(affiliate.AffiliateId, txtFirstName.Text, txtLastName.Text, txtMiddleName.Text,
                    txtPhoneNumber.Text, txtEmail.Text, txtFaxNumber.Text, txtCompany.Text,
                    txtAddress1.Text, txtAddress2.Text, txtCity.Text, txtStateProvince.Text, txtZipPostalCode.Text,
                    int.Parse(this.ddlCountry.SelectedItem.Value), affiliate.Deleted, cbActive.Checked);
            }
            else
            {
                affiliate = AffiliateManager.InsertAffiliate(txtFirstName.Text, txtLastName.Text, txtMiddleName.Text,
                   txtPhoneNumber.Text, txtEmail.Text, txtFaxNumber.Text, txtCompany.Text,
                   txtAddress1.Text, txtAddress2.Text, txtCity.Text, txtStateProvince.Text, txtZipPostalCode.Text,
                   int.Parse(this.ddlCountry.SelectedItem.Value), false, cbActive.Checked);
            }

            return affiliate;
        }

        public int AffiliateId
        {
            get
            {
                return CommonHelper.QueryStringInt("AffiliateId");
            }
        }
    }
}