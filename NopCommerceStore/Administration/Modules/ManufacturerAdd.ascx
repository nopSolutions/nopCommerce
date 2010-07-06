<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ManufacturerAddControl"
    CodeBehind="ManufacturerAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerInfo" Src="ManufacturerInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerSEO" Src="ManufacturerSEO.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ManufacturerAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ManufacturerAdd.Title")%><a href="Manufacturers.aspx"
            title="<%=GetLocaleResourceString("Admin.ManufacturerAdd.BackToManufacturers")%>">
            (<%=GetLocaleResourceString("Admin.ManufacturerAdd.BackToManufacturers")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ManufacturerAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ManufacturerAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="ManufacturerTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlManufacturerInfo" HeaderText="<% $NopResources:Admin.ManufacturerAdd.ManufacturerInfo %>">
        <ContentTemplate>
            <nopCommerce:ManufacturerInfo ID="ctrlManufacturerInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlManufacturerSEO" HeaderText="<% $NopResources:Admin.ManufacturerAdd.SEO %>">
        <ContentTemplate>
            <nopCommerce:ManufacturerSEO ID="ctrlManufacturerSEO" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>