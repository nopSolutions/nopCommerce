<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.OrderAverageReportControl"
    CodeBehind="OrderAverageReport.ascx.cs" %>
<div class="statisticsTitle">
    <%=GetLocaleResourceString("Admin.OrderAverageReport.OrderTotals")%>
</div>
<asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderAverageReport.OrderStatus %>"
            ItemStyle-Width="20%">
            <ItemTemplate>
                <%#OrderManager.GetOrderStatusName(Convert.ToInt32(Eval("OrderStatus")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderAverageReport.Today %>"
            ItemStyle-Width="16%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("SumTodayOrders")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderAverageReport.ThisWeek %>"
            ItemStyle-Width="16%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("SumThisWeekOrders")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderAverageReport.ThisMonth %>"
            ItemStyle-Width="16%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("SumThisMonthOrders")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderAverageReport.ThisYear %>"
            ItemStyle-Width="16%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("SumThisYearOrders")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.OrderAverageReport.AllTime %>"
            ItemStyle-Width="16%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("SumAllTimeOrders")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
