<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlogSettingsControl"
    CodeBehind="BlogSettings.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.BlogSettings.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlogSettings.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.BlogSettings.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" ValidationGroup="BlogSettings" OnClick="btnSave_Click"
            ToolTip="<% $NopResources:Admin.BlogSettings.SaveButton.Tooltip %>" />
    </div>
</div>
<table width="100%" class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEnableBlog" Text="<% $NopResources:Admin.BlogSettings.EnableBlog %>"
                ToolTip="<% $NopResources:Admin.BlogSettings.EnableBlog.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbEnableBlog"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowNotRegisteredUsersToLeaveComments"
                Text="<% $NopResources:Admin.BlogSettings.AllowGuestComments %>" ToolTip="<% $NopResources:Admin.BlogSettings.AllowGuestComments.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbAllowNotRegisteredUsersToLeaveComments"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNotifyAboutNewBlogComments" Text="<% $NopResources:Admin.BlogSettings.NotifyNewComments %>"
                ToolTip="<% $NopResources:Admin.BlogSettings.NotifyNewComments.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbNotifyAboutNewBlogComments"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPostsPageSize" Text="<% $NopResources:Admin.BlogSettings.PostsPageSize %>"
                ToolTip="<% $NopResources:Admin.BlogSettings.PostsPageSize.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtPostsPageSize"
                RequiredErrorMessage="<% $NopResources:Admin.BlogSettings.PostsPageSize.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.BlogSettings.PostsPageSize.RangeErrorMessage %>"
                ValidationGroup="BlogSettings"></nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
