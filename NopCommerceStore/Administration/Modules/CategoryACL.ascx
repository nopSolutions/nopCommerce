<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CategoryACLControl"
    CodeBehind="CategoryACL.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCustomerRolesControl" Src="SelectCustomerRolesControl.ascx" %>

<b><%=GetLocaleResourceString("Admin.CategoryACL.Note")%></b>
<br />
<br />
<%=GetLocaleResourceString("Admin.CategoryACL.MarkRequired")%>
<br />
<br />
<table class="adminContent">
    <tr>
        <td>
            <nopCommerce:SelectCustomerRolesControl ID="ctrlRoles" runat="server" CssClass="adminInput" />
        </td>
    </tr>
</table>