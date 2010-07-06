<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ManufacturerDetails"
    CodeBehind="ManufacturerDetails.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ManufacturerDetails" Src="Modules/ManufacturerDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ManufacturerDetails runat="server" ID="ctrlManufacturerDetails" />
</asp:Content>