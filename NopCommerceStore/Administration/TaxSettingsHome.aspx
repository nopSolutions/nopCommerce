<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="TaxSettingsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.TaxSettingsHome" %>

<%@ Register Src="Modules/TaxHome.ascx" TagName="TaxHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:TaxHome ID="ctrlTaxHome" runat="server" />
</asp:Content>
