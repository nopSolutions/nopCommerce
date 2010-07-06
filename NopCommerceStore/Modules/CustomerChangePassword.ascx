<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerChangePasswordControl"
    CodeBehind="CustomerChangePassword.ascx.cs" %>
<div class="customer-pass-recovery">
    <asp:Panel runat="server" ID="pnlChangePasswordError" CssClass="error-block">
        <div class="message-error">
            <asp:Literal ID="lChangePasswordErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
        </div>
    </asp:Panel>
    <div class="clear">
    </div>
    <div class="section-body">
        <table class="table-container">
            <tbody>
                <tr class="row">
                    <td class="item-name">
                        <%=GetLocaleResourceString("Account.OldPassword")%>:
                    </td>
                    <td class="item-value">
                        <asp:TextBox ID="txtOldPassword" runat="server" MaxLength="50" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" ControlToValidate="txtOldPassword"
                            ErrorMessage="<% $NopResources:Account.OldPasswordIsRequired %>" ToolTip="<% $NopResources:Account.OldPasswordIsRequired %>"
                            ValidationGroup="ChangePassword" />
                    </td>
                </tr>
                <tr class="row">
                    <td class="item-name">
                        <%=GetLocaleResourceString("Account.NewPassword")%>:
                    </td>
                    <td class="item-value">
                        <asp:TextBox ID="txtNewPassword" runat="server" MaxLength="50" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" ControlToValidate="txtNewPassword"
                            ErrorMessage="<% $NopResources:Account.NewPasswordIsRequired %>" ToolTip="<% $NopResources:Account.NewPasswordIsRequired %>"
                            ValidationGroup="ChangePassword" />
                    </td>
                </tr>
                <tr class="row">
                    <td class="item-name">
                        <%=GetLocaleResourceString("Account.NewPasswordConfirmation")%>:
                    </td>
                    <td class="item-value">
                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                            ErrorMessage="<% $NopResources:Account.ConfirmPasswordIsRequired %>" ToolTip="<% $NopResources:Account.ConfirmPasswordIsRequired %>"
                            ValidationGroup="ChangePassword" />
                    </td>
                </tr>
                <tr class="row">
                    <td class="item-value" colspan="2">
                        <asp:CompareValidator ID="cvPasswordCompare" runat="server" ControlToCompare="txtNewPassword"
                            ControlToValidate="txtConfirmPassword" Display="Dynamic" ErrorMessage="<% $NopResources:Account.EnteredPasswordsDoNotMatch %>"
                            ToolTip="<% $NopResources:Account.EnteredPasswordsDoNotMatch %>" ValidationGroup="ChangePassword" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="clear">
    </div>
    <div class="button">
        <asp:Button ID="btnChangePassword" runat="server" OnClick="btnChangePassword_Click"
            Text="<% $NopResources:Account.ChangePasswordButton %>" ValidationGroup="ChangePassword"
            CssClass="changepasswordbutton" />
    </div>
</div>
