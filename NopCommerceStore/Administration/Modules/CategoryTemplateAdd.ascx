<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryTemplateAddControl"
    CodeBehind="CategoryTemplateAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CategoryTemplateInfo" Src="CategoryTemplateInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.CategoryTemplateAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.CategoryTemplateAdd.Title")%>
        <a href="CategoryTemplates.aspx" title="<%=GetLocaleResourceString("Admin.CategoryTemplateAdd.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.CategoryTemplateAdd.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.CategoryTemplateAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CategoryTemplateAdd.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CategoryTemplateAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:CategoryTemplateInfo ID="ctrlCategoryTemplateInfo" runat="server" />
