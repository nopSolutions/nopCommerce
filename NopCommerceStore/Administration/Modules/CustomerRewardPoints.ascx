<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.CustomerRewardPointsControl"
    CodeBehind="CustomerRewardPoints.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<asp:Panel runat="server" ID="pnlData">
    <table class="adminContent">
        <tr>
            <td>
                <asp:UpdatePanel ID="upPoints" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gvRewardPointsHistory" runat="server" AutoGenerateColumns="False"
                            Width="100%" OnPageIndexChanging="gvRewardPointsHistory_PageIndexChanging" AllowPaging="true"
                            PageSize="15">
                            <Columns>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRewardPoints.Grid.Points %>"
                                    ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <%#Server.HtmlEncode(Eval("Points").ToString())%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRewardPoints.Grid.Balance %>"
                                    ItemStyle-Width="20%">
                                    <ItemTemplate>
                                        <%#Server.HtmlEncode(Eval("PointsBalance").ToString())%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRewardPoints.Grid.Message %>"
                                    ItemStyle-Width="40%">
                                    <ItemTemplate>
                                        <%#Server.HtmlEncode(Eval("Message").ToString())%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="<% $NopResources:Admin.CustomerRewardPoints.Grid.Date %>"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#DateTimeHelper.ConvertToUserTime((DateTime)Eval("CreatedOn"), DateTimeKind.Utc).ToString()%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upPoints">
                    <ProgressTemplate>
                        <div class="progress">
                            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif"
                                AlternateText="update" />
                            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </td>
        </tr>
    </table>
    <p>
        <strong>
            <%=GetLocaleResourceString("Admin.CustomerRewardPoints.Add")%>
        </strong>
    </p>
    <table class="adminContent">
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblNewPoints" Text="<% $NopResources:Admin.CustomerRewardPoints.Add.Points %>"
                    ToolTip="<% $NopResources:Admin.CustomerRewardPoints.Add.Points.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtNewPoints"
                    RequiredErrorMessage="<% $NopResources:Admin.CustomerRewardPoints.Add.Points.RequiredErrorMessage %>"
                    RangeErrorMessage="<% $NopResources:Admin.CustomerRewardPoints.Add.Points.RangeErrorMessage %>"
                    ValidationGroup="AddPoints" MinimumValue="-999999" MaximumValue="999999" Value="100"
                    Width="50px"></nopCommerce:NumericTextBox>
            </td>
        </tr>
        <tr>
            <td class="adminTitle">
                <nopCommerce:ToolTipLabel runat="server" ID="lblNewMessage" Text="<% $NopResources:Admin.CustomerRewardPoints.Add.Message %>"
                    ToolTip="<% $NopResources:Admin.CustomerRewardPoints.Add.Message.Tooltip %>"
                    ToolTipImage="~/Administration/Common/ico-help.gif" />
            </td>
            <td class="adminData">
                <nopCommerce:SimpleTextBox runat="server" ID="txtNewMessage" CssClass="adminInput"
                    ValidationGroup="AddPoints" ErrorMessage="<% $NopResources:Admin.CustomerRewardPoints.Add.Message.ErrorMessage %>">
                </nopCommerce:SimpleTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="left">
                <asp:Button runat="server" ID="btnAddPoints" CssClass="adminButton" Text="<% $NopResources:Admin.CustomerRewardPoints.AddButton.Text %>"
                    ValidationGroup="AddPoints" OnClick="btnAddPoints_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlMessage">
    <asp:Literal runat="server" ID="lMessage"></asp:Literal>
</asp:Panel>
