<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TopicDetailsControl"
    CodeBehind="TopicDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="TopicInfo" Src="TopicInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.TopicDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.TopicDetails.Title")%><a href="Topics.aspx" title="<%=GetLocaleResourceString("Admin.TopicDetails.BackToTopics")%>">
            (<%=GetLocaleResourceString("Admin.TopicDetails.BackToTopics")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TopicDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.TopicDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TopicDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.TopicDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:TopicInfo ID="ctrlTopicInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />