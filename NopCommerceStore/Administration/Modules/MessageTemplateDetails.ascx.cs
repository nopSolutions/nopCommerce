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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using System.Collections.Generic;
using System.Text;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class MessageTemplateDetailsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            if (this.MessageTemplate != null)
            {
                StringBuilder allowedTokensString = new StringBuilder();
                string[] allowedTokens = MessageManager.GetListOfAllowedTokens();
                for (int i = 0; i < allowedTokens.Length; i++)
                {
                    string token = allowedTokens[i];
                    allowedTokensString.Append(token);
                    if (i != allowedTokens.Length - 1)
                        allowedTokensString.Append(", ");
                }
                this.lblAllowedTokens.Text = allowedTokensString.ToString();

                this.lblTemplate.Text = this.MessageTemplate.Name;

                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }
            else
                Response.Redirect("MessageTemplates.aspx");
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ddlEmailAccount = (DropDownList)e.Item.FindControl("ddlEmailAccount");
                var txtBCCEmailAddresses = (TextBox)e.Item.FindControl("txtBCCEmailAddresses");
                var txtSubject = (TextBox)e.Item.FindControl("txtSubject");
                var txtBody = (AjaxControlToolkit.HTMLEditor.Editor)e.Item.FindControl("txtBody");
                var cbActive = (CheckBox)e.Item.FindControl("cbActive");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");
                
                int languageId = int.Parse(lblLanguageId.Text);

                var emailAccounts= MessageManager.GetAllEmailAccounts();
                ddlEmailAccount.Items.Clear();
                foreach(var emailAccount in emailAccounts)
                {
                    ListItem item = new ListItem(emailAccount.FriendlyName, emailAccount.EmailAccountId.ToString());
                    ddlEmailAccount.Items.Add(item);
                }

                var content = MessageManager.GetLocalizedMessageTemplate(this.MessageTemplate.Name, languageId);
                if (content != null)
                {
                    CommonHelper.SelectListItem(ddlEmailAccount, content.EmailAccount.EmailAccountId);
                    txtBCCEmailAddresses.Text = content.BccEmailAddresses;
                    txtSubject.Text = content.Subject;
                    txtBody.Content = content.Body;
                    cbActive.Checked = content.IsActive;
                }
                else
                {
                    CommonHelper.SelectListItem(ddlEmailAccount, MessageManager.DefaultEmailAccount.EmailAccountId);
                }
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    MessageTemplate messageTemplate = MessageManager.GetMessageTemplateById(this.MessageTemplateId);
                    if (messageTemplate != null)
                    {
                        foreach (RepeaterItem item in rptrLanguageDivs.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                var ddlEmailAccount = (DropDownList)item.FindControl("ddlEmailAccount");
                                var txtBCCEmailAddresses = (TextBox)item.FindControl("txtBCCEmailAddresses");
                                var txtSubject = (TextBox)item.FindControl("txtSubject");
                                var txtBody = (AjaxControlToolkit.HTMLEditor.Editor)item.FindControl("txtBody");
                                var cbActive = (CheckBox)item.FindControl("cbActive");
                                var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                                int emailAccountId = int.Parse(ddlEmailAccount.SelectedValue);
                                int languageId = int.Parse(lblLanguageId.Text);
                                string BCCEmailAddresses = txtBCCEmailAddresses.Text;
                                string subject = txtSubject.Text;
                                string body = txtBody.Content;
                                bool active = cbActive.Checked;

                                var content = MessageManager.GetLocalizedMessageTemplate(this.MessageTemplate.Name, languageId);
                                if (content == null)
                                {
                                    content = MessageManager.InsertLocalizedMessageTemplate(this.MessageTemplateId,
                                        languageId, emailAccountId, BCCEmailAddresses, 
                                        subject, body, active);
                                }
                                else
                                {
                                    content = MessageManager.UpdateLocalizedMessageTemplate(content.MessageTemplateLocalizedId,
                                        content.MessageTemplateId, content.LanguageId,
                                        emailAccountId, BCCEmailAddresses, subject, body, active);
                                }
                            }
                        }

                        Response.Redirect(string.Format("MessageTemplateDetails.aspx?MessageTemplateID={0}", this.MessageTemplateId));
                    }
                    else
                    {
                        Response.Redirect("MessageTemplates.aspx");
                    }
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        private MessageTemplate _messageTemplate;
        public MessageTemplate MessageTemplate
        {
            get
            {
                if (_messageTemplate == null)
                {
                    _messageTemplate = MessageManager.GetMessageTemplateById(this.MessageTemplateId);
                }
                return _messageTemplate;
            }
        }

        public int MessageTemplateId
        {
            get
            {
                return CommonHelper.QueryStringInt("MessageTemplateId");
            }
        }
    }
}