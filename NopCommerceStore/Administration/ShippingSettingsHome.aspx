<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="ShippingSettingsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.ShippingSettingsHome" %>

<%@ Register Src="Modules/ShippingHome.ascx" TagName="ShippingHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:ShippingHome ID="ctrlShippingHome" runat="server" />
</asp:Content>
