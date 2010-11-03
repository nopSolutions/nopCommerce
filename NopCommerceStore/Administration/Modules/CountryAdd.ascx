<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CountryAddControl"
    CodeBehind="CountryAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CountryInfo" Src="CountryInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.CountryAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.CountryAdd.Title")%>
        <a href="Countries.aspx" title="<%=GetLocaleResourceString("Admin.CountryAdd.BackToCountries")%>">
            (<%=GetLocaleResourceString("Admin.CountryAdd.BackToCountries")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.CountryAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CountryAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CountryAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:CountryInfo ID="ctrlCountryInfo" runat="server" />
