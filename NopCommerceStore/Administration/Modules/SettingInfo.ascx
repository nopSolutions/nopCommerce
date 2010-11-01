<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SettingInfoControl"
    CodeBehind="SettingInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.SettingInfo.Name %>"
                ToolTip="<% $NopResources:Admin.SettingInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.SettingInfo.Name.ErrorMessage %>"
                Width="400px" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblValue" Text="<% $NopResources:Admin.SettingInfo.Value %>"
                ToolTip="<% $NopResources:Admin.SettingInfo.Value.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtValue" runat="server" CssClass="adminInput" Width="400px" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.SettingInfo.Description %>"
                ToolTip="<% $NopResources:Admin.SettingInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                Height="100" Width="400px"></asp:TextBox>
        </td>
    </tr>
</table>
