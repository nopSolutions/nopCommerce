<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PollDetailsControl"
    CodeBehind="PollDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PollInfo" Src="PollInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.PollDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.PollDetails.Title")%>
        <a href="Polls.aspx" title="<%=GetLocaleResourceString("Admin.PollDetails.BackToPolls")%>">
            (<%=GetLocaleResourceString("Admin.PollDetails.BackToPolls")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.PollDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.PollDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.PollDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.PollDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:PollInfo ID="ctrlPollInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />