<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentMethodsFilterControl.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PaymentMethodsFilterControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:ToolTipLabel runat="server" Text="<% $NopResources:Admin.PaymentMethodsFilterControl.Title %>"
                ToolTip="<% $NopResources:Admin.PaymentMethodsFilterControl.Info %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            :
            <br />
            <asp:GridView ID="gvPaymentMethodCountryMap" runat="server" AutoGenerateColumns="False"
                Width="100%" />
            <br />
            <asp:Label runat="server" ID="lblMessage" />
        </td>
    </tr>
</table>
