<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlacklistNetworkDetails.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistNetworkDetailsControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlacklistNetworkInfo" Src="BlacklistNetworkInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-blacklist.png" alt="<%=GetLocaleResourceString("Admin.BlacklistNetworkDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlacklistNetworkDetails.Title")%>
        <a href="Blacklist.aspx" title="<%=GetLocaleResourceString("Admin.BlacklistNetworkDetails.BackToBlacklist")%>">
            (<%=GetLocaleResourceString("Admin.BlacklistNetworkDetails.BackToBlacklist")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlacklistNetworkDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.BlacklistNetworkDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlacklistNetworkDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.BlacklistNetworkDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<p>
</p>
<nopCommerce:BlacklistNetworkInfo ID="ctrlBlacklist" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
