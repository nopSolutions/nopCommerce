<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ActivityLog"
    CodeBehind="ActivityLog.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ActivityLog" Src="Modules/ActivityLog.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:ActivityLog runat="server" ID="ctrlActivityLog" />
</asp:Content>
