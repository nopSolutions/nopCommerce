<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.AddressEdit"
    CodeBehind="AddressEdit.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="~/Modules/EmailTextBox.ascx" %>
<table>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.FirstName")%>:
        </td>
        <td>
            <nopCommerce:SimpleTextBox runat="server" ID="txtFirstName" ErrorMessage="<% $NopResources:Address.FirstNameIsRequired %>">
            </nopCommerce:SimpleTextBox>
            <asp:Label ID="lblShippingAddressId" runat="server" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.LastName")%>:
        </td>
        <td>
            <nopCommerce:SimpleTextBox runat="server" ID="txtLastName" ErrorMessage="<% $NopResources:Address.LastNameIsRequired %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.PhoneNumber")%>:
        </td>
        <td>
            <nopCommerce:SimpleTextBox runat="server" ID="txtPhoneNumber" ErrorMessage="<% $NopResources:Address.PhoneNumberIsRequired %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.Email")%>:
        </td>
        <td>
            <nopCommerce:EmailTextBox runat="server" ID="txtEmail"></nopCommerce:EmailTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.FaxNumber")%>:
        </td>
        <td>
            <asp:TextBox ID="txtFaxNumber" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.Company")%>:
        </td>
        <td>
            <asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.Address1")%>:
        </td>
        <td>
            <nopCommerce:SimpleTextBox runat="server" ID="txtAddress1" ErrorMessage="<% $NopResources:Address.StreetAddressIsRequired %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.Address2")%>:
        </td>
        <td>
            <asp:TextBox ID="txtAddress2" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.City")%>:
        </td>
        <td>
            <nopCommerce:SimpleTextBox runat="server" ID="txtCity" ErrorMessage="<% $NopResources:Address.CityIsRequired %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.Country")%>:
        </td>
        <td>
            <asp:DropDownList ID="ddlCountry" AutoPostBack="True" runat="server" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged"
                Width="137px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.StateProvince")%>:
        </td>
        <td>
            <asp:DropDownList ID="ddlStateProvince" AutoPostBack="False" runat="server" Width="137px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <%=GetLocaleResourceString("Address.ZipPostalCode")%>:
        </td>
        <td>
            <nopCommerce:SimpleTextBox runat="server" ID="txtZipPostalCode" ErrorMessage="<% $NopResources:Address.ZipPostalCodeIsRequired %>">
            </nopCommerce:SimpleTextBox>
        </td>
    </tr>
</table>
