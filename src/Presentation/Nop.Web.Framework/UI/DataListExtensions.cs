using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Nop.Web.Framework.UI
{
    public static class DataListExtensions
    {
        public static IHtmlString DataList<T>(this HtmlHelper helper, IEnumerable<T> items, int columns,
            Func<T, HelperResult> template) 
            where T : class
        {
            if (items == null)
                return new HtmlString("");

            var sb = new StringBuilder();
            sb.Append("<table>");

            int cellIndex = 0;

            foreach (T item in items)
            {
                if (cellIndex == 0)
                    sb.Append("<tr>");

                sb.Append("<td");
                sb.Append(">");
                
                sb.Append(template(item).ToHtmlString());
                sb.Append("</td>");

                cellIndex++;

                if (cellIndex == columns)
                {
                    cellIndex = 0;
                    sb.Append("</tr>");
                }
            }

            if (cellIndex != 0)
            {
                for (; cellIndex < columns; cellIndex++)
                {
                    sb.Append("<td>&nbsp;</td>");
                }

                sb.Append("</tr>");
            }

            sb.Append("</table>");

            return new HtmlString(sb.ToString());
        }
    }
}
