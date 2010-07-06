<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CurrencyAdd"
    CodeBehind="CurrencyAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CurrencyAdd" Src="Modules/CurrencyAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CurrencyAdd runat="server" ID="ctrlCurrencyAdd" />
</asp:Content>
