<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CountryDetails"
    CodeBehind="CountryDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CountryDetails" Src="Modules/CountryDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CountryDetails runat="server" ID="ctrlCountryDetails" />
</asp:Content>
