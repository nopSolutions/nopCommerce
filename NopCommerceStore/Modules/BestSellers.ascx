<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.BestSellersControl"
    CodeBehind="BestSellers.ascx.cs" %>
<div class="bestsellers">
    <div class="boxtitle">
        <%=GetLocaleResourceString("Reports.BestSellingProducts")%>
    </div>
    <div class="clear">
    </div>
    <asp:DataList ID="dlCatalog" runat="server" RepeatColumns="3" RepeatDirection="Horizontal"
        RepeatLayout="Table" OnItemDataBound="dlCatalog_ItemDataBound" ItemStyle-CssClass="item-box" EnableViewState="false">
        <ItemTemplate>
            <div class="product-item">
                <div class="picture">
                    <asp:HyperLink ID="hlImageLink" runat="server" />
                </div>
                <h2 class="product-title">
                    <asp:HyperLink ID="hlProduct" runat="server" />
                </h2>
            </div>
        </ItemTemplate>
    </asp:DataList>
</div>
