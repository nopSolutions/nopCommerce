<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BestSellersStatControl"
    CodeBehind="BestSellersStat.ascx.cs" %>
<div class="statisticsTitle">
    <%=GetLocaleResourceString("Admin.BestSellersStat.BestSellers")%>
</div>
<asp:GridView ID="gvBestSellers" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.BestSellersStat.Product%>" ItemStyle-Width="65%">
            <ItemTemplate>
                <div style="padding-left: 10px; padding-right: 10px; text-align: left;">
                    <a href='<%#GetProductVariantUrl(Convert.ToInt32(Eval("ProductVariantId")))%>' title="View product variant details">
                        <%#Server.HtmlEncode(GetProductVariantName(Convert.ToInt32(Eval("ProductVariantId"))))%></a>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="SalesTotalCount" HeaderText="<% $NopResources:Admin.BestSellersStat.TotalCount %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.BestSellersStat.TotalAmount %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("SalesTotalAmount")), true, false))%>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
