<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_CountryAdd"
    CodeBehind="CountryAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="CountryAdd" Src="Modules/CountryAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CountryAdd runat="server" ID="ctrlCountryAdd" />
</asp:Content>
