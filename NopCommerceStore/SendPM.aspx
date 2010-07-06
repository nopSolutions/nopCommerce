<%@ Page Language="C#" MasterPageFile="~/MasterPages/OneColumn.master" ValidateRequest="false"
    AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.SendPMPage" CodeBehind="SendPM.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="PrivateMessagesSend" Src="~/Modules/PrivateMessagesSend.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:PrivateMessagesSend ID="ctrlPrivateMessagesSend" runat="server" />
</asp:Content>
