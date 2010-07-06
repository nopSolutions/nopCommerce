<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ManufacturerTemplates"
    CodeBehind="ManufacturerTemplates.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerTemplates" Src="Modules/ManufacturerTemplates.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ManufacturerTemplates runat="server" ID="ctrlManufacturerTemplates" />
</asp:Content>
