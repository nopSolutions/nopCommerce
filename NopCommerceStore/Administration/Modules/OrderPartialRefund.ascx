<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.OrderPartialRefundControl"
    CodeBehind="OrderPartialRefund.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.OrderPartialRefund.Title")%>" />
        <%=GetLocaleResourceString("Admin.OrderPartialRefund.Title")%>
    </div>
</div>
<p>
    <b>
        <asp:Literal runat="server" ID="lOrderInfo"></asp:Literal></b>
</p>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblAmountToRefund" Text="<% $NopResources:Admin.OrderPartialRefund.AmountToRefund %>"
                ToolTip="<% $NopResources:Admin.OrderPartialRefund.AmountToRefund.Tooltip %>"
                ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtAmountToRefund"
                Value="0" RequiredErrorMessage="<% $NopResources:Admin.OrderPartialRefund.AmountToRefund.RequiredErrorMessage%>"
                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.OrderPartialRefund.AmountToRefund.RangeErrorMessage %>"
                Width="100px"></nopCommerce:DecimalTextBox>
            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>] &nbsp;&nbsp;&nbsp; <span
                style="white-space: nowrap">
                <asp:Label runat="server" ID="lblMaxAmountToRefund" /></span>
        </td>
    </tr>
</table>

<nopCommerce:ConfirmationBox runat="server" ID="cbRefund" TargetControlID="btnRefund"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
<asp:Button ID="btnRefund" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.OrderPartialRefund.Refund %>"
    OnClick="btnRefund_Click" />
