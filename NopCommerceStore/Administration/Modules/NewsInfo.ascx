<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsInfoControl" CodeBehind="NewsInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
    
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLanguage" Text="<% $NopResources:Admin.NewsInfo.Language %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.Language.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLanguage" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTitle" Text="<% $NopResources:Admin.NewsInfo.Title %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.Title.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtTitle" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.NewsInfo.Title.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShort" Text="<% $NopResources:Admin.NewsInfo.Short %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.Short.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtShort" runat="server" CssClass="adminInput" TextMode="MultiLine"
                Height="100"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFull" Text="<% $NopResources:Admin.NewsInfo.Full %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.Full.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NopHTMLEditor ID="txtFull" runat="server" Height="350" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPublished" Text="<% $NopResources:Admin.NewsInfo.Published %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowComments" Text="<% $NopResources:Admin.NewsInfo.AllowComments %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.AllowComments.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbAllowComments" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.NewsInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.NewsInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:HyperLink ID="hlViewComments" runat="server" Text="View Comments" ToolTip="<% $NopResources:Admin.NewsInfo.ViewComments.Tooltip %>" />
        </td>
    </tr>
</table>
