<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ManufacturerTemplateDetails"
    CodeBehind="ManufacturerTemplateDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerTemplateDetails" Src="Modules/ManufacturerTemplateDetails.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ManufacturerTemplateDetails runat="server" ID="ctrlManufacturerTemplateDetails" />
</asp:Content>
