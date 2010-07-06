<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsSettingsControl"
    CodeBehind="NewsSettings.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsSettings.Title")%>" />
        <%=GetLocaleResourceString("Admin.NewsSettings.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.NewsSettings.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" ValidationGroup="NewsSettings" OnClick="btnSave_Click"
            ToolTip="<% $NopResources:Admin.NewsSettings.SaveButton.Tooltip %>" />
    </div>
</div>
<table width="100%" class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNewsEnabled" Text="<% $NopResources:Admin.NewsSettings.NewsEnabled %>"
                ToolTip="<% $NopResources:Admin.NewsSettings.NewsEnabled.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbNewsEnabled"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowNotRegisteredUsersToLeaveComments"
                Text="<% $NopResources:Admin.NewsSettings.AllowGuestComments %>" ToolTip="<% $NopResources:Admin.NewsSettings.AllowGuestComments.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbAllowNotRegisteredUsersToLeaveComments"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNotifyAboutNewNewsComments" Text="<% $NopResources:Admin.NewsSettings.NotifyComments %>"
                ToolTip="<% $NopResources:Admin.NewsSettings.NotifyComments.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbNotifyAboutNewNewsComments"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShowOnMainPage" Text="<% $NopResources:Admin.NewsSettings.ShowOnMainPage %>"
                ToolTip="<% $NopResources:Admin.NewsSettings.ShowOnMainPage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbShowNewsOnMainPage" Checked="true"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblMainPageNewsCount" Text="<% $NopResources:Admin.NewsSettings.NewsCount %>"
                ToolTip="<% $NopResources:Admin.NewsSettings.NewsCount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtMainPageNewsCount"
                Value="3" ValidationGroup="NewsSettings" RequiredErrorMessage="<% $NopResources:Admin.NewsSettings.NewsCount.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="99" RangeErrorMessage="<% $NopResources:Admin.NewsSettings.NewsCount.RangeErrorMessage %>">
            </nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblNewsArchivePageSize" Text="<% $NopResources:Admin.NewsSettings.NewsArchivePageSize %>"
                ToolTip="<% $NopResources:Admin.NewsSettings.NewsArchivePageSize.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewsArchivePageSize"
                RequiredErrorMessage="<% $NopResources:Admin.NewsSettings.NewsArchivePageSize.RequiredErrorMessage %>"
                MinimumValue="1" MaximumValue="200" Value="10" RangeErrorMessage="<% $NopResources:Admin.NewsSettings.NewsArchivePageSize.RangeErrorMessage %>"
                ValidationGroup="BlogSettings"></nopCommerce:NumericTextBox>
        </td>
    </tr>
</table>
