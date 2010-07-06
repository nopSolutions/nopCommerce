<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CheckoutShippingAddressControl"
    CodeBehind="CheckoutShippingAddress.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressEdit" Src="~/Modules/AddressEdit.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressDisplay" Src="~/Modules/AddressDisplay.ascx" %>
<div class="checkout-data">
    <asp:Panel runat="server" ID="pnlSelectShippingAddress">
        <div class="select-address-title">
            <%=GetLocaleResourceString("Checkout.SelectShippingAddress")%>
        </div>
        <div class="clear">
        </div>
        <div class="address-grid">
            <asp:DataList ID="dlShippingAddresses" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                RepeatLayout="Table" ItemStyle-CssClass="item-box">
                <ItemTemplate>
                    <div class="address-item">
                        <div class="select-button">
                            <asp:Button runat="server" CommandName="Select" ID="btnSelect" Text='<%#GetLocaleResourceString("Checkout.ShipToThisAddress")%>'
                                OnCommand="btnSelect_Command" ValidationGroup="SelectShippingAddress" CommandArgument='<%# Eval("AddressId") %>'
                                CssClass="selectshippingaddressbutton" />
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
        <asp:Label runat="server" ID="lEnterShippingAddress"></asp:Label>
    </div>
    <div class="clear">
    </div>
    <div class="enter-address">
        <div class="enter-address-body">
            <nopCommerce:AddressEdit ID="ctrlShippingAddress" runat="server" IsNew="true" IsBillingAddress="false"
                ValidationGroup="EnterShippingAddress" />
        </div>
        <div class="clear">
        </div>
        <div class="button">
            <asp:Button runat="server" ID="btnNextStep" Text="<% $NopResources:Checkout.NextButton %>"
                OnClick="btnNextStep_Click" CssClass="newaddressnextstepbutton" ValidationGroup="EnterShippingAddress" />
        </div>
    </div>
</div>
