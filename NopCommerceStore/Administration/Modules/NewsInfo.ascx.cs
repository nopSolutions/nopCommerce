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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class NewsInfoControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlLanguage.Items.Clear();
            var languages = IoCFactory.Resolve<ILanguageManager>().GetAllLanguages();
            foreach (Language language in languages)
            {
                ListItem item2 = new ListItem(language.Name, language.LanguageId.ToString());
                this.ddlLanguage.Items.Add(item2);
            }
        }

        private void BindData()
        {
            News news = IoCFactory.Resolve<INewsManager>().GetNewsById(this.NewsId);
            if (news != null)
            {
                CommonHelper.SelectListItem(this.ddlLanguage, news.LanguageId);
                this.txtTitle.Text = news.Title;
                this.txtShort.Text = news.Short;
                this.txtFull.Value = news.Full;
                this.cbPublished.Checked = news.Published;
                this.cbAllowComments.Checked = news.AllowComments;

                this.pnlCreatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(news.CreatedOn, DateTimeKind.Utc).ToString();

                var newsComments = news.NewsComments;
                if (newsComments.Count > 0)
                {
                    this.hlViewComments.Visible = true;
                    this.hlViewComments.Text = string.Format(GetLocaleResourceString("Admin.NewsInfo.ViewComments"), newsComments.Count);
                    this.hlViewComments.NavigateUrl = CommonHelper.GetStoreAdminLocation() + "NewsComments.aspx?NewsID=" + news.NewsId;
                }
                else
                    this.hlViewComments.Visible = false;
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.hlViewComments.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public News SaveInfo()
        {
            News news = IoCFactory.Resolve<INewsManager>().GetNewsById(NewsId);
            if (news != null)
            {
                news.LanguageId = int.Parse(this.ddlLanguage.SelectedItem.Value);
                news.Title = txtTitle.Text;
                news.Short = txtShort.Text;
                news.Full = txtFull.Value;
                news.Published = cbPublished.Checked;
                news.AllowComments = cbAllowComments.Checked;
                news.CreatedOn = DateTime.UtcNow;

                IoCFactory.Resolve<INewsManager>().UpdateNews(news);
            }
            else
            {
                news = new News()
                {
                    LanguageId = int.Parse(this.ddlLanguage.SelectedItem.Value),
                    Title = txtTitle.Text,
                    Short = txtShort.Text,
                    Full = txtFull.Value,
                    Published = cbPublished.Checked,
                    AllowComments = cbAllowComments.Checked,
                    CreatedOn = DateTime.UtcNow
                };
                IoCFactory.Resolve<INewsManager>().InsertNews(news);
            }
            return news;
        }

        public int NewsId
        {
            get
            {
                return CommonHelper.QueryStringInt("NewsId");
            }
        }
    }
}