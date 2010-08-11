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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class TopicInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            if (this.Topic != null)
            {
                this.txtSystemName.Text = this.Topic.Name;
                this.cbIsPasswordProtected.Checked = this.Topic.IsPasswordProtected;
                this.txtPassword.Text = this.Topic.Password;
                this.cbIncludeInSitemap.Checked = this.Topic.IncludeInSitemap;
            }

            var languages = this.GetLocalizableLanguagesSupported();
            rptrLanguageTabs.DataSource = languages;
            rptrLanguageTabs.DataBind();
            rptrLanguageDivs.DataSource = languages;
            rptrLanguageDivs.DataBind();
            rptrLanguageTabs_SEO.DataSource = languages;
            rptrLanguageTabs_SEO.DataBind();
            rptrLanguageDivs_SEO.DataSource = languages;
            rptrLanguageDivs_SEO.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            this.cbIsPasswordProtected.Attributes.Add("onclick", "togglePassword();");

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
                var txtTitle = (TextBox)e.Item.FindControl("txtTitle");
                var txtBody = (AjaxControlToolkit.HTMLEditor.Editor)e.Item.FindControl("txtBody");
                var pnlUrl = (HtmlTableRow)e.Item.FindControl("pnlUrl");
                var hlUrl = (HyperLink)e.Item.FindControl("hlUrl");

                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                if (this.TopicId > 0)
                {
                    var content = TopicManager.GetLocalizedTopic(this.Topic.Name, languageId);
                    if (content != null)
                    {
                        txtTitle.Text = content.Title;
                        txtBody.Content = content.Body;
                        pnlUrl.Visible = true;
                        string url = SEOHelper.GetTopicUrl(content.TopicId, content.Title);
                        hlUrl.Text = url;
                        hlUrl.NavigateUrl = url;
                    }
                    else
                    {
                        pnlUrl.Visible = false;
                    }
                }
                else
                {
                    pnlUrl.Visible = false;
                }
            }
        }

        protected void rptrLanguageDivs_SEO_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var txtMetaKeywords = (TextBox)e.Item.FindControl("txtMetaKeywords");
                var txtMetaDescription = (TextBox)e.Item.FindControl("txtMetaDescription");
                var txtMetaTitle = (TextBox)e.Item.FindControl("txtMetaTitle");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                if (this.TopicId > 0)
                {
                    int languageId = int.Parse(lblLanguageId.Text);

                    var content = TopicManager.GetLocalizedTopic(this.Topic.Name, languageId);
                    if (content != null)
                    {
                        txtMetaKeywords.Text = content.MetaKeywords;
                        txtMetaDescription.Text = content.MetaDescription;
                        txtMetaTitle.Text = content.MetaTitle;
                    }
                }
            }
        }

        public Topic SaveInfo()
        {
            //system info
            Topic topic = TopicManager.GetTopicById(this.TopicId);
            if (topic != null)
            {
                topic = TopicManager.UpdateTopic(topic.TopicId, txtSystemName.Text,
                    cbIsPasswordProtected.Checked, txtPassword.Text.Trim(), cbIncludeInSitemap.Checked);
            }
            else
            {
                topic = TopicManager.InsertTopic(txtSystemName.Text,
                    cbIsPasswordProtected.Checked, txtPassword.Text.Trim(), cbIncludeInSitemap.Checked);
            }


            //localizable info
            DateTime nowDT = DateTime.UtcNow;
            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtTitle = (TextBox)item.FindControl("txtTitle");
                    var txtBody = (AjaxControlToolkit.HTMLEditor.Editor)item.FindControl("txtBody");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string title = txtTitle.Text;
                    string body = txtBody.Content;

                    var content = TopicManager.GetLocalizedTopic(topic.Name, languageId);
                    if (content == null)
                    {
                        content = TopicManager.InsertLocalizedTopic(topic.TopicId,
                                   languageId, title, body, nowDT, nowDT,
                                   string.Empty, string.Empty, string.Empty);
                    }
                    else
                    {
                        content = TopicManager.UpdateLocalizedTopic(content.TopicLocalizedId,
                                   content.TopicId, content.LanguageId, title, body,
                                   content.CreatedOn, nowDT, content.MetaKeywords,
                                   content.MetaDescription, content.MetaTitle);
                    }
                }
            }
            foreach (RepeaterItem item in rptrLanguageDivs_SEO.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtMetaKeywords = (TextBox)item.FindControl("txtMetaKeywords");
                    var txtMetaDescription = (TextBox)item.FindControl("txtMetaDescription");
                    var txtMetaTitle = (TextBox)item.FindControl("txtMetaTitle");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string metaKeywords = txtMetaKeywords.Text;
                    string metaDescription = txtMetaDescription.Text;
                    string metaTitle = txtMetaTitle.Text;

                    var content = TopicManager.GetLocalizedTopic(topic.Name, languageId);
                    if (content == null)
                    {
                        //localized topic should be already created in previous step
                    }
                    else
                    {
                        content = TopicManager.UpdateLocalizedTopic(content.TopicLocalizedId,
                                  content.TopicId, content.LanguageId, content.Title, content.Body,
                                  content.CreatedOn, nowDT, metaKeywords,
                                  metaDescription, metaTitle);
                    }
                }
            }
            
            return topic;
        }

        private Topic _topic;
        public Topic Topic
        {
            get
            {
                if (_topic == null)
                    _topic = TopicManager.GetTopicById(this.TopicId);
                return _topic;
            }
        }

        public int TopicId
        {
            get
            {
                return CommonHelper.QueryStringInt("TopicId");
            }
        }
    }
}