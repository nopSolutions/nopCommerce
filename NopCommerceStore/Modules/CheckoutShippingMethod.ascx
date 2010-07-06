<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CheckoutShippingMethodControl"
    CodeBehind="CheckoutShippingMethod.ascx.cs" %>
<div class="checkout-data">
    <div class="shipping-options">
        <asp:Panel runat="server" ID="phSelectShippingMethod">
            <asp:DataList runat="server" ID="dlShippingOptions">
                <ItemTemplate>
                    <div class="shipping-option-item">
                        <div class="option-name">
                            <nopCommerce:GlobalRadioButton runat="server" ID="rdShippingOption" Checked="false"
                                GroupName="shippingOptionGroup" />
                            <%#Server.HtmlEncode(Eval("Name").ToString()) %>
                            <%#Server.HtmlEncode(FormatShippingOption(((ShippingOption)Container.DataItem)))%>
                            <asp:HiddenField ID="hfShippingRateComputationMethodId" runat="server" Value='<%# Eval("ShippingRateComputationMethodId") %>' />
                            <asp:HiddenField ID="hfName" runat="server" Value='<%# Eval("Name") %>' />
                        </div>
                        <div class="option-description">
                            <%#Eval("Description") %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:DataList>
            <div class="clear">
            </div>
            <div class="select-button">
                <asp:Button runat="server" ID="btnNextStep" Text="<% $NopResources:Checkout.NextButton %>"
                    OnClick="btnNextStep_Click" CssClass="shippingmethodnextstepbutton" ValidationGroup="SelectShippingMethod" />
            </div>
        </asp:Panel>
        <div class="clear">
        </div>
        <div class="error-block">
            <div class="message-error">
                <asp:Literal runat="server" ID="lShippingMethodsError" EnableViewState="false"></asp:Literal>
            </div>
        </div>
    </div>
</div>
