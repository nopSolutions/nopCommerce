<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_EmailAccountAdd"
    CodeBehind="EmailAccountAdd.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="EmailAccountAdd" Src="Modules/EmailAccountAdd.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:EmailAccountAdd runat="server" ID="ctrlEmailAccountAdd" />
</asp:Content>
