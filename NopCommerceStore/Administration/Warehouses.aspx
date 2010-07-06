<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Warehouses"
    CodeBehind="Warehouses.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Warehouses" Src="Modules/Warehouses.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Warehouses runat="server" ID="ctrlWarehouses" />
</asp:Content>