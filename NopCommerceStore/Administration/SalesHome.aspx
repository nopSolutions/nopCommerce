<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="SalesHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.SalesHome" %>

<%@ Register Src="Modules/SalesHome.ascx" TagName="SalesHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:SalesHome ID="ctrlSalesHome" runat="server" />
</asp:Content>
