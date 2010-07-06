<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductTemplateDetails"
    CodeBehind="ProductTemplateDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductTemplateDetails" Src="Modules/ProductTemplateDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductTemplateDetails runat="server" ID="ctrlProductTemplateDetails" />
</asp:Content>