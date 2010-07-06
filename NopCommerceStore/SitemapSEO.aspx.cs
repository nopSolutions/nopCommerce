using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.SEO.Sitemaps;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;

namespace NopSolutions.NopCommerce.Web
{
    public partial class SitemapSEOPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = new UTF8Encoding();

            SitemapGenerator sitemapGenerator = new SitemapGenerator();
            string siteMap = sitemapGenerator.Generate();
            Response.Write(siteMap);
            Response.End();
        }
    }
}
