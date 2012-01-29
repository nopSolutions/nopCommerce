using System;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Extensions
{
    public static class EditorExtensions
    {
        public static MvcHtmlString BBCodeEditor<TModel>(this HtmlHelper<TModel> html, string name)
        {
            var sb = new StringBuilder();
            
            var storeLocation = EngineContext.Current.Resolve<IWebHelper>().GetStoreLocation();
            string bbEditorWebRoot = String.Format("{0}Content/", storeLocation);

            sb.AppendFormat("<script src=\"{0}Content/editors/BBEditor/ed.js\" type=\"text/javascript\"></script>", storeLocation);
            sb.Append(Environment.NewLine);
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append(Environment.NewLine);
            sb.AppendFormat("    var webRoot = '{0}';", bbEditorWebRoot);
            sb.Append(Environment.NewLine);
            sb.AppendFormat("    edToolbar('{0}');", name);
            sb.Append(Environment.NewLine);
            sb.Append("</script>");
            sb.Append(Environment.NewLine);

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}