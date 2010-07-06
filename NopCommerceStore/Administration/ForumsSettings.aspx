<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Administration/main.master"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ForumsSettings"
    CodeBehind="ForumsSettings.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ForumsSettings" Src="Modules/ForumsSettings.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ForumsSettings runat="server" ID="ctrlForumsSettings" />
</asp:Content>
