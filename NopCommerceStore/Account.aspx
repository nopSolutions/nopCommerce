<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.AccountPage" CodeBehind="Account.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerInfo" Src="~/Modules/CustomerInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerAddresses" Src="~/Modules/CustomerAddresses.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerOrders" Src="~/Modules/CustomerOrders.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerReturnRequests" Src="~/Modules/CustomerReturnRequests.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerChangePassword" Src="~/Modules/CustomerChangePassword.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerAvatar" Src="~/Modules/CustomerAvatar.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerRewardPoints" Src="~/Modules/CustomerRewardPoints.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerDownloadableProducts" Src="~/Modules/CustomerDownloadableProducts.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumSubscriptions" Src="~/Modules/ForumSubscriptions.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="account-page">
        <div class="page-title">
            <h1><%=GetLocaleResourceString("Account.MyAccount")%></h1>
        </div>
        <div class="clear">
        </div>
        <div class="body">
            <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
                EnableScriptLocalization="true" ID="sm1" ScriptMode="Release" CompositeScript-ScriptMode="Release" />
            <ajaxToolkit:TabContainer runat="server" ID="CustomerTabs" ActiveTabIndex="0" CssClass="grey">
                <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerInfo" HeaderText="<% $NopResources:Account.CustomerInfo %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerInfo ID="ctrlCustomerInfo" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerAddresses" HeaderText="<% $NopResources:Account.CustomerAddresses %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerAddresses ID="ctrlCustomerAddresses" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerOrders" HeaderText="<% $NopResources:Account.CustomerOrders %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerOrders ID="ctrlCustomerOrders" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
               <ajaxToolkit:TabPanel runat="server" ID="pnlReturnRequests" HeaderText="<% $NopResources:Account.CustomerReturnRequests %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerReturnRequests ID="ctrlReturnRequests" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlDP" HeaderText="<% $NopResources:Account.DownloadableProducts %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerDownloadableProducts ID="ctrlDP" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlRewardPoints" HeaderText="<% $NopResources:Account.RewardPoints %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerRewardPoints ID="ctrlRewardPoints" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlChangePassword" HeaderText="<% $NopResources:Account.ChangePassword %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerChangePassword ID="ctrlCustomerChangePassword" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlAvatar" HeaderText="<% $NopResources:Account.Avatar %>">
                    <ContentTemplate>
                        <nopCommerce:CustomerAvatar ID="ctrlCustomerAvatar" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="pnlForumSubscriptions" HeaderText="<% $NopResources:Account.ForumSubscriptions %>">
                    <ContentTemplate>
                        <nopCommerce:ForumSubscriptions ID="ctrForumSubscriptions" runat="server" />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>
    </div>
</asp:Content>
