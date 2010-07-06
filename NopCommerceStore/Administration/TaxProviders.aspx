<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxProviders"
    CodeBehind="TaxProviders.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TaxProviders" Src="Modules/TaxProviders.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TaxProviders runat="server" ID="ctrlTaxProviders" />
</asp:Content>
