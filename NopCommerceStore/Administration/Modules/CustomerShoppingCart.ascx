<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerShoppingCartControl"
    CodeBehind="CustomerShoppingCart.ascx.cs" %>
<asp:Panel runat="server" ID="pnlEmptyCart">
    <%=GetLocaleResourceString("Admin.CustomerShoppingCart.Empty")%>
</asp:Panel>
<asp:Panel runat="server" ID="pnlCart">
    <table class="adminContent">
        <tr>
            <td class="adminData">
                <asp:GridView ID="gvProductVariants" runat="server" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.Name %>"
                            ItemStyle-Width="45%">
                            <ItemTemplate>
                                <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                                    <em><a href='<%#GetProductVariantUrl((ShoppingCartItem)Container.DataItem)%>' title="<%#GetLocaleResourceString("Admin.CustomerShoppingCart.Name.Tooltip")%>">
                                        <%#Server.HtmlEncode(GetProductVariantName((ShoppingCartItem)Container.DataItem))%></a></em>
                                    <%#GetAttributeDescription((ShoppingCartItem)Container.DataItem)%>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.Price %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#GetShoppingCartItemUnitPriceString((ShoppingCartItem)Container.DataItem)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Quantity" HeaderText="<% $NopResources:Admin.CustomerShoppingCart.Quantity %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.Total %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#GetShoppingCartItemSubTotalString((ShoppingCartItem)Container.DataItem)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
