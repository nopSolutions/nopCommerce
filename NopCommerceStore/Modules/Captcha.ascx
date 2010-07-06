<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CaptchaControl"
    CodeBehind="Captcha.ascx.cs" %>
<img src="<%=Page.ResolveUrl("~/captchaimage.aspx")%>" alt="captcha" /><br />
<p>
    <em>
        <%=GetLocaleResourceString("Captcha.EnterTheCode")%></em><br />
    <asp:TextBox ID="txtCode" runat="server"></asp:TextBox>
</p>
<p>
    <asp:Label ID="txtMessageLabel" runat="server" CssClass="message-error" EnableViewState="false"></asp:Label>
</p>
