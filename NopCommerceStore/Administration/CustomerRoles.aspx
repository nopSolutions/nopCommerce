<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CustomerRoles"
    CodeBehind="CustomerRoles.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerRoles" Src="Modules/CustomerRoles.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CustomerRoles runat="server" ID="ctrlCustomerRoles" />
</asp:Content>
