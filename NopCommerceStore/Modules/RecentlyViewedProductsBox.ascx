<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.RecentlyViewedProductsBoxControl" Codebehind="RecentlyViewedProductsBox.ascx.cs" %>
<div class="block block-recently-viewed-products">
    <div class="title">
        <%=GetLocaleResourceString("Products.RecentlyViewedProducts")%>
    </div>
    <div class="clear">
    </div>
    <div class="listbox">
        <asp:ListView ID="lvRecentlyViewedProducts" runat="server" OnItemDataBound="lvRecentlyViewedProducts_ItemDataBound" EnableViewState="false">
            <LayoutTemplate>
                <ul>
                    <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                </ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li>
                    <asp:HyperLink ID="hlProduct" runat="server" Text='<%#Server.HtmlEncode(Eval("LocalizedName").ToString()) %>' />
                </li>
            </ItemTemplate>
            <ItemSeparatorTemplate>
                <li class="separator">
                    &nbsp;
                </li>
            </ItemSeparatorTemplate>
        </asp:ListView>
    </div>
</div>
