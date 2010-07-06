<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlacklistNetworkAdd.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistNetworkAddControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlacklistNetworkInfo" Src="BlacklistNetworkInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-blacklist.png" alt="<%=GetLocaleResourceString("Admin.BlacklistNetworkAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlacklistNetworkAdd.Title")%>
        <a href="Blacklist.aspx" title="<%=GetLocaleResourceString("Admin.BlacklistNetworkAdd.BackToBlacklist")%>">
            (<%=GetLocaleResourceString("Admin.BlacklistNetworkAdd.BackToBlacklist")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlacklistNetworkAdd.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.BlacklistNetworkAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<p>
</p>
<nopCommerce:BlacklistNetworkInfo ID="ctrlBlacklist" runat="server" />
