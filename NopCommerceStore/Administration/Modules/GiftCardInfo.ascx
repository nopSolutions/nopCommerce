<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.GiftCardInfoControl"
    CodeBehind="GiftCardInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="EmailTextBox" Src="EmailTextBox.ascx" %>
<ajaxToolkit:TabContainer runat="server" ID="GiftCardTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlGiftCardInfo" HeaderText="<% $NopResources:Admin.GiftCardInfo.GiftCardInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderTitle" Text="<% $NopResources:Admin.GiftCardInfo.Order %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.Order.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblOrder" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCustomerTitle" Text="<% $NopResources:Admin.GiftCardInfo.Customer %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.Customer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblInitialValue" Text="<% $NopResources:Admin.GiftCardInfo.InitialValue %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.InitialValue.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtInitialValue"
                            Value="0" RequiredErrorMessage="<% $NopResources:Admin.GiftCardInfo.InitialValue.RequiredErrorMessage %>"
                            MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.GiftCardInfo.InitialValue.RangeErrorMessage %>">
                        </nopCommerce:DecimalTextBox>
                        [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRemainingAmountTitle" Text="<% $NopResources:Admin.GiftCardInfo.RemainingAmount %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.RemainingAmount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label runat="server" ID="lblRemainingAmount"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblIsGiftCardActivated" Text="<% $NopResources:Admin.GiftCardInfo.IsGiftCardActivated %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.IsGiftCardActivated.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox runat="server" ID="cbIsGiftCardActivated"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCouponCode" Text="<% $NopResources:Admin.GiftCardInfo.CouponCode %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.CouponCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox ID="txtCouponCode" runat="server" CssClass="adminInput" ValidationGroup="GenerateCode"></asp:TextBox>
                        <asp:Button ID="btnReGenerateNewCode" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.GiftCardInfo.CouponCode.ReGenerateNewButton %>"
                            OnClick="btnReGenerateNewCode_Click" ValidationGroup="GenerateCode"></asp:Button>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRecipientName" Text="<% $NopResources:Admin.GiftCardInfo.RecipientName %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.RecipientName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtRecipientName"
                            ErrorMessage="<% $NopResources:Admin.GiftCardInfo.RecipientName.ErrorMessage %>">
                        </nopCommerce:SimpleTextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlRecipientEmail">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRecipientEmail" Text="<% $NopResources:Admin.GiftCardInfo.RecipientEmail %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.RecipientEmail.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:EmailTextBox runat="server" ID="txtRecipientEmail" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblSenderName" Text="<% $NopResources:Admin.GiftCardInfo.SenderName %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.SenderName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" CssClass="adminInput" ID="txtSenderName"
                            ErrorMessage="<% $NopResources:Admin.GiftCardInfo.SenderName.ErrorMessage %>">
                        </nopCommerce:SimpleTextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlSenderEmail">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblSenderEmail" Text="<% $NopResources:Admin.GiftCardInfo.SenderEmail %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.SenderEmail.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:EmailTextBox runat="server" ID="txtSenderEmail" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblMessage" Text="<% $NopResources:Admin.GiftCardInfo.Message %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.Message.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox runat="server" ID="txtMessage" TextMode="MultiLine" Height="150px" Width="500px"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlIsRecipientNotified">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblIsRecipientNotifiedTooltip" Text="<% $NopResources:Admin.GiftCardInfo.IsRecipientNotified %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.IsRecipientNotified.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label runat="server" ID="lblIsRecipientNotified"></asp:Label>
                        <asp:Button ID="btnNotifyRecipient" CssClass="adminButton" runat="server" Text="<% $NopResources:Admin.GiftCardInfo.NotifyRecipientButton %>"
                            OnClick="btnNotifyRecipient_Click"></asp:Button>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPurchasedOnTooltip" Text="<% $NopResources:Admin.GiftCardInfo.PurchasedOn %>"
                            ToolTip="<% $NopResources:Admin.GiftCardInfo.PurchasedOn.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:Label runat="server" ID="lblPurchasedOn"></asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlUsageHistory" HeaderText="<% $NopResources:Admin.GiftCardInfo.UsageHistory %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td>
                        <asp:GridView ID="gvUsageHistory" runat="server" AutoGenerateColumns="False" Width="100%"
                            OnPageIndexChanging="gvUsageHistory_PageIndexChanging" AllowPaging="true" PageSize="15">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.GiftCardInfo.UsageHistory.UsedValueColumn %>"
                                    ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <%#GetUsedValueInfo((GiftCardUsageHistory)Container.DataItem)%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.GiftCardInfo.UsageHistory.ByCustomerColumn %>"
                                    ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <%#GetUsedByCustomerInfo((GiftCardUsageHistory)Container.DataItem)%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.GiftCardInfo.UsageHistory.OrderColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderId")%>">
                                            <%#GetLocaleResourceString("Admin.GiftCardInfo.UsageHistory.OrderColumn.View")%>
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.GiftCardInfo.UsageHistory.RecordDateColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
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
