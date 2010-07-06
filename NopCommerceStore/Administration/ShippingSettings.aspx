<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingSettings"
    CodeBehind="ShippingSettings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ShippingSettings" Src="Modules/ShippingSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ShippingSettings runat="server" ID="ctrlShippingSettings" />
</asp:Content>
