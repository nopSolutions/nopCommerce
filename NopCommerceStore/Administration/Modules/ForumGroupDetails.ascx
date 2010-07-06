<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumGroupDetailsControl"
    CodeBehind="ForumGroupDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumGroupInfo" Src="ForumGroupInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ForumGroupDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ForumGroupDetails.Title")%>
        <a href="Forums.aspx" title="<%=GetLocaleResourceString("Admin.ForumGroupDetails.BackToForums")%>">
            (<%=GetLocaleResourceString("Admin.ForumGroupDetails.BackToForums")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ForumGroupDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ForumGroupDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ForumGroupDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ForumGroupDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ForumGroupInfo ID="ctrlForumGroupInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />