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
        <asp:Button ID="SaveButton" runat="server" Text="<% $NopResources:Admin.CustomerRoleAdd.SaveButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.CustomerRoleAdd.SaveButton.Tooltip %>" />
        <asp:Button ID="SaveAndStayButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.CustomerRoleAdd.SaveAndStayButton.Text %>"
            OnClick="SaveAndStayButton_Click" />
    </div>
</div>
<nopCommerce:CustomerRoleInfo ID="ctrlCustomerRoleInfo" runat="server" />
