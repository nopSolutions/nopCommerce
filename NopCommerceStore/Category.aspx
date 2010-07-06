<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.CategoryPage" CodeBehind="Category.aspx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="MiniShoppingCartBox" Src="~/Modules/MiniShoppingCartBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryNavigation" Src="~/Modules/CategoryNavigation.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerNavigation" Src="~/Modules/ManufacturerNavigation.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="RecentlyViewedProducts" Src="~/Modules/RecentlyViewedProductsBox.ascx" %>
<%--<asp:Content ID="Content2" ContentPlaceHolderID="cph2" runat="server">
    <nopCommerce:CategoryNavigation ID="ctrlCategoryNavigation" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:ManufacturerNavigation ID="ctrlManufacturerNavigation" runat="server" />
    <div class="clear">
    </div>
    <nopCommerce:RecentlyViewedProducts ID="ctrlRecentlyViewedProducts" runat="server" />
    <div class="clear">
    </div>
</asp:Content>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <asp:PlaceHolder runat="server" ID="CategoryPlaceHolder"></asp:PlaceHolder>
</asp:Content>
