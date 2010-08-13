<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.OrderDetailsControl"
    CodeBehind="OrderDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.OrderDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.OrderDetails.Title")%>
        <a href="Orders.aspx" title="<%=GetLocaleResourceString("Admin.OrderDetails.BackToOrders")%>">
            (<%=GetLocaleResourceString("Admin.OrderDetails.BackToOrders")%>)</a>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.OrderDetails.BtnPrintPdfPackagingSlip.Text %>"
            CssClass="adminButtonBlue" ID="btnPrintPdfPackagingSlip" OnClick="BtnPrintPdfPackagingSlip_OnClick"
            ValidationGroup="BtnPrintPdfPackagingSlip" ToolTip="<% $NopResources:Admin.OrderDetails.BtnPrintPdfPackagingSlip.Tooltip %>" />
        <asp:Button ID="btnGetInvoicePDF" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.OrderDetails.InvoicePDF.Text %>"
            OnClick="btnGetInvoicePDF_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.OrderDetails.InvoicePDF.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.OrderDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.OrderDetails.DeleteButton.Tooltip %>" />
        <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
        <asp:Button ID="btnRefresh" runat="server" Style="display: none" CausesValidation="false"
        CssClass="adminButton" Text="Refresh" OnClick="btnRefresh_Click" ToolTip="Refresh list" />
    </div>
</div>

