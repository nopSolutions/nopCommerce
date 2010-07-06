<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CheckoutBillingAddressControl"
    CodeBehind="CheckoutBillingAddress.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressEdit" Src="~/Modules/AddressEdit.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressDisplay" Src="~/Modules/AddressDisplay.ascx" %>
<div class="checkout-data">
    <asp:Panel runat="server" ID="pnlSelectBillingAddress">
        <div class="select-address-title">
            <%=GetLocaleResourceString("Checkout.SelectBillingAddress")%>
        </div>
        <div class="clear">
        </div>
        <div class="address-grid">
            <asp:DataList ID="dlBillingAddresses" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                RepeatLayout="Table" ItemStyle-CssClass="item-box">
                <ItemTemplate>
                    <div class="address-item">
                        <div class="select-button">
                            <asp:Button runat="server" CommandName="Select" ID="btnSelect" Text='<%#GetLocaleResourceString("Checkout.BillingToThisAddress")%>'
                                OnCommand="btnSelect_Command" ValidationGroup="SelectBillingAddress" CommandArgument='<%# Eval("AddressId") %>'
                                CssClass="selectbillingaddressbutton" />
                        </div>
                        <div class="address-box">
                            <nopCommerce:AddressDisplay ID="adAddress" runat="server" Address='<%# Container.DataItem %>'
                                ShowDeleteButton="false" ShowEditButton="false"></nopCommerce:AddressDisplay>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>
    </asp:Panel>
    <div class="clear">
    </div>
    <div class="enter-address-title">
        <asp:Label runat="server" ID="lEnterBillingAddress"></asp:Label></div>
    <div class="clear">
    </div>
    <div class="enter-address">
        <div runat="server" id="pnlTheSameAsShippingAddress" class="the-same-address">
            <asp:Button runat="server" ID="btnTheSameAsShippingAddress" Text="<% $NopResources:Checkout.BillingAddressTheSameAsShippingAddress %>"
                CausesValidation="false" OnClick="btnTheSameAsShippingAddress_Click" CssClass="sameasshippingaddressbutton" />
        </div>
        <div class="enter-address-body">
            <nopCommerce:AddressEdit ID="ctrlBillingAddress" runat="server" IsNew="true" IsBillingAddress="true"
                ValidationGroup="EnterBillingAddress" />
        </div>
        <div class="clear">
        </div>
        <div class="button">
            <asp:Button runat="server" ID="btnNextStep" Text="<% $NopResources:Checkout.NextButton %>"
                OnClick="btnNextStep_Click" CssClass="newaddressnextstepbutton" ValidationGroup="EnterBillingAddress" />
        </div>
    </div>
</div>
