<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlacklistIPInfo.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistIPInfoControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBannedIP" Text="<% $NopResources:Admin.BlacklistIPInfo.BannedIP %>"
                ToolTip="<% $NopResources:Admin.BlacklistIPInfo.BannedIP.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtBannedIP" runat="server" Width="256px"></asp:TextBox>
            <span>(<%=GetLocaleResourceString("Admin.BlacklistIPInfo.Format")%>
                <b>192.168.1.100</b>)</span>
        </td>
    </tr>
    <tr>
        <td class="adminTitle" valign="top">
            <nopCommerce:ToolTipLabel runat="server" ID="lblComment" Text="<% $NopResources:Admin.BlacklistIPInfo.Comment %>"
                ToolTip="<% $NopResources:Admin.BlacklistIPInfo.Comment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="5" Width="256px"></asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td>
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.BlacklistIPInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.BlacklistIPInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td>
            <asp:Literal ID="lblCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr runat="server" id="pnlUpdatedOn">
        <td>
            <nopCommerce:ToolTipLabel runat="server" ID="lblUpdatedOnTitle" Text="<% $NopResources:Admin.BlacklistIPInfo.UpdatedOn %>"
                ToolTip="<% $NopResources:Admin.BlacklistIPInfo.UpdatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td>
            <asp:Literal ID="lblUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
</table>
