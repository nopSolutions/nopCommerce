 <%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.BulkEditProductsControl"
    CodeBehind="BulkEditProducts.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SelectCategoryControl" Src="SelectCategoryControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ConfirmationBox" Src="ConfirmationBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-catalog.png" alt="<%=GetLocaleResourceString("Admin.BulkEditProducts.Title")%>" />
        <%=GetLocaleResourceString("Admin.BulkEditProducts.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="SearchButton" runat="server" Text="<% $NopResources:Admin.BulkEditProducts.SearchButton.Text %>"
            CssClass="adminButtonBlue" OnClick="SearchButton_Click" ToolTip="<% $NopResources:Admin.BulkEditProducts.SearchButton.Tooltip %>"
            CausesValidation="false" />
        <asp:Button runat="server" Text="<% $NopResources:Admin.Products.UpdateButton.Text %>"
            CssClass="adminButtonBlue" ID="btnUpdate" OnClick="btnUpdate_Click" />
    </div>
</div>
<table width="100%">
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblProductName" Text="<% $NopResources:Admin.BulkEditProducts.ProductName %>"
                ToolTip="<% $NopResources:Admin.BulkEditProducts.ProductName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtProductName" CssClass="adminInput" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblCategory" Text="<% $NopResources:Admin.BulkEditProducts.Category %>"
                ToolTip="<% $NopResources:Admin.BulkEditProducts.Category.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <nopCommerce:SelectCategoryControl ID="ParentCategory" CssClass="adminInput" runat="server">
            </nopCommerce:SelectCategoryControl>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblManufacturer" Text="<% $NopResources:Admin.BulkEditProducts.Manufacturer %>"
                ToolTip="<% $NopResources:Admin.BulkEditProducts.Manufacturer.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlManufacturer" runat="server" CssClass="adminInput">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<p>
<%=GetLocaleResourceString("Admin.BulkEditProducts.Description")%>
</p>

<script type="text/javascript">

    $(window).bind('load', function() {
        var cbHeader = $(".cbHeader input");
        var cbRowItem = $(".cbRowItem input");
        cbHeader.bind("click", function() {
            cbRowItem.each(function() { this.checked = cbHeader[0].checked; })
        });
        cbRowItem.bind("click", function() { if ($(this).checked == false) cbHeader[0].checked = false; });
    });
    
</script>

<asp:GridView ID="gvProductVariants" runat="server" AutoGenerateColumns="False" Width="100%"
    OnPageIndexChanging="gvProductVariants_PageIndexChanging" AllowPaging="true"
    PageSize="15">
    <Columns>
        <asp:TemplateField ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="cbHeader" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="cbProductVariant" runat="server" CssClass="cbRowItem" />
                <asp:HiddenField ID="hfProductVariantId" runat="server" Value='<%# Eval("ProductVariantId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.BulkEditProducts.NameColumn %>"
            ItemStyle-Width="50%">
            <ItemTemplate>
                <a href='ProductVariantDetails.aspx?ProductVariantID=<%#Eval("ProductVariantId")%>'>
                    <%#Server.HtmlEncode(Eval("FullProductName").ToString())%></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.BulkEditProducts.PriceColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="70px" Value='<%# Eval("Price") %>'
                    ID="txtPrice" RequiredErrorMessage="<% $NopResources:Admin.BulkEditProducts.PriceColumn.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.BulkEditProducts.PriceColumn.RangeErrorMessage %>">
                </nopCommerce:DecimalTextBox>
                [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.BulkEditProducts.OldPriceColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" Width="70px" Value='<%# Eval("OldPrice") %>'
                    ID="txtOldPrice" RequiredErrorMessage="<% $NopResources:Admin.BulkEditProducts.OldPriceColumn.RequiredErrorMessage %>"
                    MinimumValue="0" MaximumValue="100000000" RangeErrorMessage="<% $NopResources:Admin.BulkEditProducts.OldPriceColumn.RangeErrorMessage %>">
                </nopCommerce:DecimalTextBox>
                [<%=CurrencyManager.PrimaryStoreCurrency.CurrencyCode%>]
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.BulkEditProducts.PublishedColumn %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:CheckBox runat="server" Checked='<%# Eval("Published") %>' ID="cbPublished">
                </asp:CheckBox>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<br />
<asp:Label runat="server" ID="lblNoProductsFound" Text="<% $NopResources: Admin.BulkEditProducts.NoProductsFound%>"
    Visible="false"></asp:Label>
