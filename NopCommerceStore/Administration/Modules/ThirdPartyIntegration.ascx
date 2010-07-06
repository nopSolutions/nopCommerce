<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ThirdPartyIntegrationControl"
    CodeBehind="ThirdPartyIntegration.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ThirdPartyIntegration.Title")%>" />
        <%=GetLocaleResourceString("Admin.ThirdPartyIntegration.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.ThirdPartyIntegration.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.SaveButton.Tooltip %>" />
    </div>
</div>
<div>

    <script type="text/javascript">
        $(document).ready(function() {
            toggleQuickBooks();
        });

        function toggleQuickBooks() {
            if (getE('<%=cbQuickBooksEnabled.ClientID %>').checked) {
                $('#pnlQuickBooksUsername').show();
                $('#pnlQuickBooksPassword').show();
                $('#pnlQuickBooksItemRef').show();
                $('#pnlQuickBooksDiscountAccountRef').show();
                $('#pnlQuickBooksShippingAccountRef').show();
                $('#pnlQuickBooksSalesTaxAccountRef').show();
                $('#pnlQuickBooksSynButton').show();
                $('#pnlQuickBooksSep1').show();
                $('#pnlQuickBooksSep2').show();
            }
            else {
                $('#pnlQuickBooksUsername').hide();
                $('#pnlQuickBooksPassword').hide();
                $('#pnlQuickBooksItemRef').hide();
                $('#pnlQuickBooksDiscountAccountRef').hide();
                $('#pnlQuickBooksShippingAccountRef').hide();
                $('#pnlQuickBooksSalesTaxAccountRef').hide();
                $('#pnlQuickBooksSynButton').hide();
                $('#pnlQuickBooksSep1').hide();
                $('#pnlQuickBooksSep2').hide();
            }
        }
    </script>

    <ajaxToolkit:TabContainer runat="server" ID="ThirdPartyIntegrationTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlQuickBooks" HeaderText="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Title %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle" colspan="2">
                            <b>QuickBooks Intergation Guide</b>
                            <br />
                            <br />
                            QuickBooks Web Connector(QBWC) is used to integrate nopCommerce with QuickBooks(QB).
                            QBWC can be downloaded <a href="http://marketplace.intuit.com/webconnector/" target="_blank">
                                here</a>.
                            <br />
                            <br />
                            nopCommerce QBWC web service(WS) configuration guide:
                            <br />
                            <ul>
                                <li>Go to "Admin area->Configuration->Third-party integration" and select "QuickBooks"
                                    tab </li>
                                <li>Check "Enabled" checkbox to enable nopCommerce WS </li>
                                <li>In "Username" field enter the username what you want to use to connect to WS
                                </li>
                                <li>In "Password" field enter the password what you want to use to connect to WS
                                </li>
                                <li>In "Item reference" field enter the item name which you created in QuickBooks for
                                    sales. To create a new item:
                                    <ul>
                                        <li>Open your QB application </li>
                                        <li>Go to "Company->Lists->Item List" </li>
                                        <li>"Right Click->New" </li>
                                        <li>Specify the item name </li>
                                        <li>Leave the price 0 </li>
                                        <li>Click "OK" </li>
                                    </ul>
                                </li>
                                <li>In "Discount account reference" field enter the name of account which will be used
                                    for discounts. You can use existing discount account or create a new one. To create
                                    a new account:
                                    <ul>
                                        <li>Open your QB application </li>
                                        <li>Go to "Company->Lists->Chart of Accounts" </li>
                                        <li>"Right Click->New" </li>
                                        <li>Follow the master to create new account </li>
                                    </ul>
                                </li>
                                <li>In "Shipping account reference" and "Sales tax account reference" fields enter the
                                    name of account which will be used for shipping and additional handling fee charges.
                                    You can use existing sales account or create a new one. See above on how to create
                                    a new account. </li>
                            </ul>
                            <br />
                            <br />
                            QBWC installation and configuration guide:
                            <br />
                            <ul>
                                <li>Download QBWC </li>
                                <li>Install QBWC on your computer </li>
                                <li>Build your own QWC file(.qwc). UserName should be equal to as you specified in nopCommerce.
                                    Example:
                                    <pre>
                                        &lt;?xml version="1.0"?&gt;
                                        &lt;QBWCXML&gt;
                                            &lt;AppID&gt;&lt;/AppID&gt;
                                            &lt;AppName&gt;nopCommerce QuickBooks connector&lt;/AppName&gt;
                                            &lt;AppDescription&gt;nopCommerce QuickBooks connector service&lt;/AppDescription&gt;
                                            &lt;AppURL&gt;http://your_site/QBConnector.asmx&lt;/AppURL&gt; 
                                            &lt;AppSupport&gt;&lt;/AppSupport&gt;
                                            &lt;UserName&gt;qb_username&lt;/UserName&gt;
                                            &lt;OwnerID&gt;{67F3B9B1-86F1-4fcc-B1EE-566DE1813D20}&lt;/OwnerID&gt;
                                            &lt;FileID&gt;{A0A44FB5-33D9-4815-AC85-BC87A7E7D1EB}&lt;/FileID&gt;
                                            &lt;QBType&gt;QBFS&lt;/QBType&gt;
                                        &lt;/QBWCXML&gt;
                                    </pre>
                                </li>
                                <li>After the QWC file was created you can add an application to QBWC.
                                    <ul>
                                        <li>Open your QB application(strongly required) </li>
                                        <li>Open QBWC </li>
                                        <li>Click "Add an application" </li>
                                        <li>Select your QWC file </li>
                                        <li>QB will ask you for permissions for that application. Set premissions as you need.
                                        </li>
                                        <li>After the application was successfully added to QBWC you should set the password.
                                            Password should be the same as the password that you specified in nopCommerce
                                        </li>
                                        <li>You can also configure the QBWC auto-run(time period when QBWC will connect to nopCommerce
                                            WS) </li>
                                    </ul>
                                </li>
                            </ul>
                            <br />
                            <br />
                            Critical notes:
                            <br />
                            <ul>
                                <li>This intergration implements only nopCommerce to QB data exchange </li>
                                <li>Be sure that your QuickBooks company file is empty, when starting to use this integration
                                </li>
                                <li>Be careful using "Syncronize" button. When you click it nopCommerce will request
                                    synchronization for all orders </li>
                                <li>Don't edit customers and invoices in QB. It will cause lost of the synchronization
                                    without ability to restore </li>
                                <li>If you have any issues, look at the system log first </li>
                                <li>Your Windows "Regional & Languages" options "Standards & Formats" should be set
                                    to "English(United States)" </li>
                                <li>All non-US(non-ASCII) characters will be trimmed while data transfering </li>
                            </ul>
                        </td>
                    </tr>
                    <tr class="adminSeparator" id="pnlQuickBooksSep1">
                        <td colspan="2">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblQuickBooksEnabled" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Enabled %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Enabled.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:CheckBox runat="server" ID="cbQuickBooksEnabled" />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksUsername">
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblQuickBooksUsername" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Username %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Username.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtQuickBooksUsername" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Username.ErrorMessage %>" />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksPassword">
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblQuickBooksPassword" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Password %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Password.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtQuickBooksPassword" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.Password.ErrorMessage %>" />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksItemRef">
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblQuickBooksItemRef" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.ItemRef %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.ItemRef.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtQuickBooksItemRef" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.ItemRef.ErrorMessage %>" />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksDiscountAccountRef">
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblQuickBooksDiscountAccountRef" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.DiscountAccountRef %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.DiscountAccountRef.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtQuickBooksDiscountAccountRef" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.DiscountAccountRef.ErrorMessage %>" />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksShippingAccountRef">
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblQuickBooksShippingAccountRef" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.ShippingAccountRef %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.ShippingAccountRef.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtQuickBooksShippingAccountRef" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.ShippingAccountRef.ErrorMessage %>" />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksSalesTaxAccountRef">
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblQuickBooksSalesTaxAccountRef" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.SalesTaxAccountRef %>"
                                ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.SalesTaxAccountRef.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtQuickBooksSalesTaxAccountRef" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.SalesTaxAccountRef.ErrorMessage %>" />
                        </td>
                    </tr>
                    <tr class="adminSeparator" id="pnlQuickBooksSep2">
                        <td colspan="2">
                            <hr />
                        </td>
                    </tr>
                    <tr id="pnlQuickBooksSynButton">
                        <td colspan="2">
                            <asp:Button runat="server" ID="btnQuickBooksSyn" Text="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.SynButton.Text %>"
            CssClass="adminButtonBlue" OnClick="btnQuickBooksSyn_Click" ToolTip="<% $NopResources:Admin.ThirdPartyIntegration.QuickBooks.SynButton.Tooltip %>" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</div>
