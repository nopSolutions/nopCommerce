<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CurrencyInfoControl"
    CodeBehind="CurrencyInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<table class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.CurrencyInfo.Name %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.CurrencyInfo.Name.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCurrencyCode" Text="<% $NopResources:Admin.CurrencyInfo.CurrencyCode %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.CurrencyCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" ID="txtCurrencyCode" CssClass="adminInput"
                ErrorMessage="<% $NopResources:Admin.CurrencyInfo.CurrencyCode.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblRate" Text="<% $NopResources:Admin.CurrencyInfo.Rate %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.Rate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            [<%=CurrencyManager.PrimaryExchangeRateCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtRate" CssClass="adminInput" RequiredErrorMessage="<% $NopResources:Admin.CurrencyInfo.Rate.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="999999" RangeErrorMessage="<% $NopResources:Admin.CurrencyInfo.Rate.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayLocale" Text="<% $NopResources:Admin.CurrencyInfo.DisplayLocale %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.DisplayLocale.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlDisplayLocale" CssClass="adminInput" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomFormatting" Text="<% $NopResources:Admin.CurrencyInfo.CustomFormatting %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.CustomFormatting.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCustomFormatting" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPublished" Text="<% $NopResources:Admin.CurrencyInfo.Published %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox ID="cbPublished" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.CurrencyInfo.DisplayOrder %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                Value="1" RequiredErrorMessage="<% $NopResources:Admin.CurrencyInfo.DisplayOrder.RequiredErrorMessage %>"
                RangeErrorMessage="<% $NopResources:Admin.CurrencyInfo.DisplayOrder.RangeErrorMessage %>"
                MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
        </td>
    </tr>
    <tr runat="server" id="pnlCreatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCreatedOnTitle" Text="<% $NopResources:Admin.CurrencyInfo.CreatedOn %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.CreatedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="pnlUpdatedOn">
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblUpdateOnTitle" Text="<% $NopResources:Admin.CurrencyInfo.UpdateOn %>"
                ToolTip="<% $NopResources:Admin.CurrencyInfo.UpdateOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:Label ID="lblUpdatedOn" runat="server"></asp:Label>
        </td>
    </tr>
</table>
