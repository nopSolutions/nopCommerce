<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerDetailsControl"
    CodeBehind="CustomerDetails.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerInfo" Src="CustomerInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerBillingAddresses" Src="CustomerBillingAddresses.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerShippingAddresses" Src="CustomerShippingAddresses.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerAvatar" Src="CustomerAvatar.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerOrders" Src="CustomerOrders.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerRoleMappings" Src="CustomerRoleMappings.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerRewardPoints" Src="CustomerRewardPoints.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerShoppingCart" Src="CustomerShoppingCart.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerWishlist" Src="CustomerWishlist.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerPlaceOrder" Src="CustomerPlaceOrder.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerSendEmail" Src="CustomerSendEmail.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerSendPrivateMessage" Src="CustomerSendPrivateMessage.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerForumSubscriptions" Src="CustomerForumSubscriptions.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-customers.png" alt="<%=GetLocaleResourceString("Admin.CustomerDetails.Title")%>" />
        <%=GetLocaleResourceString("Admin.CustomerDetails.Title")%>
        <a href="Customers.aspx" title="<%=GetLocaleResourceString("Admin.CustomerDetails.BackToCustomers")%>">
            (<%=GetLocaleResourceString("Admin.CustomerDetails.BackToCustomers")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CustomerDetails.SaveButton %>"
            OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CustomerDetails.SaveButton.Tooltip %>" />
        <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CustomerDetails.DeleteButton %>"
            OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.CustomerDetails.DeleteButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="CustomerTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerInfo" HeaderText="<% $NopResources:Admin.CustomerDetails.CustomerInfo %>">
        <ContentTemplate>
            <nopCommerce:CustomerInfo runat="server" ID="ctrlCustomerInfo"></nopCommerce:CustomerInfo>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerBillingAddresses" HeaderText="<% $NopResources:Admin.CustomerDetails.BillingAddresses %>">
        <ContentTemplate>
            <nopCommerce:CustomerBillingAddresses runat="server" ID="ctrlCustomerBillingAddresses">
            </nopCommerce:CustomerBillingAddresses>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerShippingAddresses" HeaderText="<% $NopResources:Admin.CustomerDetails.ShippingAddresses %>">
        <ContentTemplate>
            <nopCommerce:CustomerShippingAddresses runat="server" ID="ctrlCustomerShippingAddresses">
            </nopCommerce:CustomerShippingAddresses>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRoleMappings" HeaderText="<% $NopResources:Admin.CustomerDetails.Roles %>">
        <ContentTemplate>
            <nopCommerce:CustomerRoleMappings runat="server" ID="ctrlCustomerRoleMappings"></nopCommerce:CustomerRoleMappings>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRewardPoints" HeaderText="<% $NopResources:Admin.CustomerDetails.RewardPoints %>">
        <ContentTemplate>
            <nopCommerce:CustomerRewardPoints runat="server" ID="ctrlCustomerRewardPoints"></nopCommerce:CustomerRewardPoints>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerOrders" HeaderText="<% $NopResources:Admin.CustomerDetails.Orders %>">
        <ContentTemplate>
            <nopCommerce:CustomerOrders runat="server" ID="ctrlCustomerOrders"></nopCommerce:CustomerOrders>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerPlaceOrder" HeaderText="<% $NopResources:Admin.CustomerDetails.PlaceOrder %>">
        <ContentTemplate>
            <nopCommerce:CustomerPlaceOrder runat="server" ID="ctrlCustomerPlaceOrder"></nopCommerce:CustomerPlaceOrder>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerShoppingCart" HeaderText="<% $NopResources:Admin.CustomerDetails.CurrentCart %>">
        <ContentTemplate>
            <nopCommerce:CustomerShoppingCart runat="server" ID="ctrlCurrentShoppingCart"></nopCommerce:CustomerShoppingCart>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerWishlist" HeaderText="<% $NopResources:Admin.CustomerDetails.CurrentWishlist %>">
        <ContentTemplate>
            <nopCommerce:CustomerWishlist runat="server" ID="ctrlCurrentWishlist"></nopCommerce:CustomerWishlist>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerAvatar" HeaderText="<% $NopResources:Admin.CustomerDetails.CustomerAvatar %>">
        <ContentTemplate>
            <nopCommerce:CustomerAvatar runat="server" ID="ctrlCustomerAvatar"></nopCommerce:CustomerAvatar>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerSendEmail" HeaderText="<% $NopResources:Admin.CustomerDetails.CustomerSendEmail %>">
        <ContentTemplate>
            <nopCommerce:CustomerSendEmail runat="server" ID="ctrlCustomerSendEmail"></nopCommerce:CustomerSendEmail>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerSendPrivateMessage" HeaderText="<% $NopResources:Admin.CustomerDetails.CustomerSendPrivateMessage %>">
        <ContentTemplate>
            <nopCommerce:CustomerSendPrivateMessage runat="server" ID="ctrlCustomerSendPrivateMessage"></nopCommerce:CustomerSendPrivateMessage>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerForumSubscriptions" HeaderText="<% $NopResources:Admin.CustomerDetails.CustomerForumSubscriptions %>">
        <ContentTemplate>
            <nopCommerce:CustomerForumSubscriptions runat="server" ID="ctrlCustomerForumSubscriptions" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
    YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
    ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
