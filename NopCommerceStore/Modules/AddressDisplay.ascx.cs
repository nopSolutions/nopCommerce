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
using System.ComponentModel;
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

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class AddressDisplay : BaseNopUserControl
    {
        public Address Address
        {
            set
            {
                Address address = value;
                if (address != null)
                {
                    this.btnEditAddress.CommandArgument = address.AddressId.ToString();
                    this.btnDeleteAddress.CommandArgument = address.AddressId.ToString();
                    this.lFullName.Text = Server.HtmlEncode(string.Format("{0} {1}", address.FirstName, address.LastName));
                    this.lFirstName.Text = Server.HtmlEncode(address.FirstName);
                    this.lLastName.Text = Server.HtmlEncode(address.LastName);
                    this.lPhoneNumber.Text = Server.HtmlEncode(address.PhoneNumber);
                    this.lEmail.Text = Server.HtmlEncode(address.Email);
                    this.lFaxNumber.Text = Server.HtmlEncode(address.FaxNumber);
                    if (!String.IsNullOrEmpty(address.Company))
                        this.lCompany.Text = Server.HtmlEncode(address.Company);
                    else
                        this.pnlCompany.Visible = false;
                    this.lAddress1.Text = Server.HtmlEncode(address.Address1);
                    if (!String.IsNullOrEmpty(address.Address2))
                        this.lAddress2.Text = Server.HtmlEncode(address.Address2);
                    else
                        this.pnlAddress2.Visible = false;
                    this.lCity.Text = Server.HtmlEncode(address.City);
                    var country = address.Country;
                    if (country != null)
                        this.lCountry.Text = Server.HtmlEncode(country.Name);
                    else
                        this.pnlCountry.Visible = false;
                    var stateProvince = address.StateProvince;
                    if (stateProvince != null)
                        this.lStateProvince.Text = Server.HtmlEncode(stateProvince.Name);
                    this.lZipPostalCode.Text = Server.HtmlEncode(address.ZipPostalCode);
                }
            }
        }

        protected void btnEditAddress_Click(object sender, CommandEventArgs e)
        {
            int addressId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(string.Format("~/addressedit.aspx?addressid={0}", addressId));
        }

        protected void btnDeleteAddress_Click(object sender, CommandEventArgs e)
        {
            int addressId = Convert.ToInt32(e.CommandArgument);
            Response.Redirect(string.Format("~/addressedit.aspx?addressid={0}&delete={1}", addressId, true));
        }

        [DefaultValue(true)]
        public bool ShowDeleteButton
        {
            get
            {
                object obj2 = this.ViewState["ShowDeleteButton"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["ShowDeleteButton"] = value;
            }
        }

        [DefaultValue(true)]
        public bool ShowEditButton
        {
            get
            {
                object obj2 = this.ViewState["ShowEditButton"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["ShowEditButton"] = value;
            }
        }
    }
}