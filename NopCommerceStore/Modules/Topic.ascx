<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Modules.TopicControl" Codebehind="Topic.ascx.cs" %>
<div class="htmlcontent">
    <div class="htmlcontent-title">
        <h2 class="htmlcontent-header">
            <asp:Literal runat="server" ID="lTitle" EnableViewState="false"></asp:Literal></h2>
    </div>
    <div class="clear">
    </div>
    <div class="htmlcontent-body">
        <asp:PlaceHolder runat="server" ID="phPassword">
            <%=GetLocaleResourceString("TopicPage.EnterPassword")%>
            <br />
            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" />
            <asp:Button runat="server" ID="btnPassword" OnClick="btnPassword_OnClick" Text="<% $NopResources:TopicPage.btnPassword.Text %>"
                CssClass="useragreementbutton" />
            <br />
            <asp:Literal runat="server" ID="lError" EnableViewState="false" />
        </asp:PlaceHolder>
        <asp:Literal runat="server" ID="lBody" EnableViewState="false"></asp:Literal>
    </div>
</div>
