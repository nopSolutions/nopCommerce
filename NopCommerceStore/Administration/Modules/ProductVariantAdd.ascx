<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantAddControl"
    CodeBehind="ProductVariantAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantInfo" Src="ProductVariantInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantDiscounts" Src="ProductVariantDiscounts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantTierPrices" Src="ProductVariantTierPrices.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductPricesByCustomerRole" Src="ProductPricesByCustomerRole.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantAttributes" Src="ProductVariantAttributes.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductVariantAdd.AddNewProductVariant")%>" />
        <%=GetLocaleResourceString("Admin.ProductVariantAdd.AddNewProductVariant")%>:
        <asp:Label ID="lblProductName" runat="server" />
        <asp:HyperLink runat="server" ID="hlProductURL" Text="<% $NopResources:Admin.ProductVariantAdd.BackToProductDetails %>" />
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ProductVariantAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ProductVariantAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="ProductVariantTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductVariantInfo" HeaderText="<% $NopResources:Admin.ProductVariantAdd.ProductVariantInfo %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantInfo runat="server" ID="ctrlProductVariantInfo" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlTierPrices" HeaderText="<% $NopResources:Admin.ProductVariantAdd.TierPrices %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantTierPrices runat="server" ID="ctrlProductVariantTierPrices" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRolePrices" HeaderText="<% $NopResources:Admin.ProductVariantAdd.CustomerRolePrices %>">
        <ContentTemplate>
            <nopCommerce:ProductPricesByCustomerRole runat="server" ID="ctrlProductPricesByCustomerRole" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductAttributes" HeaderText="<% $NopResources:Admin.ProductVariantAdd.Attributes %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantAttributes runat="server" ID="ctrlProductVariantAttributes" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlDiscountMappings" HeaderText="<% $NopResources:Admin.ProductVariantAdd.Discounts %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantDiscounts runat="server" ID="ctrlProductVariantDiscounts" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>