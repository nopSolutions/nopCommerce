<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SystemInformation" CodeBehind="SystemInformation.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="SystemInformation" Src="Modules/SystemInformation.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:SystemInformation runat="server" ID="ctrlSystemInformationg" />
</asp:Content>
