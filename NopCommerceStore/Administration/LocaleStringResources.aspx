<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_LocaleStringResources"
    CodeBehind="LocaleStringResources.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="LocaleStringResources" Src="Modules/LocaleStringResources.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:LocaleStringResources runat="server" ID="ctrlLocaleStringResources" />
</asp:Content>
