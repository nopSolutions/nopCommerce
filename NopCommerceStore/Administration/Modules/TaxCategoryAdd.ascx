<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TaxCategoryAddControl"
    CodeBehind="TaxCategoryAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="TaxCategoryInfo" Src="TaxCategoryInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.TaxCategoryAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.TaxCategoryAdd.Title")%>
        <a href="TaxCategories.aspx" title="<%=GetLocaleResourceString("Admin.TaxCategoryAdd.BackToClasses")%>">
            (<%=GetLocaleResourceString("Admin.TaxCategoryAdd.BackToClasses")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.TaxCategoryAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.TaxCategoryAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:TaxCategoryInfo ID="ctrlTaxCategoryInfo" runat="server" />
