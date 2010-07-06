<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_Forums"
    CodeBehind="Forums.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="Forums" Src="Modules/Forums.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:Forums runat="server" ID="ctrlForums" />
</asp:Content>
