<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Manufacturers"
    CodeBehind="Manufacturers.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Manufacturers" Src="Modules/Manufacturers.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Manufacturers runat="server" ID="ctrlManufacturers" />
</asp:Content>
