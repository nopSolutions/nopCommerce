<%@ Page Title="" Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PricelistAdd"
    CodeBehind="PricelistAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PricelistAdd" Src="Modules/PricelistAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:PricelistAdd runat="server" ID="ctrlPricelistAdd" />
</asp:Content>