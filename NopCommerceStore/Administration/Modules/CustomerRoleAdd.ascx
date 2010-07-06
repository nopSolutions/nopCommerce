<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerRoleAddControl"
    CodeBehind="CustomerRoleAdd.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerRoleInfo" Src="CustomerRoleInfo.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-customers.png" alt="<%=GetLocaleResourceString("Admin.CustomerRoleAdd.Title")%>" />
        <%=GetLocaleResourceString("Admin.CustomerRoleAdd.Title")%>
        <a href="CustomerRoles.aspx" title="<%=GetLocaleResourceString("Admin.CustomerRoleAdd.BackToCustomerRoles")%>">
            (<%=GetLocaleResourceString("Admin.CustomerRoleAdd.BackToCustomerRoles")%>)</a>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.CustomerRoleAdd.AddButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.CustomerRoleAdd.AddButton.Tooltip %>" />
    </div>
</div>
<nopCommerce:CustomerRoleInfo ID="ctrlCustomerRoleInfo" runat="server" />
