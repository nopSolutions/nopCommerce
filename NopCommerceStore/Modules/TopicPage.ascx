<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.TopicPageControl"
    CodeBehind="TopicPage.ascx.cs" %>
<div class="topicpage">
    <div class="page-title">
        <h1>
            <asp:Literal runat="server" ID="lTitle" EnableViewState="false" /></h1>
    </div>
    <div class="clear">
    </div>
    <div class="topicpage-body">
        <asp:PlaceHolder runat="server" ID="phPassword">
            <%=GetLocaleResourceString("TopicPage.EnterPassword")%>
            <br />
            <asp:TextBox runat="server" ID="txtPassword" /> <asp:Button runat="server" ID="btnPassword" OnClick="btnPassword_OnClick" Text="<% $NopResources:TopicPage.btnPassword.Text %>"
                CssClass="useragreementbutton" />
            <br />
            <asp:Literal runat="server" ID="lError" EnableViewState="false" />
        </asp:PlaceHolder>
        <asp:Literal runat="server" ID="lBody" EnableViewState="false" />
    </div>
</div>
