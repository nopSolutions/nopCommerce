<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxCategoryDetails"
    CodeBehind="TaxCategoryDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TaxCategoryDetails" Src="Modules/TaxCategoryDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TaxCategoryDetails runat="server" ID="ctrlTaxCategoryDetails" />
</asp:Content>
