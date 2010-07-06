<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxCategoryAdd"
    CodeBehind="TaxCategoryAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TaxCategoryAdd" Src="Modules/TaxCategoryAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TaxCategoryAdd runat="server" ID="ctrlTaxCategoryAdd" />
</asp:Content>
