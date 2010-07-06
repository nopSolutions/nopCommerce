<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductsAlsoPurchasedControl"
    CodeBehind="ProductsAlsoPurchased.ascx.cs" %>
<div class="also-purchased-products-grid">
    <div class="title">
        <%=GetLocaleResourceString("Products.AlsoPurchased")%>
    </div>
    <div class="clear">
    </div>
    <asp:DataList ID="dlAlsoPurchasedProducts" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
        RepeatLayout="Table" OnItemDataBound="dlAlsoPurchasedProducts_ItemDataBound"
        ItemStyle-CssClass="item-box">
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
