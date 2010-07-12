<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductVariantTierPricesControl"
    CodeBehind="ProductVariantTierPrices.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">
    <asp:UpdatePanel ID="upTierPrices" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
                <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
            </asp:Panel>
            <asp:GridView ID="gvTierPrices" runat="server" AutoGenerateColumns="false" DataKeyNames="TierPriceId"
                OnRowDeleting="gvTierPrices_RowDeleting" OnRowDataBound="gvTierPrices_RowDataBound"
                OnRowCommand="gvTierPrices_RowCommand" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantTierPrices.Quantity %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtQuantity"
                                Value='<%# Eval("Quantity") %>' RequiredErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.Quantity.RequiredErrorMessage %>"
                                RangeErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.Quantity.RangeErrorMessage %>"
                                ValidationGroup="TierPrice" MinimumValue="1" MaximumValue="99999" Width="50px">
                            </nopCommerce:NumericTextBox>
                            <%#GetLocaleResourceString("Admin.ProductVariantTierPrices.Quantity.AndAbove")%>
                            <asp:HiddenField ID="hfTierPriceId" runat="server" Value='<%# Eval("TierPriceId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantTierPrices.Price %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtPrice" Value='<%# Eval("Price") %>'
                                RequiredErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.Price.RequiredErrorMessage %>"
                                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.Price.RangeErrorMessage %>"
                                ValidationGroup="TierPrice" Width="100px"></nopCommerce:DecimalTextBox>
                            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantTierPrices.Update %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantTierPrices.Update %>"
                                ValidationGroup="TierPrice" CommandName="UpdateTierPrice" ToolTip="<% $NopResources:Admin.ProductVariantTierPrices.Update.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductVariantTierPrices.Delete %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantTierPrices.Delete %>"
                                CausesValidation="false" CommandName="Delete" ToolTip="<% $NopResources:Admin.ProductVariantTierPrices.Delete.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p>
                <strong>
                    <%=GetLocaleResourceString("Admin.ProductVariantTierPrices.AddNew")%>
                </strong>
            </p>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblNewQuantity" Text="<% $NopResources:Admin.ProductVariantTierPrices.New.Quantity %>"
                            ToolTip="<% $NopResources:Admin.ProductVariantTierPrices.New.Quantity.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewQuantity"
                            RequiredErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.New.Quantity.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.New.Quantity.RangeErrorMessage %>"
                            ValidationGroup="NewTierPrice" MinimumValue="1" MaximumValue="99999" Value="2"
                            Width="50px"></nopCommerce:NumericTextBox>
                        <%=GetLocaleResourceString("Admin.ProductVariantTierPrices.New.Quantity.AndAbove")%>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblNewPrice" Text="<% $NopResources:Admin.ProductVariantTierPrices.New.Price %>"
                            ToolTip="<% $NopResources:Admin.ProductVariantTierPrices.New.Price.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtNewPrice"
                            Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.New.Price.RequiredErrorMessage %>"
                            MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductVariantTierPrices.New.Price.RangeErrorMessage %>"
                            ValidationGroup="NewTierPrice" Width="50px"></nopCommerce:DecimalTextBox>
                        [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left">
                        <asp:Button runat="server" ID="btnNewTierPrice" CssClass="adminButton" Text="<% $NopResources:Admin.ProductVariantTierPrices.New.AddNewButton.Text %>"
                            ValidationGroup="NewTierPrice" OnClick="btnNewTierPrice_Click" ToolTip="<% $NopResources:Admin.ProductVariantTierPrices.New.AddNewButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upTierPrices">
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
    <%=GetLocaleResourceString("Admin.ProductVariantTierPrices.AvailableAfterSaving")%>
</asp:Panel>
