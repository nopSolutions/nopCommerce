<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="ForumsHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.ForumsHome" %>

<%@ Register Src="Modules/ForumsHome.ascx" TagName="ForumsHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:ForumsHome ID="ctrlForumsHome" runat="server" />
</asp:Content>
