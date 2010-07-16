<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PromotionProvidersControl"
    CodeBehind="PromotionProviders.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>

<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.PromotionProviders.Title")%>" />
        <%=GetLocaleResourceString("Admin.PromotionProviders.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.PromotionProviders.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" ToolTip="<% $NopResources:Admin.PromotionProviders.SaveButton.Tooltip %>" />
    </div>
</div>

<div>
    <script type="text/javascript">
        $(document).ready(function () {
        });
    </script>

    <ajaxToolkit:TabContainer runat="server" ID="PromotionProvidersTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlFroogle" HeaderText="<% $NopResources:Admin.PromotionProviders.Froogle.Title %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblAllowPublicFroogleAccess" Text="<% $NopResources:Admin.PromotionProviders.Froogle.AllowPublicFroogleAccess %>"
                                ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.AllowPublicFroogleAccess.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:CheckBox runat="server" ID="cbAllowPublicFroogleAccess" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button runat="server" Text="<% $NopResources:Admin.PromotionProviders.Froogle.GenerateButton.Text %>"
                                CssClass="adminButtonBlue" ID="btnFroogleGenerate" CausesValidation="false" 
                                OnClick="btnFroogleGenerate_Click" ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.GenerateButton.Tooltip %>" />
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
                                    <%=GetLocaleResourceString("Admin.PromotionProviders.Froogle.FTPUpload")%>
                                </strong>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblFroogleFTPHostname" Text="<% $NopResources:Admin.PromotionProviders.Froogle.FTPHostname.Text %>"
                                ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.FTPHostname.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtFroogleFTPHostname" ValidationGroup="FroogleFTPUpload" />
                            <asp:RequiredFieldValidator ID="vldFroogleFTPHostname" runat="server" ControlToValidate="txtFroogleFTPHostname" ErrorMessage="*"
                                ValidationGroup="FroogleFTPUpload" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblFroogleFTPFilename" Text="<% $NopResources:Admin.PromotionProviders.Froogle.FTPFilename.Text %>"
                                ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.FTPFilename.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtFroogleFTPFilename" ValidationGroup="FroogleFTPUpload" />
                            <asp:RequiredFieldValidator ID="vldFroogleFTPFilename" runat="server" ControlToValidate="txtFroogleFTPFilename" ErrorMessage="*"
                                ValidationGroup="FroogleFTPUpload" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblFroogleFTPUsername" Text="<% $NopResources:Admin.PromotionProviders.Froogle.FTPUsername.Text %>"
                                ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.FTPUsername.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtFroogleFTPUsername" ValidationGroup="FroogleFTPUpload" />
                            <asp:RequiredFieldValidator ID="vldFroogleFTPUsername" runat="server" ControlToValidate="txtFroogleFTPUsername" ErrorMessage="*"
                                ValidationGroup="FroogleFTPUpload" />
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ToolTipImage="~/Administration/Common/ico-help.gif"
                                ID="lblFroogleFTPPassword" Text="<% $NopResources:Admin.PromotionProviders.Froogle.FTPPassword.Text %>"
                                ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.FTPPassword.Tooltip %>" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox runat="server" CssClass="adminInput" ID="txtFroogleFTPPassword" ValidationGroup="FroogleFTPUpload" />
                            <asp:RequiredFieldValidator ID="vldFroogleFTPPassword" runat="server" ControlToValidate="txtFroogleFTPPassword" ErrorMessage="*"
                                ValidationGroup="FroogleFTPUpload" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td class="adminData">
                            <asp:Button ID="btnFroogleFTPUpload" runat="server" Text="<% $NopResources:Admin.PromotionProviders.Froogle.FTPUploadButton %>"
                                CssClass="adminButton" OnClick="btnFroogleFTPUpload_OnClick" ValidationGroup="FroogleFTPUpload"
                                ToolTip="<% $NopResources:Admin.PromotionProviders.Froogle.FTPUploadButton.Tooltip %>" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlPriceGrabber" HeaderText="<% $NopResources:Admin.PromotionProviders.PriceGrabber.Title %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td colspan="2">
                            <asp:Button runat="server" Text="<% $NopResources:Admin.PromotionProviders.PriceGrabber.GenerateButton.Text %>"
                                CssClass="adminButtonBlue" ID="btnPriceGrabberGenerate" CausesValidation="false" 
                                OnClick="btnPriceGrabberGenerate_Click" ToolTip="<% $NopResources:Admin.PromotionProviders.PriceGrabber.GenerateButton.Tooltip %>" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlBecome" HeaderText="<% $NopResources:Admin.PromotionProviders.Become.Title %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td colspan="2">
                            <asp:Button runat="server" Text="<% $NopResources:Admin.PromotionProviders.Become.GenerateButton.Text %>"
                                CssClass="adminButtonBlue" ID="btnBecomeGenerate" CausesValidation="false" 
                                OnClick="btnBecomeGenerate_Click" ToolTip="<% $NopResources:Admin.PromotionProviders.Become.GenerateButton.Tooltip %>" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</div>
