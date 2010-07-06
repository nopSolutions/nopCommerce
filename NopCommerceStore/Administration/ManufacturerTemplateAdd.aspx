<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ManufacturerTemplateAdd"
    CodeBehind="ManufacturerTemplateAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerTemplateAdd" Src="Modules/ManufacturerTemplateAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ManufacturerTemplateAdd runat="server" ID="ctrlManufacturerTemplateAdd" />
</asp:Content>
