<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CurrentShoppingCartsControl"
    CodeBehind="CurrentShoppingCarts.ascx.cs" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.CurrentShoppingCarts.Title")%>" />
        <%=GetLocaleResourceString("Admin.CurrentShoppingCarts.Title")%>
    </div>
    <div class="options">
    </div>
</div>
<asp:GridView ID="gvCustomerSessions" runat="server" AutoGenerateColumns="False"
    Width="100%" AllowPaging="true" OnPageIndexChanging="gvCustomerSessions_PageIndexChanging"
    PageSize="10"  OnRowDataBound="gvCustomerSessions_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.CustomerColumn %>"
            ItemStyle-Width="20%">
            <ItemTemplate>
                <%#GetCustomerInfo((CustomerSession)Container.DataItem)%>
                <br />
                <%#GetLastAccessInfo((CustomerSession)Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.ShoppingCartColumn %>" ItemStyle-Width="80%">
            <ItemTemplate>
                <asp:GridView ID="gvProductVariants" runat="server" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.NameColumn %>"
                            ItemStyle-Width="45%">
                            <ItemTemplate>
                                <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                                    <em><a href='<%#GetProductVariantUrl((ShoppingCartItem)Container.DataItem)%>'>
                                        <%#Server.HtmlEncode(GetProductVariantName((ShoppingCartItem)Container.DataItem))%></a></em>
                                    <%#GetAttributeDescription((ShoppingCartItem)Container.DataItem)%>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.PriceColumn %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#GetShoppingCartItemUnitPriceString((ShoppingCartItem)Container.DataItem)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Quantity" HeaderText="<% $NopResources:Admin.CustomerShoppingCart.QuantityColumn %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerShoppingCart.TotalColumn %>"
                            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#GetShoppingCartItemSubTotalString((ShoppingCartItem)Container.DataItem)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<asp:Label runat="server" ID="lblCurrentShoppingCartsEmpty" Text="<% $NopResources:Admin.CurrentShoppingCarts.Empty %>"
    Visible="false" />