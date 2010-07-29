<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Tax.FixedRate.ConfigureTax"
    CodeBehind="ConfigureTax.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<asp:GridView ID="gvTaxCategories" runat="server" AutoGenerateColumns="False" Width="300px" OnRowDataBound="gvTaxCategories_RowDataBound" >
    <Columns>
        <asp:TemplateField HeaderText="Tax category" ItemStyle-Width="50%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
                <asp:HiddenField runat="server" ID="hfTaxCategoryId" Value='<%#Eval("TaxCategoryId")%>'>
                </asp:HiddenField>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Rate" ItemStyle-Width="50%">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" ID="txtRate" Value="0" RequiredErrorMessage="Rate is required"
                    MinimumValue="0" MaximumValue="999999" RangeErrorMessage="The value must be from 0 to 999999"
                    CssClass="adminInput" Width="50px"></nopCommerce:DecimalTextBox> [%]
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
