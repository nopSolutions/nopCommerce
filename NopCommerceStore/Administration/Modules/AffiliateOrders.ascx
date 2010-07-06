<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AffiliateOrdersControl"
    CodeBehind="AffiliateOrders.ascx.cs" %>
<asp:GridView ID="gvAffiliateOrders" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:BoundField DataField="OrderId" HeaderText="<% $NopResources:Admin.AffiliateOrders.OrderID %>" ItemStyle-Width="5%"></asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.OrderTotal %>" ItemStyle-Width="10%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("OrderTotal")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.OrderStatus %>" ItemStyle-Width="10%">
            <ItemTemplate>
                <%#OrderManager.GetOrderStatusName(Convert.ToInt32(Eval("OrderStatusId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.PaymentStatus %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#PaymentStatusManager.GetPaymentStatusName(Convert.ToInt32(Eval("PaymentStatusId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.ShippingStatus %>" ItemStyle-Width="10%">
            <ItemTemplate>
                <%#ShippingStatusManager.GetShippingStatusName(Convert.ToInt32(Eval("ShippingStatusId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.Customer %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.View %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%"
            ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderId")%>"><%#GetLocaleResourceString("Admin.AffiliateOrders.View")%> </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.AffiliateOrders.CreatedOn %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%"
            ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
