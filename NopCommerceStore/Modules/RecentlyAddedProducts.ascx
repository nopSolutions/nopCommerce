<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.RecentlyAddedProductsControl"
    CodeBehind="RecentlyAddedProducts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductBox1" Src="~/Modules/ProductBox1.ascx" %>
<div class="recently-added-products">
    <div class="page-title">
        <table width="100%">
            <tr>
                <td style="text-align: left;">
                    <h1>
                        <%=GetLocaleResourceString("Products.NewProducts")%></h1>
                </td>
                <td style="text-align: right;">
                    <a href="<%=Page.ResolveUrl("~/recentlyaddedproductsrss.aspx")%>">
                        <asp:Image ID="imgRSS" runat="server" ImageUrl="~/images/icon_rss.gif" ToolTip="<% $NopResources:RecentlyAddedProductsRSS.Tooltip %>"
                            AlternateText="RSS" /></a>
                </td>
            </tr>
        </table>
    </div>
    <div class="clear">
    </div>
    <div class="product-grid">
        <asp:DataList ID="dlCatalog" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
            RepeatLayout="Table" ItemStyle-CssClass="item-box">
            <ItemTemplate>
                <nopCommerce:ProductBox1 ID="ctrlProductBox" Product='<%# Container.DataItem %>'
                    runat="server" />
            </ItemTemplate>
        </asp:DataList>
    </div>
</div>
