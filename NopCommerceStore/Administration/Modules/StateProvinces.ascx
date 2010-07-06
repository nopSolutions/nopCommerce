<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.StateProvincesControl"
    CodeBehind="StateProvinces.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<div class="section-header">
    <div class="title">
        <img src="Common/ico-configuration.png" alt="<%=GetLocaleResourceString("Admin.StateProvinces.Title")%>" />
        <%=GetLocaleResourceString("Admin.StateProvinces.Title")%>
    </div>
    <div class="options">
        <asp:Button ID="AddButton" runat="server" Text="<% $NopResources:Admin.StateProvinces.AddNewButton.Text %>"
            CssClass="adminButtonBlue" OnClick="AddButton_Click" ToolTip="<% $NopResources:Admin.StateProvinces.AddNewButton.Tooltip %>" />
    </div>
</div>
<table>
    <tr>
        <td class="adminTitle">
            <nopCommerce:ToolTipLabel runat="server" ID="lblSelectCountry" Text="<% $NopResources:Admin.StateProvinces.SelectCountry %>"
                ToolTip="<% $NopResources:Admin.StateProvinces.SelectCountry.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
        </td>
        <td class="adminData">
            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="adminInput" AutoPostBack="true"
                OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" ToolTip="<% $NopResources:Admin.StateProvinces.SelectCountry.Tooltip2 %>">
            </asp:DropDownList>
        </td>
    </tr>
</table>
<p>
</p>
<asp:GridView ID="gvStateProvinces" runat="server" AutoGenerateColumns="False" Width="100%">
    <Columns>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.StateProvinces.Name %>" ItemStyle-Width="50%">
            <ItemTemplate>
                <%#Server.HtmlEncode(Eval("Name").ToString())%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Abbreviation" HeaderText="<% $NopResources:Admin.StateProvinces.Abbreviation %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="16%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:BoundField DataField="DisplayOrder" HeaderText="<% $NopResources:Admin.StateProvinces.DisplayOrder %>"
            HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="16%" ItemStyle-HorizontalAlign="Center">
        </asp:BoundField>
        <asp:TemplateField HeaderText="<% $NopResources:Admin.StateProvinces.Edit %>" HeaderStyle-HorizontalAlign="Center"
            ItemStyle-Width="16%" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <a href="StateProvinceDetails.aspx?StateProvinceID=<%#Eval("StateProvinceId")%>"
                    title="<%#GetLocaleResourceString("Admin.StateProvinces.Edit.Tooltip")%>">
                    <%#GetLocaleResourceString("Admin.StateProvinces.Edit")%>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
