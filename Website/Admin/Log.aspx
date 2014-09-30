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
                <td>yyyy-MM-dd hh:mm:ss</td>
            </tr>
            <tr>
                <td>Användarnamn</td>
                <td><asp:TextBox runat="server" ID="txtUsername" style="width:98%;"></asp:TextBox></td>
                <td>Empty == All</td>
            </tr>
            <tr>
                <td>Anläggning</td>
                <td>   
                    <asp:DropDownList runat="server" ID="drpFacilities" style="width:100%;">

                    </asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>Typ av händelse</td>
                <td>   
                    <asp:DropDownList runat="server" ID="drpAction" style="width:100%;">
                        <asp:ListItem Text="Alla" Value=""></asp:ListItem>
                        <asp:ListItem Text="Bokning" Value="Book"></asp:ListItem>
                        <asp:ListItem Text="Avbokning" Value="Unbook"></asp:ListItem>
                        <asp:ListItem Text="Reservbokning" Value="BookStandby"></asp:ListItem>
                        <asp:ListItem Text="Reservavbokning" Value="UnbookStandby"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>Typ av log</td>
                <td>
                    <asp:DropDownList runat="server" ID="drpType" style="width:100%;">
                        <asp:ListItem Text="Alla" Value=""></asp:ListItem>
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
            <!-- 
            <tr>
                <td>Enbart statistik</td>
                <td><asp:CheckBox runat="server" ID="chkStatistics" /></td>
                <td>Möjliggör fler än 1000 resultat, ex alla under en dag.</td>
            </tr>
            -->
            <tr>
                <td></td>
                <td><asp:Button runat="server" ID="btnGetLogs" Text="Get logs" OnClick="btnGetLogs_Click" /></td>
            </tr>
        </table>

        <hr />
        
        <table>
            <tr>
                <td style="width:200px; border-bottom: 2px solid black;"><b>Typ</b></td>
                <td style="width:150px; border-bottom: 2px solid black;"><b>Fel</b></td>
                <td style="width:150px; border-bottom: 2px solid black;"><b>Totalt (max 1000)</b></td>
            </tr>
            <tr>
                <td>Bokning</td>
                <td><%= SearchStatistics.Bookings %></td>
                <td><%= SearchStatistics.FailedBookings %></td>
            </tr>
            <tr>
                <td>Avbokning</td>
                <td><%= SearchStatistics.UnBookings %></td>
                <td><%= SearchStatistics.FailedUnBookings %></td>
            </tr>
            <tr>
                <td>Reservbokning</b></td>
                <td><%= SearchStatistics.StandbyBookings %></td>
                <td><%= SearchStatistics.FailedStandbyBookings %></td>
            </tr>
            <tr>
                <td>Reservbokning</td>
                <td><%= SearchStatistics.StandbyUnBookings %></td>
                <td><%= SearchStatistics.FailedStandbyUnBookings %></td>
            </tr>
            <tr style="font-weight: bold;">
                <td style="border-top: 2px solid black;">Total</td>
                <td style="border-top: 2px solid black;"><%= SearchStatistics.TotalErrors + " (" + (((float)SearchStatistics.TotalErrors/(float)SearchStatistics.Total) * 100).ToString().Split(',')[0] + "%)" %></td>
                <td style="border-top: 2px solid black;"><%= SearchStatistics.Total %></td>
            </tr>
        </table>

        <hr />
        
        <table style="width:100%;">
            <tr>
                <td style="width:200px; border-bottom: 2px solid black;"><b>Typ</b></td>
                <td style="border-bottom: 2px solid black;"><b>Meddelande</b></td>
                <td style="width:150px; border-bottom: 2px solid black;"><b>Antal</b></td>
            </tr>
            <asp:Repeater ID="rptrMessages" runat="server">
                <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "Type") %></td>
                        <td><%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "Message").ToString()) %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Count") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
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
