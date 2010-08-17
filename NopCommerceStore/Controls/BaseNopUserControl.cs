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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web
{
    public partial class BaseNopUserControl : UserControl
    {
        public BaseNopUserControl()
        {

        }

        protected virtual void BindJQuery()
        {
            CommonHelper.BindJQuery(this.Page);
        }

        protected void DisplayAlertMessage(string message)
        {
            if (String.IsNullOrEmpty(message))
                return;

            this.BindJQuery();
            StringBuilder alertJsStart = new StringBuilder();
            alertJsStart.AppendLine("<script type=\"text/javascript\">");
            alertJsStart.AppendLine("$(document).ready(function() {");
            alertJsStart.AppendLine(string.Format("alert('{0}');", message.Trim()));
            alertJsStart.AppendLine("});");
            alertJsStart.AppendLine("</script>");
            string js = alertJsStart.ToString();
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "alertScriptKey", js);
        }

        protected string GetLocaleResourceString(string ResourceName)
        {
            Language language = NopContext.Current.WorkingLanguage;
            return LocalizationManager.GetLocaleResourceString(ResourceName, language.LanguageId);
        }

        protected string GetLocaleResourceString(string ResourceName, params object[] args)
        {
            Language language = NopContext.Current.WorkingLanguage;
            return string.Format(
                LocalizationManager.GetLocaleResourceString(ResourceName, language.LanguageId),
                args);
        }
    }
}