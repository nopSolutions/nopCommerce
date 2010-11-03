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
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ManufacturerTemplateAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ManufacturerTemplateAdd.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ManufacturerTemplateAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:ManufacturerTemplateInfo ID="ctrlManufacturerTemplateInfo" runat="server" />
