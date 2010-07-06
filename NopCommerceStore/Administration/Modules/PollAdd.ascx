<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PollAddControl"
    CodeBehind="PollAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="PollInfo" Src="PollInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.PollAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.PollAdd.Title")%>
        <a href="Polls.aspx" title="<%=GetLocaleResourceString("Admin.PollAdd.BackToPolls")%>">
            (<%=GetLocaleResourceString("Admin.PollAdd.BackToPolls")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.PollAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.PollAdd.AddButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:PollInfo ID="ctrlPollInfo" runat="server" />
