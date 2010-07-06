<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CurrencyDetailsControl"
    CodeBehind="CurrencyDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CurrencyInfo" Src="CurrencyInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Currencies.CurrencyDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.Currencies.CurrencyDetails.Title")%>
        <a href="Currencies.aspx" title="<%=GetLocaleResourceString("Admin.Currencies.CurrencyDetails.BackToCurrencies")%>">
            (<%=GetLocaleResourceString("Admin.Currencies.CurrencyDetails.BackToCurrencies")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Currencies.CurrencyDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Currencies.CurrencyDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.Currencies.CurrencyDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.Currencies.CurrencyDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:CurrencyInfo ID="ctrlCurrencyInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
