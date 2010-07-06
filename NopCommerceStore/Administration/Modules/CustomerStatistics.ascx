<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerStatisticsControl"
    CodeBehind="CustomerStatistics.ascx.cs" EnableViewState="false" %>
<%if (this.DisplayTitle)
  { %>
<div class="statisticsTitle">
    <%=GetLocaleResourceString("Admin.CustomerStatistics.RegisteredCustomers")%>
</div>
<%} %>
<table cellpadding="0" cellspacing="0" class="tablestyle" width="100%" style="border-collapse: collapse;">
    <thead>
        <tr class="headerstyle">
            <th>
                <%=GetLocaleResourceString("Admin.CustomerStatistics.Item")%>
            </th>
            <th>
                <%=GetLocaleResourceString("Admin.CustomerStatistics.Count")%>
            </th>
            <th>
                <%=GetLocaleResourceString("Admin.CustomerStatistics.Action")%>
            </th>
        </tr>
    </thead>
    <tbody>
        <tr class="rowstyle">
            <td>
                <%=GetLocaleResourceString("Admin.CustomerStatistics.InTheLast")%>&nbsp;
                <asp:DropDownList ID="ddlDays" runat="server" AutoPostBack="true">
                    <asp:ListItem Value="7" Selected="true" Text="<% $NopResources:Admin.CustomerStatistics.7days %>"></asp:ListItem>
                    <asp:ListItem Value="14" Text="<% $NopResources:Admin.CustomerStatistics.14days %>"></asp:ListItem>
                    <asp:ListItem Value="30" Text="<% $NopResources:Admin.CustomerStatistics.month %>"></asp:ListItem>
                    <asp:ListItem Value="365" Text="<% $NopResources:Admin.CustomerStatistics.year %>"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Literal ID="lblCustomers" runat="server" />
            </td>
            <td>
                <asp:HyperLink ID="lnkViewCustomers" runat="server"><%=GetLocaleResourceString("Admin.CustomerStatistics.View")%></asp:HyperLink>
            </td>
        </tr>
    </tbody>
</table>
