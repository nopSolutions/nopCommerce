<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_WarehouseAdd"
    CodeBehind="WarehouseAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="WarehouseAdd" Src="Modules/WarehouseAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:WarehouseAdd runat="server" ID="ctrlWarehouseAdd" />
</asp:Content>