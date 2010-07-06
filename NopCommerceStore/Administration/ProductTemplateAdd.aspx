<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductTemplateAdd"
    CodeBehind="ProductTemplateAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductTemplateAdd" Src="Modules/ProductTemplateAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductTemplateAdd runat="server" ID="ctrlProductTemplateAdd" />
</asp:Content>
