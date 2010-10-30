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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class WarningsControl : BaseNopAdministrationUserControl
    {
        private int CurrentTaskNo
        {
            get
            {
                return int.Parse(ViewState["CurrentTaskNo"] as string ?? "0");
            }
            set
            {
                ViewState["CurrentTaskNo"] = value.ToString();
            }
        }

        private Action[] _Tasks;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this._Tasks = new Action[] 
            {
                () => TestStoreUrl(),
                () => TestPrimaryExchangeRateCurrency(),
                () => TestPrimaryStoreCurrency(),
                () => TestBaseWeight(),
                () => TestBaseDimension(),
                () => TestMessageTemplates(),
                () => TestShippingMethods()
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.CurrentTaskNo = 0;
            }
        }

        private void MarkAsFail(Label label, string reason, string suggestion)
        {
            label.CssClass = "fail";
            label.Text += "<br /><span class=\"error\">" + reason + "</span>" +
                "<br /><span class=\"suggestion\">" + suggestion + "</span>";
        }

        private void MarkAsWarning(Label label, string suggestion)
        {
            label.CssClass = "warning";
            label.Text += "<br /><span class=\"suggestion\">" + suggestion + "</span>";
        }

        private void MarkAsPass(Label label)
        {
            label.CssClass = "pass";
        }

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            this._Tasks[this.CurrentTaskNo]();

            this.CurrentTaskNo++;
            if (this.CurrentTaskNo == this._Tasks.Length)
            {
                this.RefreshTimer.Enabled = false;
            }
        }

        #region Tasks

        private void TestStoreUrl()
        {
            if (IoCFactory.Resolve<ISettingManager>().StoreUrl.Equals(CommonHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase))
            {
                MarkAsPass(lblStoreUrl);
            }
            else
            {
                MarkAsWarning(lblStoreUrl, string.Format("Specified store URL ({0}) doesn't match this store URL ({1})", IoCFactory.Resolve<ISettingManager>().StoreUrl, CommonHelper.GetStoreLocation(false)));
            }
        }

        private void TestPrimaryExchangeRateCurrency()
        {
            if (IoCFactory.Resolve<ICurrencyManager>().PrimaryExchangeRateCurrency != null)
            {
                MarkAsPass(lblPrimaryExchangeRateCurrency);
            }
            else
            {
                MarkAsFail(lblPrimaryExchangeRateCurrency, "Primary exchange rate currency is not set", "<a href=\"Currencies.aspx\">Set it here</a>");
            }
        }

        private void TestPrimaryStoreCurrency()
        {
            if (IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency != null)
            {
                MarkAsPass(lblPrimaryStoreCurrency);
            }
            else
            {
                MarkAsFail(lblPrimaryStoreCurrency, "Primary store currency is not set", "<a href=\"Currencies.aspx\">Set it here</a>");
            }
        }

        private void TestBaseWeight()
        {
            if (IoCFactory.Resolve<IMeasureManager>().BaseWeightIn != null)
            {
                MarkAsPass(lblDefaultWeight);
            }
            else
            {
                MarkAsFail(lblDefaultWeight, "Default weight is not set", "<a href=\"Measures.aspx\">Set it here</a>");
            }
        }

        private void TestBaseDimension()
        {
            if (IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn != null)
            {
                MarkAsPass(lblDefaultDimension);
            }
            else
            {
                MarkAsFail(lblDefaultDimension, "Default dimension is not set", "<a href=\"Measures.aspx\">Set it here</a>");
            }
        }

        private void TestMessageTemplates()
        {
            StringBuilder warningResult = new StringBuilder();
            var publishedLanguages = IoCFactory.Resolve<ILanguageManager>().GetAllLanguages(false);
            foreach (var messageTemplate in IoCFactory.Resolve<IMessageManager>().GetAllMessageTemplates())
            {
                foreach (var language in publishedLanguages)
                {
                    var localizedMessageTemplate = IoCFactory.Resolve<IMessageManager>().GetLocalizedMessageTemplate(messageTemplate.Name, language.LanguageId);
                    if (localizedMessageTemplate == null)
                    {
                        warningResult.AppendFormat("You don't have localized version of \"{0}\" message template for {1}.", messageTemplate.Name, language.Name);
                        warningResult.Append("<br />");
                    }
                }
            }
            string warnings = warningResult.ToString();
            if (String.IsNullOrEmpty(warnings))
            {
                MarkAsPass(lblMessageTemplates);
            }
            else
            {
                warningResult.Append("<a href=\"MessageTemplates.aspx\">Set it here</a>");
                warnings = warningResult.ToString();
                MarkAsWarning(lblMessageTemplates, warnings);
            }
        }

        private void TestShippingMethods()
        {
            StringBuilder warningResult = new StringBuilder();
            var srcmList = IoCFactory.Resolve<IShippingManager>().GetAllShippingRateComputationMethods(false);
            int offlineSrcmCount = 0;
            foreach (var srcm in srcmList)
            {
                if (srcm.ShippingRateComputationMethodType == ShippingRateComputationMethodTypeEnum.Offline)
                    offlineSrcmCount++;
            }
            if (offlineSrcmCount > 1)
            {
                warningResult.Append("Only one offline shipping rate computation method is recommended to use");
            }
            string warnings = warningResult.ToString();
            if (String.IsNullOrEmpty(warnings))
            {
                MarkAsPass(lblShippingMethods);
            }
            else
            {
                MarkAsWarning(lblShippingMethods, warnings);
            }
        }

        #endregion
    }
}