<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LocaleStringResourceAddControl"
    CodeBehind="LocaleStringResourceAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="LocaleStringResourceInfo" Src="LocaleStringResourceInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.LocaleStringResourceAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.LocaleStringResourceAdd.Title")%>
        <asp:HyperLink runat="server" ID="hlBackToResources" NavigateUrl="~/Administration/LocaleStringResources.aspx"
            Text="<% $NopResources:Admin.LocaleStringResourceAdd.BackToResources %>" ToolTip="<% $NopResources:Admin.LocaleStringResourceAdd.BackToResources %>" />
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.LocaleStringResourceAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.LocaleStringResourceAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:LocaleStringResourceInfo ID="ctrlLocaleStringResourceInfo" runat="server" />
