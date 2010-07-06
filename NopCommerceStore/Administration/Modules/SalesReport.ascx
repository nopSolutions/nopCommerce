<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SalesReportControl" CodeBehind="SalesReport.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.SalesReport.Title")%>" />
        <%=GetLocaleResourceString("Admin.SalesReport.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="<% $NopResources:Admin.SalesReport.SearchButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SearchButton_Click" ToolTip="<% $NopResources:Admin.SalesReport.SearchButton.Tooltip %>" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblStartDate" Text="<% $NopResources:Admin.SalesReport.StartDate %>"
                ToolTip="<% $NopResources:Admin.SalesReport.StartDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblEndDate" Text="<% $NopResources:Admin.SalesReport.EndDate %>"
                ToolTip="<% $NopResources:Admin.SalesReport.EndDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePicker" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblOrderStatus" Text="<% $NopResources:Admin.SalesReport.OrderStatus %>"
                ToolTip="<% $NopResources:Admin.SalesReport.OrderStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentStatus" Text="<% $NopResources:Admin.SalesReport.PaymentStatus %>"
                ToolTip="<% $NopResources:Admin.SalesReport.PaymentStatus.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlPaymentStatus" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblBillingCountryTitle" Text="<% $NopResources:Admin.SalesReport.BillingCountry %>"
                ToolTip="<% $NopResources:Admin.SalesReport.BillingCountry.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlBillingCountry" runat="server">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<p>
</p>
<asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.SalesReport.Name %>" ItemStyle-Width="60%">
            <ItemTemplate>
                <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                    <a href='<%#GetProductVariantUrl(Convert.ToInt32(Eval("ProductVariantId")))%>' title="<%#GetLocaleResourceString("Admin.SalesReport.Name.Tooltip")%>">
                        <%#Server.HtmlEncode(GetProductVariantName(Convert.ToInt32(Eval("ProductVariantId"))))%></a>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Quantity" HeaderText="<% $NopResources:Admin.SalesReport.TotalCount %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.SalesReport.TotalPrice %>"
            ItemStyle-Width="25%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("PriceExclTax")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
