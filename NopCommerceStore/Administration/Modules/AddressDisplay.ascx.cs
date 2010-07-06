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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class AddressDisplay : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }
        
        public Address Address
        {
            set
            {
                Address address = value;
                if (address != null)
                {
                    this.lblFirstName.Text = Server.HtmlEncode(address.FirstName);
                    this.lblLastName.Text = Server.HtmlEncode(address.LastName);
                    this.lblPhoneNumber.Text = Server.HtmlEncode(address.PhoneNumber);
                    this.lblEmail.Text = Server.HtmlEncode(address.Email);
                    this.lblFaxNumber.Text = Server.HtmlEncode(address.FaxNumber);
                    if (!String.IsNullOrEmpty(address.Company))
                        this.lblCompany.Text = Server.HtmlEncode(address.Company);
                    else
                        this.pnlCompany.Visible = false;
                    this.lblAddress1.Text = Server.HtmlEncode(address.Address1);
                    if (!String.IsNullOrEmpty(address.Address2))
                        this.lblAddress2.Text = Server.HtmlEncode(address.Address2);
                    else
                        this.pnlAddress2.Visible = false;
                    this.lblCity.Text = Server.HtmlEncode(address.City);
                    Country country = address.Country;
                    if (country != null)
                        this.lblCountry.Text = Server.HtmlEncode(country.Name);
                    else
                        this.pnlCountry.Visible = false;
                    StateProvince stateProvince = address.StateProvince;
                    if (stateProvince != null)
                        this.lblStateProvince.Text = Server.HtmlEncode(stateProvince.Name);
                    this.lblZipPostalCode.Text = Server.HtmlEncode(address.ZipPostalCode);
                }
            }
        }
                
    }
}