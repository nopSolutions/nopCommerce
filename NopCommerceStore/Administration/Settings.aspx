<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Settings"
    CodeBehind="Settings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Settings" Src="Modules/Settings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Settings runat="server" ID="ctrlSettings" />
</asp:Content>
