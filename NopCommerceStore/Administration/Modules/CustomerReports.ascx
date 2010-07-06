<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerReportsControl"
    CodeBehind="CustomerReports.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="CustomerStatistics" Src="CustomerStatistics.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-sales.png" alt="<%=GetLocaleResourceString("Admin.CustomerReports.Title")%>" />
        <%=GetLocaleResourceString("Admin.CustomerReports.Title")%>
    </div>
    <div class="options">
    </div>
</div>
<ajaxToolkit:TabContainer runat="server" ID="Reports" ActiveTabIndex="0">
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerByOrderTotal" HeaderText="<% $NopResources:Admin.CustomerReports.ByOrderTotal.Title %>">
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblStartDateByOrderTotal" Text="<% $NopResources:Admin.CustomerReports.ByOrderTotal.StartDate %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByOrderTotal.StartDate.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePickerByOrderTotal" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblEndDateByOrderTotal" Text="<% $NopResources:Admin.CustomerReports.ByOrderTotal.EndDate %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByOrderTotal.EndDate.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePickerByOrderTotal" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderStatusByOrderTotal" Text="<% $NopResources:Admin.CustomerReports.ByOrderTotal.OrderStatus %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByOrderTotal.OrderStatus.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlOrderStatusByOrderTotal" runat="server" CssClass="adminInput">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentStatusByOrderTotal" Text="<% $NopResources:Admin.CustomerReports.ByOrderTotal.PaymentStatus %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByOrderTotal.PaymentStatus.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlPaymentStatusByOrderTotal" runat="server" CssClass="adminInput">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShippingStatusByOrderTotal" Text="<% $NopResources:Admin.CustomerReports.ByOrderTotal.ShippingStatus %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByOrderTotal.ShippingStatus.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlShippingStatusByOrderTotal" runat="server" CssClass="adminInput">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                    </td>
                    <td class="adminData">
                        <asp:Button ID="btnSearchByOrderTotal" runat="server" Text="<% $NopResources:Admin.CustomerReports.ByOrderTotal.SearchButton %>"
                            CssClass="adminButtonBlue" OnClick="btnSearchByOrderTotal_Click" ToolTip="<% $NopResources:Admin.CustomerReports.ByOrderTotal.SearchButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
            <p>
            </p>
            <asp:GridView ID="gvByOrderTotal" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByOrderTotal.CustomerColumn %>"
                        ItemStyle-Width="50%">
                        <ItemTemplate>
                            <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByOrderTotal.OrderTotalColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("OrderTotal")), true, false))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByOrderTotal.NumberOfOrdersColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Eval("OrderCount")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlCustomerByNumberOfOrder" HeaderText="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.Title %>">
        <ContentTemplate>
         <table width="100%">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblStartDateByNumberOfOrder" Text="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.StartDate %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.StartDate.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DatePicker runat="server" ID="ctrlStartDatePickerByNumberOfOrder" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblEndDateByNumberOfOrder" Text="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.EndDate %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.EndDate.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:DatePicker runat="server" ID="ctrlEndDatePickerByNumberOfOrder" />
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblOrderStatusByNumberOfOrder" Text="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.OrderStatus %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.OrderStatus.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlOrderStatusByNumberOfOrder" runat="server" CssClass="adminInput">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblPaymentStatusByNumberOfOrder" Text="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.PaymentStatus %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.PaymentStatus.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlPaymentStatusByNumberOfOrder" runat="server" CssClass="adminInput">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblShippingStatusByNumberOfOrder" Text="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.ShippingStatus %>"
                            ToolTip="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.ShippingStatus.Tooltip %>"
                            ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <asp:DropDownList ID="ddlShippingStatusByNumberOfOrder" runat="server" CssClass="adminInput">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                    </td>
                    <td class="adminData">
                        <asp:Button ID="btnSearchByNumberOfOrder" runat="server" Text="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.SearchButton %>"
                            CssClass="adminButtonBlue" OnClick="btnSearchByNumberOfOrder_Click" ToolTip="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.SearchButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
            <p>
            </p>
            <asp:GridView ID="gvByNumberOfOrder" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.CustomerColumn %>"
                        ItemStyle-Width="50%">
                        <ItemTemplate>
                            <%#GetCustomerInfo(Convert.ToInt32(Eval("CustomerId")))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.OrderTotalColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Server.HtmlEncode(PriceHelper.FormatPrice(Convert.ToDecimal(Eval("OrderTotal")), true, false))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByNumberOfOrder.NumberOfOrdersColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Eval("OrderCount")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>        
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlRegisteredCustomer" HeaderText="<% $NopResources:Admin.CustomerReports.RegisteredCustomers.Title %>">
        <ContentTemplate>
            <nopCommerce:CustomerStatistics runat="server" ID="ctrlCustomerStatistics" DisplayTitle="false" />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlByLanguage" HeaderText="<% $NopResources:Admin.CustomerReports.ByLanguage.Title %>">
        <ContentTemplate>
             <%=GetLocaleResourceString("Admin.CustomerReports.ByLanguage.Tooltip")%>
             <br />
             <br />
             <asp:GridView ID="gvByLanguage" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByLanguage.LanguageColumn %>"
                        ItemStyle-Width="75%">
                        <ItemTemplate>
                            <%#GetLanguageInfo(Convert.ToInt32(Eval("LanguageId")))%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByLanguage.CustomerCountColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Eval("CustomerCount")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlByGender" HeaderText="<% $NopResources:Admin.CustomerReports.ByGender.Title %>">
        <ContentTemplate>
             <%=GetLocaleResourceString("Admin.CustomerReports.ByGender.Tooltip")%>
             <br />
             <br />
             <asp:GridView ID="gvByGender" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByGender.GenderColumn %>"
                        ItemStyle-Width="75%">
                        <ItemTemplate>
                            <%#GetGenderInfo(Eval("AttributeKey").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByGender.CustomerCountColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Eval("CustomerCount")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="pnlByCountry" HeaderText="<% $NopResources:Admin.CustomerReports.ByCountry.Title %>">
        <ContentTemplate>
             <%=GetLocaleResourceString("Admin.CustomerReports.ByCountry.Tooltip")%>
             <br />
             <br />
             <asp:GridView ID="gvByCountry" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByCountry.CountryColumn %>"
                        ItemStyle-Width="75%">
                        <ItemTemplate>
                            <%#GetCountryInfo(Eval("AttributeKey").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerReports.ByCountry.CustomerCountColumn %>"
                        ItemStyle-Width="25%">
                        <ItemTemplate>
                            <%#Eval("CustomerCount")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
