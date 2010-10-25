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
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ForumsSettingsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                FillDropDowns();
                
                BindData();
            }
        }
        
        protected void BindData()
        {
            cbForumsEnabled.Checked = IoCFactory.Resolve<IForumManager>().ForumsEnabled;
            cbRelativeDateTimeFormattingEnabled.Checked = IoCFactory.Resolve<IForumManager>().RelativeDateTimeFormattingEnabled;
            cbShowCustomersPostCount.Checked = IoCFactory.Resolve<IForumManager>().ShowCustomersPostCount;
            cbGuestsAllowedToCreatePosts.Checked = IoCFactory.Resolve<IForumManager>().AllowGuestsToCreatePosts;
            cbGuestsAllowedToCreateTopics.Checked = IoCFactory.Resolve<IForumManager>().AllowGuestsToCreateTopics;
            cbCustomersAllowedToEditPosts.Checked = IoCFactory.Resolve<IForumManager>().AllowCustomersToEditPosts;
            cbCustomersAllowedToDeletePosts.Checked = IoCFactory.Resolve<IForumManager>().AllowCustomersToDeletePosts;
            cbCustomersAllowedToManageSubscriptions.Checked = IoCFactory.Resolve<IForumManager>().AllowCustomersToManageSubscriptions;
            txtTopicsPageSize.Value = IoCFactory.Resolve<IForumManager>().TopicsPageSize;
            txtPostsPageSize.Value = IoCFactory.Resolve<IForumManager>().PostsPageSize;
            CommonHelper.SelectListItem(this.ddlForumEditor, (int)IoCFactory.Resolve<IForumManager>().ForumEditor);
            cbSignatureEnabled.Checked = IoCFactory.Resolve<IForumManager>().SignaturesEnabled;
        }

        private void FillDropDowns()
        {
            //CommonHelper.FillDropDownWithEnum(this.ddlForumEditor, typeof(EditorTypeEnum));
            this.ddlForumEditor.Items.Clear();

            ListItem ddlEditorTypeSimpleTextBoxItem = new ListItem(GetLocaleResourceString("Admin.Common.Editor.Simple"), ((int)EditorTypeEnum.SimpleTextBox).ToString());
            this.ddlForumEditor.Items.Add(ddlEditorTypeSimpleTextBoxItem);

            ListItem ddlEditorTypeBBCodeEditorItem = new ListItem(GetLocaleResourceString("Admin.Common.Editor.BBCodeEditor"), ((int)EditorTypeEnum.BBCodeEditor).ToString());
            this.ddlForumEditor.Items.Add(ddlEditorTypeBBCodeEditorItem);

            ListItem ddlEditorTypeHtmlEditorItem = new ListItem(GetLocaleResourceString("Admin.Common.Editor.HTMLEditor"), ((int)EditorTypeEnum.HtmlEditor).ToString());
            this.ddlForumEditor.Items.Add(ddlEditorTypeHtmlEditorItem);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    IoCFactory.Resolve<IForumManager>().ForumsEnabled = cbForumsEnabled.Checked;
                    IoCFactory.Resolve<IForumManager>().RelativeDateTimeFormattingEnabled = cbRelativeDateTimeFormattingEnabled.Checked;
                    IoCFactory.Resolve<IForumManager>().ShowCustomersPostCount = cbShowCustomersPostCount.Checked;
                    IoCFactory.Resolve<IForumManager>().AllowGuestsToCreatePosts = cbGuestsAllowedToCreatePosts.Checked;
                    IoCFactory.Resolve<IForumManager>().AllowGuestsToCreateTopics = cbGuestsAllowedToCreateTopics.Checked;
                    IoCFactory.Resolve<IForumManager>().AllowCustomersToEditPosts = cbCustomersAllowedToEditPosts.Checked;
                    IoCFactory.Resolve<IForumManager>().AllowCustomersToDeletePosts = cbCustomersAllowedToDeletePosts.Checked;
                    IoCFactory.Resolve<IForumManager>().AllowCustomersToManageSubscriptions = cbCustomersAllowedToManageSubscriptions.Checked;
                    IoCFactory.Resolve<IForumManager>().TopicsPageSize = txtTopicsPageSize.Value;
                    IoCFactory.Resolve<IForumManager>().PostsPageSize = txtPostsPageSize.Value;
                    IoCFactory.Resolve<IForumManager>().ForumEditor = (EditorTypeEnum)Enum.ToObject(typeof(EditorTypeEnum), int.Parse(this.ddlForumEditor.SelectedItem.Value));
                    IoCFactory.Resolve<IForumManager>().SignaturesEnabled = cbSignatureEnabled.Checked;
                    Response.Redirect("ForumsSettings.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}