<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryDetailsControl"
    CodeBehind="CategoryDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryInfo" Src="CategoryInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategorySEO" Src="CategorySEO.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryProduct" Src="CategoryProduct.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryDiscount" Src="CategoryDiscount.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.CategoryDetails.EditCategoryDetails")%>" />
        <%=GetLocaleResourceString("Admin.CategoryDetails.EditCategoryDetails")%>
        <a href="Categories.aspx" title="<%=GetLocaleResourceString("Admin.CategoryDetails.BackToCategoryList")%>">
            (<%=GetLocaleResourceString("Admin.CategoryDetails.BackToCategoryList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="PreviewButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CategoryDetails.PreviewButton.Text %>"
            ToolTip="<% $NopResources:Admin.CategoryDetails.PreviewButton.ToolTip %>" />
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CategoryDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CategoryDetails.SaveButton.ToolTip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CategoryDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CategoryDetails.DeleteButton.ToolTip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="CategoryTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlCategoryInfo" HeaderText="<% $NopResources: Admin.CategoryDetails.CategoryInfo%>">
        <ContentTemplate>
            <nopCommerce:CategoryInfo ID="ctrlCategoryInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCategorySEO" HeaderText="<% $NopResources: Admin.CategoryDetails.SEO%>">
        <ContentTemplate>
            <nopCommerce:CategorySEO ID="ctrlCategorySEO" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductCategoryMappings" HeaderText="<% $NopResources:Admin.CategoryDetails.Products %>">
        <ContentTemplate>
            <nopCommerce:CategoryProduct ID="ctrlCategoryProduct" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlDiscountMappings" HeaderText="<% $NopResources:Admin.CategoryDetails.Discounts %>">
        <ContentTemplate>
            <nopCommerce:CategoryDiscount ID="ctrlCategoryDiscount" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
