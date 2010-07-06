<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxCategories"
    CodeBehind="TaxCategories.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TaxCategories" Src="Modules/TaxCategories.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TaxCategories runat="server" ID="ctrlTaxCategories" />
</asp:Content>
