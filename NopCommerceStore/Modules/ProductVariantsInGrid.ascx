<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ProductVariantsInGridControl"
    CodeBehind="ProductVariantsInGrid.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="~/Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributes" Src="~/Modules/ProductAttributes.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductPrice1" Src="~/Modules/ProductPrice1.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="TierPrices" Src="~/Modules/TierPrices.ascx" %>
<%@ Reference Control="~/Modules/ProductAttributes.ascx" %>
<%@ Reference Control="~/Modules/EmailTextBox.ascx" %>
<div class="product-variant-list">
    <asp:Repeater ID="rptVariants" runat="server" OnItemCommand="rptVariants_OnItemCommand"
        OnItemDataBound="rptVariants_OnItemDataBound">
        <ItemTemplate>
            <div class="product-variant-line">
                <div class="picture">
                    <asp:Image ID="iProductVariantPicture" runat="server" />
                </div>
                <div class="overview">
                    <div class="productname">
                        <%#Server.HtmlEncode(Eval("LocalizedName").ToString())%>
                    </div>
                    <asp:Label runat="server" ID="ProductVariantId" Text='<%#Eval("ProductVariantId")%>'
                        Visible="false" />
                </div>
                <div class="description">
                    <asp:Literal runat="server" ID="lDescription" Visible='<%# !String.IsNullOrEmpty(Eval("LocalizedDescription").ToString()) %>'
                        Text='<%# Eval("LocalizedDescription")%>'>
                    </asp:Literal>
                </div>
                <asp:Panel runat="server" ID="pnlDownloadSample" Visible="false" CssClass="downloadsample">
                    <span class="downloadsamplebutton">
                        <asp:HyperLink runat="server" ID="hlDownloadSample" Text="<% $NopResources:Products.DownloadSample %>">
                        </asp:HyperLink>
                    </span>
                </asp:Panel>
                <nopCommerce:TierPrices ID="ctrlTierPrices" runat="server" ProductVariantID='<%#Eval("ProductVariantId") %>'>
                </nopCommerce:TierPrices>
                <div class="clear">
                </div>
                <div class="attributes">
                    <nopCommerce:ProductAttributes ID="ctrlProductAttributes" runat="server" ProductVariantID='<%#Eval("ProductVariantId") %>'>
                    </nopCommerce:ProductAttributes>
                </div>
                <div class="clear">
                </div>
                <asp:Panel ID="pnlStockAvailablity" runat="server" class="stock">
                    <asp:Label ID="lblStockAvailablity" runat="server">
                    </asp:Label>
                </asp:Panel>
                <div class="clear">
                </div>
                <asp:Panel ID="pnlGiftCardInfo" runat="server" class="giftCard">
                    <dl>
                        <dt>
                            <asp:Label runat="server" ID="lblRecipientName" Text="<% $NopResources:Products.GiftCard.RecipientName %>"
                                AssociatedControlID="txtRecipientName"></asp:Label></dt>
                        <dd>
                            <asp:TextBox runat="server" ID="txtRecipientName"></asp:TextBox></dd>
                        <dt>
                            <asp:Label runat="server" ID="lblRecipientEmail" Text="<% $NopResources:Products.GiftCard.RecipientEmail %>"
                                AssociatedControlID="txtRecipientEmail"></asp:Label></dt>
                        <dd>
                            <asp:TextBox runat="server" ID="txtRecipientEmail"></asp:TextBox></dd>
                        <dt>
                            <asp:Label runat="server" ID="lblSenderName" Text="<% $NopResources:Products.GiftCard.SenderName %>"
                                AssociatedControlID="txtSenderName"></asp:Label></dt>
                        <dd>
                            <asp:TextBox runat="server" ID="txtSenderName"></asp:TextBox></dd>
                        <dt>
                            <asp:Label runat="server" ID="lblSenderEmail" Text="<% $NopResources:Products.GiftCard.SenderEmail %>"
                                AssociatedControlID="txtSenderEmail"></asp:Label></dt>
                        <dd>
                            <asp:TextBox runat="server" ID="txtSenderEmail"></asp:TextBox></dd>
                        <dt>
                            <asp:Label runat="server" ID="lblGiftCardMessage" Text="<% $NopResources:Products.GiftCard.Message %>"
                                AssociatedControlID="txtGiftCardMessage"></asp:Label></dt>
                        <dd>
                            <asp:TextBox runat="server" ID="txtGiftCardMessage" TextMode="MultiLine" Height="100px"
                                Width="300px"></asp:TextBox></dd>
                    </dl>
                </asp:Panel>
                <div class="clear">
                </div>
                <div class="price">
                    <nopCommerce:ProductPrice1 ID="ctrlProductPrice" runat="server" ProductVariantID='<%#Eval("ProductVariantId") %>' />
                    <nopCommerce:NumericTextBox runat="server" ID="txtCustomerEnteredPrice" Value="1"
                        RequiredErrorMessage="<% $NopResources:Products.CustomerEnteredPrice.EnterPrice %>"
                        MinimumValue="0" MaximumValue="999999" Width="100"></nopCommerce:NumericTextBox>
                </div>
                <div class="add-info">
                    <nopCommerce:NumericTextBox runat="server" ID="txtQuantity" Value="1" RequiredErrorMessage="<% $NopResources:Products.EnterQuantity %>"
                        RangeErrorMessage="<% $NopResources:Products.QuantityRange %>" MinimumValue="1"
                        MaximumValue="999999" Width="50"></nopCommerce:NumericTextBox>
                    <asp:Button ID="btnAddToCart" runat="server" Text="<% $NopResources:Products.AddToCart %>"
                        CommandName="AddToCart" CommandArgument='<%#Eval("ProductVariantId")%>' CssClass="productvariantaddtocartbutton">
                    </asp:Button>
                    <asp:Button ID="btnAddToWishlist" runat="server" Text="<% $NopResources:Wishlist.AddToWishlist %>"
                        CommandName="AddToWishlist" CommandArgument='<%#Eval("ProductVariantId")%>' CssClass="productvariantaddtowishlistbutton">
                    </asp:Button>
                </div>
                <div class="clear">
                </div>
                <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="error" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
