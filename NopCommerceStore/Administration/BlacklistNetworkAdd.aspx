<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlacklistNetworkAdd.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlacklistNetworkAdd" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlacklistNetworkAdd" Src="Modules/BlacklistNetworkAdd.ascx" %>
<asp:Content ID="mainContent" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlacklistNetworkAdd ID="ctrlBlacklistNetworkAdd" runat="server" />
</asp:Content>
