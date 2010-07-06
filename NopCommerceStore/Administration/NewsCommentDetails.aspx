<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_NewsCommentDetails"
    CodeBehind="NewsCommentDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="NewsCommentDetails" Src="Modules/NewsCommentDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:NewsCommentDetails runat="server" ID="ctrlNewsCommentDetails" />
</asp:Content>
