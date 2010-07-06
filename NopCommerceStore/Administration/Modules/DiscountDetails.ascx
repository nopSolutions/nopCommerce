<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.DiscountDetailsControl"
    CodeBehind="DiscountDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DiscountInfo" Src="DiscountInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-promotions.png" alt="<%=GetLocaleResourceString("Admin.DiscountDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.DiscountDetails.Title")%>
        <a href="Discounts.aspx" title="<%=GetLocaleResourceString("Admin.DiscountDetails.BackToDiscounts")%>">
            (<%=GetLocaleResourceString("Admin.DiscountDetails.BackToDiscounts")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.DiscountDetails.SaveButton %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.DiscountDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.DiscountDetails.DeleteButton %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.DiscountDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:DiscountInfo ID="ctrlDiscountInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
