<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    CodeBehind="CustomersHome.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.ActivityLogHome" %>

<%@ Register Src="Modules/ActivityLogHome.ascx" TagName="ActivityLogHome" TagPrefix="nopCommerce" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cph1">
    <nopCommerce:ActivityLogHome ID="ctrlActivityLogHome" runat="server" />
</asp:Content>
