<%@ Page Language="C#" MasterPageFile="~/MasterPages/TwoColumn.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.NewsArchivePage" CodeBehind="NewsArchive.aspx.cs"
     %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsArchive" Src="~/Modules/NewsArchive.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="Server">
    <nopCommerce:NewsArchive ID="ctrlNewsArchive" runat="server" />
</asp:Content>
