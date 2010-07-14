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
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlTestMessage" HeaderText="<% $NopResources:Admin.SMSProviders.TestMessage.Title %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblTestMessageText" Text="<% $NopResources:Admin.SMSProviders.TestMessage.Text %>"
                                ToolTip="<% $NopResources:Admin.SMSProviders.TestMessage.Text.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtTestMessageText" ValidationGroup="TestMessage" TextMode="MultiLine" />
                            <asp:RequiredFieldValidator ID="vldTestMessageText" runat="server" ControlToValidate="txtTestMessageText" ErrorMessage="*"
                                ValidationGroup="TestMessage" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="adminData">
                            <asp:Button ID="btnTestMessageSend" runat="server" Text="<% $NopResources:Admin.SMSProviders.TestMessage.SendButton %>"
                                CssClass="adminButton" OnClick="BtnTestMessageSend_OnClick" ValidationGroup="TestMessage"
                                ToolTip="<% $NopResources:Admin.SMSProviders.TestMessage.SendButton.Tooltip %>" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</div>
