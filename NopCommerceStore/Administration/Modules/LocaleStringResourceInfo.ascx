<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LocaleStringResourceInfoControl"
    CodeBehind="LocaleStringResourceInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLanguageTitle" Text="<% $NopResources:Admin.LocaleStringResourceInfo.Language %>"
                ToolTip="<% $NopResources:Admin.LocaleStringResourceInfo.Language.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblLanguage" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblResourceName" Text="<% $NopResources:Admin.LocaleStringResourceInfo.Name %>"
                ToolTip="<% $NopResources:Admin.LocaleStringResourceInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtResourceName" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.LocaleStringResourceInfo.Name.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblResourceValue" Text="<% $NopResources:Admin.LocaleStringResourceInfo.Value %>"
                ToolTip="<% $NopResources:Admin.LocaleStringResourceInfo.Value.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtResourceValue" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.LocaleStringResourceInfo.Value.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
</table>
