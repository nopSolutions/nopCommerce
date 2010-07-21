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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CurrenciesControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindSettings();
                BindCurrencyGrid();
            }
        }

        protected void BindSettings()
        {
            cbCurrencyRateAutoUpdateEnabled.Checked = SettingManager.GetSettingValueBoolean("ExchangeRateProvider.AutoUpdateEnabled", false);
        }

        protected void BindCurrencyGrid()
        {
            var currencyCollection = CurrencyManager.GetAllCurrencies();
            gvCurrencies.DataSource = currencyCollection;
            gvCurrencies.DataBind();
        }

        protected void BindLiveRateGrid()
        {
            var exchangeRates = CurrencyManager.GetCurrencyLiveRates(CurrencyManager.PrimaryExchangeRateCurrency.CurrencyCode);
            gvLiveRates.DataSource = exchangeRates;
            gvLiveRates.DataBind();
        }
        
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //save exchange rate provider settings
                SettingManager.SetParam("ExchangeRateProvider.Current", ddlExchangeRateProviders.SelectedProvider);
                SettingManager.SetParam("ExchangeRateProvider.AutoUpdateEnabled", cbCurrencyRateAutoUpdateEnabled.Checked.ToString());

                //save currencies
                foreach (GridViewRow row in gvCurrencies.Rows)
                {
                    HiddenField hfCurrencyId = (HiddenField)row.FindControl("hfCurrencyId");
                    int currencyId = int.Parse(hfCurrencyId.Value);
                    
                    RadioButton rdbIsPrimaryExchangeRateCurrency = (RadioButton)row.FindControl("rdbIsPrimaryExchangeRateCurrency");
                    RadioButton rdbIsPrimaryStoreCurrency = (RadioButton)row.FindControl("rdbIsPrimaryStoreCurrency");
                    if (rdbIsPrimaryExchangeRateCurrency.Checked)
                        CurrencyManager.PrimaryExchangeRateCurrency = CurrencyManager.GetCurrencyById(currencyId);
                    if (rdbIsPrimaryStoreCurrency.Checked)
                        CurrencyManager.PrimaryStoreCurrency = CurrencyManager.GetCurrencyById(currencyId);
                }

                BindCurrencyGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnGetLiveRates_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    BindLiveRateGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void gvLiveRates_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ApplyLiveRate")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvLiveRates.Rows[index];

                Label lblCurrencyCode = row.FindControl("lblCurrencyCode") as Label;
                DecimalTextBox txtRate = row.FindControl("txtRate") as DecimalTextBox;

                Currency currency = CurrencyManager.GetCurrencyByCode(lblCurrencyCode.Text);
                if (currency != null)
                {
                    CurrencyManager.UpdateCurrency(currency.CurrencyId, currency.Name, currency.CurrencyCode,
                        txtRate.Value, currency.DisplayLocale, currency.CustomFormatting, currency.Published, currency.DisplayOrder,
                        currency.CreatedOn, DateTime.UtcNow);
                    BindCurrencyGrid();
                }
            }
        }

        protected void gvLiveRates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btnApplyRate = e.Row.FindControl("btnApplyRate") as Button;
                if (btnApplyRate != null)
                    btnApplyRate.CommandArgument = e.Row.RowIndex.ToString();
            }
        }
    }
}