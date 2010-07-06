<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SalesReport"
    CodeBehind="SalesReport.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="SalesReport" Src="Modules/SalesReport.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:SalesReport runat="server" ID="ctrlSalesReport" />
</asp:Content>
