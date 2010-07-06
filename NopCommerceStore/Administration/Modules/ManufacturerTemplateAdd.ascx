<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ManufacturerTemplateAddControl"
    CodeBehind="ManufacturerTemplateAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerTemplateInfo" Src="ManufacturerTemplateInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ManufacturerTemplateAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ManufacturerTemplateAdd.Title")%>
        <a href="ManufacturerTemplates.aspx" title="<%=GetLocaleResourceString("Admin.ManufacturerTemplateAdd.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.ManufacturerTemplateAdd.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.ManufacturerTemplateAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.ManufacturerTemplateAdd.AddButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ManufacturerTemplateInfo ID="ctrlManufacturerTemplateInfo" runat="server" />
