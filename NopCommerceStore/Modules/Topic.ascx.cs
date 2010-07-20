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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using System.ComponentModel;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class TopicControl : BaseNopUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.BindData();

            if (this.Topic != null)
            {
                if (this.OverrideSEO)
                {
                    //title
                    string title = this.Topic.MetaTitle;
                    if (String.IsNullOrEmpty(title))
                        title = this.Topic.Title;
                    if (!string.IsNullOrEmpty(title))
                        SEOHelper.RenderTitle(this.Page, title, true);

                    //meta
                    if (!String.IsNullOrEmpty(this.Topic.MetaDescription))
                        SEOHelper.RenderMetaTag(this.Page, "description", this.Topic.MetaDescription, true);
                    if (!String.IsNullOrEmpty(this.Topic.MetaKeywords))
                        SEOHelper.RenderMetaTag(this.Page, "keywords", this.Topic.MetaKeywords, true);
                }
            }
        }

        private void BindData()
        {
            if (this.Topic != null)
            {
                if (!string.IsNullOrEmpty(this.Topic.Title))
                {
                    lTitle.Text = Server.HtmlEncode(this.Topic.Title);
                }
                else
                {
                    lTitle.Visible = false;
                }
                if (!string.IsNullOrEmpty(this.Topic.Body))
                {
                    lBody.Text = this.Topic.Body;
                }
                else
                {
                    lBody.Visible = false;
                }
            }
            else
                this.Visible = false;
        }

        private LocalizedTopic topic = null;
        public LocalizedTopic Topic
        {
            get
            {
                if (topic == null)
                {
                    topic = TopicManager.GetLocalizedTopic(this.TopicName, NopContext.Current.WorkingLanguage.LanguageId);
                }
                return topic;
            }
        }

        public string TopicName
        {
            get
            {
                object obj2 = this.ViewState["TopicName"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["TopicName"] = value;
            }
        }

        [DefaultValue(true)]
        public bool OverrideSEO
        {
            get
            {
                object obj2 = this.ViewState["OverrideSEO"];
                if (obj2 != null)
                    return (bool)obj2;
                else
                    return true;

            }
            set
            {
                this.ViewState["OverrideSEO"] = value;
            }
        }
    }
}
