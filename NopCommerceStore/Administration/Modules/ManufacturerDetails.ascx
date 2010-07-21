<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ManufacturerDetailsControl"
    CodeBehind="ManufacturerDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerInfo" Src="ManufacturerInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerSEO" Src="ManufacturerSEO.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerProducts" Src="ManufacturerProducts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ManufacturerDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ManufacturerDetails.Title")%>
        <a href="Manufacturers.aspx" title="<%=GetLocaleResourceString("Admin.ManufacturerDetails.BackToManufacturers")%>">
            (<%=GetLocaleResourceString("Admin.ManufacturerDetails.BackToManufacturers")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="PreviewButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ManufacturerDetails.PreviewButton.Text %>"
            ToolTip="<% $NopResources:Admin.ManufacturerDetails.PreviewButton.ToolTip %>" />
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ManufacturerDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ManufacturerDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ManufacturerDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ManufacturerDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="ManufacturerTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlManufacturerInfo" HeaderText="<% $NopResources:Admin.ManufacturerDetails.ManufacturerInfo %>">
        <ContentTemplate>
            <nopCommerce:ManufacturerInfo ID="ctrlManufacturerInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlManufacturerSEO" HeaderText="<% $NopResources:Admin.ManufacturerDetails.SEO %>">
        <ContentTemplate>
            <nopCommerce:ManufacturerSEO ID="ctrlManufacturerSEO" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductManufacturerMappings" HeaderText="<% $NopResources:Admin.ManufacturerDetails.Products %>">
        <ContentTemplate>
            <nopCommerce:ManufacturerProducts ID="ctrlManufacturerProducts" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