<script type="text/javascript">
    $(document).ready(function() {
        toggleOrderTotals(false);
        toggleCC(false);
        toggleBillingAddress(false);
        toggleShippingAddress(false);
    });

    function toggleOrderTotals(editmode) {
        if (editmode) {
            $('#trEditOrderTotals').show();
            $('#<%=btnEditOrderTotals.ClientID %>').hide();
            $('#<%=btnSaveOrderTotals.ClientID %>').show();
            $('#<%=btnCancelOrderTotals.ClientID %>').show();
        }
        else {
            $('#trEditOrderTotals').hide();
            $('#<%=btnEditOrderTotals.ClientID %>').show();
            $('#<%=btnSaveOrderTotals.ClientID %>').hide();
            $('#<%=btnCancelOrderTotals.ClientID %>').hide();
        }
    }

    function toggleCC(editmode) {
        if (editmode) {
            $('#<%=lblCardType.ClientID %>').hide();
            $('#<%=txtCardType.ClientID %>').show();
            $('#<%=lblCardName.ClientID %>').hide();
            $('#<%=txtCardName.ClientID %>').show();
            $('#<%=lblCardNumber.ClientID %>').hide();
            $('#<%=txtCardNumber.ClientID %>').show();
            $('#<%=lblCardCVV2.ClientID %>').hide();
            $('#<%=txtCardCVV2.ClientID %>').show();
            $('#<%=lblCardExpirationMonth.ClientID %>').hide();
            $('#<%=txtCardExpirationMonth.ClientID %>').show();
            $('#<%=lblCardExpirationYear.ClientID %>').hide();
            $('#<%=txtCardExpirationYear.ClientID %>').show();
            $('#<%=btnEditCC.ClientID %>').hide();
            $('#<%=btnSaveCC.ClientID %>').show();
            $('#<%=btnCancelCC.ClientID %>').show();
        }
        else {
            $('#<%=lblCardType.ClientID %>').show();
            $('#<%=txtCardType.ClientID %>').hide();
            $('#<%=lblCardName.ClientID %>').show();
            $('#<%=txtCardName.ClientID %>').hide();
            $('#<%=lblCardNumber.ClientID %>').show();
            $('#<%=txtCardNumber.ClientID %>').hide();
            $('#<%=lblCardCVV2.ClientID %>').show();
            $('#<%=txtCardCVV2.ClientID %>').hide();
            $('#<%=lblCardExpirationMonth.ClientID %>').show();
            $('#<%=txtCardExpirationMonth.ClientID %>').hide();
            $('#<%=lblCardExpirationYear.ClientID %>').show();
            $('#<%=txtCardExpirationYear.ClientID %>').hide();
            $('#<%=btnEditCC.ClientID %>').show();
            $('#<%=btnSaveCC.ClientID %>').hide();
            $('#<%=btnCancelCC.ClientID %>').hide();
        }
    }

    function toggleBillingAddress(editmode) {
        if (editmode) {
            $('#<%=lblBillingFirstName.ClientID %>').hide();
            $('#<%=lblBillingLastName.ClientID %>').hide();
            $('#<%=txtBillingFirstName.ClientID %>').show();
            $('#<%=txtBillingLastName.ClientID %>').show();
            $('#<%=lblBillingEmail.ClientID %>').hide();
            $('#<%=txtBillingEmail.ClientID %>').show();
            $('#<%=lblBillingPhoneNumber.ClientID %>').hide();
            $('#<%=txtBillingPhoneNumber.ClientID %>').show();
            $('#<%=lblBillingFaxNumber.ClientID %>').hide();
            $('#<%=txtBillingFaxNumber.ClientID %>').show();
            $('#<%=lblBillingFaxNumber.ClientID %>').hide();
            $('#<%=txtBillingFaxNumber.ClientID %>').show();
            $('#<%=lblBillingCompany.ClientID %>').hide();
            $('#<%=txtBillingCompany.ClientID %>').show();
            $('#<%=lblBillingAddress1.ClientID %>').hide();
            $('#<%=txtBillingAddress1.ClientID %>').show();
            $('#<%=lblBillingAddress2.ClientID %>').hide();
            $('#<%=txtBillingAddress2.ClientID %>').show();
            $('#<%=lblBillingCity.ClientID %>').hide();
            $('#<%=txtBillingCity.ClientID %>').show();
            $('#<%=lblBillingStateProvince.ClientID %>').hide();
            $('#<%=ddlBillingStateProvince.ClientID %>').show();
            $('#<%=lblBillingZipPostalCode.ClientID %>').hide();
            $('#<%=txtBillingZipPostalCode.ClientID %>').show();
            $('#<%=lblBillingCountry.ClientID %>').hide();
            $('#<%=ddlBillingCountry.ClientID %>').show();
            $('#<%=btnEditBillingAddress.ClientID %>').hide();
            $('#<%=btnSaveBillingAddress.ClientID %>').show();
            $('#<%=btnCancelBillingAddress.ClientID %>').show();
        }
        else {
            $('#<%=lblBillingFirstName.ClientID %>').show();
            $('#<%=lblBillingLastName.ClientID %>').show();
            $('#<%=txtBillingFirstName.ClientID %>').hide();
            $('#<%=txtBillingLastName.ClientID %>').hide();
            $('#<%=lblBillingEmail.ClientID %>').show();
            $('#<%=txtBillingEmail.ClientID %>').hide();
            $('#<%=lblBillingPhoneNumber.ClientID %>').show();
            $('#<%=txtBillingPhoneNumber.ClientID %>').hide();
            $('#<%=lblBillingFaxNumber.ClientID %>').show();
            $('#<%=txtBillingFaxNumber.ClientID %>').hide();
            $('#<%=lblBillingFaxNumber.ClientID %>').show();
            $('#<%=txtBillingFaxNumber.ClientID %>').hide();
            $('#<%=lblBillingCompany.ClientID %>').show();
            $('#<%=txtBillingCompany.ClientID %>').hide();
            $('#<%=lblBillingAddress1.ClientID %>').show();
            $('#<%=txtBillingAddress1.ClientID %>').hide();
            $('#<%=lblBillingAddress2.ClientID %>').show();
            $('#<%=txtBillingAddress2.ClientID %>').hide();
            $('#<%=lblBillingCity.ClientID %>').show();
            $('#<%=txtBillingCity.ClientID %>').hide();
            $('#<%=lblBillingStateProvince.ClientID %>').show();
            $('#<%=ddlBillingStateProvince.ClientID %>').hide();
            $('#<%=lblBillingZipPostalCode.ClientID %>').show();
            $('#<%=txtBillingZipPostalCode.ClientID %>').hide();
            $('#<%=lblBillingCountry.ClientID %>').show();
            $('#<%=ddlBillingCountry.ClientID %>').hide();
            $('#<%=btnEditBillingAddress.ClientID %>').show();
            $('#<%=btnSaveBillingAddress.ClientID %>').hide();
            $('#<%=btnCancelBillingAddress.ClientID %>').hide();
        }
    }

    function toggleShippingAddress(editmode) {
        if (editmode) {
            $('#<%=lblShippingFirstName.ClientID %>').hide();
            $('#<%=lblShippingLastName.ClientID %>').hide();
            $('#<%=txtShippingFirstName.ClientID %>').show();
            $('#<%=txtShippingLastName.ClientID %>').show();
            $('#<%=lblShippingEmail.ClientID %>').hide();
            $('#<%=txtShippingEmail.ClientID %>').show();
            $('#<%=lblShippingPhoneNumber.ClientID %>').hide();
            $('#<%=txtShippingPhoneNumber.ClientID %>').show();
            $('#<%=lblShippingFaxNumber.ClientID %>').hide();
            $('#<%=txtShippingFaxNumber.ClientID %>').show();
            $('#<%=lblShippingFaxNumber.ClientID %>').hide();
            $('#<%=txtShippingFaxNumber.ClientID %>').show();
            $('#<%=lblShippingCompany.ClientID %>').hide();
            $('#<%=txtShippingCompany.ClientID %>').show();
            $('#<%=lblShippingAddress1.ClientID %>').hide();
            $('#<%=txtShippingAddress1.ClientID %>').show();
            $('#<%=lblShippingAddress2.ClientID %>').hide();
            $('#<%=txtShippingAddress2.ClientID %>').show();
            $('#<%=lblShippingCity.ClientID %>').hide();
            $('#<%=txtShippingCity.ClientID %>').show();
            $('#<%=lblShippingStateProvince.ClientID %>').hide();
            $('#<%=ddlShippingStateProvince.ClientID %>').show();
            $('#<%=lblShippingZipPostalCode.ClientID %>').hide();
            $('#<%=txtShippingZipPostalCode.ClientID %>').show();
            $('#<%=lblShippingCountry.ClientID %>').hide();
            $('#<%=ddlShippingCountry.ClientID %>').show();
            $('#<%=btnEditShippingAddress.ClientID %>').hide();
            $('#<%=btnSaveShippingAddress.ClientID %>').show();
            $('#<%=btnCancelShippingAddress.ClientID %>').show();
        }
        else {
            $('#<%=lblShippingFirstName.ClientID %>').show();
            $('#<%=lblShippingLastName.ClientID %>').show();
            $('#<%=txtShippingFirstName.ClientID %>').hide();
            $('#<%=txtShippingLastName.ClientID %>').hide();
            $('#<%=lblShippingEmail.ClientID %>').show();
            $('#<%=txtShippingEmail.ClientID %>').hide();
            $('#<%=lblShippingPhoneNumber.ClientID %>').show();
            $('#<%=txtShippingPhoneNumber.ClientID %>').hide();
            $('#<%=lblShippingFaxNumber.ClientID %>').show();
            $('#<%=txtShippingFaxNumber.ClientID %>').hide();
            $('#<%=lblShippingFaxNumber.ClientID %>').show();
            $('#<%=txtShippingFaxNumber.ClientID %>').hide();
            $('#<%=lblShippingCompany.ClientID %>').show();
            $('#<%=txtShippingCompany.ClientID %>').hide();
            $('#<%=lblShippingAddress1.ClientID %>').show();
            $('#<%=txtShippingAddress1.ClientID %>').hide();
            $('#<%=lblShippingAddress2.ClientID %>').show();
            $('#<%=txtShippingAddress2.ClientID %>').hide();
            $('#<%=lblShippingCity.ClientID %>').show();
            $('#<%=txtShippingCity.ClientID %>').hide();
            $('#<%=lblShippingStateProvince.ClientID %>').show();
            $('#<%=ddlShippingStateProvince.ClientID %>').hide();
            $('#<%=lblShippingZipPostalCode.ClientID %>').show();
            $('#<%=txtShippingZipPostalCode.ClientID %>').hide();
            $('#<%=lblShippingCountry.ClientID %>').show();
            $('#<%=ddlShippingCountry.ClientID %>').hide();
            $('#<%=btnEditShippingAddress.ClientID %>').show();
            $('#<%=btnSaveShippingAddress.ClientID %>').hide();
            $('#<%=btnCancelShippingAddress.ClientID %>').hide();
        }
    }

