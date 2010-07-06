<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantsLowStockControl"
    CodeBehind="ProductVariantsLowStock.ascx.cs" %>
<div class="section-title">
    <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductVariantsLowStock.Title")%>" />
    <%=GetLocaleResourceString("Admin.ProductVariantsLowStock.Title")%>
</div>
<asp:GridView ID="gvProductVariants" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvProductVariants_PageIndexChanging" AllowPaging="true"
    PageSize="15">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantsLowStock.Name %>" ItemStyle-Width="40%">
            <ItemTemplate>
                <a href="ProductVariantDetails.aspx?ProductVariantID=<%#Eval("ProductVariantId")%>"
                    title="<%#GetLocaleResourceString("Admin.ProductVariantsLowStock.Name.Tooltip")%>">
                    <%#Server.HtmlEncode(Eval("FullProductName").ToString())%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantsLowStock.Price %>" ItemStyle-Width="15%">
            <ItemTemplate>
                <%#((decimal)Eval("Price")).ToString("N")%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="StockQuantity" HeaderText="<% $NopResources:Admin.ProductVariantsLowStock.StockQuantity %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
        <asp:BoundField DataField="MinStockQuantity" HeaderText="<% $NopResources:Admin.ProductVariantsLowStock.MinStockQuantity %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
        <asp:BoundField DataField="Published" HeaderText="<% $NopResources:Admin.ProductVariantsLowStock.Published %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantsLowStock.Edit %>" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%"
            ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="ProductVariantDetails.aspx?ProductVariantID=<%#Eval("ProductVariantId")%>"
                    title="<%#GetLocaleResourceString("Admin.ProductVariantsLowStock.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.ProductVariantsLowStock.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <%#GetLocaleResourceString("Admin.ProductVariantsLowStock.ProductStockIsOkay")%>
    </EmptyDataTemplate>
</asp:GridView>
