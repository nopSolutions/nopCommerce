<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerAddressesControl"
    CodeBehind="CustomerAddresses.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="AddressDisplay" Src="~/Modules/AddressDisplay.ascx" %>
<div class="customer-addresses">
    <div class="section-title">
        <%=GetLocaleResourceString("Account.BillingAddressBookEntries")%></div>
    <div class="clear">
    </div>
    <div class="address-list">
        <asp:Repeater ID="rptrBillingAddresses" runat="server">
            <ItemTemplate>
                <div class="address-item">
                    <nopCommerce:AddressDisplay ID="AddressDisplayCtrl" runat="server" Address='<%# Container.DataItem %>'
                        ShowEditButton="true" ShowDeleteButton="true" />
                </div>
            </ItemTemplate>
            <SeparatorTemplate>
                <div class="clear">
                </div>
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
    <div class="clear">
    </div>
    <div class="add-button">
        <asp:Button ID="btnAddBillingAddress" OnClick="btnAddBillingAddress_Click" runat="server"
            Text="<% $NopResources:Account.AddBillingAddress %>" ValidationGroup="BillingAddresses"
            CssClass="addbillingaddressbutton" />
    </div>
    <div class="section-title">
        <%=GetLocaleResourceString("Account.ShippingAddressBookEntries")%></div>
    <div class="address-list">
        <asp:Repeater ID="rptrShippingAddresses" runat="server">
            <ItemTemplate>
                <div class="address-item">
                    <nopCommerce:AddressDisplay ID="AddressDisplayCtrl" runat="server" Address='<%# Container.DataItem %>'
                        ShowEditButton="true" ShowDeleteButton="true" />
                </div>
            </ItemTemplate>
            <SeparatorTemplate>
                <div class="clear">
                </div>
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
    <div class="add-button">
        <asp:Button ID="btnAddShippingAddress" OnClick="btnAddShippingAddress_Click" runat="server"
            Text="<% $NopResources:Account.AddShippingAddress %>" ValidationGroup="ShippingAddresses"
            CssClass="addshippingaddressbutton" />
    </div>
    <div class="clear">
    </div>
</div>
