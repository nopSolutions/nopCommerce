<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerRoleCustomersControl"
    CodeBehind="CustomerRoleCustomers.ascx.cs" %>
<table class="adminContent">
    <tr>
        <td colspan="2" width="100%">
            <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="Email" ItemStyle-Width="45%">
                        <ItemTemplate>
                            <%#Server.HtmlEncode(Eval("Email").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="45%">
                        <ItemTemplate>
                            <%#Server.HtmlEncode(Eval("FullName").ToString())%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="View" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <a href="CustomerDetails.aspx?CustomerID=<%#Eval("CustomerId")%>" title="View customer details">
                                View</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
