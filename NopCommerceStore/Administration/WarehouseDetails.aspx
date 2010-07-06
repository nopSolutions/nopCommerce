<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_WarehouseDetails"
    CodeBehind="WarehouseDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="WarehouseDetails" Src="Modules/WarehouseDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:WarehouseDetails runat="server" ID="ctrlWarehouseDetails" />
</asp:Content>