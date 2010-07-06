<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductReviewHelpfulnessControl"
    CodeBehind="ProductReviewHelpfulness.ascx.cs" %>
<div class="product-review-helpfulness">
    <asp:Label runat="server" ID="lblTitle" Text="<% $NopResources:Products.WasThisReviewHelpful? %>" />
    <asp:LinkButton runat="server" ID="btnYes" Text="<% $NopResources:Common.Yes %>"
        ValidationGroup="ProductReviewHelpfullness" OnClick="btnYes_Click" CssClass="linkButton"></asp:LinkButton>&nbsp;
    <asp:LinkButton runat="server" ID="btnNo" Text="<% $NopResources:Common.No %>" ValidationGroup="ProductReviewHelpfullness"
        OnClick="btnNo_Click" CssClass="linkButton"></asp:LinkButton>
    (<asp:Label runat="server" ID="lblHelpfulYesTotal"></asp:Label>
    /
    <asp:Label runat="server" ID="lblHelpfulNoTotal"></asp:Label>)
    <asp:Label runat="server" ID="lblOnlyRegistered"></asp:Label><br />
</div>
