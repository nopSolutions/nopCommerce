<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Templates.Categories.ProductsInLines2" Codebehind="ProductsInLines2.ascx.cs" %>
<div class="category-page">
    <div class="page-title">
        <h1><asp:Literal runat="server" ID="lName"></asp:Literal></h1>
    </div>
    <div class="clear">
    </div>
    <div class="category-description">
        <asp:Literal runat="server" ID="lDescription"></asp:Literal>
    </div>
    <div class="clear">
    </div>
    <div class="sub-category-list">
        <asp:Repeater ID="rptrSubCategories" runat="server" OnItemDataBound="rptrSubCategories_ItemDataBound">
            <ItemTemplate>
                <asp:HyperLink ID="hlCategory" runat="server" Text='<%#Server.HtmlEncode(Eval("LocalizedName").ToString()) %>' />
            </ItemTemplate>
            <SeparatorTemplate>
                <br />
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
    <div class="clear">
    </div>
    <div class="product-list2">
        <asp:ListView ID="lvCatalog" runat="server" OnItemDataBound="lvCatalog_ItemDataBound">
            <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </LayoutTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="hlProduct" runat="server" Text='<%#Server.HtmlEncode(Eval("LocalizedName").ToString()) %>' />
            </ItemTemplate>
            <ItemSeparatorTemplate>
                <br />
            </ItemSeparatorTemplate>
        </asp:ListView>
        <div class="clear">
        </div>
        <nopCommerce:Pager runat="server" ID="catalogPager" FirstButtonText="<% $NopResources:Pager.First %>"
            LastButtonText="<% $NopResources:Pager.Last %>" NextButtonText="<% $NopResources:Pager.Next %>"
            PreviousButtonText="<% $NopResources:Pager.Previous %>" CurrentPageText="Pager.CurrentPage" />
    </div>
</div>
