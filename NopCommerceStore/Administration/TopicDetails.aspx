<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TopicDetails"
    CodeBehind="TopicDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="TopicDetails" Src="Modules/TopicDetails.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph1" runat="server">
    <nopCommerce:TopicDetails runat="server" ID="ctrlTopicDetails" />
</asp:Content>
