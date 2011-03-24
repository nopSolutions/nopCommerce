<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<double?>" %>

<%= Html.Telerik().NumericTextBox()
        .Name(ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty))
        .InputHtmlAttributes(new { style = "width:100%" })
        .Value(Model)
%>
