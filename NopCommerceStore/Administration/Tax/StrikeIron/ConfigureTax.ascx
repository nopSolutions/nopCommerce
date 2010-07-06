<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="NopSolutions.NopCommerce.Web.Administration.Tax.StrikeIron.ConfigureTax" Codebehind="ConfigureTax.ascx.cs" %>
<table class="adminContent">
    <tr>
        <td colspan="2">
            <b>To use this service, you need to create a StrikeIron account and purchase its online
                tax service.
                <br />
                <b>To enable this provider, you'll need to:<br />
                </b>
                <br />
                Step 1. <a href="http://www.strikeiron.com" target="_blank">Create a StrikeIron account</a>
                <br />
                Step 2. Purchase the <a href="http://www.strikeiron.com/ProductDetail.aspx?p=444"
                    target="_blank">service</a>
                <br />
                Step 3. Fill in your StrikeIron account details below
                <br />
                <br />
            </b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            StrikeIron User ID:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtUserId" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            StrikeIron Password:
        </td>
        <td class="adminData">
            <asp:TextBox runat="server" ID="txtPassword" CssClass="adminInput"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2" width="100%">
            <hr />
            <br />
            <b>Test Online Tax Service (USA)</b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Zip Code:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtZip_TestUSA" runat="server" CssClass="adminInput" Text="10001"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
        </td>
        <td class="adminData" style="color: red">
            <asp:Label ID="lblTestResultUSA" runat="server" EnableViewState="false">
            </asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="left">
            <asp:Button runat="server" ID="btnTestUS" Text="Test (USA)" CssClass="adminButton"
                ValidationGroup="Test" OnClick="btnTestUS_Click"></asp:Button>
        </td>
    </tr>
    <tr>
        <td colspan="2" width="100%">
            <hr />
            <br />
            <b>Test Online Tax Service (Canada)</b>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
            Two Letter Province Code:
        </td>
        <td class="adminData">
            <asp:TextBox ID="txtProvince_TestCanada" runat="server" CssClass="adminInput" Text="ON"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="adminTitle">
        </td>
        <td class="adminData" style="color: red">
            <asp:Label ID="lblTestResultCanada" runat="server" EnableViewState="false">
            </asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="left">
            <asp:Button runat="server" ID="btnTestCA" Text="Test (Canada)" CssClass="adminButton"
                ValidationGroup="Test" OnClick="btnTestCA_Click"></asp:Button>
        </td>
    </tr>
    <tr>
        <td colspan="2" width="100%">
            <hr />
        </td>
    </tr>
</table>
