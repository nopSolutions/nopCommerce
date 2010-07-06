<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductAttributeAddControl"
    CodeBehind="ProductAttributeAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributeInfo" Src="ProductAttributeInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.ProductAttributeAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ProductAttributeAdd.Title")%>
        <a href="ProductAttributes.aspx" title="<%=GetLocaleResourceString("Admin.ProductAttributeAdd.BackToAttributeList")%>">
            (<%=GetLocaleResourceString("Admin.ProductAttributeAdd.BackToAttributeList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.ProductAttributeAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.ProductAttributeAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ProductAttributeInfo ID="ctrlProductAttributeInfo" runat="server" />
