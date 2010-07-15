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
using System.Linq;
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
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class TaxDisplayTypeSelectorControl : BaseNopUserControl
    {
        private void BindTaxDisplayTypes()
        {
            if (TaxManager.AllowCustomersToSelectTaxDisplayType)
            {
                this.Visible = true;
                this.ddlTaxDisplayType.Items.Clear();

                string[] taxDisplayTypes = Enum.GetNames(typeof(TaxDisplayTypeEnum));
                foreach (string taxDisplayTypeStr in taxDisplayTypes)
                {
                    int taxDisplayTypeId = (int)Enum.Parse(typeof(TaxDisplayTypeEnum), taxDisplayTypeStr, true);
                    string taxDisplayTypeName = string.Empty;

                    switch ((TaxDisplayTypeEnum)taxDisplayTypeId)
                    {
                        case TaxDisplayTypeEnum.IncludingTax:
                            taxDisplayTypeName = LocalizationManager.GetLocaleResourceString("Products.TaxInclusive");
                            break;
                        case TaxDisplayTypeEnum.ExcludingTax:
                            taxDisplayTypeName = LocalizationManager.GetLocaleResourceString("Products.TaxExclusive");
                            break;
                        default:
                            taxDisplayTypeName = CommonHelper.ConvertEnum(taxDisplayTypeStr);
                            break;
                    }

                    var item = new ListItem(taxDisplayTypeName, taxDisplayTypeId.ToString());
                    ddlTaxDisplayType.Items.Add(item);
                }

                CommonHelper.SelectListItem(this.ddlTaxDisplayType, (int)NopContext.Current.TaxDisplayType);
            }
            else
                this.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            BindTaxDisplayTypes();
            base.OnInit(e);
        }

        protected void ddlTaxDisplayType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var taxDisplayType = (TaxDisplayTypeEnum)Enum.ToObject(typeof(TaxDisplayTypeEnum), int.Parse(this.ddlTaxDisplayType.SelectedItem.Value));
            NopContext.Current.TaxDisplayType = taxDisplayType;
            CommonHelper.ReloadCurrentPage();
        }
    }
}
