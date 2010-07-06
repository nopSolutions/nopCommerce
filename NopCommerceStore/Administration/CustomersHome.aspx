<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="CustomersHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.CustomersHome" %>

<%@ Register Src="Modules/CustomersHome.ascx" TagName="CustomersHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:CustomersHome ID="ctrlCustomersHome" runat="server" />
</asp:Content>
