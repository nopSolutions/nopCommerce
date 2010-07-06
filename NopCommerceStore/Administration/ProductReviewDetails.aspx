<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductReviewDetails"
    CodeBehind="ProductReviewDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductReviewDetails" Src="Modules/ProductReviewDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductReviewDetails runat="server" ID="ctrlProductReviewDetails" />
</asp:Content>
