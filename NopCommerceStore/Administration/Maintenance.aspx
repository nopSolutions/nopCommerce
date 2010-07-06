<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Maintenance" CodeBehind="Maintenance.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Maintenance" Src="Modules/Maintenance.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Maintenance runat="server" ID="ctrlMaintenance" />
</asp:Content>
