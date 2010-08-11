<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductAddControl"
    CodeBehind="ProductAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductInfoAdd" Src="ProductInfoAdd.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductSEO" Src="ProductSEO.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductCategory" Src="ProductCategory.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductManufacturer" Src="ProductManufacturer.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="RelatedProducts" Src="RelatedProducts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CrossSellProducts" Src="CrossSellProducts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductPictures" Src="ProductPictures.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductSpecifications" Src="ProductSpecifications.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductAdd.AddNewProduct")%>" />
        <%=GetLocaleResourceString("Admin.ProductAdd.AddNewProduct")%>
        <a href="Products.aspx" title="<%=GetLocaleResourceString("Admin.ProductAdd.BackToProductList")%>">
            (<%=GetLocaleResourceString("Admin.ProductAdd.BackToProductList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ProductAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ProductAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="ProductTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductInfo" HeaderText="<% $NopResources:Admin.ProductAdd.ProductInfo %>">
        <ContentTemplate>
            <nopCommerce:ProductInfoAdd runat="server" ID="ctrlProductInfoAdd" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductSEO" HeaderText="<% $NopResources:Admin.ProductAdd.SEO %>">
        <ContentTemplate>
            <nopCommerce:ProductSEO ID="ctrlProductSEO" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCategoryMappings" HeaderText="<% $NopResources:Admin.ProductAdd.CategoryMappings %>">
        <ContentTemplate>
            <nopCommerce:ProductCategory ID="ctrlProductCategory" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlManufacturerMappings" HeaderText="<% $NopResources:Admin.ProductAdd.ManufacturerMappings %>">
        <ContentTemplate>
            <nopCommerce:ProductManufacturer ID="ctrlProductManufacturer" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlRelatedProducts" HeaderText="<% $NopResources:Admin.ProductAdd.RelatedProducts %>">
        <ContentTemplate>
            <nopCommerce:RelatedProducts ID="ctrlRelatedProducts" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCrossSellProducts" HeaderText="<% $NopResources:Admin.ProductAdd.CrossSellProducts %>">
        <ContentTemplate>
            <nopCommerce:CrossSellProducts ID="ctrlCrossSellProducts" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlPictures" HeaderText="<% $NopResources:Admin.ProductAdd.Pictures %>">
        <ContentTemplate>
            <nopCommerce:ProductPictures ID="ctrlProductPictures" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductSpecification" HeaderText="<% $NopResources:Admin.ProductAdd.ProductSpecification %>">
        <ContentTemplate>
            <nopCommerce:ProductSpecifications ID="ctrlProductSpecifications" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>