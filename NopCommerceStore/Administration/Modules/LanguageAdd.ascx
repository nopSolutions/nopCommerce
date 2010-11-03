<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LanguageAddControl"
    CodeBehind="LanguageAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="LanguageInfo" Src="LanguageInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.LanguageAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.LanguageAdd.Title")%>
        <a href="Languages.aspx" title="<%=GetLocaleResourceString("Admin.LanguageAdd.BackToLanguages")%>">
            (<%=GetLocaleResourceString("Admin.LanguageAdd.BackToLanguages")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.LanguageAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.LanguageAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LanguageAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:LanguageInfo ID="ctrlLanguageInfo" runat="server" />