</script>

<ajaxToolkit:TabContainer runat="server" ID="OrderTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlOrderInfo" HeaderText="<% $NopResources:Admin.OrderDetails.OrderInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <strong>
                            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderStatusTitle" Text="<% $NopResources:Admin.OrderDetails.OrderStatus %>"
                                ToolTip="<% $NopResources:Admin.OrderDetails.OrderStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </strong>
                    </td>
                    <td class="adminData">
                        <b>
                            <asp:Label ID="lblOrderStatus" runat="server"></asp:Label></b>&nbsp;
                        <asp:Button ID="CancelOrderButton" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.OrderDetails.CancelButton.Text %>"
                            OnClick="CancelOrderButton_Click" CausesValidation="false"></asp:Button>
                        <nopCommerce:ConfirmationBox runat="server" ID="cbCancel" TargetControlID="CancelOrderButton"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderIdTitle" Text="<% $NopResources:Admin.OrderDetails.OrderID %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.OrderID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderId" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderGuidTitle" Text="<% $NopResources:Admin.OrderDetails.OrderGUID %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.OrderGUID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderGuid" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr class="adminSeparator">
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.OrderDetails.Customer %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerIPTitle" Text="<% $NopResources:Admin.OrderDetails.CustomerIP %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CustomerIP.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        :
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCustomerIP" runat="server" />
                        <asp:Button runat="server" ID="btnBanByCustomerIP" CssClass="adminButton" CausesValidation="false"
                            Text="<% $NopResources:Admin.OrderDetails.BtnBanByCustomerIP.Text %>" ToolTip="<% $NopResources:Admin.OrderDetails.BtnBanByCustomerIP.Tooltip %>"
                            OnClick="BtnBanByCustomerIP_OnClick" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbBanByCustomerIP" TargetControlID="btnBanByCustomerIP"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                    </td>
                </tr>
                <tr runat="server" id="pnlVatNumber">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblVatNumberTitle" Text="<% $NopResources:Admin.OrderDetails.VatNumber %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.VatNumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblVatNumber" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlAffiliate">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAffiliateTitle" Text="<% $NopResources:Admin.OrderDetails.Affiliate %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.Affiliate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblAffiliate" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlOrderSubtotalInclTax">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderSubtotalInclTaxTitle" Text="<% $NopResources:Admin.OrderDetails.SubtotalInclTax %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.SubtotalInclTax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderSubtotalInclTax" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlOrderSubtotalExclTax">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderSubtotalExclTaxTitle" Text="<% $NopResources: Admin.OrderDetails.SubtotalExclTax%>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.SubtotalExclTax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderSubtotalExclTax" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlOrderShippingInclTax">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderShippingInclTaxTitle" Text="<% $NopResources:Admin.OrderDetails.ShippingInclTax %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.ShippingInclTax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderShippingInclTax" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlOrderShippingExclTax">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderShippingExclTaxTitle" Text="<% $NopResources:Admin.OrderDetails.ShippingExclTax %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.ShippingExclTax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderShippingExclTax" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlPaymentMethodAdditionalFeeInclTax">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentMethodAdditionalFeeInclTaxTitle"
                            Text="<% $NopResources:Admin.OrderDetails.PaymentMethodAdditionalFeeInclTax %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PaymentMethodAdditionalFeeInclTax.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblPaymentMethodAdditionalFeeInclTax" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlPaymentMethodAdditionalFeeExclTax">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentMethodAdditionalFeeExclTaxTitle"
                            Text="<% $NopResources:Admin.OrderDetails.PaymentMethodAdditionalFeeExclTax %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PaymentMethodAdditionalFeeExclTax.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblPaymentMethodAdditionalFeeExclTax" runat="server"></asp:Label>
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rptrTaxRates" OnItemDataBound="rptrTaxRates_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblTaxRateTitle" ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:Literal runat="server" ID="lTaxRateValue"></asp:Literal>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder runat="server" ID="phTaxTotal">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderTaxTitle" Text="<% $NopResources: Admin.OrderDetails.OrderTax%>"
                                ToolTip="<% $NopResources:Admin.OrderDetails.OrderTax.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:Label ID="lblOrderTax" runat="server"></asp:Label>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <tr runat="server" id="pnlDiscount">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderDiscountTitle" Text="<% $NopResources:Admin.OrderDetails.Discount %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.Discount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderDiscount" runat="server"></asp:Label>
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rptrGiftCards" OnItemDataBound="rptrGiftCards_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblOrderGiftCardTitle" Text="Gift card info:"
                                    ToolTip="<% $NopResources:Admin.OrderDetails.GiftCardInfo.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:Label ID="lblGiftCardAmount" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr runat="server" id="pnlRewardPoints">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRewardPointsTitle" Text="<% $NopResources:Admin.OrderDetails.RewardPoints %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.RewardPoints.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblRewardPointsAmount" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderTotalTitle" Text="<% $NopResources:Admin.OrderDetails.OrderTotal %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.OrderTotal.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderTotal" runat="server"></asp:Label>
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="phRefundedAmount">                
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRefundedAmountTitle" Text="<% $NopResources:Admin.OrderDetails.RefundedAmount %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.RefundedAmount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblRefundedAmount" runat="server"></asp:Label>
                    </td>
                </tr>
                </asp:PlaceHolder>
                <tr id="trEditOrderTotals">
                    <td colspan="2">
                        <table style="border: solid 1px black; padding: 5px;">
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderSubtotalInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.InclTax")%>
                                    <asp:TextBox ID="txtOrderSubtotalInPrimaryCurrencyInclTax" runat="server" CssClass="adminInput" />
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.ExclTax")%>
                                    <asp:TextBox ID="txtOrderSubtotalInPrimaryCurrencyExclTax" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderSubtotalInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.InclTax")%>
                                    <asp:TextBox ID="txtOrderSubtotalInCustomerCurrencyInclTax" runat="server" CssClass="adminInput" />
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.ExclTax")%>
                                    <asp:TextBox ID="txtOrderSubtotalInCustomerCurrencyExclTax" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderShippingInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.InclTax")%>
                                    <asp:TextBox ID="txtOrderShippingInPrimaryCurrencyInclTax" runat="server" CssClass="adminInput" />
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.ExclTax")%>
                                    <asp:TextBox ID="txtOrderShippingInPrimaryCurrencyExclTax" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderShippingInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.InclTax")%>
                                    <asp:TextBox ID="txtOrderShippingInCustomerCurrencyInclTax" runat="server" CssClass="adminInput" />
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.ExclTax")%>
                                    <asp:TextBox ID="txtOrderShippingInCustomerCurrencyExclTax" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderPaymentMethodAdditionalFeeInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.InclTax")%>
                                    <asp:TextBox ID="txtOrderPaymentMethodAdditionalFeeInPrimaryCurrencyInclTax" runat="server"
                                        CssClass="adminInput" />
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.ExclTax")%>
                                    <asp:TextBox ID="txtOrderPaymentMethodAdditionalFeeInPrimaryCurrencyExclTax" runat="server"
                                        CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderPaymentMethodAdditionalFeeInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.InclTax")%>
                                    <asp:TextBox ID="txtOrderPaymentMethodAdditionalFeeInCustomerCurrencyInclTax" runat="server"
                                        CssClass="adminInput" />
                                </td>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.EditOrderTotals.ExclTax")%>
                                    <asp:TextBox ID="txtOrderPaymentMethodAdditionalFeeInCustomerCurrencyExclTax" runat="server"
                                        CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderTaxRatesInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderTaxRatesInPrimaryCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderTaxRatesInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderTaxRatesInCustomerCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderTaxInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderTaxInPrimaryCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderTaxInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderTaxInCustomerCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderDiscountInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderDiscountInPrimaryCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderDiscountInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderDiscountInCustomerCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderTotalInPrimaryCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderTotalInPrimaryCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblOrderTotalInCustomerCurrencyTitle"></asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtOrderTotalInCustomerCurrency" runat="server" CssClass="adminInput" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <asp:Button ID="btnEditOrderTotals" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.EditOrderTotals.Text %>">
                        </asp:Button>
                        <asp:Button ID="btnSaveOrderTotals" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SaveOrderTotals.Text %>"
                            OnClick="btnSaveOrderTotals_Click"></asp:Button>
                    </td>
                    <td class="adminData">
                        <asp:Button ID="btnCancelOrderTotals" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.CancelOrderTotals.Text %>">
                        </asp:Button>
                    </td>
                </tr>
                <tr class="adminSeparator">
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>
                <tr runat="server" id="pnlCartType">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCartTypeTitle" Text="<% $NopResources:Admin.OrderDetails.CartType %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CartType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCardType" runat="server"></asp:Label>
                        <asp:TextBox runat="server" ID="txtCardType" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlCardName">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCardNameTitle" Text="<% $NopResources:Admin.OrderDetails.CardName %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CardName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCardName" runat="server"></asp:Label>
                        <asp:TextBox runat="server" ID="txtCardName" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlCardNumber">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCardNumberTitle" Text="<% $NopResources:Admin.OrderDetails.CardNumber %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CardNumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCardNumber" runat="server"></asp:Label>
                        <asp:TextBox runat="server" ID="txtCardNumber" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlCardCVV2">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCardCVV2Title" Text="<% $NopResources:Admin.OrderDetails.CardCVV2 %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CardCVV2.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCardCVV2" runat="server"></asp:Label>
                        <asp:TextBox runat="server" ID="txtCardCVV2" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlCardExpiryMonth">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCardExpiryMonthTitle" Text="<% $NopResources:Admin.OrderDetails.CardExpiryMonth %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CardExpiryMonth.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCardExpirationMonth" runat="server"></asp:Label>
                        <asp:TextBox runat="server" ID="txtCardExpirationMonth" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlCardExpiryYear">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCardExpiryYear" Text="<% $NopResources:Admin.OrderDetails.CardExpiryYear %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CardExpiryYear.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCardExpirationYear" runat="server"></asp:Label>
                        <asp:TextBox runat="server" ID="txtCardExpirationYear" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlEditCC">
                    <td>
                        <asp:Button ID="btnEditCC" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.EditCCButton.Text %>">
                        </asp:Button>
                        <asp:Button ID="btnSaveCC" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SaveCCButton.Text %>"
                            OnClick="btnSaveCC_Click"></asp:Button>
                    </td>
                    <td>
                        <asp:Button ID="btnCancelCC" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.CancelCCButton.Text %>">
                        </asp:Button>
                    </td>
                </tr>
                <tr runat="server" id="pnlPONumber">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPONumberTitle" Text="<% $NopResources:Admin.OrderDetails.PONumber %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PONumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblPONumber" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlAuthorizationTransactionID">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAuthorizationTransactionIDTitle" Text="<% $NopResources:Admin.OrderDetails.AuthorizationTransactionID %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.AuthorizationTransactionID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblAuthorizationTransactionID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlCaptureTransactionID">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCaptureTransactionIDTitle" Text="<% $NopResources:Admin.OrderDetails.CaptureTransactionID %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CaptureTransactionID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCaptureTransactionID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="pnlSubscriptionTransactionID">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblSubscriptionTransactionIDTitle" Text="<% $NopResources:Admin.OrderDetails.SubscriptionTransactionID %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.SubscriptionTransactionID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblSubscriptionTransactionID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentMethodTitle" Text="<% $NopResources:Admin.OrderDetails.PaymentMethod %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PaymentMethod.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblPaymentMethodName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentStatusTitle" Text="<% $NopResources:Admin.OrderDetails.PaymentStatus %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PaymentStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label runat="server" ID="lblPaymentStatus"></asp:Label>&nbsp;
                        <asp:Button ID="btnCapture" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.CaptureButton.Text %>"
                            OnClick="btnCapture_Click" ToolTip="<% $NopResources:Admin.OrderDetails.CaptureButton.Tooltip %>" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbCapture" TargetControlID="btnCapture"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                        &nbsp;
                        <asp:Button ID="btnMarkAsPaid" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.MarkAsPaidButton.Text %>"
                            OnClick="btnMarkAsPaid_Click" ToolTip="<% $NopResources:Admin.OrderDetails.MarkAsPaidButton.Tooltip %>" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbMarkAsPaid" TargetControlID="btnMarkAsPaid"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                        &nbsp;
                        <asp:Button ID="btnRefund" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.RefundButton.Text %>"
                            OnClick="btnRefund_Click" ToolTip="<% $NopResources:Admin.OrderDetails.RefundButton.Tooltip %>" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbRefund" TargetControlID="btnRefund"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                        &nbsp;
                        <asp:Button ID="btnRefundOffline" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.RefundOfflineButton.Text %>"
                            OnClick="btnRefundOffline_Click" ToolTip="<% $NopResources:Admin.OrderDetails.RefundOfflineButton.Tooltip %>" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbRefundOffline" TargetControlID="btnRefundOffline"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                        <asp:Button ID="btnPartialRefund" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.PartialRefundButton.Text %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PartialRefundButton.Tooltip %>" />
                        &nbsp;
                        <asp:Button ID="btnPartialRefundOffline" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.PartialRefundOfflineButton.Text %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.PartialRefundOfflineButton.Tooltip %>" />                        
                        &nbsp;
                        <asp:Button ID="btnVoid" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.VoidButton.Text %>"
                            OnClick="btnVoid_Click" ToolTip="<% $NopResources:Admin.OrderDetails.VoidButton.Tooltip %>" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbVoid" TargetControlID="btnVoid"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                        &nbsp;
                        <asp:Button ID="btnVoidOffline" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.VoidOfflineButton.Text %>"
                            OnClick="btnVoidOffline_Click" ToolTip="<% $NopResources:Admin.OrderDetails.VoidOfflineButton.Tooltip %>" />
                        <nopCommerce:ConfirmationBox runat="server" ID="cbVoidOffline" TargetControlID="btnVoidOffline"
                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                        <div style="color: red">
                            <b>
                                <asp:Label runat="server" ID="lblChangePaymentStatusError" EnableViewState="false"></asp:Label>
                            </b>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderCreatedOnTitle" Text="<% $NopResources:Admin.OrderDetails.CreatedOn %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.CreatedOn.Tooltip%>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCreatedOn" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlOrderBillingInfo" HeaderText="<% $NopResources:Admin.OrderDetails.BillingInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblBillingAddress" Text="<% $NopResources:Admin.OrderDetails.BillingAddress %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.BillingAddress.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <table style="border: solid 1px black; padding: 5px;">
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.FullName")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingFirstName" runat="server" />
                                    <asp:Label ID="lblBillingLastName" runat="server" />
                                    <asp:TextBox ID="txtBillingFirstName" runat="server" CssClass="adminInput"></asp:TextBox>
                                    <asp:TextBox ID="txtBillingLastName" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Email")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingEmail" runat="server" />
                                    <asp:TextBox runat="server" ID="txtBillingEmail" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Phone")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingPhoneNumber" runat="server" />
                                    <asp:TextBox ID="txtBillingPhoneNumber" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Fax")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingFaxNumber" runat="server" />
                                    <asp:TextBox ID="txtBillingFaxNumber" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Company")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingCompany" runat="server" />
                                    <asp:TextBox ID="txtBillingCompany" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Address1")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingAddress1" runat="server" />
                                    <asp:TextBox ID="txtBillingAddress1" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Address2")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingAddress2" runat="server" />
                                    <asp:TextBox ID="txtBillingAddress2" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.City")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingCity" runat="server" />
                                    <asp:TextBox ID="txtBillingCity" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.StateProvince")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingStateProvince" runat="server" />
                                    <asp:UpdatePanel ID="upEditBilling" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="ddlBillingStateProvince" AutoPostBack="False" runat="server"
                                                CssClass="adminInput">
                                            </asp:DropDownList>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlBillingCountry" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.ZipPostalCode")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingZipPostalCode" runat="server" />
                                    <asp:TextBox ID="txtBillingZipPostalCode" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.BillingAddress.Country")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblBillingCountry" runat="server" />
                                    <asp:DropDownList ID="ddlBillingCountry" AutoPostBack="True" runat="server" CssClass="adminInput"
                                        OnSelectedIndexChanged="ddlBillingCountry_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnEditBillingAddress" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.EditBillingAddressButton.Text %>">
                                    </asp:Button>
                                    <asp:Button ID="btnSaveBillingAddress" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SaveBillingAddressButton.Text %>"
                                        OnClick="btnSaveBillingAddress_Click"></asp:Button>
                                </td>
                                <td>
                                    <asp:Button ID="btnCancelBillingAddress" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.CancelBillingAddressButton.Text %>">
                                    </asp:Button>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upEditBilling">
                <ProgressTemplate>
                    <div class="progress">
                        <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                            AlternateText="update" />
                        <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlOrderShippingInfo" HeaderText="<% $NopResources:Admin.OrderDetails.ShippingInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr runat="server" id="divShippingNotRequired" visible="false">
                    <td class="adminTitle">
                    </td>
                    <td class="adminData">
                        <%=GetLocaleResourceString("Admin.OrderDetails.ShippingNotRequired")%>
                    </td>
                </tr>
                <tr runat="server" id="divShippingAddress">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShippingAddress" Text="<% $NopResources:Admin.OrderDetails.ShippingAddress %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.ShippingAddress.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <table style="border: solid 1px black; padding: 5px;">
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.FullName")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingFirstName" runat="server" />
                                    <asp:Label ID="lblShippingLastName" runat="server" />
                                    <asp:TextBox ID="txtShippingFirstName" runat="server" CssClass="adminInput"></asp:TextBox>
                                    <asp:TextBox ID="txtShippingLastName" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Email")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingEmail" runat="server" />
                                    <asp:TextBox runat="server" ID="txtShippingEmail" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Phone")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingPhoneNumber" runat="server" />
                                    <asp:TextBox ID="txtShippingPhoneNumber" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Fax")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingFaxNumber" runat="server" />
                                    <asp:TextBox ID="txtShippingFaxNumber" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Company")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingCompany" runat="server" />
                                    <asp:TextBox ID="txtShippingCompany" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Address1")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingAddress1" runat="server" />
                                    <asp:TextBox ID="txtShippingAddress1" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Address2")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingAddress2" runat="server" />
                                    <asp:TextBox ID="txtShippingAddress2" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.City")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingCity" runat="server" />
                                    <asp:TextBox ID="txtShippingCity" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.StateProvince")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingStateProvince" runat="server" />
                                    <asp:UpdatePanel ID="upEditShipping" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="ddlShippingStateProvince" AutoPostBack="False" runat="server"
                                                CssClass="adminInput">
                                            </asp:DropDownList>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlShippingCountry" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.ZipPostalCode")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingZipPostalCode" runat="server" />
                                    <asp:TextBox ID="txtShippingZipPostalCode" runat="server" CssClass="adminInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%=GetLocaleResourceString("Admin.OrderDetails.ShippingAddress.Country")%>
                                </td>
                                <td>
                                    <asp:Label ID="lblShippingCountry" runat="server" />
                                    <asp:DropDownList ID="ddlShippingCountry" AutoPostBack="True" runat="server" CssClass="adminInput"
                                        OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Button ID="btnEditShippingAddress" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.EditShippingAddressButton.Text %>">
                                    </asp:Button>
                                    <asp:Button ID="btnSaveShippingAddress" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SaveShippingAddressButton.Text %>"
                                        OnClick="btnSaveShippingAddress_Click"></asp:Button>
                                </td>
                                <td>
                                    <asp:Button ID="btnCancelShippingAddress" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.CancelShippingAddressButton.Text %>">
                                    </asp:Button>
                                </td>
                            </tr>
                        </table>
                        <asp:UpdateProgress ID="up2" runat="server" AssociatedUpdatePanelID="upEditShipping">
                            <ProgressTemplate>
                                <div class="progress">
                                    <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                                        AlternateText="update" />
                                    <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <div style="padding-top: 15px;">
                            <img alt="google maps" src="<%=CommonHelper.GetStoreLocation()%>images/GoogleMaps.gif" />
                            <asp:HyperLink ID="hlShippingAddressGoogle" runat="server" Text="<% $NopResources:Admin.OrderDetails.ShippingAddress.Google %>" Target="_blank"></asp:HyperLink>
                        </div>
                    </td>
                </tr>
                <tr runat="server" id="divShippingWeight">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderWeightTitle" Text="<% $NopResources:Admin.OrderDetails.OrderWeight %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.OrderWeight.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrderWeight" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="divShippingMethod">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShippingMethodTitle" Text="<% $NopResources:Admin.OrderDetails.ShippingMethod %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.ShippingMethod.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblShippingMethod" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="divTrackingNumber">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblTrackingNumber" Text="<% $NopResources:Admin.OrderDetails.TrackingNumber %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.TrackingNumber.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox ID="txtTrackingNumber" runat="server"></asp:TextBox>
                        <asp:Button ID="btnSetTrackingNumber" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SetTrackingNumberButton.Text %>"
                            OnClick="btnSetTrackingNumber_Click"></asp:Button>
                    </td>
                </tr>
                <tr runat="server" id="divShippedDate">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShippedDateTitle" Text="<% $NopResources:Admin.OrderDetails.ShippedDate %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.ShippedDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblShippedDate" runat="server"></asp:Label>
                        <asp:Button ID="btnSetAsShipped" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SetAsShippedButton.Text %>"
                            OnClick="btnSetAsShipped_Click"></asp:Button><nopCommerce:ConfirmationBox runat="server"
                                ID="cbSetAsShipped" TargetControlID="btnSetAsShipped" YesText="<% $NopResources:Admin.Common.Yes %>"
                                NoText="<% $NopResources:Admin.Common.No %>" ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                    </td>
                </tr>
                <tr runat="server" id="divDeliveryDate">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDeliveryDateTitle" Text="<% $NopResources:Admin.OrderDetails.DeliveryDate %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.DeliveryDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblDeliveryDate" runat="server"></asp:Label>
                        <asp:Button ID="btnSetAsDelivered" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.SetAsDeliveredButton.Text %>"
                            OnClick="btnSetAsDelivered_Click"></asp:Button><nopCommerce:ConfirmationBox runat="server"
                                ID="cbSetAsDelivered" TargetControlID="btnSetAsDelivered" YesText="<% $NopResources:Admin.Common.Yes %>"
                                NoText="<% $NopResources:Admin.Common.No %>" ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlOrderProducts" HeaderText="<% $NopResources:Admin.OrderDetails.Products %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminData">
                        <asp:GridView ID="gvOrderProductVariants" runat="server" AutoGenerateColumns="False"
                            Width="100%" OnRowDataBound="gvOrderProductVariants_RowDataBound" OnRowCommand="gvOrderProductVariants_RowCommand">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.Name %>"
                                    ItemStyle-Width="25%">
                                    <ItemTemplate>
                                        <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                                            <em><a href='<%#GetProductUrl(Convert.ToInt32(Eval("ProductVariantId")))%>' title="<%#GetLocaleResourceString("Admin.OrderDetails.Products.Name.Tooltip")%>">
                                                <%#Server.HtmlEncode(GetProductVariantName(Convert.ToInt32(Eval("ProductVariantId"))))%></a></em>
                                            <%#GetAttributeDescription((OrderProductVariant)Container.DataItem)%>
                                            <%#GetRecurringDescription((OrderProductVariant)Container.DataItem)%>
                                            <%#GetReturnRequests((OrderProductVariant)Container.DataItem)%>
                                            <asp:HiddenField ID="hfOrderProductVariantId" runat="server" Value='<%# Eval("OrderProductVariantId") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.DownloadColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#GetDownloadUrl(Container.DataItem as OrderProductVariant)%>
                                        <asp:Panel ID="pnlDownloadActivation" runat="server">
                                            <hr />
                                            <asp:Button ID="btnActivate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.OrderDetails.Products.ActivateDownload %>"
                                                CausesValidation="false" CommandName="DownloadActivation" />
                                        </asp:Panel>
                                        <asp:Panel ID="pnlLicenseDownload" runat="server">
                                            <hr />
                                            <b>
                                                <%=GetLocaleResourceString("Admin.OrderDetails.Products.License")%></b>
                                            <br />
                                            <asp:HyperLink ID="hlLicenseDownload" runat="server" Text="<% $NopResources:Admin.OrderDetails.Products.License.Download %>" />
                                            <asp:Button ID="btnRemoveLicenseDownload" CssClass="adminButton" CausesValidation="false"
                                                runat="server" Text="<% $NopResources:Admin.OrderDetails.Products.License.Remove %>"
                                                CommandName="RemoveLicenseDownload" />
                                            <br />
                                            <asp:FileUpload class="text" ID="fuLicenseDownload" CssClass="adminInput" runat="server" />
                                            <asp:Button ID="btnUploadLicenseDownload" CssClass="adminButton" CausesValidation="false"
                                                runat="server" Text="<% $NopResources:Admin.OrderDetails.Products.License.UploadButton %>"
                                                CommandName="UploadLicenseDownload" />
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.Price %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#GetProductVariantUnitPrice((OrderProductVariant)Container.DataItem)%>
                                        <div class="clear">
                                        </div>
                                        <asp:Panel runat="server" ID="pnlEditPvUnitPrice">
                                            <table class="order-edit">
                                                <tr>
                                                    <td colspan="2">
                                                        <%= string.Format(GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PrimaryStoreCurrency"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode)%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.UnitPriceInclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvUnitPriceInclTax" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("UnitPriceInclTax") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.UnitPriceExclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvUnitPriceExclTax" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("UnitPriceExclTax") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table class="order-edit">
                                                <tr>
                                                    <td colspan="2">
                                                        <%# string.Format(GetLocaleResourceString("Admin.OrderDetails.Products.Edit.CustomerCurrency"), ((Order)Eval("Order")).CustomerCurrencyCode)%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.UnitPriceInclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvUnitPriceCustCurrencyInclTax" runat="server" CssClass="adminInput"
                                                            Width="100px" Text='<%# Eval("UnitPriceInclTaxInCustomerCurrency") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.UnitPriceExclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvUnitPriceCustCurrencyExclTax" runat="server" CssClass="adminInput"
                                                            Width="100px" Text='<%# Eval("UnitPriceExclTaxInCustomerCurrency") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.Quantity %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Eval("Quantity").ToString()%>
                                        <div class="clear">
                                        </div>
                                        <asp:Panel runat="server" ID="pnlEditPvQuantity">
                                            <table class="order-edit">
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:TextBox ID="txtPvQuantity" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("Quantity") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.Discount %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#GetOrderProductVariantDiscountAmount(Container.DataItem as OrderProductVariant)%>
                                        <div class="clear">
                                        </div>
                                        <asp:Panel runat="server" ID="pnlEditPvDiscount">
                                            <table class="order-edit">
                                                <tr>
                                                    <td colspan="2">
                                                        <%= string.Format(GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PrimaryStoreCurrency"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode)%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.DiscountInclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvDiscountInclTax" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("DiscountAmountInclTax") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.DiscountExclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvDiscountExclTax" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("DiscountAmountExclTax") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.Total %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#GetOrderProductVariantSubTotal(Container.DataItem as OrderProductVariant)%>
                                        <div class="clear">
                                        </div>
                                        <asp:Panel runat="server" ID="pnlEditPvPrice">
                                            <table class="order-edit">
                                                <tr>
                                                    <td colspan="2">
                                                        <%= string.Format(GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PrimaryStoreCurrency"), CurrencyManager.PrimaryStoreCurrency.CurrencyCode)%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PriceInclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvPriceInclTax" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("PriceInclTax") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PriceExclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvPriceExclTax" runat="server" CssClass="adminInput" Width="100px"
                                                            Text='<%# Eval("PriceExclTax") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table class="order-edit">
                                                <tr>
                                                    <td colspan="2">
                                                        <%# string.Format(GetLocaleResourceString("Admin.OrderDetails.Products.Edit.CustomerCurrency"), ((Order)Eval("Order")).CustomerCurrencyCode)%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PriceInclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvPriceCustCurrencyInclTax" runat="server" CssClass="adminInput"
                                                            Width="100px" Text='<%# Eval("PriceInclTaxInCustomerCurrency") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%=GetLocaleResourceString("Admin.OrderDetails.Products.Edit.PriceExclTax")%>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPvPriceCustCurrencyExclTax" runat="server" CssClass="adminInput"
                                                            Width="100px" Text='<%# Eval("PriceExclTaxInCustomerCurrency") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.Products.Edit %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditOpv" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.Products.Edit.EditButton.Text %>" />
                                        <asp:Button ID="btnDeleteOpv" CssClass="adminButton" runat="server" CommandName="DeleteOpv"
                                            Text="<% $NopResources:Admin.OrderDetails.Products.Edit.DeleteButton.Text %>" />
                                        <nopCommerce:ConfirmationBox runat="server" ID="cbDeleteOpv" TargetControlID="btnDeleteOpv"
                                            YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
                                            ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                                        <asp:Button ID="btnSaveOpv" CssClass="adminButton" runat="server" CommandName="EditOpv"
                                            Text="<% $NopResources:Admin.OrderDetails.Products.Edit.SaveButton.Text %>" />
                                        <asp:Button ID="btnCancelOpv" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.Products.Edit.Cancelutton.Text %>" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="adminData">
                        <asp:Literal runat="server" ID="lCheckoutAttributes"></asp:Literal>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlOrderNotes" HeaderText="<% $NopResources:Admin.OrderDetails.OrderNotes %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminData" colspan="2">
                        <asp:GridView ID="gvOrderNotes" runat="server" DataKeyNames="OrderNoteId" AutoGenerateColumns="False"
                            Width="100%" OnRowDeleting="gvOrderNotes_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.OrderNotes.CreatedOn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.OrderNotes.Note %>"
                                    ItemStyle-Width="60%">
                                    <ItemTemplate>
                                        <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                                            <%#OrderManager.FormatOrderNoteText((string)Eval("Note"))%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.OrderNotes.DisplayToCustomerColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <nopCommerce:ImageCheckBox runat="server" ID="cbDisplayToCustomer" Checked='<%# Eval("DisplayToCustomer") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderDetails.OrderNotes.Delete %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnDelete" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.OrderDetails.OrderNotes.Delete %>"
                                            CausesValidation="false" CommandName="Delete" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="adminData" colspan="2">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblNewOrderNote" Text="<% $NopResources:Admin.OrderDetails.OrderNotes.New.Note %>"
                            ToolTip="<% $NopResources:Admin.OrderDetails.OrderNotes.New.Note.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtNewOrderNote" TextMode="MultiLine" Height="150px"
                            Width="500px"></asp:TextBox>
                        <br />
                        <asp:CheckBox runat="server" ID="cbNewDisplayToCustomer" Text="<% $NopResources:Admin.OrderDetails.OrderNotes.New.DisplayToCustomer.Text %>" />
                        <br />
                        <asp:Button ID="btnAddNewOrderNote" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.OrderDetails.OrderNotes.New.AddNewButton.Text %>"
                            OnClick="btnAddNewOrderNote_Click" ToolTip="<% $NopResources:Admin.OrderDetails.OrderNotes.New.AddNewButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
