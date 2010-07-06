<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.LocaleStringResourceDetailsControl"
    CodeBehind="LocaleStringResourceDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="LocaleStringResourceInfo" Src="LocaleStringResourceInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-content.png" alt="<%=GetLocaleResourceString("Admin.LocaleStringResourceDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.LocaleStringResourceDetails.Title")%>
        <asp:HyperLink runat="server" ID="hlBackToResources" NavigateUrl="~/Administration/LocaleStringResources.aspx"
            Text="<% $NopResources:Admin.LocaleStringResourceDetails.BackToResources %>"
            ToolTip="<% $NopResources:Admin.LocaleStringResourceDetails.BackToResources %>" />
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LocaleStringResourceDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.LocaleStringResourceDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.LocaleStringResourceDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.LocaleStringResourceDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:LocaleStringResourceInfo ID="ctrlLocaleStringResourceInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
