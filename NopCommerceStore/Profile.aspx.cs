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
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web
{
    /// <summary>
    /// ProfilePage page handler.
    /// </summary>
    public partial class ProfilePage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
                if (customer == null || customer.IsGuest)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                if (!CustomerManager.AllowViewingProfiles)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                string name = CustomerManager.FormatUserName(customer);
                lTitle.Text = string.Format(GetLocaleResourceString("Profile.ProfileOf"), Server.HtmlEncode(name));
            }

            string title = GetLocaleResourceString("PageTitle.Profile");
            SEOHelper.RenderTitle(this, title, true);
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("UserId");
            }
        }

        public override PageSslProtectionEnum SslProtected
        {
            get
            {
                return PageSslProtectionEnum.No;
            }
        }
    }
}