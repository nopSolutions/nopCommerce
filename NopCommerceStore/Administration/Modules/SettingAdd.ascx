<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SettingAddControl"
    CodeBehind="SettingAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SettingInfo" Src="SettingInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.SettingAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.SettingAdd.Title")%><a href="Settings.aspx" title="<%=GetLocaleResourceString("Admin.SettingAdd.BackToSettings")%>">
            (<%=GetLocaleResourceString("Admin.SettingAdd.BackToSettings")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.SettingAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.SettingAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:SettingInfo runat="server" ID="ctrlSettingInfo" />
