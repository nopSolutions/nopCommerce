<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumsSettingsControl"
    CodeBehind="ForumsSettings.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ForumsSettings.Title")%>" />
        <%=GetLocaleResourceString("Admin.ForumsSettings.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ForumsSettings.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" ValidationGroup="ForumSettings" OnClick="btnSave_Click"
            ToolTip="<% $NopResources:Admin.ForumsSettings.SaveButton.Tooltip %>" />
    </div>
</div>
<table width="100%" class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblForumsEnabled" Text="<% $NopResources:Admin.ForumsSettings.ForumsEnabled %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.ForumsEnabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbForumsEnabled"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShowCustomersPostCount" Text="<% $NopResources:Admin.ForumsSettings.ShowPostCount %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.ShowPostCount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbShowCustomersPostCount" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGuestsAllowedToCreatePosts" Text="<% $NopResources:Admin.ForumsSettings.AllowGuestsToCreatePosts %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.AllowGuestsToCreatePosts.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbGuestsAllowedToCreatePosts" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGuestsAllowedToCreateTopics" Text="<% $NopResources:Admin.ForumsSettings.AllowGuestsToCreateTopics %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.AllowGuestsToCreateTopics.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbGuestsAllowedToCreateTopics" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomersAllowedToEditPosts" Text="<% $NopResources:Admin.ForumsSettings.AllowToEditPosts %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.AllowToEditPosts.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCustomersAllowedToEditPosts" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomersAllowedToDeletePosts" Text="<% $NopResources:Admin.ForumsSettings.AllowToDeletePosts %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.AllowToDeletePosts.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCustomersAllowedToDeletePosts" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomersAllowedToManageSubscriptions" Text="<% $NopResources:Admin.ForumsSettings.AllowToManageSubscriptions %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.AllowToManageSubscriptions.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbCustomersAllowedToManageSubscriptions" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblTopicsPageSize" Text="<% $NopResources:Admin.ForumsSettings.TopicsPageSize %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.TopicsPageSize.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtTopicsPageSize"
                RequiredErrorMessage="<% $NopResources:Admin.ForumsSettings.TopicsPageSize.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ForumsSettings.TopicsPageSize.RangeErrorMessage %>"
                ValidationGroup="ForumSettings"></nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPostsPageSize" Text="<% $NopResources:Admin.ForumsSettings.PostsPageSize %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.PostsPageSize.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtPostsPageSize"
                RequiredErrorMessage="<% $NopResources:Admin.ForumsSettings.PostsPageSize.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" Value="10" RangeErrorMessage="<% $NopResources:Admin.ForumsSettings.PostsPageSize.RangeErrorMessage %>"
                ValidationGroup="ForumSettings"></nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblForumEditor" Text="<% $NopResources:Admin.ForumsSettings.ForumEditor %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.ForumEditor.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList runat="server" ID="ddlForumEditor" CssClass="adminInputNoWidth">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSignatureEnabled" Text="<% $NopResources:Admin.ForumsSettings.SignatureEnabled %>"
                ToolTip="<% $NopResources:Admin.ForumsSettings.SignatureEnabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbSignatureEnabled"></asp:CheckBox>
        </td>
    </tr>
</table>
