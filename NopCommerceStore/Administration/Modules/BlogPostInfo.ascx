<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogPostInfoControl"
    CodeBehind="BlogPostInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register Assembly="NopCommerceStore" Namespace="NopSolutions.NopCommerce.Web.Controls"
    TagPrefix="nopCommerce" %>
    
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLanguage" Text="<% $NopResources:Admin.BlogPostInfo.Language %>"
                ToolTip="<% $NopResources:Admin.BlogPostInfo.Language.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLanguage" AutoPostBack="False" CssClass="adminInput" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.BlogPostInfo.Name %>"
                ToolTip="<% $NopResources:Admin.BlogPostInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtBlogPostTitle"
                ErrorMessage="<% $NopResources:Admin.BlogPostInfo.Name.ErrorMessage %>"></nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBody" Text="<% $NopResources:Admin.BlogPostInfo.Body %>"
                ToolTip="<% $NopResources:Admin.BlogPostInfo.Body.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NopHTMLEditor ID="txtBlogPostBody" runat="server" Height="350" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowComments" Text="<% $NopResources:Admin.BlogPostInfo.AllowComments %>"
                ToolTip="<% $NopResources:Admin.BlogPostInfo.AllowComments.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbBlogPostAllowComments" runat="server" Checked="True"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTags" Text="<% $NopResources:Admin.BlogPostInfo.Tags%>"
                ToolTip="<% $NopResources: Admin.BlogPostInfo.Tags.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtTags" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.BlogPostInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.BlogPostInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:HyperLink ID="hlViewComments" runat="server" Text="View Comments"></asp:HyperLink>
        </td>
    </tr>
</table>
