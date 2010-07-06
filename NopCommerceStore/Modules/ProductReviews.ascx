<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductReviews"
    CodeBehind="ProductReviews.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductReviewHelpfulness" Src="~/Modules/ProductReviewHelpfulness.ascx" %>
<div class="product-review-box">
    <div>
        <asp:Button runat="server" ID="btnWriteReview" Text="<% $NopResources:Products.WriteReview %>"
            OnClick="btnWriteReview_Click" CssClass="productwritereviewbutton"></asp:Button>
    </div>
    <div class="clear">
    </div>
    <asp:Panel class="product-review-list" runat="server" ID="pnlReviews">
        <asp:Repeater ID="rptrProductReviews" runat="server">
            <ItemTemplate>
                <div class="product-review-item">
                    <div class="review-title">
                        <%#Server.HtmlEncode((string)Eval("Title"))%>
                    </div>
                    <div class="rating">
                        <ajaxToolkit:Rating ID="productRating" AutoPostBack="false" runat="server" MaxRating="5"
                            StarCssClass="rating-star" WaitingStarCssClass="saved-rating-star" FilledStarCssClass="filled-rating-star"
                            EmptyStarCssClass="empty-rating-star" ReadOnly="true" CurrentRating='<%#Eval("Rating")%>' />
                    </div>
                    <div class="clear">
                    </div>
                    <%#ProductManager.FormatProductReviewText((string)Eval("ReviewText"))%>
                    <p>
                        <%=GetLocaleResourceString("Products.ProductReviewFrom")%>:
                        <%#Server.HtmlEncode(GetCustomerInfo(Convert.ToInt32(Eval("CustomerId"))))%>
                        |
                        <%=GetLocaleResourceString("Products.ProductReviewCreatedOn")%>:
                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString("g")%>
                    </p>
                    <nopCommerce:ProductReviewHelpfulness ID="ctrlProductReviewHelpfulness" runat="server"
                        ProductReviewID='<%#Eval("ProductReviewId")%>'></nopCommerce:ProductReviewHelpfulness>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</div>
