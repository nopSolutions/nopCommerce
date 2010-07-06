<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CountryDetailsControl"
    CodeBehind="CountryDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CountryInfo" Src="CountryInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.CountryDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.CountryDetails.Title")%>
        <a href="Countries.aspx" title="<%=GetLocaleResourceString("Admin.CountryDetails.BackToCountries")%>">
            (<%=GetLocaleResourceString("Admin.CountryDetails.BackToCountries")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CountryDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CountryDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CountryDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CountryDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:CountryInfo ID="ctrlCountryInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
