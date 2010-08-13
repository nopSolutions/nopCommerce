<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.DiscountInfoControl"
    CodeBehind="DiscountInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCustomerRolesControl" Src="SelectCustomerRolesControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>

<script type="text/javascript">
    $(document).ready(function () {
        toggleLimitation();
        toggleUsePercentage();
        toggleRequiresCouponCode();
    });


    function toggleLimitation() {
        var selectedDiscountLimitation = document.getElementById('<%=ddlDiscountLimitation.ClientID %>');
        var selectedDiscountLimitationId = selectedDiscountLimitation.options[selectedDiscountLimitation.selectedIndex].value;
        if (selectedDiscountLimitationId == 15 || selectedDiscountLimitationId == 25) {
            //'N Times Only' or 'N Times Per Customer'
            $('#pnlLimitationTimes').show();
        }
        else {
            $('#pnlLimitationTimes').hide();

        }
    }

    function toggleUsePercentage() {
        if (getE('<%=cbUsePercentage.ClientID %>').checked) {
            $('#pnlDiscountPercentage').show();
            $('#pnlDiscountAmount').hide();
        }
        else {
            $('#pnlDiscountPercentage').hide();
            $('#pnlDiscountAmount').show();
        }
    }

    function toggleRequiresCouponCode() {
        if (getE('<%=cbRequiresCouponCode.ClientID %>').checked) {
            $('#pnlCouponCode').show();
        }
        else {
            $('#pnlCouponCode').hide();
        }
    }
</script>

