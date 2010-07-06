<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CheckoutOnePageControl"
    CodeBehind="CheckoutOnePage.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutShippingAddress" Src="~/Modules/CheckoutShippingAddress.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutBillingAddress" Src="~/Modules/CheckoutBillingAddress.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutShippingMethod" Src="~/Modules/CheckoutShippingMethod.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutPaymentMethod" Src="~/Modules/CheckoutPaymentMethod.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutPaymentInfo" Src="~/Modules/CheckoutPaymentInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutConfirm" Src="~/Modules/CheckoutConfirm.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="OrderSummary" Src="~/Modules/OrderSummary.ascx" %>
<div class="checkout-one-page">
    <div class="page-title">
        <h1>
            <%=GetLocaleResourceString("CheckoutOnePage.Title")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="checkout-data">
        <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true" ID="sm1" ScriptMode="Release" CompositeScript-ScriptMode="Release"
            CombineScripts="false" />
        <%--<asp:UpdatePanel ID="upCheckout" runat="server">
            <ContentTemplate>--%>
        <!-- ShippingAddress -->
                <div runat="server" id="pnlShippingAddress" class="checkoutstep">
                    <div class="steptitle">
                        <%=GetLocaleResourceString("CheckoutOnePage.ShippingAddress.Title")%>
                        <div style="float: right;">
                            <asp:Button runat="server" ID="btnModifyShippingAddress" Text="<% $NopResources:CheckoutOnePage.ShippingAddress.Modify %>"
                                OnClick="BtnModifyShippingAddress_OnClick" />
                            <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpeShippingAddress" TargetControlID="pnlShippingAddressContent" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlShippingAddressContent" class="stepcontent">
                        <nopCommerce:CheckoutShippingAddress ID="ctrlCheckoutShippingAddress" runat="server"
                            OnePageCheckout="true" OnCheckoutStepChanged="ctrlCheckoutShippingAddress_CheckoutStepChanged" />
                    </asp:Panel>
                </div>
                <!-- BillingAddress -->
                <div runat="server" id="pnlBillingAddress" class="checkoutstep">
                    <div class="steptitle">
                        <%=GetLocaleResourceString("CheckoutOnePage.BillingAddress.Title")%>
                        <div style="float: right;">
                            <asp:Button runat="server" ID="btnModifyBillingAddress" Text="<% $NopResources:CheckoutOnePage.BillingAddress.Modify %>"
                                OnClick="BtnModifyBillingAddress_OnClick" />
                            <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpeBillingAddress" TargetControlID="pnlBillingAddressContent" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlBillingAddressContent" class="stepcontent">
                        <nopCommerce:CheckoutBillingAddress ID="ctrlCheckoutBillingAddress" runat="server"
                            OnePageCheckout="true" OnCheckoutStepChanged="ctrlCheckoutBillingAddress_CheckoutStepChanged" />
                    </asp:Panel>
                </div>
                <!-- ShippingMethods -->
                <div runat="server" id="pnlShippingMethods" class="checkoutstep">
                    <div class="steptitle">
                        <%=GetLocaleResourceString("CheckoutOnePage.ShippingMethods.Title")%>
                        <div style="float: right;">
                            <asp:Button runat="server" ID="btnModifyShippingMethod" Text="<% $NopResources:CheckoutOnePage.ShippingMethods.Modify %>"
                                OnClick="BtnModifyShippingMethod_OnClick" />
                            <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpeShippingMethods" TargetControlID="pnlShippingMethodsContent" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlShippingMethodsContent" class="stepcontent">
                        <nopCommerce:CheckoutShippingMethod ID="ctrlCheckoutShippingMethod" runat="server"
                            OnePageCheckout="true" OnCheckoutStepChanged="ctrlCheckoutShippingMethod_CheckoutStepChanged" />
                    </asp:Panel>
                </div>
                <!-- PaymentMethods -->
                <div runat="server" id="pnlPaymentMethods" class="checkoutstep">
                    <div class="steptitle">
                        <%=GetLocaleResourceString("CheckoutOnePage.PaymentMethods.Title")%>
                        <div style="float: right;">
                            <asp:Button runat="server" ID="btnModifyPaymentMethod" Text="<% $NopResources:CheckoutOnePage.PaymentMethods.Modify %>"
                                OnClick="BtnModifyPaymentMethod_OnClick" />
                            <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpePaymentMethods" TargetControlID="pnlPaymentMethodsContent" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlPaymentMethodsContent" class="stepcontent">
                        <nopCommerce:CheckoutPaymentMethod ID="ctrlCheckoutPaymentMethod" runat="server"
                            OnePageCheckout="true" OnCheckoutStepChanged="ctrlCheckoutPaymentMethod_CheckoutStepChanged" />
                    </asp:Panel>
                </div>
                <!-- PaymentInfo -->
                <div runat="server" id="pnlPaymentInfo" class="checkoutstep">
                    <div class="steptitle">
                        <%=GetLocaleResourceString("CheckoutOnePage.PaymentInfo.Title")%>
                        <div style="float: right;">
                            <asp:Button runat="server" ID="btnModifyPaymentInfo" Text="<% $NopResources:CheckoutOnePage.PaymentInfo.Modify %>"
                                OnClick="BtnModifyPaymentInfo_OnClick" />
                            <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpePaymentInfo" TargetControlID="pnlPaymentInfoContent" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlPaymentInfoContent" class="stepcontent">
                        <nopCommerce:CheckoutPaymentInfo ID="ctrlCheckoutPaymentInfo" runat="server" OnePageCheckout="true"
                            OnCheckoutStepChanged="ctrlCheckoutPaymentInfo_CheckoutStepChanged" />
                    </asp:Panel>
                </div>
                <!-- Confirm -->
                <div runat="server" id="pnlConfirm" class="checkoutstep">
                    <div class="steptitle">
                        <%=GetLocaleResourceString("CheckoutOnePage.Confirm.Title")%>
                        <div style="float: right;">
                            <%--<asp:Button runat="server" ID="btnModifyConfirm" Text="<% $NopResources:CheckoutOnePage.Confirm.Modify %>" OnClick="BtnModifyConfirm_OnClick" />--%>
                            <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpeConfirm" TargetControlID="pnlConfirmContent" />
                        </div>
                    </div>
                    <asp:Panel runat="server" ID="pnlConfirmContent" class="stepcontent">
                        <nopCommerce:CheckoutConfirm ID="ctrlCheckoutConfirm" runat="server" OnePageCheckout="true"
                            OnCheckoutStepChanged="ctrlCheckoutConfirm_CheckoutStepChanged" />
                        <div class="clear">
                        </div>
                        <div class="order-summary-title">
                            <%=GetLocaleResourceString("Checkout.OrderSummary")%>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="order-summary-body">
                            <nopCommerce:OrderSummary ID="OrderSummaryControl" runat="server" IsShoppingCart="false" />
                        </div>
                    </asp:Panel>
                </div>
            <%--</ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="uProgressCheckout" runat="server" AssociatedUpdatePanelID="upCheckout">
            <ProgressTemplate>
                <div class="progress">
                    <asp:Image ID="imgUpd" runat="server" ImageUrl="~/images/UpdateProgress.gif" AlternateText="<% $NopResources:Admin.Common.Wait... %>" />
                    <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>--%>
    </div>
</div>
