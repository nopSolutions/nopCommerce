<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CustomerReports"
    CodeBehind="CustomerReports.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CustomerReports" Src="Modules/CustomerReports.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CustomerReports runat="server" ID="ctrlCustomerReports" />
</asp:Content>