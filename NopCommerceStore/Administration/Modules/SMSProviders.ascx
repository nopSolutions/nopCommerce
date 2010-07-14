<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.SMSProvidersControl"
    CodeBehind="SMSProviders.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SMSProviderInfo" Src="SMSProviderInfo.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.SMSProviders.Title")%>" />
        <%=GetLocaleResourceString("Admin.SMSProviders.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.SMSProviders.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" ToolTip="<% $NopResources:Admin.SMSProviders.SaveButton.Tooltip %>" />
    </div>
</div>

<div>
    <script type="text/javascript">
        $(document).ready(function () {
        });
    </script>

    <ajaxToolkit:TabContainer runat="server" ID="SMSProvidersTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlClickatell" HeaderText="<% $NopResources:Admin.SMSProviders.Clickatell.Title %>">
            <ContentTemplate>
                <nopCommerce:SMSProviderInfo runat="server" ID="ctrlClickatellProviderInfo" SMSProviderSystemKeyword="SMSPROVIDERS_CLICKATELL" />
                <table class="adminContent">
                    <tr class="adminSeparator">
                        <td colspan="2">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle" colspan="2">
                            To receive an SMS notification when an order is placed from your store you need
                            to follow a few simple steps, which are shown below:
                            <ul>
                                <li><a href="http://www.clickatell.com" target='_blank'>Register for a Clickatell account
                                    here</a></li>
                                <li>Clickatell works with all countries and includes 10 free messages so you can test
                                    SMS notifications</li>
                                <li>Fill in the form below with your Clickatell account details, including the number
                                    you want the notification messages to be sent to</li>
                                <li>Click 'Save' button</li>
                                <li>Now when you receive a new order, an SMS text message will be sent to the number
                                    you enter below automatically</li>
                            </ul>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblClickatellPhoneNumber" Text="<% $NopResources:Admin.SMSProviders.Clickatell.PhoneNumber %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Clickatell.PhoneNumber.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" ID="txtClickatellPhoneNumber" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblClickatellAPIId" Text="<% $NopResources:Admin.SMSProviders.Clickatell.APIID %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Clickatell.APIID.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" ID="txtClickatellAPIId" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblClickatellUsername" Text="<% $NopResources:Admin.SMSProviders.Clickatell.Username %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Clickatell.Username.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" ID="txtClickatellUsername" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblClickatellPassword" Text="<% $NopResources:Admin.SMSProviders.Clickatell.Password %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Clickatell.Password.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" ID="txtClickatellPassword" />
                        </td>
                    </tr>
                    <tr class="adminSeparator">
                        <td colspan="2">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p>
                                <strong>
                                    <%=GetLocaleResourceString("Admin.SMSProviders.Clickatell.TestMessage")%>
                                </strong>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblClickatellTestMessageText" Text="<% $NopResources:Admin.SMSProviders.Clickatell.TestMessage.Text %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Clickatell.TestMessageText.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtClickatellTestMessageText" ValidationGroup="ClickatellTestMessage" TextMode="MultiLine" />
                            <asp:RequiredFieldValidator ID="vldClickatellTestMessageText" runat="server" ControlToValidate="txtClickatellTestMessageText" ErrorMessage="*"
                                ValidationGroup="ClickatellTestMessage" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="adminData">
                            <asp:Button ID="btnClickatellTestMessageSend" runat="server" Text="<% $NopResources:Admin.SMSProviders.Clickatell.TestMessage.SendButton %>"
                                CssClass="adminButton" OnClick="BtnClickatellTestMessageSend_OnClick" ValidationGroup="ClickatellTestMessage"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Clickatell.TestMessage.SendButton.Tooltip %>" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlVerizon" HeaderText="<% $NopResources:Admin.SMSProviders.Verizon.Title %>">
            <ContentTemplate>
                <nopCommerce:SMSProviderInfo runat="server" ID="ctrlVerizonProviderInfo" SMSProviderSystemKeyword="SMSPROVIDERS_VERIZON" />
                <table class="adminContent">
                    <tr class="adminSeparator">
                        <td colspan="2">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblVerizonEmail" Text="<% $NopResources:Admin.SMSProviders.Verizon.Email %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Verizon.Email.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:EmailTextBox runat="server" ID="txtVerizonEmail" />
                        </td>
                    </tr>
                    <tr class="adminSeparator">
                        <td colspan="2">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p>
                                <strong>
                                    <%=GetLocaleResourceString("Admin.SMSProviders.Verizon.TestMessage")%>
                                </strong>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblVerizonTestMessageText" Text="<% $NopResources:Admin.SMSProviders.Verizon.TestMessage.Text %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Verizon.TestMessageText.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtVerizonTestMessageText" ValidationGroup="VerizonTestMessage" TextMode="MultiLine" />
                            <asp:RequiredFieldValidator ID="vldVerizonTestMessageText" runat="server" ControlToValidate="txtVerizonTestMessageText" ErrorMessage="*"
                                ValidationGroup="VerizonTestMessage" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="adminData">
                            <asp:Button ID="btnVerizonTestMessageSend" runat="server" Text="<% $NopResources:Admin.SMSProviders.Verizon.TestMessage.SendButton %>"
                                CssClass="adminButton" OnClick="BtnVerizonTestMessageSend_OnClick" ValidationGroup="VerizonTestMessage"
                                ToolTip="<% $NopResources:Admin.SMSProviders.Verizon.TestMessage.SendButton.Tooltip %>" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</div>
