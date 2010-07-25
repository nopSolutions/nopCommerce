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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using System.Globalization;
using NopSolutions.NopCommerce.Controls;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductAttributesControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CreateAttributeControls();
        }

        public void CreateAttributeControls()
        {
            var productVariant = ProductManager.GetProductVariantById(this.ProductVariantId);
            if (productVariant != null)
            {
                this.phAttributes.Controls.Clear();
                var productVariantAttributes = productVariant.ProductVariantAttributes;
                if (productVariantAttributes.Count > 0)
                {
                    StringBuilder adjustmentTableScripts = new StringBuilder();
                    StringBuilder attributeScripts = new StringBuilder();

                    this.Visible = true;
                    foreach (var attribute in productVariantAttributes)
                    {
                        var divAttribute = new Panel();
                        var attributeTitle = new Label();
                        if (attribute.IsRequired)
                            attributeTitle.Text = "<span>*</span> ";

                        //text prompt / title
                        string textPrompt = string.Empty;
                        if (!string.IsNullOrEmpty(attribute.TextPrompt))
                            textPrompt = attribute.TextPrompt;
                        else
                            textPrompt = attribute.ProductAttribute.LocalizedName;

                        attributeTitle.Text += Server.HtmlEncode(textPrompt);
                        attributeTitle.Style.Add("font-weight", "bold");

                        //description
                        if (!string.IsNullOrEmpty(attribute.ProductAttribute.LocalizedDescription))
                            attributeTitle.Text += string.Format("<br /><span>{0}</span>", Server.HtmlEncode(attribute.ProductAttribute.LocalizedDescription));

                        bool addBreak = true;
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlTypeEnum.TextBox:
                                {
                                    addBreak = false;
                                }
                                break;
                            default:
                                break;
                        }
                        if (addBreak)
                        {
                            attributeTitle.Text += "<br />";
                        }
                        else
                        {
                            attributeTitle.Text += "&nbsp;&nbsp;&nbsp;";
                        }
                        divAttribute.Controls.Add(attributeTitle);
                        phAttributes.Controls.Add(divAttribute);

                        string controlId = string.Format("{0}_{1}", attribute.ProductAttribute.ProductAttributeId, attribute.ProductVariantAttributeId);
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlTypeEnum.DropdownList:
                                {
                                    var ddlAttributes = new DropDownList();
                                    ddlAttributes.ID = controlId;
                                    divAttribute.Controls.Add(ddlAttributes);
                                    ddlAttributes.Items.Clear();

                                    if (!attribute.IsRequired)
                                    {
                                        ddlAttributes.Items.Add(new ListItem("---", "0"));
                                    }
                                    var pvaValues = attribute.ProductVariantAttributeValues;

                                    adjustmentTableScripts.AppendFormat("{0}['{1}'] = new Array(", AdjustmentTableName, ddlAttributes.ClientID);
                                    attributeScripts.AppendFormat("$('#{0}').change(function(){{{1}();}});\n", ddlAttributes.ClientID, AdjustmentFuncName);

                                    bool preSelectedSet = false;
                                    foreach (var pvaValue in pvaValues)
                                    {
                                        string pvaValueName = pvaValue.LocalizedName;
                                        if (!this.HidePrices &&
                                            (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                                            (NopContext.Current.User != null &&
                                            !NopContext.Current.User.IsGuest)))
                                        {
                                            decimal taxRate = decimal.Zero;
                                            decimal priceAdjustmentBase = TaxManager.GetPrice(productVariant, pvaValue.PriceAdjustment, out taxRate);
                                            decimal priceAdjustment = CurrencyManager.ConvertCurrency(priceAdjustmentBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                            if(priceAdjustmentBase > decimal.Zero)
                                            {
                                                pvaValueName += string.Format(" [+{0}]", PriceHelper.FormatPrice(priceAdjustment, false, false));
                                            }
                                            adjustmentTableScripts.AppendFormat(CultureInfo.InvariantCulture, "{0},", (float)priceAdjustment);
                                        }
                                        var pvaValueItem = new ListItem(pvaValueName, pvaValue.ProductVariantAttributeValueId.ToString());
                                        if (!preSelectedSet && pvaValue.IsPreSelected)
                                        {
                                            pvaValueItem.Selected = pvaValue.IsPreSelected;
                                            preSelectedSet = true;
                                        }
                                        ddlAttributes.Items.Add(pvaValueItem);
                                    }
                                    adjustmentTableScripts.Length -= 1;
                                    adjustmentTableScripts.Append(");\n");
                                }
                                break;
                            case AttributeControlTypeEnum.RadioList:
                                {
                                    var rblAttributes = new RadioButtonList();
                                    rblAttributes.ID = controlId;
                                    divAttribute.Controls.Add(rblAttributes);
                                    rblAttributes.Items.Clear();

                                    var pvaValues = attribute.ProductVariantAttributeValues;
                                    bool preSelectedSet = false;
                                    foreach (var pvaValue in pvaValues)
                                    {
                                        string pvaValueName = pvaValue.LocalizedName;
                                        if (!this.HidePrices &&
                                            (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                                            (NopContext.Current.User != null &&
                                            !NopContext.Current.User.IsGuest)))
                                        {
                                            decimal taxRate = decimal.Zero;
                                            decimal priceAdjustmentBase = TaxManager.GetPrice(productVariant, pvaValue.PriceAdjustment, out taxRate);
                                            decimal priceAdjustment = CurrencyManager.ConvertCurrency(priceAdjustmentBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                            if(priceAdjustmentBase > decimal.Zero)
                                            {
                                                pvaValueName += string.Format(" [+{0}]", PriceHelper.FormatPrice(priceAdjustment, false, false));
                                            }
                                            adjustmentTableScripts.AppendFormat(CultureInfo.InvariantCulture, "{0}['{1}_{2}'] = {3};\n", AdjustmentTableName, rblAttributes.ClientID, rblAttributes.Items.Count, (float)priceAdjustment);
                                            attributeScripts.AppendFormat("$('#{0}_{1}').click(function(){{{2}();}});\n", rblAttributes.ClientID, rblAttributes.Items.Count, AdjustmentFuncName);
                                        }
                                        var pvaValueItem = new ListItem(Server.HtmlEncode(pvaValueName), pvaValue.ProductVariantAttributeValueId.ToString());
                                        if (!preSelectedSet && pvaValue.IsPreSelected)
                                        {
                                            pvaValueItem.Selected = pvaValue.IsPreSelected;
                                            preSelectedSet = true;
                                        }
                                        rblAttributes.Items.Add(pvaValueItem);
                                    }
                                }
                                break;
                            case AttributeControlTypeEnum.Checkboxes:
                                {
                                    var cblAttributes = new CheckBoxList();
                                    cblAttributes.ID = controlId;
                                    divAttribute.Controls.Add(cblAttributes);
                                    cblAttributes.Items.Clear();

                                    var pvaValues = attribute.ProductVariantAttributeValues;
                                    foreach (var pvaValue in pvaValues)
                                    {
                                        string pvaValueName = pvaValue.LocalizedName;
                                        if (!this.HidePrices &&
                                            (!SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                                            (NopContext.Current.User != null &&
                                            !NopContext.Current.User.IsGuest)))
                                        {
                                            decimal taxRate = decimal.Zero;
                                            decimal priceAdjustmentBase = TaxManager.GetPrice(productVariant, pvaValue.PriceAdjustment, out taxRate);
                                            decimal priceAdjustment = CurrencyManager.ConvertCurrency(priceAdjustmentBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                            if (priceAdjustmentBase > decimal.Zero)
                                                pvaValueName += string.Format(" [+{0}]", PriceHelper.FormatPrice(priceAdjustment, false, false));
                                            adjustmentTableScripts.AppendFormat(CultureInfo.InvariantCulture, "{0}['{1}_{2}'] = {3};\n", AdjustmentTableName, cblAttributes.ClientID, cblAttributes.Items.Count, (float)priceAdjustment);
                                            attributeScripts.AppendFormat("$('#{0}_{1}').click(function(){{{2}();}});\n", cblAttributes.ClientID, cblAttributes.Items.Count, AdjustmentFuncName);
                                        }
                                        var pvaValueItem = new ListItem(Server.HtmlEncode(pvaValueName), pvaValue.ProductVariantAttributeValueId.ToString());
                                        pvaValueItem.Selected = pvaValue.IsPreSelected;
                                        cblAttributes.Items.Add(pvaValueItem);
                                    }
                                }
                                break;
                            case AttributeControlTypeEnum.TextBox:
                                {
                                    var txtAttribute = new TextBox();
                                    txtAttribute.Width = SettingManager.GetSettingValueInteger("ProductAttribute.Textbox.Width", 300);
                                    txtAttribute.ID = controlId;
                                    divAttribute.Controls.Add(txtAttribute);
                                }
                                break;
                            case AttributeControlTypeEnum.MultilineTextbox:
                                {
                                    var txtAttribute = new TextBox();
                                    txtAttribute.ID = controlId;
                                    txtAttribute.TextMode = TextBoxMode.MultiLine;
                                    txtAttribute.Width = SettingManager.GetSettingValueInteger("ProductAttribute.MultiTextbox.Width", 300);
                                    txtAttribute.Height = SettingManager.GetSettingValueInteger("ProductAttribute.MultiTextbox.Height", 150);
                                    divAttribute.Controls.Add(txtAttribute);
                                }
                                break;
                            case AttributeControlTypeEnum.Datepicker:
                                {
                                    var datePicker = new NopDatePicker();
                                    //changes these properties in order to change year range
                                    datePicker.FirstYear = DateTime.Now.Year;
                                    datePicker.LastYear = DateTime.Now.Year + 1;
                                    datePicker.ID = controlId;
                                    divAttribute.Controls.Add(datePicker);
                                }
                                break;
                            default:
                                break;
                        }
                        
                    }
                 
                    lblAdjustmentTableScripts.Text = adjustmentTableScripts.ToString();
                    lblAttributeScripts.Text = attributeScripts.ToString();
                }
                else
                {
                    this.Visible = false;
                }
            }
            else
            {
                this.Visible = false;
            }
        }

        public string SelectedAttributes
        {
            get
            {
                string selectedAttributes = string.Empty;
                var productVariantAttributes = ProductAttributeManager.GetProductVariantAttributesByProductVariantId(this.ProductVariantId);
                foreach (ProductVariantAttribute attribute in productVariantAttributes)
                {
                    string controlId = string.Format("{0}_{1}", attribute.ProductAttribute.ProductAttributeId, attribute.ProductVariantAttributeId);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlTypeEnum.DropdownList:
                            {
                                var ddlAttributes = phAttributes.FindControl(controlId) as DropDownList;
                                if (ddlAttributes != null)
                                {
                                    int selectedAttributeId = 0;
                                    if (!String.IsNullOrEmpty(ddlAttributes.SelectedValue))
                                    {
                                        selectedAttributeId = int.Parse(ddlAttributes.SelectedValue);
                                    }
                                    if (selectedAttributeId > 0)
                                    {
                                        selectedAttributes = ProductAttributeHelper.AddProductAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                    }
                                }
                            }
                            break;
                        case AttributeControlTypeEnum.RadioList:
                            {
                                var rblAttributes =
                                    phAttributes.FindControl(controlId) as RadioButtonList;
                                if (rblAttributes != null)
                                {
                                    int selectedAttributeId = 0;
                                    if (!String.IsNullOrEmpty(rblAttributes.SelectedValue))
                                    {
                                        selectedAttributeId = int.Parse(rblAttributes.SelectedValue);
                                    }
                                    if (selectedAttributeId > 0)
                                    {
                                        selectedAttributes = ProductAttributeHelper.AddProductAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                    }
                                }
                            }
                            break;
                        case AttributeControlTypeEnum.Checkboxes:
                            {
                                var cblAttributes = phAttributes.FindControl(controlId) as CheckBoxList;
                                if (cblAttributes != null)
                                {
                                    foreach (ListItem item in cblAttributes.Items)
                                    {
                                        if (item.Selected)
                                        {
                                            int selectedAttributeId = 0;
                                            if (!String.IsNullOrEmpty(item.Value))
                                            {
                                                selectedAttributeId = int.Parse(item.Value);
                                            }
                                            if (selectedAttributeId > 0)
                                            {
                                                selectedAttributes = ProductAttributeHelper.AddProductAttribute(selectedAttributes, 
                                                    attribute, selectedAttributeId.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case AttributeControlTypeEnum.TextBox:
                            {
                                var txtAttribute = phAttributes.FindControl(controlId) as TextBox;
                                if (txtAttribute != null)
                                {
                                    string enteredText = txtAttribute.Text.Trim();
                                    if (!String.IsNullOrEmpty(enteredText))
                                    {
                                        selectedAttributes = ProductAttributeHelper.AddProductAttribute(selectedAttributes,
                                            attribute, enteredText);
                                    }
                                }
                            }
                            break;
                        case AttributeControlTypeEnum.MultilineTextbox:
                            {
                                var txtAttribute = phAttributes.FindControl(controlId) as TextBox;
                                if (txtAttribute != null)
                                {
                                    string enteredText = txtAttribute.Text.Trim();
                                    if (!String.IsNullOrEmpty(enteredText))
                                    {
                                        selectedAttributes = ProductAttributeHelper.AddProductAttribute(selectedAttributes,
                                            attribute, enteredText);
                                    }
                                }
                            }
                            break;
                        case AttributeControlTypeEnum.Datepicker:
                            {
                                var datePicker = phAttributes.FindControl(controlId) as NopDatePicker;
                                if (datePicker != null)
                                {
                                    DateTime? selectedDate = datePicker.SelectedDate;
                                    if (selectedDate.HasValue)
                                    {
                                        selectedAttributes = ProductAttributeHelper.AddProductAttribute(selectedAttributes,
                                            attribute, selectedDate.Value.ToString("D"));
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                return selectedAttributes;
            }
        }

        public int ProductVariantId
        {
            get
            {
                if (ViewState["ProductVariantId"] == null)
                    return 0;
                else
                    return (int)ViewState["ProductVariantId"];
            }
            set
            {
                ViewState["ProductVariantId"] = value;
            }
        }

        public string AdjustmentTableName
        {
            get
            {
                return String.Format("adjustmentTable_{0}", ProductVariantId);
            }
        }

        public string AdjustmentFuncName
        {
            get
            {
                return String.Format("adjustPrice_{0}", ProductVariantId);
            }
        }

        public string PriceVarName
        {
            get
            {
                return String.Format("priceValForDynUpd_{0}", ProductVariantId);
            }
        }

        public string PriceVarClass
        {
            get
            {
                return String.Format("price-val-for-dyn-upd-{0}", ProductVariantId);
            }
        }

        public bool HidePrices
        {
            get
            {
                if (ViewState["HidePrices"] == null)
                    return false;
                else
                    return (bool)ViewState["HidePrices"];
            }
            set
            {
                ViewState["HidePrices"] = value;
            }
        }
    }
}