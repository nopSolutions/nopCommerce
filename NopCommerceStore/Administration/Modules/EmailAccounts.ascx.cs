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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Web.Administration.Modules;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class EmailAccountsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        protected void BindGrid()
        {
            var emailAccounts = MessageManager.GetAllEmailAccounts();
            gvEmailAccounts.DataSource = emailAccounts;
            gvEmailAccounts.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //save email accounts
                foreach (GridViewRow row in gvEmailAccounts.Rows)
                {
                    HiddenField hfEmailAccountId = (HiddenField)row.FindControl("hfEmailAccountId");
                    int emailAccountId = int.Parse(hfEmailAccountId.Value);

                    RadioButton rdbIsDefault = (RadioButton)row.FindControl("rdbIsDefault");
                    if (rdbIsDefault.Checked)
                        MessageManager.DefaultEmailAccount = MessageManager.GetEmailAccountById(emailAccountId);
                }

                BindGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }
    }
}