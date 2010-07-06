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
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CurrencyInfoControl : BaseNopAdministrationUserControl
    {
        private int CompareCultures(CultureInfo x, CultureInfo y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {

                    return x.IetfLanguageTag.CompareTo(y.IetfLanguageTag);
                }
            }
        }

        private void FillDropDowns()
        {
            List<CultureInfo> cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();
            cultures.Sort(CompareCultures);
            this.ddlDisplayLocale.Items.Clear();
            foreach (CultureInfo ci in cultures)
            {
                string name = string.Format("{0}. {1}", ci.IetfLanguageTag, ci.EnglishName);
                ListItem item2 = new ListItem(name, ci.IetfLanguageTag);
                this.ddlDisplayLocale.Items.Add(item2);
            }
        }

        private void BindData()
        {
            Currency currency = CurrencyManager.GetCurrencyById(this.CurrencyId);
            if (currency != null)
            {
                this.txtName.Text = currency.Name;
                this.txtCurrencyCode.Text = currency.CurrencyCode;

                ListItem ciItem = ddlDisplayLocale.Items.FindByValue(currency.DisplayLocale);
                if (ciItem != null)
                    ciItem.Selected = true;

                this.txtCustomFormatting.Text = currency.CustomFormatting;
                this.txtRate.Value = currency.Rate;
                this.cbPublished.Checked = currency.Published;
                this.txtDisplayOrder.Value = currency.DisplayOrder;
                this.pnlCreatedOn.Visible = true;
                this.pnlUpdatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(currency.CreatedOn, DateTimeKind.Utc).ToString();
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(currency.UpdatedOn, DateTimeKind.Utc).ToString();
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public Currency SaveInfo()
        {
            string displayLocale = ddlDisplayLocale.SelectedItem.Value;

            Currency currency = CurrencyManager.GetCurrencyById(this.CurrencyId);
            if (currency != null)
            {
                currency = CurrencyManager.UpdateCurrency(currency.CurrencyId, txtName.Text, txtCurrencyCode.Text,
                    txtRate.Value, displayLocale, txtCustomFormatting.Text, cbPublished.Checked, txtDisplayOrder.Value,
                    currency.CreatedOn, DateTime.UtcNow);
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                currency = CurrencyManager.InsertCurrency(txtName.Text, txtCurrencyCode.Text,
                    txtRate.Value, displayLocale, txtCustomFormatting.Text, 
                    cbPublished.Checked, txtDisplayOrder.Value, now, now);
            }

            return currency;
        }

        public int CurrencyId
        {
            get
            {
                return CommonHelper.QueryStringInt("CurrencyId");
            }
        }
    }
}