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
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.Messages;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerSendPrivateMessage : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
            LoadBBCodeEditorJS();
        }

        private void LoadBBCodeEditorJS()
        {
            string bbCodeJS = "<script src='" + Page.ResolveUrl("~/editors/BBEditor/ed.js") + "' type='text/javascript'></script>";
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "BBCodeEditor", bbCodeJS, false);
        }

        private void BindData()
        {
            if (ForumManager.AllowPrivateMessages)
            {
                pnlSendPriveteMessage.Visible = true;
                pnlNotAllowed.Visible = false;
            }
            else
            {
                pnlSendPriveteMessage.Visible = false;
                pnlNotAllowed.Visible = true;
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string subject = txtSubject.Text.Trim();
                string message = txtMessageBBCode.Text.Trim();

                var customer = CustomerManager.GetCustomerById(this.CustomerId);
                if (customer != null)
                {
                    var pm = ForumManager.InsertPrivateMessage(
                        NopContext.Current.User.CustomerId, 
                        customer.CustomerId, subject, message,
                        false, false, false, DateTime.UtcNow);
                }

                Response.Redirect(string.Format("CustomerDetails.aspx?CustomerID={0}", CustomerId));
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }
    }
}
