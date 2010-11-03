<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CurrencyAddControl"
    CodeBehind="CurrencyAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CurrencyInfo" Src="CurrencyInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Currencies.CurrencyAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.Currencies.CurrencyAdd.Title")%>
        <a href="Currencies.aspx" title="<%=GetLocaleResourceString("Admin.Currencies.CurrencyAdd.BackToCurrencies")%>">
            (<%=GetLocaleResourceString("Admin.Currencies.CurrencyAdd.BackToCurrencies")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.Currencies.CurrencyAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.Currencies.CurrencyAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CurrencyAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:CurrencyInfo ID="ctrlCurrencyInfo" runat="server" />
