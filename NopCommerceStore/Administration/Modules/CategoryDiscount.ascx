<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryDiscountControl"
    CodeBehind="CategoryDiscount.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectDiscountsControl" Src="SelectDiscountsControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCategoryControl" Src="SelectCategoryControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:SelectDiscountsControl ID="DiscountMappingControl" runat="server" CssClass="adminInput">
            </nopCommerce:SelectDiscountsControl>
        </td>
    </tr>
</table>
