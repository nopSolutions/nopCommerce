<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.TopicPageControl"
    CodeBehind="TopicPage.ascx.cs" %>
<div class="topicpage">
    <div class="page-title">
        <h1>
            <asp:Literal runat="server" ID="lTitle" EnableViewState="false"></asp:Literal></h1>
    </div>
    <div class="clear">
    </div>
    <div class="topicpage-body">
        <asp:Literal runat="server" ID="lBody" EnableViewState="false"></asp:Literal>
    </div>
</div>
