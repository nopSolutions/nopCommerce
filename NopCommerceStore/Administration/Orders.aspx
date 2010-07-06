<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Orders"
    CodeBehind="Orders.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Orders" Src="Modules/Orders.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Orders runat="server" ID="ctrlOrders" />
</asp:Content>