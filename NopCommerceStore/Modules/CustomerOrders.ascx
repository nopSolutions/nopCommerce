<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.CustomerOrdersControl"
    CodeBehind="CustomerOrders.ascx.cs" %>
<div class="customer-orders">
    <asp:Panel runat="server" ID="pnlRecurringPayments" CssClass="recurring-payments">
        <div class="section-title">
            <%=GetLocaleResourceString("Order.RecurringPayments")%>
        </div>
        <div class="clear">
        </div>
        <div class="recurring-payments-box">
            <asp:GridView ID="gvRecurringPayments" runat="server" AutoGenerateColumns="False"
                Width="100%" DataKeyNames="RecurringPaymentId" OnRowDataBound="gvRecurringPayments_RowDataBound"
                OnRowCommand="gvRecurringPayments_RowCommand" EnableViewState="false">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.RecurringPayments.StartDateColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("StartDate"), DateTimeKind.Utc).ToString()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.RecurringPayments.CycleInfoColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#GetCycleInfo((RecurringPayment)Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.RecurringPayments.NextPaymentColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#GetNextPaymentInfo((RecurringPayment)Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TotalCycles" HeaderText="<% $NopResources:Order.RecurringPayments.TotalCyclesColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="CyclesRemaining" HeaderText="<% $NopResources:Order.RecurringPayments.CyclesRemainingColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.RecurringPayments.InitialOrderColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#GetInitialOrderInfo((RecurringPayment)Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Order.RecurringPayments.CancelColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button runat="server" ID="btnCancelRecurringPayment" CommandName="CancelRecurringPayment"
                                Text="<% $NopResources:Order.RecurringPayments.Cancel %>" ValidationGroup="CancelRecurringPaymentButton"
                                CssClass="cancelrecurringorderbutton" />
                            <asp:HiddenField ID="hfRecurringPaymentId" runat="server" Value='<%# Eval("RecurringPaymentId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
        </div>
    </asp:Panel>
    <div class="clear">
    </div>
    <div class="order-list">
        <asp:Repeater ID="rptrOrders" runat="server" EnableViewState="false" OnItemDataBound="rptrOrders_ItemDataBound">
            <ItemTemplate>
                <div class="order-item">
                    <table width="100%" cellspacing="0" cellpadding="2" border="0">
                        <tbody>
                            <tr>
                                <td style="vertical-align: middle;">
                                    <b>
                                        <%=GetLocaleResourceString("Account.OrderNumber")%>:
                                        <%#Eval("OrderId")%></b>
                                </td>
                                <td align="right">
                                    <asp:PlaceHolder runat="server" ID="phReturnRequest">
                                        <asp:Button runat="server" ID="btnReturnItems" OnCommand="btnReturnItems_Click" Text="<% $NopResources:OrderDetails.ReturnItemsButton %>"
                                            ValidationGroup="OrderDetails" CommandArgument='<%# Eval("OrderId") %>' CssClass="returnitemsbutton" />
                                        &nbsp;&nbsp;&nbsp; </asp:PlaceHolder>
                                    <asp:Button runat="server" ID="btnOrderDetails" OnCommand="btnOrderDetails_Click"
                                        Text="<% $NopResources:Common.Details %>" ValidationGroup="OrderDetails" CommandArgument='<%# Eval("OrderId") %>'
                                        CssClass="orderdetailsbutton" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table cellspacing="0" cellpadding="2" border="0">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <div>
                                                        <%=GetLocaleResourceString("Order.OrderStatus")%>
                                                        <%#OrderManager.GetOrderStatusName(Convert.ToInt32(Eval("OrderStatusId")))%></div>
                                                    <div>
                                                        <%=GetLocaleResourceString("Account.OrderDate")%>:
                                                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%></div>
                                                    <div>
                                                        <%=GetLocaleResourceString("Account.OrderTotal")%>:
                                                        <%# GetOrderTotal(Container.DataItem as Order)%>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </ItemTemplate>
            <SeparatorTemplate>
                <div class="clear">
                </div>
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
</div>
