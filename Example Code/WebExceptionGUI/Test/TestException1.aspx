<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestException1.aspx.cs" Inherits="WebExceptionGUI.TestExceptio1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <p>
    Dette er test part 1. Test scenarioet: <br />
    Gå til test side 2, men det så oppstår en exception i Page_Load..<br />
    <br />
    <a href="TestException2.aspx">Start Test: Gå til side2</a>
    
    </p>
    </div>
    </form>
</body>
</html>
