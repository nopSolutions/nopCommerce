<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="LocationSettingsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.LocationSettingsHome" %>

<%@ Register Src="Modules/LocationHome.ascx" TagName="LocationHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:LocationHome ID="ctrlLocationHome" runat="server" />
</asp:Content>
