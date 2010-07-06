<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SettingDetailsControl"
    CodeBehind="SettingDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SettingInfo" Src="SettingInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.SettingDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.SettingDetails.Title")%>
        <a href="Settings.aspx" title="<%=GetLocaleResourceString("Admin.SettingDetails.BackToSettings")%>">
            (<%=GetLocaleResourceString("Admin.SettingDetails.BackToSettings")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.SettingDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.SettingDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.SettingDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.SettingDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:SettingInfo runat="server" ID="ctrlSettingInfo" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
