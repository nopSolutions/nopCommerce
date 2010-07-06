<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.NewsAddControl"
    CodeBehind="NewsAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NewsInfo" Src="NewsInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.NewsAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.NewsAdd.Title")%>
        <a href="News.aspx" title="<%=GetLocaleResourceString("Admin.NewsAdd.BackToNews")%>">
            (<%=GetLocaleResourceString("Admin.NewsAdd.BackToNews")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.NewsAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.NewsAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:NewsInfo ID="ctrlNewsInfo" runat="server" />
