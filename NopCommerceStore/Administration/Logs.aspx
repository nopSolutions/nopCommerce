<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Logs" CodeBehind="Logs.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Logs" Src="Modules/Logs.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Logs runat="server" ID="ctrlLogs" />
</asp:Content>
