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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CountryInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Country country = IoCFactory.Resolve<ICountryService>().GetCountryById(this.CountryId);
            if (country != null)
            {
                this.txtName.Text = country.Name;
                this.cbAllowsRegistration.Checked = country.AllowsRegistration;
                this.cbAllowsBilling.Checked = country.AllowsBilling;
                this.cbAllowsShipping.Checked = country.AllowsShipping;
                this.txtTwoLetterISOCode.Text = country.TwoLetterIsoCode;
                this.txtThreeLetterISOCode.Text = country.ThreeLetterIsoCode;
                this.txtNumericISOCode.Value = country.NumericIsoCode;
                this.cbSubjectToVAT.Checked = country.SubjectToVAT;
                this.cbPublished.Checked = country.Published;
                this.txtDisplayOrder.Value = country.DisplayOrder;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public Country SaveInfo()
        {
            Country country = IoCFactory.Resolve<ICountryService>().GetCountryById(this.CountryId);

            if (country != null)
            {
                country.Name = txtName.Text;
                country.AllowsRegistration = cbAllowsRegistration.Checked;
                country.AllowsBilling = cbAllowsBilling.Checked;
                country.AllowsShipping= cbAllowsShipping.Checked;
                country.TwoLetterIsoCode = txtTwoLetterISOCode.Text;
                country.ThreeLetterIsoCode = txtThreeLetterISOCode.Text;
                country.NumericIsoCode = txtNumericISOCode.Value;
                country.SubjectToVAT = cbSubjectToVAT.Checked;
                country.Published = cbPublished.Checked;
                country.DisplayOrder = txtDisplayOrder.Value;
                IoCFactory.Resolve<ICountryService>().UpdateCountry(country);
            }
            else
            {
                country = new Country()
                {
                    Name = txtName.Text,
                    AllowsRegistration = cbAllowsRegistration.Checked,
                    AllowsBilling = cbAllowsBilling.Checked,
                    AllowsShipping = cbAllowsShipping.Checked,
                    TwoLetterIsoCode = txtTwoLetterISOCode.Text,
                    ThreeLetterIsoCode = txtThreeLetterISOCode.Text,
                    NumericIsoCode = txtNumericISOCode.Value,
                    SubjectToVAT = cbSubjectToVAT.Checked,
                    Published = cbPublished.Checked,
                    DisplayOrder = txtDisplayOrder.Value
                };
                IoCFactory.Resolve<ICountryService>().InsertCountry(country);
            }

            return country;
        }

        public int CountryId
        {
            get
            {
                return CommonHelper.QueryStringInt("CountryId");
            }
        }
    }
}