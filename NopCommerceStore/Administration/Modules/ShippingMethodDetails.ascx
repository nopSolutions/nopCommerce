<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ShippingMethodDetailsControl"
    CodeBehind="ShippingMethodDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ShippingMethodInfo" Src="ShippingMethodInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingMethodDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.ShippingMethodDetails.Title")%>
        <a href="ShippingMethods.aspx" title="<%=GetLocaleResourceString("Admin.ShippingMethodDetails.BackToMethods")%>">
            (<%=GetLocaleResourceString("Admin.ShippingMethodDetails.BackToMethods")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ShippingMethodDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ShippingMethodDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ShippingMethodDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ShippingMethodDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ShippingMethodInfo ID="ctrlShippingMethodInfo" runat="server" />
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
