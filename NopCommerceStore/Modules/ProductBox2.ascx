<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductBox2Control"
    CodeBehind="ProductBox2.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductPrice2" Src="~/Modules/ProductPrice2.ascx" %>
<div class="product-item">
    <h2 class="product-title">
        <asp:HyperLink ID="hlProduct" runat="server" />
    </h2>
    <div class="picture">
        <asp:HyperLink ID="hlImageLink" runat="server" />
    </div>
    <div class="description">
        <asp:Literal runat="server" ID="lShortDescription"></asp:Literal>
    </div>
    <div class="prices-wrapper">
        <div class="prices">
            <nopCommerce:ProductPrice2 ID="ctrlProductPrice" runat="server" ProductID='<%#Eval("ProductId") %>' />
        </div>
        <div class="buttons">
            <asp:Button runat="server" ID="btnProductDetails" OnCommand="btnProductDetails_Click"
                Text="<% $NopResources:Products.ProductDetails %>" ValidationGroup="ProductDetails"
                CommandArgument='<%# Eval("ProductId") %>' CssClass="productlistproductdetailbutton" />
            <br />
            <asp:Button runat="server" ID="btnAddToCart" OnCommand="btnAddToCart_Click" Text="<% $NopResources:Products.AddToCart %>"
                ValidationGroup="ProductDetails" CommandArgument='<%# Eval("ProductId") %>' CssClass="productlistaddtocartbutton" />
        </div>
    </div>
</div>
