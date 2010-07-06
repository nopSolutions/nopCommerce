<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.RelatedProductsControl"
    CodeBehind="RelatedProducts.ascx.cs" %>
<div class="related-products-grid">
    <div class="title">
        <%=GetLocaleResourceString("Products.RelatedProducts")%>
    </div>
    <div class="clear">
    </div>
    <asp:DataList ID="dlRelatedProducts" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
        RepeatLayout="Table" OnItemDataBound="dlRelatedProducts_ItemDataBound" ItemStyle-CssClass="item-box">
        <ItemTemplate>
            <div class="item">
                <div class="product-title">
                    <asp:HyperLink ID="hlProduct" runat="server" />
                </div>
                <div class="picture">
                    <asp:HyperLink ID="hlImageLink" runat="server" />
                </div>
            </div>
        </ItemTemplate>
    </asp:DataList>
</div>
