<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.ProductPricesByCustomerRoleControl"
    CodeBehind="ProductPricesByCustomerRole.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<asp:Panel runat="server" ID="pnlData">
    <asp:UpdatePanel ID="upPrices" runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
                <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
            </asp:Panel>
            <asp:GridView ID="gvPrices" runat="server" AutoGenerateColumns="false" DataKeyNames="CustomerRoleProductPriceId"
                OnRowDeleting="gvPrices_RowDeleting" OnRowDataBound="gvPrices_RowDataBound"
                OnRowCommand="gvPrices_RowCommand" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPricesByCustomerRole.CustomerRole %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCustomerRole"></asp:Label>
                            <asp:HiddenField ID="hfCustomerRoleProductPriceId" runat="server" Value='<%# Eval("CustomerRoleProductPriceId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPricesByCustomerRole.Price %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtPrice" Value='<%# Eval("Price") %>'
                                RequiredErrorMessage="<% $NopResources:Admin.ProductPricesByCustomerRole.Price.RequiredErrorMessage %>"
                                MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductPricesByCustomerRole.Price.RangeErrorMessage %>"
                                ValidationGroup="CustomerRolePrice" Width="100px"></nopCommerce:DecimalTextBox>
                            [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPricesByCustomerRole.Update %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.ProductPricesByCustomerRole.Update %>"
                                ValidationGroup="CustomerRolePrice" CommandName="UpdateCustomerRolePrice" ToolTip="<% $NopResources:Admin.ProductPricesByCustomerRole.Update.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.ProductPricesByCustomerRole.Delete %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" CssClass="adminButton"
                                Text="Delete" CausesValidation="false" CommandName="<% $NopResources:Admin.ProductPricesByCustomerRole.Delete %>"
                                ToolTip="<% $NopResources:Admin.ProductPricesByCustomerRole.Delete.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p>
                <strong>
                    <%=GetLocaleResourceString("Admin.ProductPricesByCustomerRole.AddNew")%>
                </strong>
            </p>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblNewCustomerRole" Text="<% $NopResources:Admin.ProductPricesByCustomerRole.New.CustomerRole %>"
                            ToolTip="<% $NopResources:Admin.ProductPricesByCustomerRole.New.CustomerRole.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                       <asp:DropDownList ID="ddlNewCustomerRole" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblNewPrice" Text="<% $NopResources:Admin.ProductPricesByCustomerRole.New.Price %>"
                            ToolTip="<% $NopResources:Admin.ProductPricesByCustomerRole.New.Price.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtNewPrice"
                            Value="0" RequiredErrorMessage="<% $NopResources:Admin.ProductPricesByCustomerRole.New.Price.RequiredErrorMessage %>"
                            MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.ProductPricesByCustomerRole.New.Price.RangeErrorMessage %>"
                            ValidationGroup="NewCustomerRolePrice" Width="50px"></nopCommerce:DecimalTextBox>
                        [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left">
                        <asp:Button runat="server" ID="btnNewPrice" CssClass="adminButton" Text="<% $NopResources:Admin.ProductPricesByCustomerRole.New.AddNewButton.Text %>"
                            ValidationGroup="NewCustomerRolePrice" OnClick="btnNewPrice_Click" ToolTip="<% $NopResources:Admin.ProductPricesByCustomerRole.New.AddNewButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upPrices">
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
    <asp:Label runat="server" ID="lblMessage"></asp:Label>
</asp:Panel>
