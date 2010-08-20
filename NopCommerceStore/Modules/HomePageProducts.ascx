<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.HomePageProductsControl"
    CodeBehind="HomePageProducts.ascx.cs" %>
<div class="home-page-product-grid">
    <div class="boxtitle">
        <%=GetLocaleResourceString("HomePage.FeaturedProducts")%>
    </div>
    <div class="clear">
    </div>
    <asp:DataList ID="dlCatalog" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
        RepeatLayout="Table" OnItemDataBound="dlCatalog_ItemDataBound" ItemStyle-CssClass="item-box" EnableViewState="false">
        <ItemTemplate>
            <div class="product-item">
                <h2 class="product-title">
                    <asp:HyperLink ID="hlProduct" runat="server" /></h2>
                <div class="picture">
                    <asp:HyperLink ID="hlImageLink" runat="server" />
                </div>
            </div>
        </ItemTemplate>
    </asp:DataList>
</div>
