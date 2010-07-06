<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductSpecificationsControl"
    CodeBehind="ProductSpecifications.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">
    <asp:UpdatePanel ID="upSpecs" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
                <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
            </asp:Panel>
            <asp:GridView ID="gvProductSpecificationAttributes" runat="server" AutoGenerateColumns="false"
                DataKeyNames="ProductSpecificationAttributeId" OnRowDeleting="gvProductSpecificationAttributes_RowDeleting"
                OnRowDataBound="gvProductSpecificationAttributes_RowDataBound" OnRowCommand="gvProductSpecificationAttributes_RowCommand"
                Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.Attribute %>"
                        ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Literal ID="lblSpecificationAttributeName" runat="server" />
                            <asp:HiddenField ID="hfProductSpecificationAttributeId" runat="server" Value='<%# Eval("ProductSpecificationAttributeId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.AttributeOption %>"
                        ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlSpecificationAttributeOption" runat="server" CssClass="adminInput" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.AllowFiltering %>"
                        ItemStyle-Width="15%">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkAllowFiltering" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.ShowOnProductPage %>"
                        ItemStyle-Width="15%">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkShowOnProductPage" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.DisplayOrder %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" Width="50px" ID="txtProductSpecificationAttributeDisplayOrder"
                                Value='<%# Eval("DisplayOrder") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductSpecifications.DisplayOrder.RequiredErrorMessage %>"
                                RangeErrorMessage="<% $NopResources:Admin.ProductSpecifications.DisplayOrder.RangeErrorMessage %>"
                                ValidationGroup="ProductSpecification" MinimumValue="-99999" MaximumValue="99999">
                            </nopCommerce:NumericTextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.Update %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductSpecifications.Update %>"
                                ValidationGroup="ProductSpecification" CommandName="UpdateProductSpecificationAttribute"
                                ToolTip="<% $NopResources:Admin.ProductSpecifications.Update.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductSpecifications.Delete %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnDeleteProductSpecificationAttribute" runat="server" CssClass="adminButton"
                                Text="<% $NopResources:Admin.ProductSpecifications.Delete %>" CausesValidation="false"
                                CommandName="Delete" ToolTip="<% $NopResources:Admin.ProductSpecifications.Delete.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p>
                <strong>
                    <%=GetLocaleResourceString("Admin.ProductSpecifications.AddNew")%></strong>
            </p>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeType" Text="<% $NopResources:Admin.ProductSpecifications.New.AttributeType %>"
                            ToolTip="<% $NopResources:Admin.ProductSpecifications.New.AttributeType.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList class="text" ID="ddlNewProductSpecificationAttribute" AutoPostBack="true"
                            CssClass="adminInput" runat="server" OnSelectedIndexChanged="OnSpecificationAttributeIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAttributeOption" Text="<% $NopResources:Admin.ProductSpecifications.New.AttributeOption %>"
                            ToolTip="<% $NopResources:Admin.ProductSpecifications.New.AttributeOption.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlNewProductSpecificationAttributeOption" runat="server" ErrorMessage="Attribute option is required"
                            CssClass="adminInput" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAllowFiltering" Text="<% $NopResources:Admin.ProductSpecifications.New.AllowFiltering%>"
                            ToolTip="<% $NopResources:Admin.ProductSpecifications.New.AllowFiltering.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox ID="chkNewAllowFiltering" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShowOnProductPage" Text="<% $NopResources:Admin.ProductSpecifications.New.ShowOnProductPage %>"
                            ToolTip="<% $NopResources:Admin.ProductSpecifications.New.ShowOnProductPage.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox ID="chkNewShowOnProductPage" runat="server" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.ProductSpecifications.New.DisplayOrder %>"
                            ToolTip="<% $NopResources:Admin.ProductSpecifications.New.DisplayOrder.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewProductSpecificationAttributeDisplayOrder"
                            Value="1" RequiredErrorMessage="<% $NopResources:Admin.ProductSpecifications.New.DisplayOrder.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.ProductSpecifications.New.DisplayOrder.RangeErrorMessage %>"
                            MinimumValue="-99999" MaximumValue="99999" ValidationGroup="NewProductSpecification">
                        </nopCommerce:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left">
                        <asp:Button runat="server" ID="btnNewProductSpecification" CssClass="adminButton"
                            Text="<% $NopResources:Admin.ProductSpecifications.AddNewButton %>" ValidationGroup="NewProductSpecification"
                            OnClick="btnNewProductSpecification_Click" ToolTip="<% $NopResources:Admin.ProductSpecifications.AddNewButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upSpecs">
        <ProgressTemplate>
            <div class="progress">
                <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                    AlternateText="update" />
                <%=GetLocaleResourceString("Admin.Common.Wait...")%>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <%=GetLocaleResourceString("Admin.ProductSpecifications.AvailableAfterSaving")%>
</asp:Panel>
