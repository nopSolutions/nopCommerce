<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CurrencyDetails"
    CodeBehind="CurrencyDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CurrencyDetails" Src="Modules/CurrencyDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CurrencyDetails runat="server" ID="ctrlCurrencyDetails" />
</asp:Content>
