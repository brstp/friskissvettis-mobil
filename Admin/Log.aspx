<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Log.aspx.cs" Inherits="Admin_Log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Sök: <br />
        <table>
            <tr>
                <td>Datum</td>
                <td><asp:TextBox runat="server" ID="txtTimestampFrom"></asp:TextBox> - <asp:TextBox runat="server" ID="txtTimestampTo"></asp:TextBox></td>
                <td>yyyy-MM-dd</td>
            </tr>
            <tr>
                <td>Användarnamn</td>
                <td><asp:TextBox runat="server" ID="txtUsername" style="width:98%;"></asp:TextBox></td>
                <td>Empty == All</td>
            </tr>
            <tr>
                <td>Typ av log</td>
                <td>
                    <asp:DropDownList runat="server" ID="drpType" style="width:100%;">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Debug" Value="Debug"></asp:ListItem>
                        <asp:ListItem Text="Information" Value="Information"></asp:ListItem>
                        <asp:ListItem Text="Warning" Value="Warning"></asp:ListItem>
                        <asp:ListItem Text="Error" Value="Error"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>Provider</td>
                <td>
                    <asp:DropDownList runat="server" ID="drpProvider" style="width:100%;">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Pastell" Value="Pastell"></asp:ListItem>
                        <asp:ListItem Text="BRP" Value="BRP"></asp:ListItem>
                        <asp:ListItem Text="Defendo" Value="Defendo"></asp:ListItem>
                        <asp:ListItem Text="DLSoftware" Value="DLSoftware"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Button runat="server" ID="btnGetLogs" Text="Get logs" OnClick="btnGetLogs_Click" /></td>
            </tr>
        </table>

        <hr />

        <table>
            <tr>
                <td>Timestamp</td>
                <td>Provider</td>
                <td>Facility</td>
                <td>Action</td>
                <td>Username</td>
                <td>Message</td>
                <td>Type</td>
            </tr>
            <asp:Repeater runat="server" ID="rptrLogs">
                <ItemTemplate>
                    <tr>
                        <td style="width: 200px;"><%# DataBinder.Eval(Container.DataItem, "Timestamp") %></td>
                        <td style="width: 100px;"><%# DataBinder.Eval(Container.DataItem, "Provider") %></td>
                        <td style="width: 100px;"><%# DataBinder.Eval(Container.DataItem, "Facility") %></td>
                        <td style="width: 100px;"><%# DataBinder.Eval(Container.DataItem, "Action") %></td>
                        <td style="width: 200px;"><%# DataBinder.Eval(Container.DataItem, "Username") %></td>
                        <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Message").ToString()) %></td>
                        <td style="width: 100px;"><%# DataBinder.Eval(Container.DataItem, "Type") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    </form>
</body>
</html>
