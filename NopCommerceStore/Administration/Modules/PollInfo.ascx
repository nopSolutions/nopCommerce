<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Administration.Modules.PollInfoControl" CodeBehind="PollInfo.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ToolTipLabel" Src="ToolTipLabelControl.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="NumericTextBox" Src="NumericTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="SimpleTextBox.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="DatePicker" Src="DatePicker.ascx" %>

<asp:UpdatePanel ID="upPools" runat="server">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlError" EnableViewState="false" Visible="false" class="messageBox messageBoxError">
            <asp:Literal runat="server" ID="lErrorTitle" EnableViewState="false" />
        </asp:Panel>
        <table class="adminContent">
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblLanguage" Text="<% $NopResources:Admin.PollInfo.Language %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.Language.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:DropDownList ID="ddlLanguage" AutoPostBack="False" CssClass="adminInput" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblName" Text="<% $NopResources:Admin.PollInfo.Name %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.Name.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:SimpleTextBox runat="server" ID="txtName" CssClass="adminInput" ErrorMessage="<% $NopResources:Admin.PollInfo.Name.ErrorMessage %>">
                    </nopCommerce:SimpleTextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblSystemKeyword" Text="<% $NopResources:Admin.PollInfo.SystemKeyword %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.SystemKeyword.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:TextBox ID="txtSystemKeyword" CssClass="adminInput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblPublished" Text="<% $NopResources:Admin.PollInfo.Published %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.Published.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:CheckBox ID="cbPublished" runat="server" Checked="True"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblShowOnHomePage" Text="<% $NopResources:Admin.PollInfo.ShowOnHomePage %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.ShowOnHomePage.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <asp:CheckBox ID="cbShowOnHomePage" runat="server" Checked="True" />
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblDisplayOrder" Text="<% $NopResources:Admin.PollInfo.DisplayOrder %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtDisplayOrder"
                        Value="1" RequiredErrorMessage="<% $NopResources:Admin.PollInfo.DisplayOrder.RequiredErrorMessage %>"
                        RangeErrorMessage="<% $NopResources:Admin.PollInfo.DisplayOrder.RangeErrorMessage %>"
                        MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblStartDate" Text="<% $NopResources:Admin.PollInfo.StartDate %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.StartDate.ToolTip %>"
                        ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:DatePicker runat="server" ID="ctrlStartDate" />
                </td>
            </tr>
            <tr>
                <td class="adminTitle">
                    <nopCommerce:ToolTipLabel runat="server" ID="lblEndDate" Text="<% $NopResources:Admin.PollInfo.EndDate %>"
                        ToolTip="<% $NopResources:Admin.PollInfo.EndDate.ToolTip %>"
                        ToolTipImage="~/Administration/Common/ico-help.gif" />
                </td>
                <td class="adminData">
                    <nopCommerce:DatePicker runat="server" ID="ctrlEndDate" />
                </td>
            </tr>
        </table>
        <div runat="server" id="pnlPollAnswers">
            <div>
                <hr />
            </div>
            <asp:GridView ID="gvPollAnswers" runat="server" AutoGenerateColumns="false" DataKeyNames="PollAnswerId"
                OnRowDeleting="gvPollAnswers_RowDeleting" OnRowDataBound="gvPollAnswers_RowDataBound"
                OnRowCommand="gvPollAnswers_RowCommand" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PollInfo.PollAnswerColumn %>"
                        ItemStyle-Width="40%">
                        <ItemTemplate>
                            <nopCommerce:SimpleTextBox runat="server" ID="txtName" Text='<%# Eval("Name") %>'
                                ErrorMessage="<% $NopResources:Admin.PollInfo.PollAnswerColumn.ErrorMessage %>"
                                CssClass="adminInput" ValidationGroup="UpdatePollAnswer"></nopCommerce:SimpleTextBox>
                            <asp:HiddenField ID="hfPollAnswerId" runat="server" Value='<%# Eval("PollAnswerId") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Count" HeaderText="<% $NopResources:Admin.PollInfo.CountColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PollInfo.DisplayOrderColumn %>"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtDisplayOrder"
                                Value='<%# Eval("DisplayOrder") %>' ValidationGroup="UpdatePollAnswer" Width="50px"
                                RequiredErrorMessage="<% $NopResources:Admin.PollInfo.DisplayOrderColumn.RequiredErrorMessage %>"
                                RangeErrorMessage="<% $NopResources:Admin.PollInfo.DisplayOrderColumn.RangeErrorMessage %>"
                                MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PollInfo.UpdateColumn %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnUpdate" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.PollInfo.UpdateColumn %>"
                                ValidationGroup="UpdatePollAnswer" CommandName="UpdatePollAnswer" ToolTip="<% $NopResources:Admin.PollInfo.UpdateColumn.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<% $NopResources:Admin.PollInfo.DeleteColumn %>" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" CssClass="adminButton" Text="<% $NopResources:Admin.PollInfo.DeleteColumn %>"
                                CausesValidation="false" CommandName="Delete" ToolTip="<% $NopResources:Admin.PollInfo.DeleteColumn.Tooltip %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <p>
                <strong>
                    <%=GetLocaleResourceString("Admin.PollInfo.New.Title")%>
                </strong>
            </p>
            <table class="adminContent">
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAnswerName" Text="<% $NopResources:Admin.PollInfo.New.AnswerName %>"
                            ToolTip="<% $NopResources:Admin.PollInfo.New.AnswerName.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:SimpleTextBox runat="server" ID="txtPollAnswerName" CssClass="adminInput"
                            ErrorMessage="<% $NopResources:Admin.PollInfo.New.AnswerName.ErrorMessage %>"
                            ValidationGroup="AddPollAnswer"></nopCommerce:SimpleTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="adminTitle">
                        <nopCommerce:ToolTipLabel runat="server" ID="lblAnswerDisplayOrder" Text="<% $NopResources:Admin.PollInfo.New.DisplayOrder %>"
                            ToolTip="<% $NopResources:Admin.PollInfo.New.DisplayOrder.Tooltip %>" ToolTipImage="~/Administration/Common/ico-help.gif" />
                    </td>
                    <td class="adminData">
                        <nopCommerce:NumericTextBox runat="server" CssClass="adminInput" ID="txtPollAnswerDisplayOrder"
                            Value="1" RequiredErrorMessage="<% $NopResources:Admin.PollInfo.New.DisplayOrder.RequiredErrorMessage %>"
                            RangeErrorMessage="<% $NopResources:Admin.PollInfo.New.DisplayOrder.RangeErrorMessage %>"
                            ValidationGroup="AddPollAnswer" MinimumValue="-99999" MaximumValue="99999"></nopCommerce:NumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left">
                        <asp:Button runat="server" ID="btnAddPollAnswer" Text="<% $NopResources:Admin.PollInfo.New.AddNewAnswerButton.Text %>"
                            CssClass="adminButton" ValidationGroup="AddPollAnswer" OnClick="btnAddPollAnswer_Click"
                            ToolTip="<% $NopResources:Admin.PollInfo.New.AddNewAnswerButton.Tooltip %>" />
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="up1" runat="server" AssociatedUpdatePanelID="upPools">
    <ProgressTemplate>
        <div class="progress">
            <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/images/UpdateProgress.gif" AlternateText="update" />
            <%=GetLocaleResourceString("Admin.Common.Wait...")%>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
