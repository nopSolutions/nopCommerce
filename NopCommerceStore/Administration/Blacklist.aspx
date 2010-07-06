<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Blacklist.aspx.cs"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Blacklist" MasterPageFile="~/Administration/main.master" %>

<%@ Register TagPrefix="nopCommerce" TagName="Blacklist" Src="Modules/Blacklist.ascx" %>
<asp:Content ID="mainContent" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Blacklist ID="ctrlBlacklist" runat="server" />
</asp:Content>
