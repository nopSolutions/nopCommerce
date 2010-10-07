<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryDiscountControl"
    CodeBehind="CategoryDiscount.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectDiscountsControl" Src="SelectDiscountsControl.ascx" %>

<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:SelectDiscountsControl ID="DiscountMappingControl" runat="server" CssClass="adminInput">
            </nopCommerce:SelectDiscountsControl>
        </td>
    </tr>
</table>
