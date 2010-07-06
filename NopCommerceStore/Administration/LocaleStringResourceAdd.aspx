<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_LocaleStringResourceAdd"
    CodeBehind="LocaleStringResourceAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="LocaleStringResourceAdd" Src="Modules/LocaleStringResourceAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:LocaleStringResourceAdd runat="server" ID="ctrlLocaleStringResourceAdd" />
</asp:Content>
