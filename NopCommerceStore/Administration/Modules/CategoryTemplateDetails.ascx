<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryTemplateDetailsControl"
    CodeBehind="CategoryTemplateDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryTemplateInfo" Src="CategoryTemplateInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.CategoryTemplateDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.CategoryTemplateDetails.Title")%>
        <a href="CategoryTemplates.aspx" title="<%=GetLocaleResourceString("Admin.CategoryTemplateDetails.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.CategoryTemplateDetails.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CategoryTemplateDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CategoryTemplateDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CategoryTemplateDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CategoryTemplateDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:CategoryTemplateInfo ID="ctrlCategoryTemplateInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
