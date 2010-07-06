<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_LanguageAdd"
    CodeBehind="LanguageAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="LanguageAdd" Src="Modules/LanguageAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:LanguageAdd runat="server" ID="ctrlLanguageAdd" />
</asp:Content>
