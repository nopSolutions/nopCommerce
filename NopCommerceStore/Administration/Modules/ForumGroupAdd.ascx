<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumGroupAddControl"
    CodeBehind="ForumGroupAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumGroupInfo" Src="ForumGroupInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ForumGroupAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ForumGroupAdd.Title")%>
        <a href="Forums.aspx" title="<%=GetLocaleResourceString("Admin.ForumGroupAdd.BackToForums")%>">
            (<%=GetLocaleResourceString("Admin.ForumGroupAdd.BackToForums")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ForumGroupAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ForumGroupAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ForumGroupAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:ForumGroupInfo ID="ctrlForumGroupInfo" runat="server" />
