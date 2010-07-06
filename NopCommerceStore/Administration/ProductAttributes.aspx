<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductAttributes"
    CodeBehind="ProductAttributes.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributes" Src="Modules/ProductAttributes.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductAttributes runat="server" ID="ctrlProductAttributes" />
</asp:Content>
