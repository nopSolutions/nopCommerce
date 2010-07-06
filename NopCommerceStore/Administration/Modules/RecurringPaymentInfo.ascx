<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.RecurringPaymentInfoControl"
    CodeBehind="RecurringPaymentInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>

<ajaxToolkit:TabContainer runat="server" ID="RecurringPaymentTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlRecurringPaymentInfo" HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.RecurringPaymentInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblInitialOrderTitle" Text="<% $NopResources:Admin.RecurringPaymentInfo.InitialOrder %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.InitialOrder.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblInitialOrder" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.RecurringPaymentInfo.Customer %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCycleLength" Text="<% $NopResources:Admin.RecurringPaymentInfo.CycleLength %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.CycleLength.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" ID="txtCycleLength" CssClass="adminInput"
                            Value="1" RequiredErrorMessage="<% $NopResources:Admin.RecurringPaymentInfo.CycleLength.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.RecurringPaymentInfo.CycleLength.RangeErrorMessage %>"
                            MinimumValue="1" MaximumValue="99999"></nopCommerce:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCyclePeriod" Text="<% $NopResources:Admin.RecurringPaymentInfo.CyclePeriod %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.CyclePeriod.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlCyclePeriod" CssClass="adminInput" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblTotalCycles" Text="<% $NopResources:Admin.RecurringPaymentInfo.TotalCycles %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.TotalCycles.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" ID="txtTotalCycles" CssClass="adminInput"
                            Value="1" RequiredErrorMessage="<% $NopResources:Admin.RecurringPaymentInfo.TotalCycles.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.RecurringPaymentInfo.TotalCycles.RangeErrorMessage %>"
                            MinimumValue="1" MaximumValue="99999"></nopCommerce:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCyclesRemainingTitle" Text="<% $NopResources:Admin.RecurringPaymentInfo.CyclesRemaining %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.CyclesRemaining.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCyclesRemaining" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRecurringPaymentTypeTitle" Text="<% $NopResources:Admin.RecurringPaymentInfo.RecurringPaymentType %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.RecurringPaymentType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblRecurringPaymentType" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblStartDateTitle" Text="<% $NopResources:Admin.RecurringPaymentInfo.StartDate %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.StartDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblIsActive" Text="<% $NopResources:Admin.RecurringPaymentInfo.IsActive %>"
                            ToolTip="<% $NopResources:Admin.RecurringPaymentInfo.IsActive.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox runat="server" ID="cbIsActive"></asp:CheckBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlRecurringPaymentHistory" HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.History %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblNextPaymentDate"></asp:Label>
                        <asp:Button ID="btnProcessNextPayment" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.RecurringPaymentInfo.ProcessNextPaymentButton.Text %>"
                            OnClick="btnProcessNextPayment_Click"></asp:Button><nopCommerce:ConfirmationBox runat="server"
                                ID="cbProcessNextPayment" TargetControlID="btnProcessNextPayment" YesText="<% $NopResources:Admin.Common.Yes %>"
                                NoText="<% $NopResources:Admin.Common.No %>" ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
<asp:Button ID="btnCancelPayment" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.RecurringPaymentInfo.CancelPaymentButton.Text %>"
                            OnClick="btnCancelPayment_Click"></asp:Button><nopCommerce:ConfirmationBox runat="server"
                                ID="cbCancelPayment" TargetControlID="btnCancelPayment" YesText="<% $NopResources:Admin.Common.Yes %>"
                                NoText="<% $NopResources:Admin.Common.No %>" ConfirmText="<% $NopResources:Admin.Common.AreYouSure %>" />
                    </td>
                </tr>
            </table>
            <table class="adminContent">
                <tr>
                    <td>
                        <asp:GridView ID="gvRecurringPaymentHistory" runat="server" AutoGenerateColumns="False"
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.History.OrderColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderId")%>">
                                            <%#GetLocaleResourceString("Admin.RecurringPaymentInfo.History.OrderColumn.View")%>
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.History.OrderStatusColumn %>"
                                    ItemStyle-Width="10%">
                                    <ItemTemplate>
                                        <%#OrderManager.GetOrderStatusName(((Order)(Eval("Order"))).OrderStatusId)%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.History.PaymentStatusColumn %>"
                                    ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <%#PaymentStatusManager.GetPaymentStatusName(((Order)(Eval("Order"))).PaymentStatusId)%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.History.ShippingStatusColumn %>"
                                    ItemStyle-Width="15%">
                                    <ItemTemplate>
                                        <%#ShippingStatusManager.GetShippingStatusName(((Order)(Eval("Order"))).ShippingStatusId)%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.RecurringPaymentInfo.History.CreatedOnColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
