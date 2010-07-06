<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Customers"
    CodeBehind="Customers.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Customers" Src="Modules/Customers.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Customers runat="server" ID="ctrlCustomers" />
</asp:Content>
