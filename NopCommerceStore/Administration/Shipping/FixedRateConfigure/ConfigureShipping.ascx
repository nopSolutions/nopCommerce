<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Shipping.FixedRateConfigure.ConfigureShipping" Codebehind="ConfigureShipping.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>


<asp:GridView ID="gvShippingMethods" runat="server" AutoGenerateColumns="False" Width="300px" OnRowDataBound="gvShippingMethods_RowDataBound" >
    <Columns>
        <asp:TemplateField HeaderText="Shipping Method" ItemStyle-Width="50%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
                <asp:HiddenField runat="server" ID="hfShippingMethodId" Value='<%#Eval("ShippingMethodId")%>'>
                </asp:HiddenField>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Rate" ItemStyle-Width="50%">
            <ItemTemplate>
                <nopCommerce:DecimalTextBox runat="server" ID="txtRate" Value="0"
                RequiredErrorMessage="Fixed rate is required" MinimumValue="0" MaximumValue="100000000"
                RangeErrorMessage="The value must be from 0 to 100,000,000" Width="50px" CssClass="adminInput">
            </nopCommerce:DecimalTextBox> [<%=this.CurrencyService.PrimaryStoreCurrency.CurrencyCode%>]
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>