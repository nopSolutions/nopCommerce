<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.EmailAccountDetailsControl"
    CodeBehind="EmailAccountDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailAccountInfo" Src="EmailAccountInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.EmailAccountDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.EmailAccountDetails.Title")%><a href="EmailAccounts.aspx"
            title="<%=GetLocaleResourceString("Admin.EmailAccountDetails.BackTo")%>"> (<%=GetLocaleResourceString("Admin.EmailAccountDetails.BackTo")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.EmailAccountDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.EmailAccountDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" />
    </div>
</div>
<nopCommerce:EmailAccountInfo ID="ctrlEmailAccountInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
