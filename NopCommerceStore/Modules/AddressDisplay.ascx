<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.AddressDisplay"
    CodeBehind="AddressDisplay.ascx.cs" %>
<table width="100%" cellspacing="0" cellpadding="2" border="0">
    <tbody>
        <tr>
            <td style="vertical-align: middle; padding-left: 15px;">
                <b>
                    <asp:Literal ID="lFullName" runat="server"></asp:Literal></b>
            </td>
            <td align="right" style="padding: 5px 5px 5px 0px;">
                <%if (ShowEditButton)
                  { %>
                <asp:Button runat="server" ID="btnEditAddress" OnCommand="btnEditAddress_Click" Text="<% $NopResources:Address.Edit %>"
                    ValidationGroup="EditAddress" CssClass="editaddressbutton" />
                <%} %>
                <%if (ShowDeleteButton)
                  { %>
                <asp:Button runat="server" ID="btnDeleteAddress" OnCommand="btnDeleteAddress_Click"
                    Text="<% $NopResources:Address.Delete %>" CausesValidation="false" CssClass="deleteaddressbutton" />
                <%} %>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table cellspacing="0" cellpadding="2" border="0">
                    <tbody>
                        <tr>
                            <td width="10">
                                <img height="1" width="10" border="0" src="<%=Page.ResolveUrl("~/images/sp.gif")%>"
                                    alt="sp" />
                            </td>
                            <td>
                                <asp:Literal ID="lFirstName" runat="server"></asp:Literal>
                                <asp:Literal ID="lLastName" runat="server"></asp:Literal><br />
                                <div>
                                    <%=GetLocaleResourceString("Address.Email")%>:
                                    <asp:Literal ID="lEmail" runat="server"></asp:Literal></div>
                                <div>
                                    <%=GetLocaleResourceString("Address.Phone")%>:
                                    <asp:Literal ID="lPhoneNumber" runat="server"></asp:Literal></div>
                                <div>
                                    <%=GetLocaleResourceString("Address.Fax")%>:
                                    <asp:Literal ID="lFaxNumber" runat="server"></asp:Literal></div>
                                <asp:Panel ID="pnlCompany" runat="server">
                                    <asp:Literal ID="lCompany" runat="server"></asp:Literal></asp:Panel>
                                <div>
                                    <asp:Literal ID="lAddress1" runat="server"></asp:Literal></div>
                                <asp:Panel ID="pnlAddress2" runat="server">
                                    <asp:Literal ID="lAddress2" runat="server"></asp:Literal></asp:Panel>
                                <div>
                                    <asp:Literal ID="lCity" runat="server"></asp:Literal>,
                                    <asp:Literal ID="lStateProvince" runat="server"></asp:Literal>
                                    <asp:Literal ID="lZipPostalCode" runat="server"></asp:Literal></div>
                                <asp:Panel ID="pnlCountry" runat="server">
                                    <asp:Literal ID="lCountry" runat="server"></asp:Literal></asp:Panel>
                            </td>
                            <td width="10">
                                <img height="1" width="10" border="0" src="<%=Page.ResolveUrl("~/images/sp.gif")%>"
                                    alt="sp" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
