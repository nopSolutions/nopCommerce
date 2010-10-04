<%@ Page Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.ProductReviewAddPage" CodeBehind="ProductReviewAdd.aspx.cs"
     %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductCategoryBreadcrumb" Src="~/Modules/ProductCategoryBreadcrumb.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductWriteReview" Src="~/Modules/ProductWriteReview.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ProductCategoryBreadcrumb ID="ctrlProductCategoryBreadcrumb" runat="server">
    </nopCommerce:ProductCategoryBreadcrumb>
    <div class="clear">
    </div>
    <nopCommerce:ProductWriteReview ID="ctrlProductWriteReview" runat="server"></nopCommerce:ProductWriteReview>
</asp:Content>
