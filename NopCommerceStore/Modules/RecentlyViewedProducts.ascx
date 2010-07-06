<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.RecentlyViewedProductsControl" Codebehind="RecentlyViewedProducts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductBox1" Src="~/Modules/ProductBox1.ascx" %>
<div class="recently-viewed-products">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("Products.RecentlyViewedProducts")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="product-grid">
        <asp:DataList ID="dlCatalog" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
            RepeatLayout="Table" ItemStyle-CssClass="item-box">
            <ItemTemplate>
                <nopCommerce:ProductBox1 ID="ctrlProductBox" Product='<%# Container.DataItem %>' runat="server" />
            </ItemTemplate>
        </asp:DataList>
    </div>
</div>
