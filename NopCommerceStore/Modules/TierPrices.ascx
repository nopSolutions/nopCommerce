<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.TierPriceControl"
    CodeBehind="TierPrices.ascx.cs" %>
<div class="tier-prices">
    <div class="prices-header">
        <%=GetLocaleResourceString("Products.TierPricesTitle") %>
    </div>
    <div class="prices">
        <asp:ListView ID="lvTierPrices" runat="server" OnItemDataBound="lvTierPrices_ItemDataBound" >
            <LayoutTemplate>
                <table class="prices-table">
                    <tr>
                        <td>
                            <table class="header-table">
                                <tr>
                                    <td class="field-header" runat="server">
                                        <%=GetLocaleResourceString("Products.TierPricesQuantityTitle") %>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="field-header" runat="server">
                                        <%=GetLocaleResourceString("Products.TierPricesPriceTitle") %>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                    </tr>
                </table>
            </LayoutTemplate>
            <GroupTemplate>
                <td>
                    <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
                </td>
            </GroupTemplate>
            <ItemTemplate>
                <table class="item-table">
                    <tr>
                        <td class="item-quantity">
                            <asp:Label ID="lblQuantity" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="item-price">
                            <asp:Label ID="lblPrice" runat="server" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:ListView>
    </div>
</div>
