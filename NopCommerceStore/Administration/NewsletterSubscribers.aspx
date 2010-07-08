<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_NewsletterSubscribers"
    CodeBehind="NewsletterSubscribers.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsletterSubscribers" Src="Modules/NewsletterSubscribers.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:NewsletterSubscribers runat="server" ID="ctrlNewsletterSubscribers" />
</asp:Content>