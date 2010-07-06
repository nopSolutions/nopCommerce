<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_ShippingRateComputationMethodDetails"
    CodeBehind="ShippingRateComputationMethodDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="Modules/ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="Modules/ConfirmationBox.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.ShippingRateComputationMethodDetails.Title")%>" />
            <%=GetLocaleResourceString("Admin.ShippingRateComputationMethodDetails.Title")%>
            <a href="ShippingRateComputationMethods.aspx" title="<%=GetLocaleResourceString("Admin.ShippingRateComputationMethodDetails.BackToMethods")%>">
                (<%=GetLocaleResourceString("Admin.ShippingRateComputationMethodDetails.BackToMethods")%>)</a>
        </div>
        <div class="options">
            <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ShippingRateComputationMethodDetails.SaveButton.Text %>"
                OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodDetails.SaveButton.Tooltip %>" />
            <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.ShippingRateComputationMethodDetails.DeleteButton.Text %>"
                OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodDetails.DeleteButton.Tooltip %>" />
        </div>
    </div>
    <ajaxToolkit:TabContainer runat="server" ID="ShippingTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlProviderInfo" HeaderText="<% $NopResources:Admin.ShippingRateComputationMethodDetails.InfoPanel %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Name %>"
                                ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Name.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Name.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Description %>"
                                ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Description.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                                Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblConfigureTemplatePath" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ConfigureTemplatePath %>"
                                ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ConfigureTemplatePath.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtConfigureTemplatePath" CssClass="adminInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblClassName" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ClassName %>"
                                ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ClassName.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtClassName"
                                ErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.ClassName.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblActive" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Active %>"
                                ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.Active.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:CheckBox ID="cbActive" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder %>"
                                ToolTip="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                                Value="1" RequiredErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder.RequiredErrorMessage %>"
                                MinimumValue="-99999" MaximumValue="99999" RangeErrorMessage="<% $NopResources:Admin.ShippingRateComputationMethodInfo.DisplayOrder.RangeErrorMessage %>">
                            </nopCommerce:NumericTextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlConfigure" HeaderText="<% $NopResources:Admin.ShippingRateComputationMethodDetails.ConfigurePanel %>">
            <ContentTemplate>
                <asp:PlaceHolder runat="server" ID="ConfigurePlaceHolder"></asp:PlaceHolder>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
        YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
        ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
</asp:Content>
