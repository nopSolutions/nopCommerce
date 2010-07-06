<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlacklistNetworkDetails.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlacklistNetworkDetails" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlacklistNetworkDetails" Src="Modules/BlacklistNetworkDetails.ascx" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:BlacklistNetworkDetails ID="ctrlBlacklistNetworkDetails" runat="server" />
</asp:Content>
