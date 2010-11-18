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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
            cbForumsEnabled.Checked = this.ForumService.ForumsEnabled;
            cbRelativeDateTimeFormattingEnabled.Checked = this.ForumService.RelativeDateTimeFormattingEnabled;
            cbShowCustomersPostCount.Checked = this.ForumService.ShowCustomersPostCount;
            cbGuestsAllowedToCreatePosts.Checked = this.ForumService.AllowGuestsToCreatePosts;
            cbGuestsAllowedToCreateTopics.Checked = this.ForumService.AllowGuestsToCreateTopics;
            cbCustomersAllowedToEditPosts.Checked = this.ForumService.AllowCustomersToEditPosts;
            cbCustomersAllowedToDeletePosts.Checked = this.ForumService.AllowCustomersToDeletePosts;
            cbCustomersAllowedToManageSubscriptions.Checked = this.ForumService.AllowCustomersToManageSubscriptions;
            txtTopicsPageSize.Value = this.ForumService.TopicsPageSize;
            txtPostsPageSize.Value = this.ForumService.PostsPageSize;
            CommonHelper.SelectListItem(this.ddlForumEditor, (int)this.ForumService.ForumEditor);
            cbSignatureEnabled.Checked = this.ForumService.SignaturesEnabled;
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
                    this.ForumService.ForumsEnabled = cbForumsEnabled.Checked;
                    this.ForumService.RelativeDateTimeFormattingEnabled = cbRelativeDateTimeFormattingEnabled.Checked;
                    this.ForumService.ShowCustomersPostCount = cbShowCustomersPostCount.Checked;
                    this.ForumService.AllowGuestsToCreatePosts = cbGuestsAllowedToCreatePosts.Checked;
                    this.ForumService.AllowGuestsToCreateTopics = cbGuestsAllowedToCreateTopics.Checked;
                    this.ForumService.AllowCustomersToEditPosts = cbCustomersAllowedToEditPosts.Checked;
                    this.ForumService.AllowCustomersToDeletePosts = cbCustomersAllowedToDeletePosts.Checked;
                    this.ForumService.AllowCustomersToManageSubscriptions = cbCustomersAllowedToManageSubscriptions.Checked;
                    this.ForumService.TopicsPageSize = txtTopicsPageSize.Value;
                    this.ForumService.PostsPageSize = txtPostsPageSize.Value;
                    this.ForumService.ForumEditor = (EditorTypeEnum)Enum.ToObject(typeof(EditorTypeEnum), int.Parse(this.ddlForumEditor.SelectedItem.Value));
                    this.ForumService.SignaturesEnabled = cbSignatureEnabled.Checked;
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