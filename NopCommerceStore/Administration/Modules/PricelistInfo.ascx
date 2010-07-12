<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PricelistInfoControl"
    CodeBehind="PricelistInfo.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<ajaxToolkit:TabContainer runat="server" ID="PricelistsTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlPricelistInfo" HeaderText="<% $NopResources:Admin.PricelistInfo.PricelistInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        &nbsp;
                    </td>
                    <td class="adminData">
                        <b>
                            <%=GetLocaleResourceString("Admin.PricelistInfo.AllowedTokens")%></b>
                        <br />
                        <asp:Label ID="lblAllowedTokens" runat="server" />
                        <br />
                        <br />
                        <%=GetLocaleResourceString("Admin.PricelistInfo.RequestTooltip")%>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayName" Text="<% $NopResources:Admin.PricelistInfo.DisplayName %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.DisplayName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" ID="txtDisplayName" CssClass="adminInput"
                            ErrorMessage="<% $NopResources:Admin.PricelistInfo.DisplayName.ErrorMessage %>" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShortName" Text="<% $NopResources:Admin.PricelistInfo.ShortName %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.ShortName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtShortName" CssClass="adminInput" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPricelistGuid" Text="<% $NopResources:Admin.PricelistInfo.PricelistGUID %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.PricelistGUID.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtPricelistGuid" CssClass="adminInput" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblHeader" Text="<% $NopResources:Admin.PricelistInfo.Header %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.Header.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtHeader" CssClass="adminInput" Width="100%" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblBody" Text="<% $NopResources:Admin.PricelistInfo.Body %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.Body.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" ID="txtBody" CssClass="adminInput" Width="100%"
                            ErrorMessage="<% $NopResources:Admin.PricelistInfo.Body.ErrorMessage %>" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblFooter" Text="<% $NopResources:Admin.PricelistInfo.Footer %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.Footer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtFooter" CssClass="adminInput" Width="100%" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblExportMode" Text="<% $NopResources:Admin.PricelistInfo.ExportMode %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.ExportMode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlExportMode" CssClass="adminInput" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlExportMode_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblExportType" Text="<% $NopResources:Admin.PricelistInfo.ExportType %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.ExportType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlExportType" CssClass="adminInput" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAffiliate" Text="<% $NopResources:Admin.PricelistInfo.Affiliate %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.Affiliate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlAffiliate" CssClass="adminInput" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblFormatLocalization" Text="<% $NopResources:Admin.PricelistInfo.FormatLocalization %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.FormatLocalization.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlFormatLocalization" CssClass="adminInput" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCacheTime" Text="<% $NopResources:Admin.PricelistInfo.CacheTime %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.CacheTime.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" ID="txtCacheTime" CssClass="adminInput"
                            MinimumValue="0" MaximumValue="64000" Value="0" RangeErrorMessage="<% $NopResources:Admin.PricelistInfo.CacheTime.RangeErrorMessage %>"
                            RequiredErrorMessage="<% $NopResources:Admin.PricelistInfo.CacheTime.RequiredErrorMessage %>" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDescription" Text="<% $NopResources:Admin.PricelistInfo.Description %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.Description.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="adminInput" Width="99%"
                            TextMode="MultiLine" Rows="5" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAdminNotes" Text="<% $NopResources:Admin.PricelistInfo.AdminNotes %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.AdminNotes.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtAdminNotes" CssClass="adminInput" Width="99%"
                            TextMode="MultiLine" Rows="5" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOverride" Text="<% $NopResources:Admin.PricelistInfo.Override %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.Override.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox ID="chkOverrideIndivAdjustment" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPriceAdjustmentType" Text="<% $NopResources:Admin.PricelistInfo.PriceAdjustmentType %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.PriceAdjustmentType.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlPriceAdjustmentType" CssClass="adminInput" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPriceAdjustment" Text="<% $NopResources:Admin.PricelistInfo.PriceAdjustment %>"
                            ToolTip="<% $NopResources:Admin.PricelistInfo.PriceAdjustment.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox ID="txtPriceAdjustment" runat="server" CssClass="adminInput"
                            MinimumValue="0" MaximumValue="100000000" Value="0" RequiredErrorMessage="<% $NopResources:Admin.PricelistInfo.PriceAdjustment.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.PricelistInfo.PriceAdjustment.RangeErrorMessage %>" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlProductVariants" HeaderText="<% $NopResources:Admin.PricelistInfo.ProductVariants %>">
        <ContentTemplate>
            <asp:GridView ID="gvProductVariants" runat="server" OnRowDataBound="gvProductVariants_RowDataBound"
                AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PricelistInfo.ProductVariants.ProductVariant %>">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelected" runat="server" />
                            <asp:Label ID="lblFullProductName" runat="server" Text='<%# Eval("FullProductName") %>' />
                            <asp:HiddenField ID="hfProductVariantPricelistId" runat="server" />
                            <asp:HiddenField ID="hfProductVariantId" runat="server" Value='<%# Eval("ProductVariantId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PricelistInfo.ProductVariants.PriceAdjustmentType %>">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlPriceAdjustmentType" runat="server" CssClass="adminInput" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PricelistInfo.ProductVariants.PriceAdjustment %>">
                        <ItemTemplate>
                            <nopCommerce:DecimalTextBox ID="txtPriceAdjustment" runat="server" CssClass="adminInput"
                                Value="0" MinimumValue="0" MaximumValue="100000000" RequiredErrorMessage="<% $NopResources:Admin.PricelistInfo.ProductVariants.PriceAdjustment.RequiredErrorMessage %>"
                                RangeErrorMessage="<% $NopResources:Admin.PricelistInfo.ProductVariants.PriceAdjustment.RangeErrorMessage %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <%#GetLocaleResourceString("Admin.PricelistInfo.ProductVariants.Empty")%>
                </EmptyDataTemplate>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
