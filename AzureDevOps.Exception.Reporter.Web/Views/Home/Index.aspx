<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AzureDevOps.Exception.Reporter.Web.Models.FileNameAndItemsViewModel>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server">
    <h2>Upload exceptions - v2.0-preview</h2>

    <div id="dialog" title="View exception file">
        <% Html.BeginForm("Upload", "Home", FormMethod.Post, new {enctype = "multipart/form-data"});
           %>
                <p><input type="file" id="fileUpload" name="fileUpload" size="23"/> </p>
                <p><input type="submit" value="View exception file" /></p>
        <% Html.EndForm(); %>
    </div>
<br />
<br />
    <br />
    <p>
    <%= "There are " + Model.Exceptions.Count + " exceptions in the file." %>
    </p>
    <table width="100%">
        <tr>
            <th>
                Reporter
            </th>
            <th>
                Comment
            </th>
            <th>
                Message
            </th>
            <th>
                [Class].[Method]
            </th>
        </tr>
        <tr>
            <td>
        <div id="CommitItemsTop" title="Commit">
    <% using (Html.BeginForm("Commit", "Home", FormMethod.Post))
       {%>
             <%=Html.Hidden("filename", Model.FileName)%>
            <p><input type="submit" value="Upload Exceptions." /></p>
        <%
       }
%>
        </div>
        </td>
    </tr>
    <% foreach (var item in Model.Exceptions) { 
           %>
        <tr>
            <td style="vertical-align:middle">
               <%= Html.Encode(item.Reporter) %>
            </td>
            <td style="vertical-align:middle">
               <%= Html.Encode(item.Comment) %>
            </td>
           <td style="vertical-align:middle">
               <%= Html.Encode(item.ExceptionMessage) %>
            </td>
            <td style="vertical-align:middle">
               <%= Html.Encode("[" + item.TheClass + "].[" + item.TheMethod + "]") %>
            </td>
        </tr>
    <% } %>
    </table>
    <%  using (Html.BeginForm("Commit", "Home", FormMethod.Post))
        {
%>     
            <%=Html.Hidden("filename", Model.FileName)%>
            <p><input type="submit" value="Upload Exceptions" />
                
        </p> 
        <%
        }
%>
        
        <%  using (Html.BeginForm("Test", "Home", FormMethod.Post))
            {
        %>    <p>
            <input type="submit"  value="SendTestException"  Text="Send Test Exception" ToolTip="Sends a test exception to NemoTest project" />
        </p> 
        <%
            }
        %>
        


    </form>

</asp:Content>

