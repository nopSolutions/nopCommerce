<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="NewsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.NewsHome" %>

<%@ Register Src="Modules/NewsHome.ascx" TagName="NewsHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:NewsHome ID="ctrlNewsHome" runat="server" />
</asp:Content>
