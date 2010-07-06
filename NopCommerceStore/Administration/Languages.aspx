<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Languages"
    CodeBehind="Languages.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Languages" Src="Modules/Languages.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Languages runat="server" ID="ctrlLanguages" />
</asp:Content>
