using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Security.Honeypot
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString GenerateHoneypotInput(this HtmlHelper helper)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<div style=\"display:none;\">");
            sb.Append(Environment.NewLine);

            var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
            var hpInput = helper.TextBox(securitySettings.HoneypotInputName);
            sb.Append(hpInput.ToString());

            sb.Append(Environment.NewLine);
            sb.Append("</div>");

            return MvcHtmlString.Create(sb.ToString());

            //var hpInput = helper.TextBox(securitySettings.HoneypotInputName, "", new { @class = "hp" });
            //var hpInput = helper.Hidden(securitySettings.HoneypotInputName);
            //return hpInput;
        }
    }
}
