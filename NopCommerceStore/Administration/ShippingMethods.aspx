<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingMethods"
    CodeBehind="ShippingMethods.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ShippingMethods" Src="Modules/ShippingMethods.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ShippingMethods runat="server" ID="ctrlShippingMethods" />
</asp:Content>