<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_PaymentMethodDetails"
    CodeBehind="PaymentMethodDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="Modules/ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="Modules/ConfirmationBox.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.PaymentMethodDetails.Title")%>" />
            <%=GetLocaleResourceString("Admin.PaymentMethodDetails.Title")%>
            <a href="PaymentMethods.aspx" title="<%=GetLocaleResourceString("Admin.PaymentMethodDetails.BackToMethodList")%>">
                (<%=GetLocaleResourceString("Admin.PaymentMethodDetails.BackToMethodList")%>)</a>
        </div>
        <div class="options">
            <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.PaymentMethodDetails.SaveButton.Text %>"
                OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.PaymentMethodDetails.SaveButton.Tooltip %>" />
            <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.PaymentMethodDetails.DeleteButton.Text %>"
                OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.PaymentMethodDetails.DeleteButton.Tooltip %>" />
        </div>
    </div>
    <ajaxToolkit:TabContainer runat="server" ID="PaymentTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlProviderInfo" HeaderText="<% $NopResources:Admin.PaymentMethodDetails.InfoPanel %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.PaymentMethodInfo.Name %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.Name.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblVisibleName" Text="<% $NopResources:Admin.PaymentMethodInfo.VisibleName %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.VisibleName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtVisibleName" CssClass="adminInput"
                                ErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.VisibleName.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.PaymentMethodInfo.Description %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" CssClass="adminInput"
                                Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblConfigureTemplatePath" Text="<% $NopResources:Admin.PaymentMethodInfo.ConfigureTemplatePath %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.ConfigureTemplatePath.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtConfigureTemplatePath" runat="server" CssClass="adminInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblUserTemplatePath" Text="<% $NopResources:Admin.PaymentMethodInfo.UserTemplatePath %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.UserTemplatePath.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtUserTemplatePath" CssClass="adminInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblClassName" Text="<% $NopResources:Admin.PaymentMethodInfo.ClassName %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.ClassName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtClassName"
                                ErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.ClassName.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblSystemKeyword" Text="<% $NopResources:Admin.PaymentMethodInfo.SystemKeyword %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.SystemKeyword.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtSystemKeyword" CssClass="adminInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblHidePaymentInfoForZeroOrders" Text="<% $NopResources:Admin.PaymentMethodInfo.HidePaymentInfoForZeroOrders %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.HidePaymentInfoForZeroOrders.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:CheckBox ID="cbHidePaymentInfoForZeroOrders" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblActive" Text="<% $NopResources:Admin.PaymentMethodInfo.Active %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.Active.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:CheckBox ID="cbActive" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                                Value="1" RequiredErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder.RequiredErrorMessage %>"
                                RangeErrorMessage="<% $NopResources:Admin.PaymentMethodInfo.DisplayOrder.RangeErrorMessage %>"
                                MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblCanCaptureTooltip" Text="<% $NopResources:Admin.PaymentMethodInfo.CanCapture %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.CanCapture.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:Label ID="lblCanCapture" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblCanRefundTooltip" Text="<% $NopResources:Admin.PaymentMethodInfo.CanRefund %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.CanRefund.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:Label ID="lblCanRefund" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblCanPartiallyRefundTooltip" Text="<% $NopResources:Admin.PaymentMethodInfo.CanPartiallyRefund %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.CanPartiallyRefund.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:Label ID="lblCanPartiallyRefund" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblCanVoidTooltip" Text="<% $NopResources:Admin.PaymentMethodInfo.CanVoid %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.CanVoid.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:Label ID="lblCanVoid" runat="server"></asp:Label>
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblSupportRecurringPaymentsTooltip" Text="<% $NopResources:Admin.PaymentMethodInfo.SupportRecurringPayments %>"
                                ToolTip="<% $NopResources:Admin.PaymentMethodInfo.SupportRecurringPayments.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:Label ID="lblSupportRecurringPayments" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlConfigure" HeaderText="<% $NopResources:Admin.PaymentMethodDetails.ConfigurePanel %>">
            <ContentTemplate>
                <asp:PlaceHolder runat="server" ID="ConfigureMethodHolder"></asp:PlaceHolder>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
        YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
        ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
</asp:Content>
