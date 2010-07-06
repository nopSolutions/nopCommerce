<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingMethodAdd"
    CodeBehind="ShippingMethodAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ShippingMethodAdd" Src="Modules/ShippingMethodAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ShippingMethodAdd runat="server" ID="ctrlShippingMethodAdd" />
</asp:Content>