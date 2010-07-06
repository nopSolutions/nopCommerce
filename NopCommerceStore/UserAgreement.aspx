<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/OneColumn.master" CodeBehind="UserAgreement.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.UserAgreementPage" %>
<%@ Register TagPrefix="nopCommerce" TagName="UserAgreementControl" Src="~/Modules/UserAgreementControl.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:UserAgreementControl runat="server" />
</asp:Content>
