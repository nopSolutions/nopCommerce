<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlacklistIPDetails.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlacklistIPDetails" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlacklistIPDetails" Src="Modules/BlacklistIPDetails.ascx" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:BlacklistIPDetails ID="ctrlBlacklistIPDetails" runat="server" />
</asp:Content>
