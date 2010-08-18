<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Tax.GeneralTaxConfigure.TaxRatesControl"
    CodeBehind="TaxRates.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="DecimalTextBox" Src="../../Modules/DecimalTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="../../Modules/ToolTipLabelControl.ascx" %>
<table class="adminContent">
    <tr>
        <td>
            <asp:Panel ID="pnlGrid" runat="server">
                <asp:Button runat="server" Text="Add new tax rate" CssClass="adminButtonBlue" ID="btnAddNew1"
                    OnClick="btnAddNew_Click" ToolTip="Add a new tax rate" />
                <br />
                <asp:GridView ID="gvTaxRates" runat="server" AutoGenerateColumns="False" Width="100%"
                    OnRowEditing="gvTaxRates_RowEditing" OnRowDeleting="gvTaxRates_RowDeleting">
                    <Columns>
                        <asp:TemplateField HeaderText="Country" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <%#((Country)Eval("Country")).Name%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="State/province" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <%#GetStateProvinceInfo(Convert.ToInt32(Eval("StateProvinceId")))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Zip" ItemStyle-Width="10%">
                            <ItemTemplate>
                                <%#GetZipInfo(Convert.ToString(Eval("Zip")))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tax category" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <%#((TaxCategory)Eval("TaxCategory")).Name%>
                                <asp:HiddenField runat="server" ID="hfTaxRateId" Value='<%#Eval("TaxRateId")%>'>
                                </asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Percentage" ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#Eval("Percentage")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Link" Text="Edit" HeaderText="Edit" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" CausesValidation="false"
                            CommandName="Edit" />
                        <asp:ButtonField ButtonType="Link" Text="Delete" HeaderText="Delete" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" CausesValidation="false"
                            CommandName="Delete" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" Text="Add new tax rate" CssClass="adminButtonBlue" ID="btnAddNew2"
                    OnClick="btnAddNew_Click" ToolTip="Add a new tax rate" />
            </asp:Panel>
            <asp:Panel ID="pnlEdit" runat="server">
                <table class="adminContent">
                    <tr>
                        <td class="adminTitle" colspan="2">
                            <b>Adding new tax rate</b>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblCountry" Text="Country:" ToolTip="The country."
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:DropDownList ID="ddlCountry" AutoPostBack="True" runat="server" CssClass="adminInput"
                                OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:Label ID="lblTaxRateId" runat="server" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblStateProvince" Text="State / province:"
                                ToolTip="If an asteriks is selected, then this tax rate will apply to all customers from the given country, regardless of the state."
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:DropDownList ID="ddlStateProvince" runat="server" CssClass="adminInput">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblZip" Text="Zip:" ToolTip="Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code."
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:TextBox ID="txtZip" runat="server" CssClass="adminInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                            <nopCommerce:ToolTipLabel runat="server" ID="lblTaxCategory" Text="Tax class:" ToolTip="The tax class."
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <asp:DropDownList ID="ddlTaxCategory" runat="server" CssClass="adminInput">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="adminTitle">
                             <nopCommerce:ToolTipLabel runat="server" ID="lblPercentage" Text="Percentage:" ToolTip="The tax rate."
                                ToolTipImage="~/Administration/Common/ico-help.gif" />
                        </td>
                        <td class="adminData">
                            <nopCommerce:DecimalTextBox runat="server" CssClass="adminInput" ID="txtPercentage"
                                Value="0" RequiredErrorMessage="Percentage is required" MinimumValue="0" MaximumValue="999999"
                                RangeErrorMessage="The value must be from 0 to 999999" ValidationGroup="NewTaxRate">
                            </nopCommerce:DecimalTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <asp:Button ID="SaveButton" runat="server" CssClass="adminButton" Text="Save" ValidationGroup="NewTaxRate"
                                OnClick="SaveButton_Click"></asp:Button>
                            <asp:Button ID="CancelButton" runat="server" CssClass="adminButton" Text="Cancel"
                                CausesValidation="false" OnClick="CancelButton_Click"></asp:Button>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
