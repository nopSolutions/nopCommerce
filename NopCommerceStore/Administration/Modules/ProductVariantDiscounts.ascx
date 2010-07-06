<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantDiscountsControl"
    CodeBehind="ProductVariantDiscounts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectDiscountsControl" Src="SelectDiscountsControl.ascx" %>
<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:SelectDiscountsControl ID="DiscountMappingControl" runat="server" CssClass="adminInput">
            </nopCommerce:SelectDiscountsControl>
        </td>
    </tr>
</table>
