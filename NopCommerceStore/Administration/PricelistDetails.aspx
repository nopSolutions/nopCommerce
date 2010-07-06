<%@ Page Title="" Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PricelistDetails"
    CodeBehind="PricelistDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PricelistDetails" Src="Modules/PricelistDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:PricelistDetails runat="server" ID="ctrlPricelistDetails" />
</asp:Content>
