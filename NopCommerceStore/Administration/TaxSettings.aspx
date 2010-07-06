<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxSettings"
    CodeBehind="TaxSettings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TaxSettings" Src="Modules/TaxSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TaxSettings runat="server" ID="ctrlTaxSettings" />
</asp:Content>
