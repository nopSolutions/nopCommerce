<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Warnings" CodeBehind="Warnings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Warnings" Src="Modules/Warnings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Warnings runat="server" ID="ctrlWarnings" />
</asp:Content>
