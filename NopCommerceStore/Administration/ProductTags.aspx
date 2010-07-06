<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductTags" CodeBehind="ProductTags.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductTags" Src="Modules/ProductTags.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductTags runat="server" ID="ctrlProductTags" />
</asp:Content>
