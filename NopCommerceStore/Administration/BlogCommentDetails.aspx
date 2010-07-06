<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    ValidateRequest="false" Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_BlogCommentDetails"
    CodeBehind="BlogCommentDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="BlogCommentDetails" Src="Modules/BlogCommentDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:BlogCommentDetails runat="server" ID="ctrlBlogCommentDetails" />
</asp:Content>
