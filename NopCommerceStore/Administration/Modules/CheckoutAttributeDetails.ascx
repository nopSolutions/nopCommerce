<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CheckoutAttributeDetailsControl"
    CodeBehind="CheckoutAttributeDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributeInfo" Src="CheckoutAttributeInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributeValues" Src="CheckoutAttributeValues.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.CheckoutAttributeDetails.Edit")%>" />
        <%=GetLocaleResourceString("Admin.CheckoutAttributeDetails.Edit")%>
        <a href="checkoutattributes.aspx" title="<%=GetLocaleResourceString("Admin.CheckoutAttributeDetails.BackToList")%>">
            (<%=GetLocaleResourceString("Admin.CheckoutAttributeDetails.BackToList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CheckoutAttributeDetails.SaveButton.Text %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CheckoutAttributeDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CheckoutAttributeDetails.DeleteButton.Text %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CheckoutAttributeDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="AttributeTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlAttributeInfo" HeaderText="<% $NopResources:Admin.CheckoutAttributeDetails.AttributeInfo %>">
        <ContentTemplate>
            <nopCommerce:CheckoutAttributeInfo ID="ctrlCheckoutAttributeInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlValues" HeaderText="<% $NopResources:Admin.CheckoutAttributeDetails.Values %>">
        <ContentTemplate>
            <nopCommerce:CheckoutAttributeValues ID="ctrlCheckoutAttributeValues" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
