<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShippingMethodsFilterControl.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ShippingMethodsFilterControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:ToolTipLabel runat="server" Text="<% $NopResources:Admin.ShippingMethodsFilterControl.Title %>"
                ToolTip="<% $NopResources:Admin.ShippingMethodsFilterControl.Info %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            :
            <br />
            <asp:GridView ID="gvShippingMethodCountryMap" runat="server" AutoGenerateColumns="False"
                Width="100%" />
            <br />
            <asp:Label runat="server" ID="lblMessage" />
        </td>
    </tr>
</table>
