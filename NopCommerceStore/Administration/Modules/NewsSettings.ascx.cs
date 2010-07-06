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
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Profile;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class NewsSettingsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            cbNewsEnabled.Checked = NewsManager.NewsEnabled;
            cbAllowNotRegisteredUsersToLeaveComments.Checked = NewsManager.AllowNotRegisteredUsersToLeaveComments;
            cbNotifyAboutNewNewsComments.Checked = NewsManager.NotifyAboutNewNewsComments;
            cbShowNewsOnMainPage.Checked = NewsManager.ShowNewsOnMainPage;
            txtMainPageNewsCount.Value = NewsManager.MainPageNewsCount;
            txtNewsArchivePageSize.Value = NewsManager.NewsArchivePageSize;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    NewsManager.NewsEnabled = cbNewsEnabled.Checked;
                    NewsManager.AllowNotRegisteredUsersToLeaveComments = cbAllowNotRegisteredUsersToLeaveComments.Checked;
                    NewsManager.NotifyAboutNewNewsComments = cbNotifyAboutNewNewsComments.Checked;
                    NewsManager.ShowNewsOnMainPage = cbShowNewsOnMainPage.Checked;
                    NewsManager.MainPageNewsCount = txtMainPageNewsCount.Value;
                    NewsManager.NewsArchivePageSize = txtNewsArchivePageSize.Value;

                    Response.Redirect("NewsSettings.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}