<%@ Page Language="C#" MasterPageFile="~/MasterPages/ThreeColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.PasswordRecoveryPage" CodeBehind="PasswordRecovery.aspx.cs"
     %>

<%@ Register TagPrefix="nopCommerce" TagName="PasswordRecovery" Src="~/Modules/CustomerPasswordRecovery.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:PasswordRecovery ID="ctrlPasswordRecovery" runat="server" />
</asp:Content>
