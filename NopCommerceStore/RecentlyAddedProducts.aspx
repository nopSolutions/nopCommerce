<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.RecentlyAddedProductsPage" Codebehind="RecentlyAddedProducts.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="RecentlyAddedProducts" Src="~/Modules/RecentlyAddedProducts.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:RecentlyAddedProducts ID="ctrlRecentlyAddedProducts" runat="server" />
</asp:Content>
