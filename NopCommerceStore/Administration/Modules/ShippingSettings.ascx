<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ShippingSettingsControl"
    CodeBehind="ShippingSettings.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingSettings.Title")%>" />
        <%=GetLocaleResourceString("Admin.ShippingSettings.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ShippingSettings.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" ToolTip="<% $NopResources:Admin.ShippingSettings.SaveButton.Tooltip %>" />
    </div>
</div>
<table width="100%" class="adminContent">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblFreeShippingOverX" Text="<% $NopResources:Admin.ShippingSettings.FreeShippingOverX %>"
                ToolTip="<% $NopResources:Admin.ShippingSettings.FreeShippingOverX.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbFreeShippingOverX" Checked="false"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblValueOfX" Text="<% $NopResources:Admin.ShippingSettings.ValueOfX %>"
                ToolTip="<% $NopResources:Admin.ShippingSettings.ValueOfX.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtFreeShippingOverX"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.ShippingSettings.ValueOfX.RequiredErrorMessage %>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ShippingSettings.ValueOfX.RangeErrorMessage %>">
            </nopCommerce:DecimalTextBox> [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
        </td>
    </tr>
    <tr class="adminSeparator">
        <td colspan="2">
            <hr />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEstimateShippingEnabled" Text="<% $NopResources:Admin.ShippingSettings.EstimateShippingEnabled %>"
                ToolTip="<% $NopResources:Admin.ShippingSettings.EstimateShippingEnabled.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbEstimateShippingEnabled" Checked="false"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShippingOriginCountry" Text="<% $NopResources:Admin.ShippingSettings.OriginCountry %>"
                ToolTip="<% $NopResources:Admin.ShippingSettings.OriginCountry.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlShippingOriginCountry" AutoPostBack="True" runat="server"
                CssClass="adminInput" OnSelectedIndexChanged="ddlShippingOriginCountry_SelectedIndexChanged">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShippingOriginStateProvinceTitle"
                Text="<% $NopResources:Admin.ShippingSettings.OriginState %>" ToolTip="<% $NopResources:Admin.ShippingSettings.OriginState.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlShippingOriginStateProvince" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShippingOriginZipPostalTitle" Text="<% $NopResources:Admin.ShippingSettings.ShippingOriginZip %>"
                ToolTip="<% $NopResources:Admin.ShippingSettings.ShippingOriginZip.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShippingOriginZipPostalCode" CssClass="adminInput">
            </asp:TextBox>
        </td>
    </tr>
</table>
