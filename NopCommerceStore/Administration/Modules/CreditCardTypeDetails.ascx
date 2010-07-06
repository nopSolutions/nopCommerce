<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CreditCardTypeDetailsControl"
    CodeBehind="CreditCardTypeDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CreditCardTypeInfo" Src="CreditCardTypeInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.CreditCardTypeDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.CreditCardTypeDetails.Title")%><a href="CreditCardTypes.aspx"
            title="<%=GetLocaleResourceString("Admin.CreditCardTypeDetails.BackToCards")%>">
            (<%=GetLocaleResourceString("Admin.CreditCardTypeDetails.BackToCards")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CreditCardTypeDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CreditCardTypeDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CreditCardTypeDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CreditCardTypeDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:CreditCardTypeInfo ID="ctrlCreditCardTypeInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
