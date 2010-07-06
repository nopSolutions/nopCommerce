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
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.ForumGroupAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.ForumGroupAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ForumGroupInfo ID="ctrlForumGroupInfo" runat="server" />
