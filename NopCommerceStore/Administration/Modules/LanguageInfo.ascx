<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LanguageInfoControl"
    CodeBehind="LanguageInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.LanguageInfo.Name %>"
                ToolTip="<% $NopResources:Admin.LanguageInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.LanguageInfo.Name.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblLanguageCulture" Text="<% $NopResources:Admin.LanguageInfo.LanguageCulture %>"
                ToolTip="<% $NopResources:Admin.LanguageInfo.LanguageCulture.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlLanguageCulture" CssClass="adminInput" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFlagImageFileName" Text="<% $NopResources:Admin.LanguageInfo.FlagImageFileName %>"
                ToolTip="<% $NopResources:Admin.LanguageInfo.FlagImageFileName.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtFlagImageFileName" runat="server" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPublished" Text="<% $NopResources:Admin.LanguageInfo.Published %>"
                ToolTip="<% $NopResources:Admin.LanguageInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.LanguageInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.LanguageInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.LanguageInfo.DisplayOrder.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.LanguageInfo.DisplayOrder.RangeErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr runat="server" id="pnlImport">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblImportResources" Text="<% $NopResources:Admin.LanguageInfo.Import %>"
                ToolTip="<% $NopResources:Admin.LanguageInfo.Import.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:FileUpload ID="fuImportResources" CssClass="adminInput" runat="server" />
            <asp:Button ID="btnImportResources" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.LanguageInfo.ImportButton %>"
                OnClick="btnImportResources_Click" ToolTip="<% $NopResources:Admin.LanguageInfo.ImportButton.Tooltip %>" />
        </td>
    </tr>
</table>
