<%@ Page Language="C#" MasterPageFile="~/Administration/main.master" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Administration_TaxProviderDetails"
    CodeBehind="TaxProviderDetails.aspx.cs" %>

<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="Modules/ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="Modules/SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="Modules/NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="Modules/ConfirmationBox.ascx" %>
<asp:Content ID="c1" ContentPlaceHolderID="cph1" runat="Server">
    <div class="section-header">
        <div class="title">
            <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.TaxProviderDetails.Title")%>" />
            <%=GetLocaleResourceString("Admin.TaxProviderDetails.Title")%>
            <a href="TaxProviders.aspx" title="<%=GetLocaleResourceString("Admin.TaxProviderDetails.BackToProviders")%>">
                (<%=GetLocaleResourceString("Admin.TaxProviderDetails.BackToProviders")%>)</a>
        </div>
        <div class="options">
            <asp:Button ID="SaveButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TaxProviderDetails.SaveButton.Text %>"
                OnClick="SaveButton_Click" ToolTip="<% $NopResources:Admin.TaxProviderDetails.SaveButton.Tooltip %>" />
            <asp:Button ID="DeleteButton" runat="server" CssClass="adminButtonBlue" Text="<% $NopResources:Admin.TaxProviderDetails.DeleteButton.Text %>"
                OnClick="DeleteButton_Click" CausesValidation="false" ToolTip="<% $NopResources:Admin.TaxProviderDetails.DeleteButton.Tooltip %>" />
        </div>
    </div>
    <ajaxToolkit:TabContainer runat="server" ID="TaxTabs" ActiveTabIndex="0">
        <ajaxToolkit:TabPanel runat="server" ID="pnlProviderInfo" HeaderText="<% $NopResources:Admin.TaxProviderInfo.InfoPanel %>">
            <ContentTemplate>
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.TaxProviderInfo.Name %>"
                                ToolTip="<% $NopResources:Admin.TaxProviderInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.TaxProviderInfo.Name.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.TaxProviderInfo.Description %>"
                                ToolTip="<% $NopResources:Admin.TaxProviderInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" TextMode="MultiLine"
                                Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblConfigureTemplatePath" Text="<% $NopResources:Admin.TaxProviderInfo.ConfigureTemplatePath %>"
                                ToolTip="<% $NopResources:Admin.TaxProviderInfo.ConfigureTemplatePath.Tooltip %>"
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtConfigureTemplatePath" CssClass="adminInput" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblClassName" Text="<% $NopResources:Admin.TaxProviderInfo.ClassName %>"
                                ToolTip="<% $NopResources:Admin.TaxProviderInfo.ClassName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtClassName"
                                ErrorMessage="<% $NopResources:Admin.TaxProviderInfo.ClassName.ErrorMessage %>">
                            </nopCommerce:SimpleTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder %>"
                                ToolTip="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:NumericTextBox runat="server" ID="txtDisplayOrder" CssClass="adminInput"
                                Value="1" RequiredErrorMessage="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder.RequiredErrorMessage %>"
                                MinimumValue="-99999" MaximumValue="99999" RangeErrorMessage="<% $NopResources:Admin.TaxProviderInfo.DisplayOrder.RangeErrorMessage %>">
                            </nopCommerce:NumericTextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="pnlConfigure" HeaderText="<% $NopResources:Admin.TaxProviderInfo.ConfigurePanel %>">
         <ContentTemplate>
            <asp:PlaceHolder runat="server" ID="ConfigurePlaceHolder"></asp:PlaceHolder>
             </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <nopCommerce:ConfirmationBox runat="server" ID="cbDelete" TargetControlID="DeleteButton"
        YesText="<% $NopResources:Admin.Common.Yes %>" NoText="<% $NopResources:Admin.Common.No %>"
        ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
</asp:Content>
