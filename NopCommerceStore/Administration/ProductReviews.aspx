<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductReviews"
    CodeBehind="ProductReviews.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductReviews" Src="Modules/ProductReviews.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductReviews runat="server" ID="ctrlProductReviews" />
</asp:Content>
