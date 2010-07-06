<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlacklistIPAdd.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistIPAddControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlacklistIPInfo" Src="BlacklistIPInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-blacklist.png" alt="<%=GetLocaleResourceString("Admin.BlacklistIPAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlacklistIPAdd.Title")%>
        <a href="Blacklist.aspx" title="<%=GetLocaleResourceString("Admin.BlacklistIPAdd.BackToBlacklist")%>">
            (<%=GetLocaleResourceString("Admin.BlacklistIPAdd.BackToBlacklist")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlacklistIPAdd.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.BlacklistIPAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<p>
</p>
<nopCommerce:BlacklistIPInfo ID="ctrlBlacklist" runat="server" />
