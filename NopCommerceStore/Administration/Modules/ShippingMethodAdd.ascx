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
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.ShippingMethodAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.ShippingMethodAdd.SaveButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:ShippingMethodInfo ID="ctrlShippingMethodInfo" runat="server" />
