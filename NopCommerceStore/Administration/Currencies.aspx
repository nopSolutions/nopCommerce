<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Currencies"
    CodeBehind="Currencies.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Currencies" Src="Modules/Currencies.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Currencies runat="server" ID="ctrlCurrencies" />
</asp:Content>
