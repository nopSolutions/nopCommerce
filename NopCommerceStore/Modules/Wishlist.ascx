<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.WishlistControl"
    CodeBehind="Wishlist.ascx.cs" %>
<asp:Panel class="wishlist-content" runat="server" ID="pnlEmptyCart">
    <%=GetLocaleResourceString("Wishlist.WishlistIsEmpty")%>
</asp:Panel>
<asp:Panel class="wishlist-content" runat="server" ID="pnlCart">
    <asp:Panel runat="server" ID="pnlCommonWarnings" CssClass="warning-box" EnableViewState="false"
        Visible="false">
        <asp:Label runat="server" ID="lblCommonWarning" CssClass="warning-text" EnableViewState="false"
            Visible="false"></asp:Label>
    </asp:Panel>
    <table class="cart">
        <%if (IsEditable)
                  { %>
        <col width="1" />
        <%} %>
        <col width="1" />
        <%if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowSKU"))
                  {%>
        <col width="1" />
        <%} %>
        <%if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowProductImagesOnWishList"))
                  {%>
        <col width="1" class="picture" />
        <%} %>
        <col />
        <col width="1" />
        <col width="1" />
        <col width="1" class="end" />
        <thead>
            <tr class="cart-header-row">
                <%if (IsEditable)
                  { %>
                <th>
                    <%=GetLocaleResourceString("Wishlist.Remove")%>
                </th>
                <%} %>
                <th>
                    <%=GetLocaleResourceString("Wishlist.AddToCart")%>
                </th>
                <%if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowSKU"))
                  {%>
                <th>
                    <%=GetLocaleResourceString("ShoppingCart.SKU")%>
                </th>
                <%} %>
                <%if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowProductImagesOnWishList"))
                  {%>
                <th class="picture">
                </th>
                <%} %>
                <th>
                    <%=GetLocaleResourceString("Wishlist.Product(s)")%>
                </th>
                <th>
                    <%=GetLocaleResourceString("Wishlist.UnitPrice")%>
                </th>
                <th>
                    <%=GetLocaleResourceString("Wishlist.Quantity")%>
                </th>
                <th class="end">
                    <%=GetLocaleResourceString("Wishlist.ItemTotal")%>
                </th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater ID="rptShoppingCart" runat="server">
                <ItemTemplate>
                    <tr class="cart-item-row">
                        <%if (IsEditable)
                          { %>
                        <td>
                            <asp:CheckBox runat="server" ID="cbRemoveFromCart" />
                        </td>
                        <%} %>
                        <td>
                            <asp:CheckBox runat="server" ID="cbAddToCart" />
                        </td>
                        <%if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowSKU"))
                          {%>
                        <td style="white-space: nowrap;">
                            <%#Server.HtmlEncode(((ShoppingCartItem)Container.DataItem).ProductVariant.SKU)%>
                        </td>
                        <%} %>
                        <%if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.ShowProductImagesOnWishList"))
                          {%>
                        <td class="productpicture">
                            <asp:Image ID="iProductVariantPicture" runat="server" ImageUrl='<%#GetProductVariantImageUrl((ShoppingCartItem)Container.DataItem)%>'
                                AlternateText="Product picture" />
                        </td>
                        <%} %>
                        <td class="product">
                            <a href='<%#GetProductUrl((ShoppingCartItem)Container.DataItem)%>'>
                                <%#Server.HtmlEncode(GetProductVariantName((ShoppingCartItem)Container.DataItem))%></a>
                            <%#GetAttributeDescription((ShoppingCartItem)Container.DataItem)%>
                            <%#GetRecurringDescription((ShoppingCartItem)Container.DataItem)%>
                            <asp:Panel runat="server" ID="pnlWarnings" CssClass="warning-box" EnableViewState="false"
                                Visible="false">
                                <asp:Label runat="server" ID="lblWarning" CssClass="warning-text" EnableViewState="false"
                                    Visible="false"></asp:Label>
                            </asp:Panel>
                        </td>
                        <td style="white-space: nowrap;">
                            <%#GetShoppingCartItemUnitPriceString((ShoppingCartItem)Container.DataItem)%>
                        </td>
                        <td style="white-space: nowrap;">
                            <%if (IsEditable)
                              { %>
                            <asp:TextBox ID="txtQuantity" size="4" runat="server" Text='<%# Eval("Quantity") %>'
                                SkinID="WishListQuantityText" />
                            <%}
                              else
                              { %>
                            <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>' CssClass="Label" />
                            <%} %>
                        </td>
                        <td style="white-space: nowrap;" class="end">
                            <%#GetShoppingCartItemSubTotalString((ShoppingCartItem)Container.DataItem)%>
                            <asp:Label ID="lblShoppingCartItemId" runat="server" Visible="false" Text='<%# Eval("ShoppingCartItemId") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    <%if (IsEditable)
      { %>
    <asp:Button ID="btnUpdate" OnClick="btnUpdate_Click" runat="server" Text="<% $NopResources:Wishlist.UpdateWishlist %>"
        CssClass="updatewishlistbutton" />
    <%} %>
    <asp:Button ID="btnAddToCart" OnClick="btnAddToCart_Click" runat="server" Text="<% $NopResources:Wishlist.AddToCartButton %>"
        CssClass="updatewishlistbutton" />
    <%if (IsEditable)
      { %>
    <asp:Button ID="btnEmailWishlist" OnClick="btnEmailWishlist_Click" runat="server"
        Text="<% $NopResources:Wishlist.EmailButton %>" CssClass="updatewishlistbutton" />
    <%} %>
</asp:Panel>
