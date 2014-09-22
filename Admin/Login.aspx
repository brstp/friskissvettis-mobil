<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Admin_Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>
    <form id="form1" runat="server">

        <asp:Label runat="server" ID="lblMessage"></asp:Label>

        <h1>Login</h1>
        <table>
            <tr>
                <td class="first">Username: </td>
                <td><asp:TextBox runat="server" ID="txtUsername" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Password</td>
                <td><asp:TextBox TextMode="Password" runat="server" ID="txtPassword" /></td>
                <td class="help"></td>
            </tr>

            <asp:Button runat="server" ID="btnLogin" Text="Login" OnClick="btnLogin_OnClick" />
        </table>
    </form>
</body>
</html>
