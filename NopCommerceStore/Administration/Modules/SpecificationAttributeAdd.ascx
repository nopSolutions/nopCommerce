<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SpecificationAttributeAddControl"
    CodeBehind="SpecificationAttributeAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributeInfo" Src="SpecificationAttributeInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributeOptions" Src="SpecificationAttributeOptions.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.SpecificationAttributeAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.SpecificationAttributeAdd.Title")%>
        <a href="SpecificationAttributes.aspx" title="<%=GetLocaleResourceString("Admin.SpecificationAttributeAdd.BackToAttributeList")%>">
            (<%=GetLocaleResourceString("Admin.SpecificationAttributeAdd.BackToAttributeList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.SpecificationAttributeAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.SpecificationAttributeAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="AttributeTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlAttributeInfo" HeaderText="<% $NopResources:Admin.SpecificationAttributeAdd.AttributeInfo %>">
        <ContentTemplate>
            <nopCommerce:SpecificationAttributeInfo ID="ctrlSpecificationAttributeInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlOptions" HeaderText="<% $NopResources:Admin.SpecificationAttributeAdd.Options %>">
        <ContentTemplate>
            <nopCommerce:SpecificationAttributeOptions ID="ctrlSpecificationAttributeOptions" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>