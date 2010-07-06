<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductAttributeDetails"
    CodeBehind="ProductAttributeDetails.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributeDetails" Src="Modules/ProductAttributeDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductAttributeDetails runat="server" ID="ctrlProductAttributeDetails" />
</asp:Content>
