<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ForumDetailsControl"
    CodeBehind="ForumDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumInfo" Src="ForumInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.ForumDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ForumDetails.Title")%>
        <a href="Forums.aspx" title="<%=GetLocaleResourceString("Admin.ForumDetails.BackToForums")%>">
            (<%=GetLocaleResourceString("Admin.ForumDetails.BackToForums")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ForumDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ForumDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ForumDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ForumDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ForumInfo ID="ctrlForumInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
