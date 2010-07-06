<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ProductAttributeAdd"
    CodeBehind="ProductAttributeAdd.aspx.cs" ValidateRequest="false" %>

<%@ Register TagPrefix="nopCommerce" TagName="ProductAttributeAdd" Src="Modules/ProductAttributeAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ProductAttributeAdd runat="server" ID="ctrlProductAttributeAdd" />
</asp:Content>
