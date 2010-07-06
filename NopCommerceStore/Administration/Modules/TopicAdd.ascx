<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TopicAddControl"
    CodeBehind="TopicAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="TopicInfo" Src="TopicInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.TopicAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.TopicAdd.Title")%><a href="Topics.aspx" title="<%=GetLocaleResourceString("Admin.TopicAdd.BackToTopics")%>">
            (<%=GetLocaleResourceString("Admin.TopicAdd.BackToTopics")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.TopicAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.TopicAdd.AddButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:TopicInfo ID="ctrlTopicInfo" runat="server" />
