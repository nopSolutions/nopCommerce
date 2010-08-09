<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OneVariant.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Templates.Products.OneVariant" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductCategoryBreadcrumb" Src="~/Modules/ProductCategoryBreadcrumb.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductRating" Src="~/Modules/ProductRating.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductEmailAFriendButton" Src="~/Modules/ProductEmailAFriendButton.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductAddToCompareList" Src="~/Modules/ProductAddToCompareList.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductSpecs" Src="~/Modules/ProductSpecifications.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="RelatedProducts" Src="~/Modules/RelatedProducts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductReviews" Src="~/Modules/ProductReviews.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductsAlsoPurchased" Src="~/Modules/ProductsAlsoPurchased.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="~/Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="~/Modules/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributes" Src="~/Modules/ProductAttributes.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="GiftCardAttributes" Src="~/Modules/GiftCardAttributes.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductPrice1" Src="~/Modules/ProductPrice1.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="TierPrices" Src="~/Modules/TierPrices.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductTags" Src="~/Modules/ProductTags.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductShareButton" Src="~/Modules/ProductShareButton.ascx" %>

<ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
    EnableScriptLocalization="true" ID="sm1" ScriptMode="Release" CompositeScript-ScriptMode="Release" />
<% if (SettingManager.GetSettingValueBoolean("Media.CategoryBreadcrumbEnabled"))
   { %>
<nopCommerce:ProductCategoryBreadcrumb ID="ctrlProductCategoryBreadcrumb" runat="server" />
<% } %>
<div class="clear">
</div>
<div class="product-details-page">
    <div class="product-essential product-details-info">

        <script language="javascript" type="text/javascript">
            function UpdateMainImage(url) {
                var imgMain = document.getElementById('<%=defaultImage.ClientID%>');
                imgMain.src = url;
            }
        </script>

        <div class="picture">
            <a runat="server" id="lnkMainLightbox">
                <asp:Image ID="defaultImage" runat="server" />
            </a>
            <asp:ListView ID="lvProductPictures" runat="server" GroupItemCount="3">
                <LayoutTemplate>
                    <table style="margin-top: 10px;">
                        <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <GroupTemplate>
                    <tr>
                        <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
                    </tr>
                </GroupTemplate>
                <ItemTemplate>
                    <td align="left">
                        <a href="<%#PictureManager.GetPictureUrl((Picture)Container.DataItem)%>" rel="lightbox-p"
                            title="<%= lProductName.Text%>">
                            <img src="<%#PictureManager.GetPictureUrl((Picture)Container.DataItem, 70)%>" alt="Product image" /></a>
                    </td>
                </ItemTemplate>
            </asp:ListView>
        </div>
        <div class="overview">
            <h3 class="productname">
                <asp:Literal ID="lProductName" runat="server" />
            </h3>
            <div class="clear">
            </div>
            <div class="shortdescription">
                <asp:Literal ID="lShortDescription" runat="server" />
            </div>
            <asp:PlaceHolder runat="server" ID="phSKU">
                <div class="clear">
                </div>
                <div class="sku">
                    <%=GetLocaleResourceString("Products.SKU")%>
                    <asp:Literal runat="server" ID="lSKU" />
                </div>
            </asp:PlaceHolder>
            <div class="clear">
            </div>
            <asp:PlaceHolder runat="server" ID="phManufacturers">
                <div class="manufacturers">
                    <asp:Literal ID="lManufacturersTitle" runat="server" />
                    <asp:Repeater runat="server" ID="rptrManufacturers">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlManufacturer" runat="server" Text='<%#Server.HtmlEncode(Eval("LocalizedName").ToString()) %>' NavigateUrl='<%#SEOHelper.GetManufacturerUrl((Manufacturer)(Container.DataItem)) %>' />
                        </ItemTemplate>
                        <SeparatorTemplate>
                            ,
                        </SeparatorTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>
            <div class="clear">
            </div>
        <div class="clear">
        </div>
            <div class="product-collateral">
                <nopCommerce:ProductRating ID="ctrlProductRating" runat="server" />
                <br />
                <div class="one-variant-price">
                    <nopCommerce:ProductPrice1 ID="ctrlProductPrice" runat="server" />
                    <nopCommerce:DecimalTextBox runat="server" ID="txtCustomerEnteredPrice" Value="1"
                        RequiredErrorMessage="<% $NopResources:Products.CustomerEnteredPrice.EnterPrice %>"
                        MinimumValue="0" MaximumValue="100000000" Width="100" />
                </div>
                <div class="add-info">
                    <nopCommerce:NumericTextBox runat="server" ID="txtQuantity" Value="1" RequiredErrorMessage="<% $NopResources:Products.EnterQuantity %>"
                        RangeErrorMessage="<% $NopResources:Products.QuantityRange %>" MinimumValue="1"
                        MaximumValue="999999" Width="50" />
                    <asp:Button ID="btnAddToCart" runat="server" OnCommand="OnCommand" Text="<% $NopResources:Products.AddToCart %>"
                        CommandName="AddToCart" CommandArgument='<%#Eval("ProductVariantId")%>' CssClass="productvariantaddtocartbutton" />
                    <asp:Button ID="btnAddToWishlist" runat="server" OnCommand="OnCommand" Text="<% $NopResources:Wishlist.AddToWishlist %>"
                        CommandName="AddToWishlist" CommandArgument='<%#Eval("ProductVariantId")%>' CssClass="productvariantaddtowishlistbutton" />
                </div>
                <asp:Panel runat="server" ID="pnlDownloadSample" Visible="false" CssClass="one-variant-download-sample">
                    <span class="downloadsamplebutton">
                        <asp:HyperLink runat="server" ID="hlDownloadSample" Text="<% $NopResources:Products.DownloadSample %>" />
                    </span>
                </asp:Panel>
                <br />
                <asp:Panel ID="pnlStockAvailablity" runat="server" class="stock">
                    <asp:Label ID="lblStockAvailablity" runat="server" />
                </asp:Panel>
                <br />
                <nopCommerce:ProductEmailAFriendButton ID="ctrlProductEmailAFriendButton" runat="server" />
                <nopCommerce:ProductAddToCompareList ID="ctrlProductAddToCompareList" runat="server" />
                <div class="clear">
                </div>
                <nopCommerce:ProductShareButton ID="ctrlProductShareButton" runat="server" />
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
    <div class="product-collateral">
        <div class="product-variant-line">
            <asp:Label runat="server" ID="lblError" EnableViewState="false" CssClass="error" />
            <div class="clear">
            </div>
            <nopCommerce:TierPrices ID="ctrlTierPrices" runat="server" />
            <div class="clear">
            </div>
            <div class="attributes">
                <nopCommerce:ProductAttributes ID="ctrlProductAttributes" runat="server" />
            </div>
            <div class="clear">
            </div>
            <nopCommerce:GiftCardAttributes ID="ctrlGiftCardAttributes" runat="server" />
            <div class="clear">
            </div>
            <div class="fulldescription">
                <asp:Literal ID="lFullDescription" runat="server" />
            </div>
        </div>
        <div class="clear">
        </div>
        <div>
            <nopCommerce:ProductsAlsoPurchased ID="ctrlProductsAlsoPurchased" runat="server" />
        </div>
        <div class="clear">
        </div>
        <div>
            <nopCommerce:RelatedProducts ID="ctrlRelatedProducts" runat="server" />
        </div>
        <div class="clear">
        </div>
        <ajaxToolkit:TabContainer runat="server" ID="ProductsTabs" ActiveTabIndex="1" CssClass="grey">
            <ajaxToolkit:TabPanel runat="server" ID="pnlProductReviews" HeaderText="<% $NopResources:Products.ProductReviews %>">
                <ContentTemplate>
                    <nopCommerce:ProductReviews ID="ctrlProductReviews" runat="server" ShowWriteReview="true" />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="pnlProductSpecs" HeaderText="<% $NopResources:Products.ProductSpecs %>">
                <ContentTemplate>
                    <nopCommerce:ProductSpecs ID="ctrlProductSpecs" runat="server" />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="pnlProductTags" HeaderText="<% $NopResources:Products.ProductTags %>">
                <ContentTemplate>
                    <nopCommerce:ProductTags ID="ctrlProductTags" runat="server" />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </div>
</div>
