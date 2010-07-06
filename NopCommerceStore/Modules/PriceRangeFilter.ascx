<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.PriceRangeFilterControl" Codebehind="PriceRangeFilter.ascx.cs" %>
<div class="price-range-filter">
    <div class="title">
        <%=GetLocaleResourceString("Common.PriceRangeFilter")%>
    </div>
    <div class="clear">
    </div>
    <asp:Panel runat="server" ID="pnlPriceRangeSelector" CssClass="PriceRangeSelector">
        <asp:Repeater ID="rptrPriceRange" runat="server" OnItemDataBound="rptrPriceRange_ItemDataBound">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <asp:HyperLink ID="hlPriceRange" runat="server"></asp:HyperLink>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlSelectedPriceRange" CssClass="selected-price-range"
        Visible="false">
        <asp:Label ID="lblSelectedPriceRange" runat="server"></asp:Label>
        <p>
        </p>
        <asp:HyperLink ID="hlRemoveFilter" runat="server" CssClass="remove-price-range-filter">
            <%=GetLocaleResourceString("Common.PriceRangeFilterRemove")%>
        </asp:HyperLink>
    </asp:Panel>
</div>
