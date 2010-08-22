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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.SEO;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BlogMonthsControl : BaseNopUserControl
    {
        private void BindData()
        {
            var blogPosts = BlogManager.GetAllBlogPosts(NopContext.Current.WorkingLanguage.LanguageId);
            if (blogPosts.Count > 0)
            {
                var months = new SortedDictionary<DateTime, int>();
               
                DateTime first =  blogPosts[blogPosts.Count-1].CreatedOn;

                while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
                {
                    var list = blogPosts.GetPostsByDate(new DateTime(first.Year, first.Month, 1), new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
                    if (list.Count > 0)
                    {
                        DateTime date = new DateTime(first.Year, first.Month, 1);
                        months.Add(date, list.Count);
                    }

                    first = first.AddMonths(1);
                }

                string html = RenderMonths(months);
                lMonths.Text = html;
            }
            else
            {
                this.Visible = false;
            }
        }

        private string RenderMonths(SortedDictionary<DateTime, int> Months)
        {
            if (Months.Keys.Count == 0)
                return string.Empty;

            HtmlGenericControl ul = new HtmlGenericControl("ul");
            ul.Attributes.Add("id", "blogMonthList");
            HtmlGenericControl year = null;
            HtmlGenericControl list = null;
            int current = 0;

            foreach (DateTime date in Months.Keys)
            {
                if (current == 0)
                    current = date.Year;

                if (date.Year > current || ul.Controls.Count == 0)
                {
                    list = new HtmlGenericControl("ul");
                    list.ID = "year" + date.Year.ToString();

                    year = new HtmlGenericControl("li");
                    year.Attributes.Add("class", "year");
                    year.InnerHtml = date.Year.ToString();
                    year.Controls.Add(list);

                    ul.Controls.AddAt(0, year);
                }

                HtmlGenericControl li = new HtmlGenericControl("li");

                HtmlAnchor anc = new HtmlAnchor();
                string url = SEOHelper.GetBlogUrlByMonth(date);
                anc.HRef = url;
                anc.InnerHtml = new DateTime(date.Year, date.Month, 1).ToString("MMMM") + " (" + Months[date] + ")";

                li.Controls.Add(anc);
                list.Controls.AddAt(0, li);
                current = date.Year;
            }

            StringWriter sw = new StringWriter();
            ul.RenderControl(new HtmlTextWriter(sw));
            return sw.ToString();
        }

        private SortedDictionary<string, Guid> SortGategories(Dictionary<Guid, string> categories)
        {
            SortedDictionary<string, Guid> dic = new SortedDictionary<string, Guid>();
            foreach (Guid key in categories.Keys)
            {
                dic.Add(categories[key], key);
            }

            return dic;
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindData();

            base.OnPreRender(e);
        }
    }
}