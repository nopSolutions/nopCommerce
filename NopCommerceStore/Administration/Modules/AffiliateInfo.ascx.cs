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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class AffiliateInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Affiliate affiliate = IoCFactory.Resolve<IAffiliateManager>().GetAffiliateById(this.AffiliateId);

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
            List<Country> countryCollection = IoCFactory.Resolve<ICountryManager>().GetAllCountriesForRegistration();
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
            Affiliate affiliate = IoCFactory.Resolve<IAffiliateManager>().GetAffiliateById(this.AffiliateId);

            if (affiliate != null)
            {
                affiliate.FirstName = txtFirstName.Text;
                affiliate.LastName = txtLastName.Text;
                affiliate.MiddleName = txtMiddleName.Text;
                affiliate.PhoneNumber = txtPhoneNumber.Text;
                affiliate.Email = txtEmail.Text;
                affiliate.FaxNumber = txtFaxNumber.Text;
                affiliate.Company = txtCompany.Text;
                affiliate.Address1 = txtAddress1.Text;
                affiliate.Address2 = txtAddress2.Text;
                affiliate.City = txtCity.Text;
                affiliate.StateProvince = txtStateProvince.Text;
                affiliate.ZipPostalCode =  txtZipPostalCode.Text;
                affiliate.CountryId = int.Parse(this.ddlCountry.SelectedItem.Value);
                affiliate.Active = cbActive.Checked;
                IoCFactory.Resolve<IAffiliateManager>().UpdateAffiliate(affiliate);
            }
            else
            {
                affiliate = new Affiliate()
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    MiddleName = txtMiddleName.Text,
                    PhoneNumber = txtPhoneNumber.Text,
                    Email = txtEmail.Text,
                    FaxNumber = txtFaxNumber.Text,
                    Company = txtCompany.Text,
                    Address1 = txtAddress1.Text,
                    Address2 = txtAddress2.Text,
                    City = txtCity.Text,
                    StateProvince = txtStateProvince.Text,
                    ZipPostalCode = txtZipPostalCode.Text,
                    CountryId = int.Parse(this.ddlCountry.SelectedItem.Value),
                    Active = cbActive.Checked
                };
                IoCFactory.Resolve<IAffiliateManager>().InsertAffiliate(affiliate);
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