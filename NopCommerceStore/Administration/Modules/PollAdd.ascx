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
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.PollAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.PollAdd.AddButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.PollAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:PollInfo ID="ctrlPollInfo" runat="server" />
