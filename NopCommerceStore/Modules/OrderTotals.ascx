<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.OrderTotalsControl"
    CodeBehind="OrderTotals.ascx.cs" %>
<div class="total-info">
    <table class="cart-total">
        <tbody>
            <tr>
                <td class="cart_total_left">
                    <strong><span style="white-space: nowrap;">
                        <%=GetLocaleResourceString("ShoppingCart.Sub-Total")%>:</span></strong>
                </td>
                <td class="cart_total_right">
                    <span style="white-space: nowrap;">
                        <asp:Label ID="lblSubTotalAmount" runat="server" CssClass="productPrice" />
                    </span>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phOrderSubTotalDiscount" Visible="false">
                <tr>
                    <td class="cart_total_left">
                        <strong><span style="white-space: nowrap;">
                            <%=GetLocaleResourceString("ShoppingCart.OrderDiscount")%><asp:LinkButton runat="server"
                                ID="btnRemoveOrderSubTotalDiscount" Text="" CommandName="remove" OnCommand="btnRemoveOrderSubTotalDiscount_Command"
                                CssClass="removediscountbutton" />: </span></strong>
                    </td>
                    <td class="cart_total_right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblOrderSubTotalDiscountAmount" runat="server" CssClass="productPrice" />
                        </span>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="cart_total_left">
                    <strong><span style="white-space: nowrap;">
                        <%=GetLocaleResourceString("ShoppingCart.Shipping")%>: </span></strong>
                </td>
                <td class="cart_total_right">
                    <span style="white-space: nowrap;">
                        <asp:Label ID="lblShippingAmount" runat="server" CssClass="productPrice" />
                    </span>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phPaymentMethodAdditionalFee">
                <tr>
                    <td class="cart_total_left">
                        <strong><span style="white-space: nowrap;">
                            <%=GetLocaleResourceString("ShoppingCart.PaymentMethodAdditionalFee")%>: </span>
                        </strong>
                    </td>
                    <td class="cart_total_right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblPaymentMethodAdditionalFee" runat="server" CssClass="productPrice" />
                        </span>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:Repeater runat="server" ID="rptrTaxRates" OnItemDataBound="rptrTaxRates_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td class="cart_total_left">
                            <strong><span style="white-space: nowrap;">
                                <asp:Literal runat="server" ID="lTaxRateTitle"></asp:Literal>: </span></strong>
                        </td>
                        <td class="cart_total_right">
                            <span style="white-space: nowrap;">
                                <asp:Literal runat="server" ID="lTaxRateValue"></asp:Literal>
                            </span>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder runat="server" ID="phTaxTotal">
                <tr>
                    <td class="cart_total_left">
                        <strong><span style="white-space: nowrap;">
                            <%=GetLocaleResourceString("ShoppingCart.Tax")%>: </span></strong>
                    </td>
                    <td class="cart_total_right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblTaxAmount" runat="server" CssClass="productPrice" />
                        </span>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="phOrderTotalDiscount" Visible="false">
                <tr>
                    <td class="cart_total_left">
                        <strong><span style="white-space: nowrap;">
                            <%=GetLocaleResourceString("ShoppingCart.OrderDiscount")%><asp:LinkButton runat="server"
                                ID="btnRemoveOrderTotalDiscount" Text="" CommandName="remove" OnCommand="btnRemoveOrderTotalDiscount_Command"
                                CssClass="removediscountbutton" />:</span></strong>
                    </td>
                    <td class="cart_total_right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblOrderTotalDiscountAmount" runat="server" CssClass="productPrice" />
                        </span>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:Repeater runat="server" ID="rptrGiftCards" OnItemDataBound="rptrGiftCards_ItemDataBound"
                Visible="false" OnItemCommand="rptrGiftCards_ItemCommand">
                <ItemTemplate>
                    <tr>
                        <td class="cart_total_left">
                            <strong><span style="white-space: nowrap;">
                                <asp:Literal runat="server" ID="lGiftCard"></asp:Literal><asp:LinkButton runat="server"
                                    ID="btnRemoveGC" Text="" CommandName="remove" CommandArgument='<%# Eval("GiftCardId")%>'
                                    CssClass="removegiftcardbutton" />:</span></strong>
                        </td>
                        <td class="cart_total_right">
                            <span style="white-space: nowrap;">
                                <asp:Label ID="lblGiftCardAmount" runat="server" CssClass="productPrice" />
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td class="cart_total_left_below">
                            <span style="white-space: nowrap;">
                                <asp:Literal runat="server" ID="lGiftCardRemaining"></asp:Literal></span>
                        </td>
                        <td>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder runat="server" ID="phRewardPoints">
                <tr>
                    <td class="cart_total_left">
                        <strong><span style="white-space: nowrap;">
                            <asp:Literal runat="server" ID="lRewardPointsTitle"></asp:Literal>:</span></strong>
                    </td>
                    <td class="cart_total_right">
                        <span style="white-space: nowrap;">
                            <asp:Label ID="lblRewardPointsAmount" runat="server" CssClass="productPrice" />
                        </span>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="cart_total_left">
                    <strong><span style="white-space: nowrap;">
                        <%=GetLocaleResourceString("ShoppingCart.OrderTotal")%>:</span></strong>
                </td>
                <td class="cart_total_right">
                    <span style="white-space: nowrap;">
                        <asp:Label ID="lblTotalAmount" runat="server" CssClass="productPrice" />
                    </span>
                </td>
            </tr>
        </tbody>
    </table>
</div>
