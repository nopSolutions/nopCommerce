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
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.TopicAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.TopicAdd.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TopicAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:TopicInfo ID="ctrlTopicInfo" runat="server" />
