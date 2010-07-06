<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_AddressAdd"
    CodeBehind="AddressAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="AddressAdd" Src="Modules/AddressAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:AddressAdd runat="server" ID="ctrlAddressAdd" />
</asp:Content>
