<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true" 
CodeBehind="CatalogHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.CatalogHome" %>

<%@ Register TagPrefix="nopCommerce" TagName="CatalogHome" Src="Modules/CatalogHome.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:CatalogHome runat="server" ID="ctrlCatalogHome" />
</asp:Content>
