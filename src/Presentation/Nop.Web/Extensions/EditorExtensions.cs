using System;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web
{
    public static class EditorExtensions
    {
        public static MvcHtmlString BBCodeEditor<TModel>(this HtmlHelper<TModel> html, string name)
        {
            var sb = new StringBuilder();
            
            var storeLocation = EngineContext.Current.Resolve<IWebHelper>().GetStoreLocation();
            string BBEditorWebRoot = String.Format("{0}Content/", storeLocation);
            
            sb.Append("<script src=\"/Content/editors/BBEditor/ed.js\" type=\"text/javascript\"></script>");
            sb.Append(System.Environment.NewLine);
            sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
            sb.Append(System.Environment.NewLine);
            sb.AppendFormat("    var webRoot = '{0}';", BBEditorWebRoot);
            sb.Append(System.Environment.NewLine);
            sb.AppendFormat("    edToolbar('{0}');", name);
            sb.Append(System.Environment.NewLine);
            sb.Append("</script>");
            sb.Append(System.Environment.NewLine);

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}