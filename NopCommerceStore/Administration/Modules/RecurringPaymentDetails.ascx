<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.RecurringPaymentDetailsControl"
    CodeBehind="RecurringPaymentDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="RecurringPaymentInfo" Src="RecurringPaymentInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.RecurringPaymentDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.RecurringPaymentDetails.Title")%>
        <a href="RecurringPayments.aspx" title="<%=GetLocaleResourceString("Admin.RecurringPaymentDetails.BackToRecurringPayments")%>">
            (<%=GetLocaleResourceString("Admin.RecurringPaymentDetails.BackToRecurringPayments")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.RecurringPaymentDetails.SaveButton %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.RecurringPaymentDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.RecurringPaymentDetails.DeleteButton %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.RecurringPaymentDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:RecurringPaymentInfo ID="ctrlRecurringPaymentInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
