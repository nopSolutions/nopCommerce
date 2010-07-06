<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductTemplates"
    CodeBehind="ProductTemplates.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductTemplates" Src="Modules/ProductTemplates.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductTemplates runat="server" ID="ctrlProductTemplates" />
</asp:Content>