<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_News" CodeBehind="News.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="News" Src="Modules/News.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:News runat="server" ID="ctrlNews" />
</asp:Content>