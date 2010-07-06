<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.AddressDisplay" Codebehind="AddressDisplay.ascx.cs" %>
<div style="font-weight: bold">
    <asp:Literal ID="lblFirstName" runat="server"></asp:Literal>
    <asp:Literal ID="lblLastName" runat="server"></asp:Literal>
   </div>
<div>
    Email:
    <asp:Literal ID="lblEmail" runat="server"></asp:Literal></div>
<div>
    Phone:
    <asp:Literal ID="lblPhoneNumber" runat="server"></asp:Literal></div>
<div>
    Fax:
    <asp:Literal ID="lblFaxNumber" runat="server"></asp:Literal></div>
<asp:Panel ID="pnlCompany" runat="server">
    <asp:Literal ID="lblCompany" runat="server"></asp:Literal></asp:Panel>
<div>
    <asp:Literal ID="lblAddress1" runat="server"></asp:Literal></div>
<asp:Panel ID="pnlAddress2" runat="server">
    <asp:Literal ID="lblAddress2" runat="server"></asp:Literal></asp:Panel>
<div>
    <asp:Literal ID="lblCity" runat="server"></asp:Literal>,
    <asp:Literal ID="lblStateProvince" runat="server"></asp:Literal>
    <asp:Literal ID="lblZipPostalCode" runat="server"></asp:Literal></div>
<asp:Panel ID="pnlCountry" runat="server">
    <asp:Literal ID="lblCountry" runat="server"></asp:Literal></asp:Panel>
