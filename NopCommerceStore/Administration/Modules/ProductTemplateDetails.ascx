<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductTemplateDetailsControl"
    CodeBehind="ProductTemplateDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductTemplateInfo" Src="ProductTemplateInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ProductTemplateDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ProductTemplateDetails.Title")%>
        <a href="ProductTemplates.aspx" title="<%=GetLocaleResourceString("Admin.ProductTemplateDetails.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.ProductTemplateDetails.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductTemplateDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ProductTemplateDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductTemplateDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ProductTemplateDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ProductTemplateInfo ID="ctrlProductTemplateInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
