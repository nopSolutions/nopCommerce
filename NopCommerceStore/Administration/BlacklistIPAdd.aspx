<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlacklistIPAdd.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlacklistIPAdd" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlacklistIPAdd" Src="Modules/BlacklistIPAdd.ascx" %>
<asp:Content ID="mainContent" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlacklistIPAdd ID="ctrlBlacklistIPAdd" runat="server" />
</asp:Content>
