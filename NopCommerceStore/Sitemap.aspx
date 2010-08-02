<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/OneColumn.master"
    CodeBehind="Sitemap.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Sitemap" %>

<asp:Content runat="server" ContentPlaceHolderID="cph1">
    <div class="sitemap-page">
        <div class="page-title">
            <h1>
                <%=GetLocaleResourceString("Sitemap.Title")%></h1>
        </div>
        <div class="entity">
            <asp:DataList ID="dlTopics" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                RepeatLayout="Table" OnItemDataBound="dlTopics_ItemDataBound" EnableViewState="false"
                ItemStyle-CssClass="topic-box" Width="100%" ItemStyle-VerticalAlign="Bottom" ItemStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <div class="item">
                        <asp:HyperLink ID="hlLink" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>
        <div class="clear">
        </div>
        <div class="entity">
            <asp:DataList ID="dlCategories" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                RepeatLayout="Table" OnItemDataBound="dlCategories_ItemDataBound" EnableViewState="false"
                ItemStyle-CssClass="category-box" Width="100%" ItemStyle-VerticalAlign="Bottom" ItemStyle-HorizontalAlign="Left">
                <HeaderTemplate>
                    <h2>
                        <%=GetLocaleResourceString("Sitemap.Categories")%></h2>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="item">
                        <asp:HyperLink ID="hlLink" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>
        <div class="clear">
        </div>
        <div class="entity">
            <asp:DataList ID="dlManufacturers" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                RepeatLayout="Table" OnItemDataBound="dlManufacturers_ItemDataBound" EnableViewState="false"
                ItemStyle-CssClass="manufacturer-box" Width="100%" ItemStyle-VerticalAlign="Bottom" ItemStyle-HorizontalAlign="Left">
                <HeaderTemplate>
                    <h2>
                        <%=GetLocaleResourceString("Sitemap.Manufacturers")%></h2>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="item">
                        <asp:HyperLink ID="hlLink" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>
        <div class="clear">
        </div>
        <div class="entity">
            <asp:DataList ID="dlProducts" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
                RepeatLayout="Table" OnItemDataBound="dlProducts_ItemDataBound" EnableViewState="false"
                ItemStyle-CssClass="product-box" Width="100%" ItemStyle-VerticalAlign="Bottom" ItemStyle-HorizontalAlign="Left">
                <HeaderTemplate>
                    <h2>
                        <%=GetLocaleResourceString("Sitemap.Products")%></h2>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="item">
                        <asp:HyperLink ID="hlLink" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:DataList>
        </div>
    </div>
</asp:Content>
