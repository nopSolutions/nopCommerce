<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CurrenciesControl"
    CodeBehind="Currencies.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ExchangeRateProviderList" Src="ExchangeRateProviderList.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.Currencies.Title")%>" />
        <%=GetLocaleResourceString("Admin.Currencies.Title")%>
    </div>
    <div class="options">
        <asp:Button runat="server" Text="<% $NopResources:Admin.Currencies.SaveButton.Text %>"
            CssClass="adminButtonBlue" ID="btnSave" OnClick="btnSave_Click" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Currencies.GetRateButton.Text %>"
            CssClass="adminButtonBlue" ID="btnGetLiveRates" ValidationGroup="GetLiveRates"
            OnClick="btnGetLiveRates_Click" ToolTip="<% $NopResources:Admin.Currencies.GetRateButton.Tooltip %>" />
        <input type="button" onclick="location.href='CurrencyAdd.aspx'" value="<%=GetLocaleResourceString("Admin.Currencies.AddNewButton.Text")%>"
            id="btnAddNew" class="adminButtonBlue" title="<%=GetLocaleResourceString("Admin.Currencies.AddNewButton.Tooltip")%>" />
    </div>
</div>
<table>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblExchangeRateProvider" Text="<% $NopResources:Admin.Currencies.ExchangeRateProvider %>"
                ToolTip="<% $NopResources:Admin.Currencies.ExchangeRateProvider.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:ExchangeRateProviderList ID="ddlExchangeRateProviders" runat="server"
                AutoPostBack="false" CssClass="adminInput" />
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCurrencyRateAutoUpdateEnabled" Text="<% $NopResources:Admin.Currencies.CurrencyRateAutoUpdateEnabled %>"
                ToolTip="<% $NopResources:Admin.Currencies.CurrencyRateAutoUpdateEnabled.ToolTip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">            
            <asp:CheckBox ID="cbCurrencyRateAutoUpdateEnabled" runat="server" Checked="False"></asp:CheckBox>
        </td>
    </tr>
</table>
<br />
<asp:GridView ID="gvCurrencies" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.Name %>" ItemStyle-Width="20%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
                <asp:HiddenField runat="server" ID="hfCurrencyId" Value='<%#Eval("CurrencyId")%>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="CurrencyCode" HeaderText="<% $NopResources:Admin.Currencies.CurrencyCode %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="DisplayLocale" HeaderText="<% $NopResources:Admin.Currencies.DisplayLocale %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="Rate" HeaderText="<% $NopResources:Admin.Currencies.Rate %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="DisplayOrder" HeaderText="<% $NopResources:Admin.Currencies.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.PrimaryExchangeRateCurrency %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:GlobalRadioButton runat="server" ID="rdbIsPrimaryExchangeRateCurrency"
                    Checked='<%#Eval("IsPrimaryExchangeRateCurrency")%>' GroupName="PrimaryExchangeRateCurrency"
                    ToolTip="<% $NopResources:Admin.Currencies.PrimaryExchangeRateCurrency.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.PrimaryStoreCurrency %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:GlobalRadioButton runat="server" ID="rdbIsPrimaryStoreCurrency" Checked='<%#Eval("IsPrimaryStoreCurrency")%>'
                    GroupName="PrimaryStoreCurrency" ToolTip="<% $NopResources:Admin.Currencies.PrimaryStoreCurrency.Tooltip %>" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.Published %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:ImageCheckBox runat="server" ID="cbPublished" Checked='<%# Eval("Published") %>'>
                </nopCommerce:ImageCheckBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="CurrencyDetails.aspx?CurrencyID=<%#Eval("CurrencyId")%>" title="<%#GetLocaleResourceString("Admin.Currencies.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.Currencies.Edit")%></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<p>
</p>
<h4>
    <%=GetLocaleResourceString("Admin.Currencies.LiveCurrencyRates")%></h4>
<p>
</p>
<asp:GridView ID="gvLiveRates" runat="server" AutoGenerateColumns="False" Width="400px"
    OnRowCommand="gvLiveRates_RowCommand" OnRowDataBound="gvLiveRates_RowDataBound">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.LiveRates.CurrencyCode %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblCurrencyCode" Text='<%# Eval("CurrencyCode") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.LiveRates.Rate %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="50px" Value='<%# Eval("Rate") %>'
                    ID="txtRate" RequiredErrorMessage="<% $NopResources:Admin.Currencies.LiveRates.Rate.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="99999" ValidationGroup="ApplyLiveRateGrid" RangeErrorMessage="<% $NopResources:Admin.Currencies.LiveRates.Rate.RangeErrorMessage %>">
                </nopCommerce:DecimalTextBox>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.Currencies.LiveRates.ApplyRate %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Button ID="btnApplyRate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.Currencies.LiveRates.ApplyRateButton %>"
                    ValidationGroup="ApplyLiveRateGrid" CommandName="ApplyLiveRate" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
