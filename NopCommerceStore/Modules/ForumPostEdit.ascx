<%@ Control Language="C#" AutoEventWireup="true" Inherits="NopSolutions.NopCommerce.Web.Modules.ForumPostEditControl"
    CodeBehind="ForumPostEdit.ascx.cs" %>
<%@ Register TagPrefix="nopCommerce" TagName="ForumBreadcrumb" Src="~/Modules/ForumBreadcrumb.ascx" %>
<%@ Register TagPrefix="nopCommerce" TagName="SimpleTextBox" Src="~/Modules/SimpleTextBox.ascx" %>
<div class="postedit">
    
    <nopCommerce:ForumBreadcrumb ID="ctrlForumBreadcrumb" runat="server" />
    <div class="title">
        <asp:Label ID="lblTitle" runat="server" />
    </div>
    <div class="wrapper">
        <asp:Panel runat="server" ID="pnlError" CssClass="error-block">
            <div class="message-error">
                <asp:Literal ID="lErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
            </div>
        </asp:Panel>
        <div class="clear">
        </div>
        <table class="posttopic">
            <asp:PlaceHolder runat="server" ID="phForumName">
                <tr>
                    <td class="fieldname">
                        <%=GetLocaleResourceString("Forum.ForumName")%>:
                    </td>
                    <td>
                        <asp:Label ID="lblForumName" runat="server"></asp:Label>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="fieldname">
                    <%=GetLocaleResourceString("Forum.TopicTitle")%>:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtTopicTitle" ValidationGroup="Submit" SkinID="ForumTopicTitleText"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTopicTitle" runat="server" ControlToValidate="txtTopicTitle"
                        ErrorMessage="<% $NopResources:Forum.TopicSubjectCannotBeEmpty %>" ToolTip="<% $NopResources:Forum.TopicSubjectCannotBeEmpty %>"
                        ValidationGroup="Submit">*</asp:RequiredFieldValidator>
                    <asp:Label ID="lblTopicTitle" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="fieldname">
                </td>
                <td>
                    <asp:TextBox ID="txtTopicBodySimple" runat="server" Width="100%" Height="300px" TextMode="MultiLine"></asp:TextBox>
                    <%if (ForumManager.ForumEditor == EditorTypeEnum.BBCodeEditor)
                      {%>

                    <script language="javascript" type="text/javascript">
                        var webRoot = '<%=CommonHelper.GetStoreLocation()%>';
                        edToolbar('<%=txtTopicBodyBBCode.ClientID %>'); 
                    </script>

                    <%} %>
                    <asp:TextBox ID="txtTopicBodyBBCode" runat="server" Width="100%" Height="300px" TextMode="MultiLine"
                        SkinID="BBEditorText" />
                    <asp:RequiredFieldValidator ID="rfvTopicBody" runat="server" ControlToValidate="txtTopicBodySimple"
                        ErrorMessage="<% $NopResources:Forum.TextCannotBeEmpty %>" ToolTip="<% $NopResources:Forum.TextCannotBeEmpty %>"
                        ValidationGroup="Submit">*</asp:RequiredFieldValidator>
                    <%if (ForumManager.ForumEditor == EditorTypeEnum.HtmlEditor)
                      {%>
                    <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true"
                        EnableScriptLocalization="true" ID="sm1" ScriptMode="Release" CompositeScript-ScriptMode="Release" />
                    <HTMLEditor:Editor ID="txtTopicBodyHtml" runat="server" Height="300" Visible="false"
                        NoScript="true" />
                    <%} %>
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="phPriority">
                <tr>
                    <td class="fieldname">
                        <%=GetLocaleResourceString("Forum.Priority")%>:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPriority" runat="server" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="phSubscribe">
                <tr>
                    <td class="fieldname">
                        <%=GetLocaleResourceString("Forum.Options")%>:
                    </td>
                    <td>
                        <asp:CheckBox ID="cbSubscribe" runat="server" Text="<% $NopResources:Forum.NotifyWhenSomeonePostsInThisTopic %>"
                            CssClass="forumtopicoptionscheck" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td colspan="2" class="options">
                    <asp:Button runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" Text="<% $NopResources:Forum.Submit %>"
                        CssClass="submitforumtopicbutton" ValidationGroup="Submit" />
                    <asp:Button runat="server" ID="btnCancel" OnClick="btnCancel_Click" Text="<% $NopResources:Forum.Cancel %>"
                        CssClass="cancelforumtopicbutton" />
                </td>
            </tr>
        </table>
    </div>
</div>
