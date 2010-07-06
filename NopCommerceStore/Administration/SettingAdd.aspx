<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" ValidateRequest="false"
    AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_SettingAdd"
    CodeBehind="SettingAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="SettingAdd" Src="Modules/SettingAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:SettingAdd runat="server" ID="ctrlSettingAdd" />
</asp:Content>
