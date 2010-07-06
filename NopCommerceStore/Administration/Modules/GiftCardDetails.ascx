<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.GiftCardDetailsControl"
    CodeBehind="GiftCardDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="GiftCardInfo" Src="GiftCardInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.GiftCardDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.GiftCardDetails.Title")%>
        <a href="PurchasedGiftCards.aspx" title="<%=GetLocaleResourceString("Admin.GiftCardDetails.BackToGiftCards")%>">
            (<%=GetLocaleResourceString("Admin.GiftCardDetails.BackToGiftCards")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.GiftCardDetails.SaveButton %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.GiftCardDetails.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:GiftCardInfo ID="ctrlGiftCardInfo" runat="server" />