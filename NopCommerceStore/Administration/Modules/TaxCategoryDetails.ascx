<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.TaxCategoryDetailsControl"
    CodeBehind="TaxCategoryDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="TaxCategoryInfo" Src="TaxCategoryInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.TaxCategoryDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.TaxCategoryDetails.Title")%>
        <a href="TaxCategories.aspx" title="<%=GetLocaleResourceString("Admin.TaxCategoryDetails.BackToClasses")%>">
            (<%=GetLocaleResourceString("Admin.TaxCategoryDetails.BackToClasses")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TaxCategoryDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.TaxCategoryDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TaxCategoryDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.TaxCategoryDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:TaxCategoryInfo ID="ctrlTaxCategoryInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />