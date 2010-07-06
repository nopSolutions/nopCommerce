<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlacklistIPDetails.ascx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BlacklistIPDetailsControl" %>
<%@ Register TagPrefix="nopCommerce" TagName="BlacklistIPInfo" Src="BlacklistIPInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-blacklist.png" alt="<%=GetLocaleResourceString("Admin.BlacklistIPDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.BlacklistIPDetails.Title")%>
        <a href="Blacklist.aspx" title="<%=GetLocaleResourceString("Admin.BlacklistIPDetails.BackToBlacklist")%>">
            (<%=GetLocaleResourceString("Admin.BlacklistIPDetails.BackToBlacklist")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlacklistIPDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.BlacklistIPDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.BlacklistIPDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.BlacklistIPDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<p>
</p>
<nopCommerce:BlacklistIPInfo ID="ctrlBlacklist" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />