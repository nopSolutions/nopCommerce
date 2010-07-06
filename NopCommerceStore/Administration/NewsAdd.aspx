<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_NewsAdd"
    CodeBehind="NewsAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsAdd" Src="Modules/NewsAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:NewsAdd runat="server" ID="ctrlNewsAdd" />
</asp:Content>
