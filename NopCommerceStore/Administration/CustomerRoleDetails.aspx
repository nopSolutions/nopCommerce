<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CustomerRoleDetails"
    CodeBehind="CustomerRoleDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerRoleDetails" Src="Modules/CustomerRoleDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CustomerRoleDetails runat="server" ID="ctrlCustomerRoleDetails" />
</asp:Content>
