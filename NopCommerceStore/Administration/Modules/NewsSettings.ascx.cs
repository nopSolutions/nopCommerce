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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
            cbNewsEnabled.Checked = IoC.Resolve<INewsService>().NewsEnabled;
            cbAllowNotRegisteredUsersToLeaveComments.Checked = IoC.Resolve<INewsService>().AllowNotRegisteredUsersToLeaveComments;
            cbNotifyAboutNewNewsComments.Checked = IoC.Resolve<INewsService>().NotifyAboutNewNewsComments;
            cbShowNewsOnMainPage.Checked = IoC.Resolve<INewsService>().ShowNewsOnMainPage;
            txtMainPageNewsCount.Value = IoC.Resolve<INewsService>().MainPageNewsCount;
            txtNewsArchivePageSize.Value = IoC.Resolve<INewsService>().NewsArchivePageSize;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    IoC.Resolve<INewsService>().NewsEnabled = cbNewsEnabled.Checked;
                    IoC.Resolve<INewsService>().AllowNotRegisteredUsersToLeaveComments = cbAllowNotRegisteredUsersToLeaveComments.Checked;
                    IoC.Resolve<INewsService>().NotifyAboutNewNewsComments = cbNotifyAboutNewNewsComments.Checked;
                    IoC.Resolve<INewsService>().ShowNewsOnMainPage = cbShowNewsOnMainPage.Checked;
                    IoC.Resolve<INewsService>().MainPageNewsCount = txtMainPageNewsCount.Value;
                    IoC.Resolve<INewsService>().NewsArchivePageSize = txtNewsArchivePageSize.Value;

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