<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_LanguageDetails"
    CodeBehind="LanguageDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="LanguageDetails" Src="Modules/LanguageDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:LanguageDetails runat="server" ID="ctrlLanguageDetails" />
</asp:Content>
