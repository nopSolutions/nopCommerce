<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/TwoColumn.master"
    CodeBehind="Manufacturers.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.ManufacturersPage" %>

<%@ Register TagPrefix="nopCommerce" TagName="MiniShoppingCartBox" Src="~/Modules/MiniShoppingCartBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryNavigation" Src="~/Modules/CategoryNavigation.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerNavigation" Src="~/Modules/ManufacturerNavigation.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="RecentlyViewedProducts" Src="~/Modules/RecentlyViewedProductsBox.ascx" %>
<%--<asp:Content ID="Content2" ContentPlaceHolderID="cph2" runat="server">
    <nopCommerce:MiniShoppingCartBox ID="ctrlMiniShoppingCartBox" runat="server" />
    <div class="clear">
    </div>
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
<asp:Content runat="server" ContentPlaceHolderID="cph1">
    <div class="manufacturerlist-page">
        <div class="page-title">
            <h1>
                <%=GetLocaleResourceString("Common.ManufacturerList")%></h1>
        </div>
        <div class="clear">
        </div>
        <div class="manufacturer-grid">
            <asp:DataList ID="dlManufacturers" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
                RepeatLayout="Table" OnItemDataBound="DlManufacturers_OnItemDataBound" ItemStyle-CssClass="item-box">
                <ItemTemplate>
                    <div class="manufacturer-item">
                        <h2 class="man-title">
                            <asp:HyperLink ID="hlManufacturer" runat="server" />
                        </h2>
                        <div class="picture">
                            <asp:HyperLink ID="hlImageLink" runat="server" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>
    </div>
</asp:Content>
