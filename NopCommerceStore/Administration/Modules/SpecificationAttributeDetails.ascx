<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SpecificationAttributeDetailsControl"
    CodeBehind="SpecificationAttributeDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributeInfo" Src="SpecificationAttributeInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SpecificationAttributeOptions" Src="SpecificationAttributeOptions.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.SpecificationAttributeDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.SpecificationAttributeDetails.Title")%>
        <a href="SpecificationAttributes.aspx" title="<%=GetLocaleResourceString("Admin.SpecificationAttributeDetails.BackToAttributeList")%>">
            (<%=GetLocaleResourceString("Admin.SpecificationAttributeDetails.BackToAttributeList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.SpecificationAttributeDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.SpecificationAttributeDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.SpecificationAttributeDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.SpecificationAttributeDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="AttributeTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlAttributeInfo" HeaderText="<% $NopResources:Admin.SpecificationAttributeDetails.AttributeInfo %>">
        <ContentTemplate>
            <nopCommerce:SpecificationAttributeInfo ID="ctrlSpecificationAttributeInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlOptions" HeaderText="<% $NopResources:Admin.SpecificationAttributeDetails.Options %>">
        <ContentTemplate>
            <nopCommerce:SpecificationAttributeOptions ID="ctrlSpecificationAttributeOptions" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
