<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_NewsComments"
    CodeBehind="NewsComments.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsComments" Src="Modules/NewsComments.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:NewsComments runat="server" ID="ctrlNewsComments" />
</asp:Content>