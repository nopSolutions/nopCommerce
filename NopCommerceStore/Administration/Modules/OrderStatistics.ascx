<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.OrderStatisticsControl"
    CodeBehind="OrderStatistics.ascx.cs" %>
<div class="statisticsTitle">
    <%=GetLocaleResourceString("Admin.OrderStatistics.IncompleteOrders")%>
</div>
<table cellpadding="0" cellspacing="0" class="tablestyle" width="100%" style="border-collapse: collapse;">
    <thead>
        <tr class="headerstyle">
            <th>
                <%=GetLocaleResourceString("Admin.OrderStatistics.Item")%>
            </th>
            <th>
                <%=GetLocaleResourceString("Admin.OrderStatistics.Total")%>
            </th>
            <th>
                <%=GetLocaleResourceString("Admin.OrderStatistics.Count")%>
            </th>
        </tr>
    </thead>
    <tbody>
        <tr class="rowstyle">
            <td>
                <b>
                    <%=GetLocaleResourceString("Admin.OrderStatistics.TotalUnpaidOrders")%></b>
            </td>
            <td>
                <asp:Literal ID="lblTotalUnpaidValue" runat="server" />
            </td>
            <td>
                <asp:Literal ID="lblTotalUnpaid" runat="server" />
            </td>
        </tr>
        <tr class="altrowstyle">
            <td>
                <b>
                    <%=GetLocaleResourceString("Admin.OrderStatistics.TotalNotShippedOrders")%></b>
            </td>
            <td>
                <asp:Literal ID="lblTotalUnshippedValue" runat="server" />
            </td>
            <td>
                <asp:Literal ID="lblTotalUnshipped" runat="server" />
            </td>
        </tr>
        <tr class="rowstyle">
            <td>
                <b>
                    <%=GetLocaleResourceString("Admin.OrderStatistics.TotalIncompleteOrders")%></b>
            </td>
            <td>
                <asp:Literal ID="lblTotalIncompleteValue" runat="server" />
            </td>
            <td>
                <asp:Literal ID="lblTotalIncomplete" runat="server" />
            </td>
        </tr>
    </tbody>
</table>
