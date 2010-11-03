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
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.LocaleStringResourceAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.LocaleStringResourceAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LocaleStringResourceAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:LocaleStringResourceInfo ID="ctrlLocaleStringResourceInfo" runat="server" />
