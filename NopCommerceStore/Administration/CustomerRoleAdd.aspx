<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CustomerRoleAdd"
    CodeBehind="CustomerRoleAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerRoleAdd" Src="Modules/CustomerRoleAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CustomerRoleAdd runat="server" ID="ctrlCustomerRoleAdd" />
</asp:Content>