<ajaxToolkit:TabContainer runat="server" ID="DiscountTabs" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlDiscountInfo" HeaderText="<% $NopResources:Admin.DiscountInfo.DiscountInfo %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDiscountType" Text="<% $NopResources:Admin.DiscountInfo.DiscountType %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.DiscountType.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlDiscountType" CssClass="adminInput" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDiscountRequirement" Text="<% $NopResources:Admin.DiscountInfo.DiscountRequirement %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.DiscountRequirement.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlDiscountRequirement" AutoPostBack="True" CssClass="adminInput"
                            runat="server" OnSelectedIndexChanged="ddlDiscountRequirement_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="pnlRestrictedProductVariants">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRestrictedProductVariantsTitle" Text="<% $NopResources:Admin.DiscountInfo.RestrictedProductVariants %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.RestrictedProductVariants.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox ID="txtRestrictedProductVariants" runat="server" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
                <tr runat="server" id="pnlRequirementSpentAmount">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRequirementSpentAmount" Text="<% $NopResources:Admin.DiscountInfo.RequirementSpentAmount %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.RequirementSpentAmount.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtRequirementSpentAmount"
                            Value="0" RequiredErrorMessage="<% $NopResources:Admin.DiscountInfo.RequirementSpentAmount.RequiredErrorMessage %>"
                            MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.DiscountInfo.RequirementSpentAmount.RangeErrorMessage %>">
                        </nopCommerce:DecimalTextBox>
                        [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                    </td>
                </tr>                
                <tr runat="server" id="pnlRequirementBillingCountryIs">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRequirementBillingCountryIs" Text="<% $NopResources:Admin.DiscountInfo.RequirementBillingCountryIs %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.RequirementBillingCountryIs.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlRequirementBillingCountryIs" AutoPostBack="False" runat="server" CssClass="adminInput" />
                    </td>
                </tr>             
                <tr runat="server" id="pnlRequirementShippingCountryIs">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRequirementShippingCountryIs" Text="<% $NopResources:Admin.DiscountInfo.RequirementShippingCountryIs %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.RequirementShippingCountryIs.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlRequirementShippingCountryIs" AutoPostBack="False" runat="server" CssClass="adminInput" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDiscountLimitation" Text="<% $NopResources:Admin.DiscountInfo.DiscountLimitation %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.DiscountLimitation.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlDiscountLimitation" CssClass="adminInput" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="pnlLimitationTimes">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblLimitationTimes" Text="<% $NopResources:Admin.DiscountInfo.LimitationTimes %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.LimitationTimes.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtLimitationTimes"
                            Value="1" RequiredErrorMessage="<% $NopResources:Admin.DiscountInfo.LimitationTimes.RequiredErrorMessage%>"
                            MinimumValue="1" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.DiscountInfo.LimitationTimes.RangeErrorMessage %>">
                        </nopCommerce:NumericTextBox> <%=GetLocaleResourceString("Admin.DiscountInfo.LimitationTimes.Times")%>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.DiscountInfo.Name %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.DiscountInfo.Name.ErrorMessage %>">
                        </nopCommerce:SimpleTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblUsePercentage" Text="<% $NopResources:Admin.DiscountInfo.UsePercentage %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.UsePercentage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox runat="server" ID="cbUsePercentage"></asp:CheckBox>
                    </td>
                </tr>
                <tr id="pnlDiscountPercentage">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDiscountPercentage" Text="<% $NopResources:Admin.DiscountInfo.DiscountPercentage %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.DiscountPercentage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtDiscountPercentage"
                            Value="0" RequiredErrorMessage="<% $NopResources:Admin.DiscountInfo.DiscountPercentage.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.DiscountInfo.DiscountPercentage.RangeErrorMessage %>"
                            MinimumValue="0" MaximumValue="100"></nopCommerce:DecimalTextBox>
                    </td>
                </tr>
                <tr id="pnlDiscountAmount">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblDiscountAmount" Text="<% $NopResources:Admin.DiscountInfo.DiscountAmount %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.DiscountAmount.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                       </td>
                    <td class="adminData">
                        <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtDiscountAmount"
                            Value="0" RequiredErrorMessage="<% $NopResources:Admin.DiscountInfo.DiscountAmount.RequiredErrorMessage %>"
                            MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.DiscountInfo.DiscountAmount.RangeErrorMessage %>">
                        </nopCommerce:DecimalTextBox>
                        [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblStartDate" Text="<% $NopResources:Admin.DiscountInfo.StartDate %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.StartDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePicker" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblEndDate" Text="<% $NopResources:Admin.DiscountInfo.EndDate %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.EndDate.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePicker" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblRequiresCouponCode" Text="<% $NopResources:Admin.DiscountInfo.RequiresCouponCode %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.RequiresCouponCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:CheckBox runat="server" ID="cbRequiresCouponCode"></asp:CheckBox>
                    </td>
                </tr>
                <tr id="pnlCouponCode">
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblCouponCode" Text="<% $NopResources:Admin.DiscountInfo.CouponCode %>"
                            ToolTip="<% $NopResources:Admin.DiscountInfo.CouponCode.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:TextBox ID="txtCouponCode" runat="server" CssClass="adminInput"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerRoles" HeaderText="<% $NopResources:Admin.DiscountInfo.CustomerRoles %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td>
                        <nopCommerce:SelectCustomerRolesControl ID="CustomerRoleMappingControl" runat="server"
                            CssClass="adminInput"></nopCommerce:SelectCustomerRolesControl>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlUsageHistory" HeaderText="<% $NopResources:Admin.DiscountInfo.UsageHistory %>">
        <ContentTemplate>
            <table class="adminContent">
                <tr>
                    <td>
                        <asp:GridView ID="gvDiscountUsageHistory" runat="server" AutoGenerateColumns="False"
                            Width="100%" OnPageIndexChanging="gvDiscountUsageHistory_PageIndexChanging" AllowPaging="true"
                            PageSize="15">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.DiscountInfo.UsageHistory.CustomerColumn %>"
                                    ItemStyle-Width="40%">
                                    <ItemTemplate>
                                        <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.DiscountInfo.UsageHistory.OrderColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="40%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <a href="OrderDetails.aspx?OrderID=<%#Eval("OrderId")%>">
                                            <%#GetLocaleResourceString("Admin.DiscountInfo.UsageHistory.OrderColumn.View")%>
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.DiscountInfo.UsageHistory.UsedOnColumn %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.DiscountInfo.UsageHistory.DeleteColumn %>" HeaderStyle-HorizontalAlign="Center"
                                    ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="DeleteUsageHistoryButton" runat="server" CssClass="adminButton" CommandName="DeleteUsageHistory"
                                            Text="<% $NopResources:Admin.DiscountInfo.UsageHistory.DeleteButton.Text %>" CommandArgument='<%#Eval("DiscountUsageHistoryId")%>'
                                            OnCommand="DeleteUsageHistoryButton_OnCommand" CausesValidation="false" ToolTip="<% $NopResources:Admin.DiscountInfo.UsageHistory.DeleteButton.Tooltip %>" />
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
