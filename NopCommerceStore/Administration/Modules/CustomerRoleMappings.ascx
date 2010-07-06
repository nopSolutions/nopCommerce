<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerRoleMappingsControl"
    CodeBehind="CustomerRoleMappings.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCustomerRolesControl" Src="SelectCustomerRolesControl.ascx" %>
<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:SelectCustomerRolesControl ID="CustomerRoleMappingControl" runat="server"
                CssClass="adminInput"></nopCommerce:SelectCustomerRolesControl>
        </td>
    </tr>
</table>
