<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ShippingMethodAddControl"
    CodeBehind="ShippingMethodAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ShippingMethodInfo" Src="ShippingMethodInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingMethodAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.ShippingMethodAdd.Title")%>
        <a href="ShippingMethods.aspx" title="<%=GetLocaleResourceString("Admin.ShippingMethodAdd.BackToMethods")%>">
            (<%=GetLocaleResourceString("Admin.ShippingMethodAdd.BackToMethods")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.ShippingMethodAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ShippingMethodAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ShippingMethodAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:ShippingMethodInfo ID="ctrlShippingMethodInfo" runat="server" />
