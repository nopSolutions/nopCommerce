<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CheckoutAttributeAddControl"
    CodeBehind="CheckoutAttributeAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributeInfo" Src="CheckoutAttributeInfo.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CheckoutAttributeValues" Src="CheckoutAttributeValues.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.CheckoutAttributeAdd.AddNew")%>" />
        <%=GetLocaleResourceString("Admin.CheckoutAttributeAdd.AddNew")%>
        <a href="checkoutattributes.aspx" title="<%=GetLocaleResourceString("Admin.GetLocaleResourceString.BackToList")%>">
            (<%=GetLocaleResourceString("Admin.GetLocaleResourceString.BackToList")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.CheckoutAttributeAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CheckoutAttributeAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="AttributeTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlAttributeInfo" HeaderText="<% $NopResources:Admin.CheckoutAttributeAdd.AttributeInfo %>">
        <ContentTemplate>
            <nopCommerce:CheckoutAttributeInfo ID="ctrlCheckoutAttributeInfo" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlValues" HeaderText="<% $NopResources:Admin.CheckoutAttributeAdd.Values %>">
        <ContentTemplate>
            <nopCommerce:CheckoutAttributeValues ID="ctrlCheckoutAttributeValues" runat="server" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>