<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.RecentlyViewedProductsPage" Codebehind="RecentlyViewedProducts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="RecentlyViewedProducts" Src="~/Modules/RecentlyViewedProducts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:RecentlyViewedProducts ID="ctrlRecentlyViewedProducts" runat="server" />
</asp:Content>
