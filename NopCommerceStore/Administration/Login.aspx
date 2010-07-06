<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NopSolutions.NopCommerce.Web.Administration.LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NopCommerce Administration: Login</title>
</head>
<body style="background-color: #efefef;">
    <form id="form1" runat="server">
    <div class="login-block">
        <asp:Login runat="server" ID="LoginForm" TitleText="" DestinationPageUrl="~/administration/default.aspx"
            RememberMeSet="True" FailureText="<% $NopResources:Login.FailureText %>">
            <LayoutTemplate>
                <table class="login-table-container">
                    <tbody>
                        <tr class="row">
                            <td class="item-name">
                                <asp:Literal runat="server" ID="lUsernameOrEmail" Text="E-Mail" />:
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-value">
                                <asp:TextBox ID="UserName" runat="server" CssClass="adminInput" Style="width: 200px;" />
                                <asp:RequiredFieldValidator ID="UserNameOrEmailRequired" runat="server" ControlToValidate="UserName"
                                    ErrorMessage="Username is required." ToolTip="Username is required." ValidationGroup="LoginForm">
                                            *
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-name">
                                <asp:Literal runat="server" ID="lPassword" Text="<% $NopResources:Login.Password %>" />:
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-value">
                                <asp:TextBox ID="Password" TextMode="Password" runat="server" MaxLength="50" CssClass="adminInput"
                                    Style="width: 200px;" />
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                    ErrorMessage="<% $NopResources:Login.PasswordRequired %>" ToolTip="<% $NopResources:Login.PasswordRequired %>"
                                    ValidationGroup="LoginForm">
                                            *
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="item-value">
                                <asp:CheckBox ID="RememberMe" runat="server" Text="<% $NopResources:Login.RememberMe %>" />
                            </td>
                        </tr>
                        <tr class="row">
                            <td class="message-error">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                            </td>
                        </tr>
                        <tr class="row">
                            <td>
                                <div class="buttons">
                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="<% $NopResources:Login.LoginButton %>"
                                        ValidationGroup="LoginForm" CssClass="adminButtonBlue" />
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </LayoutTemplate>
        </asp:Login>
    </div>
    </form>
</body>
</html>
