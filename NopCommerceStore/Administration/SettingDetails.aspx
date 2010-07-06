<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" ValidateRequest="false"
    AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SettingDetails"
    CodeBehind="SettingDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="SettingDetails" Src="Modules/SettingDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:SettingDetails runat="server" ID="ctrlSettingDetails" />
</asp:Content>
