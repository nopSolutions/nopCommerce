<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerAddControl"
    CodeBehind="CustomerAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerInfo" Src="CustomerInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerRoleMappings" Src="CustomerRoleMappings.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-customers.png" alt="<%=GetLocaleResourceString("Admin.CustomerAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.CustomerAdd.Title")%>
        <a href="Customers.aspx" title="<%=GetLocaleResourceString("Admin.CustomerAdd.BackToCustomers")%>">
            (<%=GetLocaleResourceString("Admin.CustomerAdd.BackToCustomers")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.CustomerAdd.SaveButton %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ProductAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="CustomerTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerInfo" HeaderText="<% $NopResources:Admin.CustomerAdd.CustomerInfo %>">
        <ContentTemplate>
            <nopCommerce:CustomerInfo ID="ctrlCustomerInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRoleMappings" HeaderText="<% $NopResources:Admin.CustomerAdd.Roles %>">
        <ContentTemplate>
            <nopCommerce:CustomerRoleMappings runat="server" ID="ctrlCustomerRoleMappings"></nopCommerce:CustomerRoleMappings>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>

