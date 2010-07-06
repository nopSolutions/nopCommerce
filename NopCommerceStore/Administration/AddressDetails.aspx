<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_AddressDetails"
    CodeBehind="AddressDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="AddressDetails" Src="Modules/AddressDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:AddressDetails runat="server" ID="ctrlAddressDetails" />
</asp:Content>