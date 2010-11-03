<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumAddControl"
    CodeBehind="ForumAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumInfo" Src="ForumInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ForumAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ForumAdd.Title")%>
        <a href="Forums.aspx" title="<%=GetLocaleResourceString("Admin.ForumAdd.BackToForums")%>">
            (<%=GetLocaleResourceString("Admin.ForumAdd.BackToForums")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ForumAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ForumAdd.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ForumAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:ForumInfo ID="ctrlForumInfo" runat="server" />
