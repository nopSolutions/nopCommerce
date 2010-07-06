<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlacklistNetworkInfo.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistNetworkInfoControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBannedIP" Text="<% $NopResources:Admin.BlacklistNetworkInfo.BannedIP %>"
                ToolTip="<% $NopResources:Admin.BlacklistNetworkInfo.BannedIP.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtBannedIP" runat="server" Width="256px"></asp:TextBox>
            <span>(<%=GetLocaleResourceString("Admin.BlacklistNetworkInfo.Format")%>
                <b>192.168.1.100-192.168.1.200</b>)</span>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblIpException" Text="<% $NopResources:Admin.BlacklistNetworkInfo.Exception %>"
                ToolTip="<% $NopResources:Admin.BlacklistNetworkInfo.Exception.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtIpException" runat="server" Width="256px"></asp:TextBox>
            <span>(<%=GetLocaleResourceString("Admin.BlacklistNetworkInfo.Format")%>
                <b>192.168.1.101;192.168.1.102</b>;
                <%=GetLocaleResourceString("Admin.BlacklistNetworkInfo.OrLeaveEmpty")%>)</span>
        </td>
    </tr>
    <tr>
        <td class="adminTitle" valign="top">
            <nopCommerce:ToolTipLabel runat="server" ID="lblComment" Text="<% $NopResources:Admin.BlacklistNetworkInfo.Comment %>"
                ToolTip="<% $NopResources:Admin.BlacklistNetworkInfo.Comment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="5" Width="256px"></asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td>
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.BlacklistNetworkInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.BlacklistNetworkInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td>
            <asp:Literal ID="lblCreatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
    <tr runat="server" id="pnlUpdatedOn">
        <td>
            <nopCommerce:ToolTipLabel runat="server" ID="lblUpdatedOnTitle" Text="<% $NopResources:Admin.BlacklistNetworkInfo.UpdatedOn %>"
                ToolTip="<% $NopResources:Admin.BlacklistNetworkInfo.UpdatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td>
            <asp:Literal ID="lblUpdatedOn" runat="server"></asp:Literal>
        </td>
    </tr>
</table>
