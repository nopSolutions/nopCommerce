<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MiniShoppingCartBox.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Modules.MiniShoppingCartBoxControl" %>
<div class="block block-shoppingcart">
    <div class="title">
        <%=GetLocaleResourceString("MiniShoppingCartBox.Information")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <asp:Literal runat="server" ID="lShoppingCart" EnableViewState="false" />
        <asp:PlaceHolder runat="server" ID="phCheckoutInfo" EnableViewState="false">
            <br />
            <asp:Label runat="server" ID="lblOrderSubtotal" CssClass="subtotal" />
            <div class="buttons">
                <asp:Button runat="server" ID="btnCheckout" Text="<% $NopResources:MiniShoppingCartBox.CheckoutButton %>"
                    OnClick="BtnCheckout_OnClick" CausesValidation="false" CssClass="minicartcheckoutbutton" />
            </div>
        </asp:PlaceHolder>
        <asp:ListView ID="lvCart" runat="server" OnItemDataBound="lvCart_ItemDataBound" EnableViewState="false">
            <LayoutTemplate>
                <div class="items">
                    <ul>
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                    </ul>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <li>
                    <asp:Label runat="server" ID="lblQty" CssClass="quantity"></asp:Label><asp:HyperLink ID="hlProduct"
                        runat="server" />
                </li>
            </ItemTemplate>
            <ItemSeparatorTemplate>
                <li class="separator">&nbsp; </li>
            </ItemSeparatorTemplate>
        </asp:ListView>
    </div>
</div>
