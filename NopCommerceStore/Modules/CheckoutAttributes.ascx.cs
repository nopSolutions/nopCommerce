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
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Controls;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CheckoutAttributesControl : BaseNopUserControl
    {
        protected ShoppingCart GetCart()
        {
            return ShoppingCartManager.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
        }

        protected List<CheckoutAttribute> GetCheckoutAttributes()
        {
            ShoppingCart cart = GetCart();
            if (cart == null || cart.Count == 0)
                return new List<CheckoutAttribute>();

            bool shoppingCartRequiresShipping = ShippingManager.ShoppingCartRequiresShipping(cart);
            var checkoutAttributes = CheckoutAttributeManager.GetAllCheckoutAttributes(!shoppingCartRequiresShipping);
            return checkoutAttributes;
        }

        protected override void OnInit(EventArgs e)
        {
            CreateAttributeControls();
            base.OnInit(e);
        }

        public void CreateAttributeControls()
        {
            this.phAttributes.Controls.Clear();

            var checkoutAttributes = GetCheckoutAttributes();
            if (checkoutAttributes.Count > 0)
            {
                this.Visible = true;
                foreach (var attribute in checkoutAttributes)
                {
                    var divAttribute = new Panel();
                    var attributeTitle = new Label();
                    if (attribute.IsRequired)
                        attributeTitle.Text = "<span>*</span> ";

                    //text prompt / title
                    string textPrompt = string.Empty;
                    if (!string.IsNullOrEmpty(attribute.LocalizedTextPrompt))
                        textPrompt = attribute.LocalizedTextPrompt;
                    else
                        textPrompt = attribute.LocalizedName;

                    attributeTitle.Text += Server.HtmlEncode(textPrompt);
                    attributeTitle.Style.Add("font-weight", "bold");

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

                    string controlId = attribute.CheckoutAttributeId.ToString();
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlTypeEnum.DropdownList:
                            {
                                //add control items
                                var ddlAttributes = new DropDownList();
                                ddlAttributes.ID = controlId;
                                if (!attribute.IsRequired)
                                {
                                    ddlAttributes.Items.Add(new ListItem("---", "0"));
                                }
                                var caValues = attribute.CheckoutAttributeValues;

                                bool preSelectedSet = false;
                                foreach (var caValue in caValues)
                                {
                                    string caValueName = caValue.LocalizedName;
                                    if (!this.HidePrices)
                                    {
                                        decimal priceAdjustmentBase = TaxManager.GetCheckoutAttributePrice(caValue);
                                        decimal priceAdjustment = CurrencyManager.ConvertCurrency(priceAdjustmentBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                        if (priceAdjustmentBase > decimal.Zero)
                                            caValueName += string.Format(" [+{0}]", PriceHelper.FormatPrice(priceAdjustment));
                                    }
                                    var caValueItem = new ListItem(caValueName, caValue.CheckoutAttributeValueId.ToString());
                                    if (!preSelectedSet && caValue.IsPreSelected)
                                    {
                                        caValueItem.Selected = caValue.IsPreSelected;
                                        preSelectedSet = true;
                                    }
                                    ddlAttributes.Items.Add(caValueItem);
                                }

                                //set already selected attributes
                                if (NopContext.Current.User != null)
                                {
                                    string selectedCheckoutAttributes = NopContext.Current.User.CheckoutAttributes;
                                    if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                                    {
                                        //clear default selection
                                        foreach (ListItem item in ddlAttributes.Items)
                                        {
                                            item.Selected = false;
                                        }
                                        //select new values
                                        var selectedCaValues = CheckoutAttributeHelper.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                        foreach (var caValue in selectedCaValues)
                                        {
                                            foreach (ListItem item in ddlAttributes.Items)
                                            {
                                                if (caValue.CheckoutAttributeValueId == Convert.ToInt32(item.Value))
                                                {
                                                    item.Selected = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                divAttribute.Controls.Add(ddlAttributes);
                            }
                            break;
                        case AttributeControlTypeEnum.RadioList:
                            {
                                var rblAttributes = new RadioButtonList();
                                rblAttributes.ID = controlId;
                                var caValues = attribute.CheckoutAttributeValues;

                                bool preSelectedSet = false;
                                foreach (var caValue in caValues)
                                {
                                    string caValueName = caValue.LocalizedName;
                                    if (!this.HidePrices)
                                    {
                                        decimal priceAdjustmentBase = TaxManager.GetCheckoutAttributePrice(caValue);
                                        decimal priceAdjustment = CurrencyManager.ConvertCurrency(priceAdjustmentBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                        if (priceAdjustmentBase > decimal.Zero)
                                            caValueName += string.Format(" [+{0}]", PriceHelper.FormatPrice(priceAdjustment));
                                    }
                                    var caValueItem = new ListItem(Server.HtmlEncode(caValueName), caValue.CheckoutAttributeValueId.ToString());
                                    if (!preSelectedSet && caValue.IsPreSelected)
                                    {
                                        caValueItem.Selected = caValue.IsPreSelected;
                                        preSelectedSet = true;
                                    }
                                    rblAttributes.Items.Add(caValueItem);
                                }

                                //set already selected attributes
                                if (NopContext.Current.User != null)
                                {
                                    string selectedCheckoutAttributes = NopContext.Current.User.CheckoutAttributes;
                                    if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                                    {
                                        //clear default selection
                                        foreach (ListItem item in rblAttributes.Items)
                                        {
                                            item.Selected = false;
                                        }
                                        //select new values
                                        var selectedCaValues = CheckoutAttributeHelper.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                        foreach (var caValue in selectedCaValues)
                                        {
                                            foreach (ListItem item in rblAttributes.Items)
                                            {
                                                if (caValue.CheckoutAttributeValueId == Convert.ToInt32(item.Value))
                                                {
                                                    item.Selected = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                divAttribute.Controls.Add(rblAttributes);
                            }
                            break;
                        case AttributeControlTypeEnum.Checkboxes:
                            {
                                var cblAttributes = new CheckBoxList();
                                cblAttributes.ID = controlId;
                                var caValues = attribute.CheckoutAttributeValues;
                                foreach (var caValue in caValues)
                                {
                                    string caValueName = caValue.LocalizedName;
                                    if (!this.HidePrices)
                                    {
                                        decimal priceAdjustmentBase = TaxManager.GetCheckoutAttributePrice(caValue);
                                        decimal priceAdjustment = CurrencyManager.ConvertCurrency(priceAdjustmentBase, CurrencyManager.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                                        if (priceAdjustmentBase > decimal.Zero)
                                            caValueName += string.Format(" [+{0}]", PriceHelper.FormatPrice(priceAdjustment));
                                    }
                                    var caValueItem = new ListItem(Server.HtmlEncode(caValueName), caValue.CheckoutAttributeValueId.ToString());
                                    caValueItem.Selected = caValue.IsPreSelected;
                                    cblAttributes.Items.Add(caValueItem);
                                }

                                //set already selected attributes
                                if (NopContext.Current.User != null)
                                {
                                    string selectedCheckoutAttributes = NopContext.Current.User.CheckoutAttributes;
                                    if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                                    {
                                        //clear default selection
                                        foreach (ListItem item in cblAttributes.Items)
                                        {
                                            item.Selected = false;
                                        }
                                        //select new values
                                        var selectedCaValues = CheckoutAttributeHelper.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                        foreach (var caValue in selectedCaValues)
                                        {
                                            foreach (ListItem item in cblAttributes.Items)
                                            {
                                                if (caValue.CheckoutAttributeValueId == Convert.ToInt32(item.Value))
                                                {
                                                    item.Selected = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                divAttribute.Controls.Add(cblAttributes);
                            }
                            break;
                        case AttributeControlTypeEnum.TextBox:
                            {
                                var txtAttribute = new TextBox();
                                txtAttribute.Width = SettingManager.GetSettingValueInteger("CheckoutAttribute.Textbox.Width", 300);
                                txtAttribute.ID = controlId;

                                //set already selected attributes
                                if (NopContext.Current.User != null)
                                {
                                    string selectedCheckoutAttributes = NopContext.Current.User.CheckoutAttributes;
                                    if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                                    {
                                        //clear default selection
                                        txtAttribute.Text = string.Empty;

                                        //select new values
                                        var enteredText = CheckoutAttributeHelper.ParseValues(selectedCheckoutAttributes,attribute.CheckoutAttributeId);
                                        if (enteredText.Count > 0)
                                        {
                                            txtAttribute.Text = enteredText[0];
                                        }
                                    }
                                }
                                divAttribute.Controls.Add(txtAttribute);
                            }
                            break;
                        case AttributeControlTypeEnum.MultilineTextbox:
                            {
                                var txtAttribute = new TextBox();
                                txtAttribute.ID = controlId;
                                txtAttribute.TextMode = TextBoxMode.MultiLine;
                                txtAttribute.Width = SettingManager.GetSettingValueInteger("CheckoutAttribute.MultiTextbox.Width", 300);
                                txtAttribute.Height = SettingManager.GetSettingValueInteger("CheckoutAttribute.MultiTextbox.Height", 150);

                                //set already selected attributes
                                if (NopContext.Current.User != null)
                                {
                                    string selectedCheckoutAttributes = NopContext.Current.User.CheckoutAttributes;
                                    if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                                    {
                                        //clear default selection
                                        txtAttribute.Text = string.Empty;

                                        //select new values
                                        var enteredText = CheckoutAttributeHelper.ParseValues(selectedCheckoutAttributes, attribute.CheckoutAttributeId);
                                        if (enteredText.Count > 0)
                                        {
                                            txtAttribute.Text = enteredText[0];
                                        }
                                    }
                                }
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
                    phAttributes.Controls.Add(divAttribute);
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
                var checkoutAttributes = GetCheckoutAttributes();

                foreach (var attribute in checkoutAttributes)
                {
                    string controlId = attribute.CheckoutAttributeId.ToString();
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
                                        selectedAttributes = CheckoutAttributeHelper.AddCheckoutAttribute(selectedAttributes,
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
                                        selectedAttributes = CheckoutAttributeHelper.AddCheckoutAttribute(selectedAttributes,
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
                                                selectedAttributes = CheckoutAttributeHelper.AddCheckoutAttribute(selectedAttributes, 
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
                                        selectedAttributes = CheckoutAttributeHelper.AddCheckoutAttribute(selectedAttributes,
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
                                        selectedAttributes = CheckoutAttributeHelper.AddCheckoutAttribute(selectedAttributes,
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
                                        selectedAttributes = CheckoutAttributeHelper.AddCheckoutAttribute(selectedAttributes,
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

        public bool HasAttributes
        {
            get
            {
                var checkoutAttributes = GetCheckoutAttributes();
                bool result = checkoutAttributes.Count > 0;
                return result;
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