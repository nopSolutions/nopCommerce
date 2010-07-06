<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ManufacturerTemplateDetailsControl"
    CodeBehind="ManufacturerTemplateDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerTemplateInfo" Src="ManufacturerTemplateInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ManufacturerTemplateDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ManufacturerTemplateDetails.Title")%>
        <a href="ManufacturerTemplates.aspx" title="<%=GetLocaleResourceString("Admin.ManufacturerTemplateDetails.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.ManufacturerTemplateDetails.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ManufacturerTemplateDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ManufacturerTemplateDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ManufacturerTemplateDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ManufacturerTemplateDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ManufacturerTemplateInfo ID="ctrlManufacturerTemplateInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
