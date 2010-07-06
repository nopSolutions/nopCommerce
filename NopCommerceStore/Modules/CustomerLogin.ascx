<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerLoginControl"
    CodeBehind="CustomerLogin.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="Captcha" Src="~/Modules/Captcha.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="Topic" Src="~/Modules/Topic.ascx" %>
<div class="login-page">
    <div class="page-title">
        <h1><%=GetLocaleResourceString("Login.Welcome")%></h1>
    </div>
    <div class="clear">
    </div>
    <div class="wrapper">
        <%if (!CheckoutAsGuestQuestion)
          { %>
        <div class="new-wrapper">
            <span class="register-title">
                <%=GetLocaleResourceString("Login.NewCustomer")%></span>
            <div class="register-block" runat="server" id="pnlRegisterBlock">
                <table>
                    <tbody>
                        <tr>
                            <td>
                                <%=GetLocaleResourceString("Login.NewCustomerText")%>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="padding-right: 20px; padding-top: 20px;">
                                <asp:Button runat="server" ID="btnRegister" Text="<% $NopResources:Account.Register %>"
                                    OnClick="btnRegister_Click" CssClass="registerbutton" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <%}
          else
          { %>
        <div class="new-wrapper">
            <span class="register-title">
                <%=GetLocaleResourceString("Checkout.CheckoutAsGuestOrRegister")%></span>
            <div class="checkout-as-guest-or-register-block">
                <table>
                    <tbody>
                        <tr>
                            <td>
                                <nopCommerce:Topic ID="topicCheckoutAsGuestOrRegister" runat="server" TopicName="CheckoutAsGuestOrRegister"
                                    OverrideSEO="false"></nopCommerce:Topic>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="padding-right: 20px; padding-top: 20px;">
                                <asp:Button runat="server" ID="btnCheckoutAsGuest" Text="<% $NopResources:Checkout.CheckoutAsGuest %>"
                                    OnClick="btnCheckoutAsGuest_Click" CssClass="checkoutasguestbutton" />
                                <asp:Button runat="server" ID="btnRegister2" Text="<% $NopResources:Account.Register %>"
                                    OnClick="btnRegister_Click" CssClass="registerbutton" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <%} %>
        <div class="returning-wrapper">
            <span class="login-title">
                <%=GetLocaleResourceString("Login.ReturningCustomer")%></span>
            <asp:Panel ID="pnlLogin" runat="server" DefaultButton="LoginForm$LoginButton" CssClass="login-block">
                <asp:Login ID="LoginForm" TitleText="" OnLoggedIn="OnLoggedIn" OnLoggingIn="OnLoggingIn"
                    runat="server" CreateUserUrl="~/register.aspx" DestinationPageUrl="~/Default.aspx"
                    OnLoginError="OnLoginError" RememberMeSet="True" FailureText="<% $NopResources:Login.FailureText %>">
                    <LayoutTemplate>
                        <table class="login-table-container">
                            <tbody>
                                <tr class="row">
                                    <td class="item-name">
                                        <asp:Literal runat="server" ID="lUsernameOrEmail" Text="E-Mail" />:
                                    </td>
                                    <td class="item-value">
                                        <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameOrEmailRequired" runat="server" ControlToValidate="UserName"
                                            ErrorMessage="Username is required." ToolTip="Username is required." ValidationGroup="LoginForm">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="item-name">
                                        <asp:Literal runat="server" ID="lPassword" Text="<% $NopResources:Login.Password %>" />:
                                    </td>
                                    <td class="item-value">
                                        <asp:TextBox ID="Password" TextMode="Password" runat="server" MaxLength="50"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                            ErrorMessage="<% $NopResources:Login.PasswordRequired %>" ToolTip="<% $NopResources:Login.PasswordRequired %>"
                                            ValidationGroup="LoginForm">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="item-value" colspan="2">
                                        <asp:CheckBox ID="RememberMe" runat="server" Text="<% $NopResources:Login.RememberMe %>">
                                        </asp:CheckBox>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="message-error" colspan="2">
                                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td class="forgot-password" colspan="2">
                                        <asp:HyperLink ID="hlForgotPassword" runat="server" NavigateUrl="~/passwordrecovery.aspx"
                                            Text="<% $NopResources:Login.ForgotPassword %>"></asp:HyperLink>
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td colspan="2">
                                        <nopCommerce:Captcha ID="CaptchaCtrl" runat="server" />
                                    </td>
                                </tr>
                                <tr class="row">
                                    <td colspan="2">
                                        <div class="buttons">
                                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="<% $NopResources:Login.LoginButton %>"
                                                ValidationGroup="LoginForm" CssClass="loginbutton" />
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                </asp:Login>
            </asp:Panel>
        </div>
    </div>
    <div class="clear">
    </div>
    <nopCommerce:Topic ID="topicLoginRegistrationInfoText" runat="server" TopicName="LoginRegistrationInfo"
        OverrideSEO="false"></nopCommerce:Topic>
</div>
