<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductRatingControl"
    CodeBehind="ProductRating.ascx.cs" %>
<div class="product-rating-box">
    <ajaxToolkit:Rating ID="productRating" AutoPostBack="true" runat="server" CurrentRating="2"
        MaxRating="5" StarCssClass="rating-star" WaitingStarCssClass="saved-rating-star"
        FilledStarCssClass="filled-rating-star" EmptyStarCssClass="empty-rating-star" OnChanged="productRating_Changed"
        Style="float: left;" />
    <br />
    <br />
    <asp:Label runat="server" ID="lblProductRatingResult"></asp:Label>
</div>
