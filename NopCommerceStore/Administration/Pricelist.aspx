<%@ Page Title="" Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Pricelist"
    CodeBehind="Pricelist.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Pricelist" Src="Modules/Pricelist.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Pricelist runat="server" ID="ctrlPricelist" />
</asp:Content>
