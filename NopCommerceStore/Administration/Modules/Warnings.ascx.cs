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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class WarningsControl : BaseNopAdministrationUserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            StringBuilder warningResult = new StringBuilder();

            //currencies
            if (CurrencyManager.PrimaryExchangeRateCurrency == null)
            {
                warningResult.Append("Primary exchange rate currency is not set. <a href=\"Currencies.aspx\">Set now</a>");
                warningResult.Append("<br />");
                warningResult.Append("<br />");
            }
            if (CurrencyManager.PrimaryStoreCurrency == null)
            {
                warningResult.Append("Primary store currency is not set. <a href=\"Currencies.aspx\">Set now</a>");
                warningResult.Append("<br />");
                warningResult.Append("<br />");
            }

            //measures
            if (MeasureManager.BaseWeightIn == null)
            {
                warningResult.Append("The weight that will can used as a default is not set. <a href=\"GlobalSettings.aspx\">Set now</a>");
                warningResult.Append("<br />");
                warningResult.Append("<br />");
            }
            if (MeasureManager.BaseDimensionIn == null)
            {
                warningResult.Append("The dimension that will can used as a default is not set. <a href=\"GlobalSettings.aspx\">Set now</a>");
                warningResult.Append("<br />");
                warningResult.Append("<br />");
            }

            //languages
            var publishedLanguages = LanguageManager.GetAllLanguages(false);
            foreach (MessageTemplate messageTemplate in MessageManager.GetAllMessageTemplates())
            {
                foreach (Language language in publishedLanguages)
                {
                    LocalizedMessageTemplate localizedMessageTemplate = MessageManager.GetLocalizedMessageTemplate(messageTemplate.Name, language.LanguageId);
                    if (localizedMessageTemplate == null)
                    {
                        warningResult.AppendFormat("You don't have localized version of message template [{0}] for {1}. <a href=\"MessageTemplates.aspx\">Create it now</a>", messageTemplate.Name, language.Name);
                        warningResult.Append("<br />");
                        warningResult.Append("<br />");
                    }
                }
            }

            //shipping methods
            var srcmList = ShippingRateComputationMethodManager.GetAllShippingRateComputationMethods(false);
            int offlineSrcmCount = 0;
            foreach (var srcm in srcmList)
            {
                if (srcm.ShippingRateComputationMethodType == ShippingRateComputationMethodTypeEnum.Offline)
                    offlineSrcmCount++;
            }
            if (offlineSrcmCount > 1)
            {
                warningResult.Append("Only one offline shipping rate computation method is recommended to use");
                warningResult.Append("<br />");
                warningResult.Append("<br />");
            }

            string warnings = warningResult.ToString();
            if (!String.IsNullOrEmpty(warnings))
            {
                lblWarnings.Text = warnings;
            }
            else
                this.Visible = false;

            base.OnPreRender(e);
        }
    }
}