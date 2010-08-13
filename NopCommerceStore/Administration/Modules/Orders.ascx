<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.OrdersControl" CodeBehind="Orders.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.Orders.Title")%>" />
        <%=GetLocaleResourceString("Admin.Orders.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="<% $NopResources:Admin.Orders.SearchButton %>"
            CssClass="adminButtonBlue" OnClick="SearchButton_Click" ToolTip="<% $NopResources:Admin.Orders.SearchButton.Tooltip %>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Orders.BtnPrintPdfPackagingSlips.Text %>" CssClass="adminButtonBlue" ID="btnPrintPdfPackagingSlips" OnClick="BtnPrintPdfPackagingSlips_OnClick" ValidationGroup="BtnPrintPdfPackagingSlips" ToolTip="<% $NopResources:Admin.Orders.BtnPrintPdfPackagingSlips.Tooltip %>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Orders.ExportXMLButton %>"
            CssClass="adminButtonBlue" ID="btnExportXML" OnClick="btnExportXML_Click" ValidationGroup="ExportXML"
            ToolTip="<% $NopResources:Admin.Orders.ExportXMLButton.Tooltip %>" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Orders.ExportXLSButton %>"
            CssClass="adminButtonBlue" ID="btnExportXLS" OnClick="btnExportXLS_Click" ValidationGroup="ExportXLS"
            ToolTip="<% $NopResources:Admin.Orders.ExportXLSButton.Tooltip %>" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStartDate" Text="<% $NopResources:Admin.Orders.StartDate %>"
                ToolTip="<% $NopResources:Admin.Orders.StartDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEndDate" Text="<% $NopResources:Admin.Orders.EndDate %>"
                ToolTip="<% $NopResources:Admin.Orders.EndDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerEmail" Text="<% $NopResources:Admin.Orders.CustomerEmail %>"
                ToolTip="<% $NopResources:Admin.Orders.CustomerEmail.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtCustomerEmail" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderStatus" Text="<% $NopResources:Admin.Orders.OrderStatus %>"
                ToolTip="<% $NopResources:Admin.Orders.OrderStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentStatus" Text="<% $NopResources:Admin.Orders.PaymentStatus %>"
                ToolTip="<% $NopResources:Admin.Orders.PaymentStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlPaymentStatus" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblShippingStatus" Text="<% $NopResources:Admin.Orders.ShippingStatus %>"
                ToolTip="<% $NopResources:Admin.Orders.ShippingStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlShippingStatus" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderGuid" Text="<% $NopResources:Admin.Orders.OrderGuid %>"
                ToolTip="<% $NopResources:Admin.Orders.OrderGuid.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtOrderGuid" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblGoDirectlyToOrderNumber" Text="<% $NopResources:Admin.Orders.GoDirectly %>"
                ToolTip="<% $NopResources:Admin.Orders.GoDirectly.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtOrderId" Width="150px"
                ValidationGroup="GoDirectly" ErrorMessage="<% $NopResources:Admin.Orders.GoDirectly.ErrorMessage %>">
            </nopCommerce:SimpleTextBox>
            <asp:Button runat="server" Text="<% $NopResources:Admin.Orders.GoButton.Text %>"
                CssClass="adminButtonBlue" ID="btnGoDirectlyToOrderNumber" OnClick="btnGoDirectlyToOrderNumber_Click"
                ValidationGroup="GoDirectly" ToolTip="<% $NopResources:Admin.Orders.GoButton.Tooltip %>" />
        </td>
    </tr>
</table>
<p>
</p>
<asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvOrders_PageIndexChanging" AllowPaging="true" PageSize="15">
    <Columns>
        <asp:BoundField DataField="OrderId" HeaderText="<% $NopResources:Admin.Orders.OrderIDColumn %>"
            ItemStyle-Width="10%"></asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.OrderTotalColumn %>"
            ItemStyle-Width="10%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("OrderTotal")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.OrderStatusColumn %>"
            ItemStyle-Width="10%">
            <ItemTemplate>
                <%#OrderManager.GetOrderStatusName(Convert.ToInt32(Eval("OrderStatusId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.PaymentStatusColumn %>"
            ItemStyle-Width="20%">
            <ItemTemplate>
                <%#PaymentStatusManager.GetPaymentStatusName(Convert.ToInt32(Eval("PaymentStatusId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.ShippingStatusColumn %>"
            ItemStyle-Width="15%">
            <ItemTemplate>
                <%#ShippingStatusManager.GetShippingStatusName(Convert.ToInt32(Eval("ShippingStatusId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.CustomerColumn %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.ViewColumn %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderId")%>" title="<%#GetLocaleResourceString("Admin.Orders.ViewColumn.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Orders.ViewColumn")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Orders.CreatedOnColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<asp:Label runat="server" ID="lblNoOrdersFound" Text="<% $NopResources:Admin.Orders.NoOrdersFound %>"
    Visible="false"></asp:Label>