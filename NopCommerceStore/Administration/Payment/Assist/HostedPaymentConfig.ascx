<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostedPaymentConfig.ascx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.Payment.Assist.HostedPaymentConfig" %>

<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>

<table class="adminContent">
    <tr>
        <td colspan="2">
            <ul>
                <li>
                    <b>
                        Должна быть активирована опция «Возвращаться в магазин по URL для возврата» на https://secure.assist.ru/members/ в разделе «Дизайн страниц»
                    </b>
                </li>
            </ul>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Shop ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtShopId" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Gateway URL:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtGatewayUrl" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Authorize only:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbAuthorizeOnly" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Test mode:
        </td>
        <td class="adminData">
            <asp:CheckBox runat="server" ID="cbTestMode" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Additional fee [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]:
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" ID="txtAdditionalFee" Value="0"
                RequiredErrorMessage="Additional fee is required" MinimumValue="0" MaximumValue="100000000"
                RangeErrorMessage="The value must be from 0 to 100,000,000" CssClass="adminInput">
            </nopCommerce:DecimalTextBox>
        </td>
    </tr>
</table>
