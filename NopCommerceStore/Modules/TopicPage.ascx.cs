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
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using System.ComponentModel;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class TopicPageControl : BaseNopUserControl
    {
        private bool ValidatePassword()
        {
            bool passwordOK = true;
            if (this.Topic != null)
            {
                if (this.Topic.IsPasswordProtected &&
                    !this.Topic.Password.Equals(this.EnteredPassword))
                {
                    passwordOK = false;
                }
            }
            return passwordOK;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            this.BindData();

            if (this.LocalizedTopic != null)
            {
                //title
                string title = this.LocalizedTopic.MetaTitle;
                if (String.IsNullOrEmpty(title))
                    title = this.LocalizedTopic.Title;
                if (!string.IsNullOrEmpty(title))
                    SEOHelper.RenderTitle(this.Page, title, true);

                //meta
                SEOHelper.RenderMetaTag(this.Page, "description", this.LocalizedTopic.MetaDescription, true);
                SEOHelper.RenderMetaTag(this.Page, "keywords", this.LocalizedTopic.MetaKeywords, true);
            }
        }

        protected void btnPassword_OnClick(object sender, EventArgs e)
        {
            this.EnteredPassword = txtPassword.Text;
            bool passwordOK = ValidatePassword();
            if (!passwordOK)
            {
                lError.Text = GetLocaleResourceString("TopicPage.WrongPassword");
            }
            BindData();
        }

        private void BindData()
        {
            if (this.LocalizedTopic != null)
            {
                bool passwordOK = ValidatePassword();
                if (passwordOK)
                {
                    phPassword.Visible = false;
                    lTitle.Visible = true;
                    lBody.Visible = true;

                    if (!string.IsNullOrEmpty(this.LocalizedTopic.Title))
                    {
                        lTitle.Text = Server.HtmlEncode(this.LocalizedTopic.Title);
                    }
                    else
                    {
                        lTitle.Visible = false;
                    }
                    if (!string.IsNullOrEmpty(this.LocalizedTopic.Body))
                    {
                        lBody.Text = this.LocalizedTopic.Body;
                    }
                    else
                    {
                        lBody.Visible = false;
                    }
                }
                else
                {
                    phPassword.Visible = true;
                    lTitle.Visible = false;
                    lBody.Visible = false;
                }
            }
            else
            {
                Response.Redirect(CommonHelper.GetStoreLocation());
            }
        }

        private LocalizedTopic localizedTopic = null;
        public LocalizedTopic LocalizedTopic
        {
            get
            {
                if (localizedTopic == null)
                {
                    localizedTopic = TopicManager.GetLocalizedTopic(this.TopicId, NopContext.Current.WorkingLanguage.LanguageId);
                }
                return localizedTopic;
            }
        }

        private Topic topic = null;
        public Topic Topic
        {
            get
            {
                if (topic == null)
                {
                    topic = TopicManager.GetTopicById(this.TopicId);
                }
                return topic;
            }
        }

        public int TopicId
        {
            get
            {
                return CommonHelper.QueryStringInt("TopicId");
            }
        }
        
        public string EnteredPassword
        {
            get
            {
                object obj2 = this.ViewState["EnteredPassword"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["EnteredPassword"] = value;
            }
        }
    }
}
