<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantAttributesControl"
    CodeBehind="ProductVariantAttributes.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectProductAttributes" Src="~/Modules/ProductAttributes.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">
<asp:UpdatePanel ID="upAttr" runat="server">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
            <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
        </asp:Panel>
        <ajaxToolkit:TabContainer runat="server" ID="AttrTabs" ActiveTabIndex="0">
            <ajaxToolkit:TabPanel runat="server" ID="pnlAttr" HeaderText="<% $NopResources:Admin.ProductVariantAttributes.Attributes %>">
                <ContentTemplate>
                    <div>
                        <asp:GridView ID="gvProductVariantAttributes" runat="server" AutoGenerateColumns="false"
                            DataKeyNames="ProductVariantAttributeId" OnRowDeleting="gvProductVariantAttributes_RowDeleting"
                            OnRowDataBound="gvProductVariantAttributes_RowDataBound" OnRowCommand="gvProductVariantAttributes_RowCommand"
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.Attribute %>"
                                    ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlProductAttribute" runat="server" />
                                        <asp:HiddenField ID="hfProductVariantAttributeId" runat="server" Value='<%# Eval("ProductVariantAttributeId") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.TextPrompt %>"
                                    ItemStyle-Width="15%">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtTextPrompt" runat="server" Value='<%# Eval("TextPrompt") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.IsRequired %>"
                                    ItemStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbIsRequired" runat="server" Checked='<%# Eval("IsRequired") %>'
                                            ToolTip="<% $NopResources:Admin.ProductVariantAttributes.IsRequired.Tooltip %>" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.ControlType %>"
                                    ItemStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlAttributeControlType" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.DisplayOrder %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtDisplayOrder"
                                            Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.DisplayOrder.RequiredErrorMessage %>"
                                            RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.DisplayOrder.RangeErrorMessage %>"
                                            ValidationGroup="ProductVariantAttribute" MinimumValue="-99999" MaximumValue="99999">
                                        </nopCommerce:NumericTextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.Values %>"
                                    ItemStyle-Width="15%">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="hlAttributeValues" Text="View/Edit values"></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.Update %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantAttributes.Update %>"
                                            ValidationGroup="ProductVariantAttribute" CommandName="UpdateProductVariantAttribute"
                                            ToolTip="<% $NopResources:Admin.ProductVariantAttributes.Update.Tooltip %>" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.Delete %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnDeleteProductVariantAttribute" runat="server" CssClass="adminButton"
                                            Text="<% $NopResources:Admin.ProductVariantAttributes.Delete %>" CausesValidation="false"
                                            CommandName="Delete" ToolTip="<% $NopResources:Admin.ProductVariantAttributes.Delete.Tooltip %>" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div>
                        <br />
                        <strong>
                            <%=GetLocaleResourceString("Admin.ProductVariantAttributes.AddNew")%>
                        </strong>
                    </div>
                    <table class="adminContent">
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblAttribute" Text="<% $NopResources:Admin.ProductVariantAttributes.New.Attribute %>"
                                    ToolTip="<% $NopResources:Admin.ProductVariantAttributes.New.Attribute.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:DropDownList class="text" ID="ddlNewProductAttributes" AutoPostBack="False"
                                    CssClass="adminInput" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblTextPrompt" Text="<% $NopResources:Admin.ProductVariantAttributes.New.TextPrompt %>"
                                    ToolTip="<% $NopResources:Admin.ProductVariantAttributes.New.TextPrompt.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:TextBox runat="server" CssClass="adminInput" ID="txtNewTextPrompt"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeRequired" Text="<% $NopResources:Admin.ProductVariantAttributes.New.Required %>"
                                    ToolTip="<% $NopResources:Admin.ProductVariantAttributes.New.Required.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:CheckBox ID="cbNewProductVariantAttributeIsRequired" runat="server" Checked="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeControlType" Text="<% $NopResources:Admin.ProductVariantAttributes.New.ControlType %>"
                                    ToolTip="<% $NopResources:Admin.ProductVariantAttributes.New.ControlType.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <asp:DropDownList class="text" ID="ddlAttributeControlType" AutoPostBack="False"
                                    CssClass="adminInput" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="adminTitle">
                                <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeDisplayOrder" Text="<% $NopResources:Admin.ProductVariantAttributes.New.DisplayOrder %>"
                                    ToolTip="<% $NopResources:Admin.ProductVariantAttributes.New.DisplayOrder.Tooltip %>"
                                    ToolTipImage="~/Administration/Common/ico-help.gif" />
                            </td>
                            <td class="adminData">
                                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewProductVariantAttributeDisplayOrder"
                                    Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.New.DisplayOrder.RequiredErrorMessage %>"
                                    RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.New.DisplayOrder.RangeErrorMessage %>"
                                    MinimumValue="-99999" MaximumValue="99999" ValidationGroup="NewProductVariantAttribute">
                                </nopCommerce:NumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="left">
                                <asp:Button runat="server" ID="btnNewProductVariantAttribute" CssClass="adminButton"
                                    Text="<% $NopResources:Admin.ProductVariantAttributes.New.AddNewButton.Text %>"
                                    ValidationGroup="NewProductVariantAttribute" OnClick="btnNewProductVariantAttribute_Click"
                                    ToolTip="<% $NopResources:Admin.ProductVariantAttributes.New.AddNewButton.Tooltip %>" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="pnlComb" HeaderText="<% $NopResources:Admin.ProductAdd.AttributeCombinations %>">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlCombinations">
                        <div>
                            <asp:GridView ID="gvCombinations" runat="server" AutoGenerateColumns="false" DataKeyNames="ProductVariantAttributeCombinationId"
                                OnRowDeleting="gvCombinations_RowDeleting" OnRowDataBound="gvCombinations_RowDataBound"
                                OnRowCommand="gvCombinations_RowCommand" Width="100%">
                                <Columns>
                                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.Attributes %>"
                                        ItemStyle-Width="50%">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblAttributes"></asp:Label>
                                            <br />
                                            <asp:Label runat="server" ID="lblWarnings" Style="color: Red;"></asp:Label>
                                            <asp:HiddenField ID="hfProductVariantAttributeCombinationId" runat="server" Value='<%# Eval("ProductVariantAttributeCombinationId") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity %>"
                                        ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtStockQuantity"
                                                Value='<%# Eval("StockQuantity") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity.RequiredErrorMessage %>"
                                                RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.StockQuantity.RangeErrorMessage %>"
                                                ValidationGroup="UpdateProductVariantAttributeCombination" MinimumValue="-99999"
                                                MaximumValue="99999"></nopCommerce:NumericTextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.AllowOutOfStockOrders %>"
                                        ItemStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="cbAllowOutOfStockOrders" runat="server" Checked='<%# Eval("AllowOutOfStockOrders") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.Update %>"
                                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.Update %>"
                                                ValidationGroup="UpdateProductVariantAttributeCombination" CommandName="UpdateProductVariantAttributeCombination" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.Delete %>"
                                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Button ID="btnDelete" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantAttributes.CombinationsGrid.Delete %>"
                                                CausesValidation="false" CommandName="Delete" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div style="margin: 10px 0px 10px 0px;">
                            <%=GetLocaleResourceString("Admin.ProductVariantAttributes.Combinations.SelectCombination")%>
                        </div>
                        <div style="border: 1px dashed #2F4F4F; padding: 15px; margin: 10px 0px 10px 0px;">
                            <nopCommerce:SelectProductAttributes ID="ctrlSelectProductAttributes" runat="server"
                                HidePrices="true"></nopCommerce:SelectProductAttributes>
                            <asp:Panel runat="server" ID="pnlCombinationWarningsr" EnableViewState="false" Visible="false"
                                class="messageBox messageBoxError">
                                <asp:Literal runat="server" ID="lCombinationWarnings" EnableViewState="false" />
                            </asp:Panel>
                        </div>
                        <div>
                            <table class="adminContent">
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblStockQuantity" Text="<% $NopResources:Admin.ProductVariantAttributes.Combinations.StockQuantity %>"
                                            ToolTip="<% $NopResources:Admin.ProductVariantAttributes.Combinations.StockQuantity.Tooltip %>"
                                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtStockQuantity"
                                            RequiredErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.Combinations.StockQuantity.RequiredErrorMessage %>"
                                            MinimumValue="-999999" MaximumValue="999999" Value="10000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantAttributes.Combinations.StockQuantity.RangeErrorMessage %>"
                                            ValidationGroup="NewProductVariantAttributeCombination"></nopCommerce:NumericTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="adminTitle">
                                        <nopCommerce:ToolTipLabel runat="server" ID="lblAllowOutOfStockOrders" Text="<% $NopResources:Admin.ProductVariantAttributes.Combinations.AllowOutOfStockOrders %>"
                                            ToolTip="<% $NopResources:Admin.ProductVariantAttributes.Combinations.AllowOutOfStockOrders.Tooltip %>"
                                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                                    </td>
                                    <td class="adminData">
                                        <asp:CheckBox ID="cbAllowOutOfStockOrders" runat="server"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="left">
                                        <asp:Button runat="server" ID="btnNewProductVariantAttributeCombination" CssClass="adminButton"
                                            Text="<% $NopResources:Admin.ProductVariantAttributes.Combinations.AddNewButton.Text %>"
                                            ValidationGroup="NewProductVariantAttributeCombination" OnClick="btnNewProductVariantAttributeCombination_Click" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upAttr">
    <ProgressTemplate>
        <div class="progress">
            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif" AlternateText="update" />
            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <%=GetLocaleResourceString("Admin.ProductVariantAttributes.AvailableAfterSaving")%>
</asp:Panel>
