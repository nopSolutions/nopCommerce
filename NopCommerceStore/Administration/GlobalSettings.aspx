<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true" ValidateRequest="false"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_GlobalSettings"
    CodeBehind="GlobalSettings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="GlobalSettings" Src="Modules/GlobalSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:GlobalSettings runat="server" ID="ctrlGlobalSettings" />
</asp:Content>
