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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class NewsArchiveControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        protected string GetNewsRSSUrl()
        {
            return SEOHelper.GetNewsRssUrl();
        }

        protected void BindData()
        {
            int totalRecords = 0;
            int pageSize = NewsManager.NewsArchivePageSize;

            var newsCollection = NewsManager.GetAllNews(NopContext.Current.WorkingLanguage.LanguageId, CurrentPageIndex, pageSize, out totalRecords);
            if(newsCollection.Count > 0)
            {
                newsPager.PageSize = pageSize;
                newsPager.TotalRecords = totalRecords;
                newsPager.PageIndex = CurrentPageIndex;

                rptrNews.DataSource = newsCollection;
                rptrNews.DataBind();
            }
            else
            {
                Visible = false;
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                int pageIndex = CommonHelper.QueryStringInt(newsPager.QueryStringProperty);
                pageIndex--;
                if(pageIndex < 0)
                    pageIndex = 0;
                return pageIndex;
            }
        }
    }
}