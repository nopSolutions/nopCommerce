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
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PrivateMessageSendButtonControl : BaseNopUserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            if (ForumManager.AllowPrivateMessages)
            {
                var customer = CustomerManager.GetCustomerById(this.CustomerId);
                if (customer != null && !customer.IsGuest)
                {
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
            else
            {
                this.Visible = false;
            }

            base.OnPreRender(e);
        }

        protected void btnSendPM_Click(object sender, EventArgs e)
        {
            var customer = CustomerManager.GetCustomerById(this.CustomerId);
            if (customer != null)
            {
                string url = string.Format("{0}SendPM.aspx?ToID={1}", CommonHelper.GetStoreLocation(), customer.CustomerId).ToLowerInvariant();
                Response.Redirect(url);
            }
            else
            {
                CommonHelper.ReloadCurrentPage();
            }
        }

        public int CustomerId
        {
            get
            {
                object obj2 = this.ViewState["CustomerId"];
                if (obj2 != null)
                    return (int)obj2;
                else
                    return 0;
            }
            set
            {
                this.ViewState["CustomerId"] = value;
            }
        }
    }
}
