<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingMethodDetails"
    CodeBehind="ShippingMethodDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ShippingMethodDetails" Src="Modules/ShippingMethodDetails.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:ShippingMethodDetails runat="server" ID="ctrlShippingMethodDetails" />
</asp:Content>
