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
using FredCK.FCKeditorV2;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
                var txtBody = (FCKeditor)e.Item.FindControl("txtBody");
                var pnlUrl = (HtmlTableRow)e.Item.FindControl("pnlUrl");
                var hlUrl = (HyperLink)e.Item.FindControl("hlUrl");

                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");

                int languageId = int.Parse(lblLanguageId.Text);

                if (this.TopicId > 0)
                {
                    var content = IoC.Resolve<ITopicService>().GetLocalizedTopic(this.Topic.Name, languageId);
                    if (content != null)
                    {
                        txtTitle.Text = content.Title;
                        txtBody.Value = content.Body;
                        pnlUrl.Visible = true;
                        string url = SEOHelper.GetTopicUrl(content.TopicId, content.Title, false);
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

                    var content = IoC.Resolve<ITopicService>().GetLocalizedTopic(this.Topic.Name, languageId);
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
            Topic topic = IoC.Resolve<ITopicService>().GetTopicById(this.TopicId);
            if (topic != null)
            {
                topic.Name = txtSystemName.Text;
                topic.IsPasswordProtected = cbIsPasswordProtected.Checked;
                topic.Password = txtPassword.Text.Trim();
                topic.IncludeInSitemap = cbIncludeInSitemap.Checked;

                IoC.Resolve<ITopicService>().UpdateTopic(topic);
            }
            else
            {
                topic = new Topic()
                {
                    Name = txtSystemName.Text,
                    IsPasswordProtected = cbIsPasswordProtected.Checked,
                    Password = txtPassword.Text.Trim(),
                    IncludeInSitemap = cbIncludeInSitemap.Checked
                };

                IoC.Resolve<ITopicService>().InsertTopic(topic);
            }


            //localizable info
            DateTime nowDT = DateTime.UtcNow;
            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var txtTitle = (TextBox)item.FindControl("txtTitle");
                    var txtBody = (FCKeditor)item.FindControl("txtBody");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int languageId = int.Parse(lblLanguageId.Text);
                    string title = txtTitle.Text;
                    string body = txtBody.Value;

                    var content = IoC.Resolve<ITopicService>().GetLocalizedTopic(topic.Name, languageId);
                    if (content == null)
                    {
                        content = new LocalizedTopic()
                        {
                            TopicId = topic.TopicId,
                            LanguageId = languageId,
                            Title = title,
                            Body = body,
                            CreatedOn = nowDT,
                            UpdatedOn = nowDT
                        };

                        IoC.Resolve<ITopicService>().InsertLocalizedTopic(content);
                    }
                    else
                    {
                        content.LanguageId = languageId;
                        content.Title = title;
                        content.Body = body;
                        content.UpdatedOn = nowDT;

                        IoC.Resolve<ITopicService>().UpdateLocalizedTopic(content);
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

                    var content = IoC.Resolve<ITopicService>().GetLocalizedTopic(topic.Name, languageId);
                    if (content == null)
                    {
                        //localized topic should be already created on the previous step
                    }
                    else
                    {
                        content.UpdatedOn = nowDT;
                        content.MetaKeywords = metaKeywords;
                        content.MetaDescription = metaDescription;
                        content.MetaTitle = metaTitle;

                        IoC.Resolve<ITopicService>().UpdateLocalizedTopic(content);
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
                    _topic = IoC.Resolve<ITopicService>().GetTopicById(this.TopicId);
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