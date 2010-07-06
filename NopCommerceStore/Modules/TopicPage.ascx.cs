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
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.Topic != null)
            {
                //title
                string title = this.Topic.MetaTitle;
                if (String.IsNullOrEmpty(title))
                    title = this.Topic.Title;
                if (!string.IsNullOrEmpty(title))
                    SEOHelper.RenderTitle(this.Page, title, true);

                //meta
                SEOHelper.RenderMetaTag(this.Page, "description", this.Topic.MetaDescription, true);
                SEOHelper.RenderMetaTag(this.Page, "keywords", this.Topic.MetaKeywords, true);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
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
                Response.Redirect(CommonHelper.GetStoreLocation());
        }

        private LocalizedTopic topic = null;
        public LocalizedTopic Topic
        {
            get
            {
                if (topic == null)
                {
                    topic = TopicManager.GetLocalizedTopic(this.TopicId, NopContext.Current.WorkingLanguage.LanguageId);
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
    }
}
