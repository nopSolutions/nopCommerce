<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductTemplateAddControl"
    CodeBehind="ProductTemplateAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductTemplateInfo" Src="ProductTemplateInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ProductTemplateAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ProductTemplateAdd.Title")%><a href="ProductTemplates.aspx"
            title="<%=GetLocaleResourceString("Admin.ProductTemplateAdd.BackToTemplates")%>">
            (<%=GetLocaleResourceString("Admin.ProductTemplateAdd.BackToTemplates")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.ProductTemplateAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.ProductTemplateAdd.AddButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ProductTemplateInfo ID="ctrlProductTemplateInfo" runat="server" />
