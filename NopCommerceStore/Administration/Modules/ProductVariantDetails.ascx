<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantDetailsControl"
    CodeBehind="ProductVariantDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantInfo" Src="ProductVariantInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantTierPrices" Src="ProductVariantTierPrices.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductPricesByCustomerRole" Src="ProductPricesByCustomerRole.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantAttributes" Src="ProductVariantAttributes.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductVariantDiscounts" Src="ProductVariantDiscounts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductVariantDetails.EditProductVariant")%>" />
        <%=GetLocaleResourceString("Admin.ProductVariantDetails.EditProductVariant")%>:
        <asp:Label ID="lblProductName" runat="server" />
        <asp:HyperLink runat="server" ID="hlProductURL" Text="<% $NopResources:Admin.ProductVariantDetails.BackToProductDetails %>" />
    </div>
    <div class="options">
        <asp:Button ID="PreviewButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductVariantDetails.PreviewButton.Text %>"
            ToolTip="<% $NopResources:Admin.ProductVariantDetails.PreviewButton.ToolTip %>" />
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductVariantDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ProductVariantDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductVariantDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ProductVariantDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="ProductVariantTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductVariantInfo" HeaderText="<% $NopResources:Admin.ProductVariantDetails.ProductVariantInfo %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantInfo runat="server" ID="ctrlProductVariantInfo" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlTierPrices" HeaderText="<% $NopResources:Admin.ProductVariantDetails.TierPrices %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantTierPrices runat="server" ID="ctrlProductVariantTierPrices" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRolePrices" HeaderText="<% $NopResources:Admin.ProductVariantDetails.CustomerRolePrices %>">
        <ContentTemplate>
            <nopCommerce:ProductPricesByCustomerRole runat="server" ID="ctrlProductPricesByCustomerRole" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>    
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductAttributes" HeaderText="<% $NopResources:Admin.ProductVariantDetails.Attributes %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantAttributes runat="server" ID="ctrlProductVariantAttributes" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlDiscountMappings" HeaderText="<% $NopResources:Admin.ProductVariantDetails.Discounts %>">
        <ContentTemplate>
            <nopCommerce:ProductVariantDiscounts runat="server" ID="ctrlProductVariantDiscounts" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
