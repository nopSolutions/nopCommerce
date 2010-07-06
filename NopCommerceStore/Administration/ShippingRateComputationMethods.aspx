<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingRateComputationMethods"
    CodeBehind="ShippingRateComputationMethods.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ShippingRateComputationMethods" Src="Modules/ShippingRateComputationMethods.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ShippingRateComputationMethods runat="server" ID="ctrlShippingRateComputationMethods" />
</asp:Content>

