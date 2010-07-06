<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_NewsSettings"
    CodeBehind="NewsSettings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsSettings" Src="Modules/NewsSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:NewsSettings runat="server" ID="ctrlNewsSettings" />
</asp:Content>
