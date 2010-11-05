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
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using NopSolutions.NopCommerce.Common;

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
            Currency currency = IoCFactory.Resolve<ICurrencyService>().GetCurrencyById(this.CurrencyId);
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

            try
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(displayLocale);
            }
            catch (Exception)
            {
                throw new NopException("Specified display locale culture is not supported");
            }

            Currency currency = IoCFactory.Resolve<ICurrencyService>().GetCurrencyById(this.CurrencyId);
            if (currency != null)
            {
                currency.Name = txtName.Text;
                currency.CurrencyCode = txtCurrencyCode.Text;
                currency.Rate = txtRate.Value;
                currency.DisplayLocale = displayLocale;
                currency.CustomFormatting = txtCustomFormatting.Text;
                currency.Published = cbPublished.Checked;
                currency.DisplayOrder = txtDisplayOrder.Value;
                currency.UpdatedOn = DateTime.UtcNow;

                IoCFactory.Resolve<ICurrencyService>().UpdateCurrency(currency);
            }
            else
            {
                currency = new Currency()
                {
                    Name = txtName.Text,
                    CurrencyCode = txtCurrencyCode.Text,
                    Rate = txtRate.Value,
                    DisplayLocale = displayLocale,
                    CustomFormatting = txtCustomFormatting.Text,
                    Published = cbPublished.Checked,
                    DisplayOrder = txtDisplayOrder.Value,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };
                IoCFactory.Resolve<ICurrencyService>().InsertCurrency(currency);
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