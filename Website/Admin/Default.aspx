<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="css/default.css" rel="stylesheet" type="text/css" />

</head>
<body>
    <form id="form1" runat="server">
    
        <a class="edit" href="Edit.aspx?auth=<%= FriskisSvettisLib.AdminAuth.AuthGuid %>">Lägg till anläggning</a><br /><br />

        <asp:Repeater runat="server" ID="rptrFacilities">
            <HeaderTemplate>
                <table class="gray">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="name"><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
                            <td class="city"><%# DataBinder.Eval(Container.DataItem, "City")%></td> 
                            <td class="edit"><a href="Edit.aspx?localid=<%# DataBinder.Eval(Container.DataItem, "LocalId")%>&auth=<%= FriskisSvettisLib.AdminAuth.AuthGuid %>">Edit</a></td>
                            <%-- <td class="delete"><asp:LinkButton runat="server" ID="btnRemove" OnClick="btnRemove_OnClick" OnDataBinding="btnRemove_OnDataBinding">Remove</asp:LinkButton></td> --%>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
