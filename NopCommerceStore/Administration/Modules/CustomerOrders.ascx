<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerOrdersControl"
    CodeBehind="CustomerOrders.ascx.cs" %>
<asp:Repeater ID="rptrOrders" runat="server">
    <ItemTemplate>
        <div>
            <p>
                <strong>
                    <%#GetLocaleResourceString("Admin.CustomerOrders.OrderId")%>
                    <%#Eval("OrderId")%>
                </strong>
            </p>
            <%#GetLocaleResourceString("Admin.CustomerOrders.Date")%>
            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            <br />
            <%#GetLocaleResourceString("Admin.CustomerOrders.OrderStatus")%>
            <%#((Order)Container.DataItem).OrderStatus.GetOrderStatusName()%>
            <br />
            <%#GetLocaleResourceString("Admin.CustomerOrders.PaymentStatus")%>
            <%#((Order)Container.DataItem).PaymentStatus.GetPaymentStatusName()%>
            <br />
            <%#GetLocaleResourceString("Admin.CustomerOrders.ShippingStatus")%>
            <%#((Order)Container.DataItem).ShippingStatus.GetShippingStatusName() %>
            <br />
            <%#GetLocaleResourceString("Admin.CustomerOrders.OrderTotal")%>
            <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("OrderTotal")), true, false))%>
            <p>
                <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderId")%>">
                    <%#GetLocaleResourceString("Admin.CustomerOrders.Details")%></a>
            </p>
        </div>
    </ItemTemplate>
    <SeparatorTemplate>
        <hr />
    </SeparatorTemplate>
</asp:Repeater>
