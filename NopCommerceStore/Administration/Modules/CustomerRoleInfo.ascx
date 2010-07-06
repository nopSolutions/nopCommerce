<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerRoleInfoControl"
    CodeBehind="CustomerRoleInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblRoleName" Text="<% $NopResources:Admin.CustomerRoleInfo.RoleName %>"
                ToolTip="<% $NopResources:Admin.CustomerRoleInfo.RoleName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.CustomerRoleInfo.RoleName.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFreeShipping" Text="<% $NopResources:Admin.CustomerRoleInfo.FreeShipping %>"
                ToolTip="<% $NopResources:Admin.CustomerRoleInfo.FreeShipping.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbFreeShipping" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxExempt" Text="<% $NopResources:Admin.CustomerRoleInfo.TaxExempt %>"
                ToolTip="<% $NopResources:Admin.CustomerRoleInfo.TaxExempt.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbTaxExempt" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblActive" Text="<% $NopResources:Admin.CustomerRoleInfo.Active %>"
                ToolTip="<% $NopResources:Admin.CustomerRoleInfo.Active.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbActive" runat="server"></asp:CheckBox>
        </td>
    </tr>
</table>
