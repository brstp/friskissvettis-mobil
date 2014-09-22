<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Admin_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="css/default.css" rel="stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="js/tiny_mce/tiny_mce.js"></script>
    <script language="javascript" type="text/javascript">
        tinyMCE.init({
            theme: "advanced",
            mode: "textareas",
            theme_advanced_toolbar_location: "top",
            /* paste_preprocess: function (event, paste) {
                // strip html when pasting text
                paste.content = $(paste.content).text();
            } */
        });
    </script>

    
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
    
        <%-- Id --%>
        <h1>Id</h1>
        <table>
            <tr>
                <td class="first">LocalId</td>
                <td><asp:TextBox Enabled="false" runat="server" ID="txtLocalId" /></td>
                <td class="help">Ej redigerbart, används internt på vår server.</td>
            </tr>
            <tr>
                <td>Id</td>
                <td><asp:TextBox runat="server" ID="txtId" /></td>
                <td class="help">Anläggningsid: BRP=NN, Pastell=NNNN, Defendo=Guid, DLSoftware=?</td>
            </tr>
            <tr>
                <td>Id2</td>
                <td><asp:TextBox runat="server" ID="txtId2" /></td>
                <td class="help">AnläggningsId: Pastell=NNNNNNNNNNNNNNNNNN</td>
            </tr>
        </table>
            
        <%-- Visible --%>
        <h1>Visible</h1>
        <table>
            <tr>
                <td class="first">Visible</td>
                <td><asp:CheckBox runat="server" ID="chkVisible" /></td>
                <td class="help">Om anläggningen ska synas på siten</td>
            </tr>
            <tr>
                <td>VisibleGuid</td>
                <td><asp:TextBox runat="server" ID="txtVisibleGuid" /></td>
                <td class="help">Om den ska grupperas, default är Guid.Empty. Används för testning. Om ni inte vet vad den gör ska ni antagligen inte använda den :)</td>
            </tr>
        </table>
            
        <%-- Description --%>
        <h1>Description</h1>
        <table>
            <tr>
                <td class="first">Name</td>
                <td><asp:TextBox runat="server" ID="txtName" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>HtmlInfo</td>
                <td><asp:TextBox runat="server" ID="txtHtmlInfo" TextMode="MultiLine" Rows="15" /></td>
                <td class="help">Beskrivning under "Anläggningsinfo"</td>
            </tr>
            <tr>
                <td>News</td>
                <td><asp:TextBox runat="server" ID="txtNews" TextMode="MultiLine" Rows="15" /></td>
                <td class="help">Nyheter för föreningen</td>
            </tr>
            <tr>
                <td>Offers</td>
                <td><asp:TextBox runat="server" ID="txtOffers" TextMode="MultiLine" Rows="15" /></td>
                <td class="help">Erbjudanden från anläggningen</td>
            </tr>
            <tr>
                <td>About</td>
                <td><asp:TextBox runat="server" ID="txtAbout" TextMode="MultiLine" Rows="15" /></td>
                <td class="help">Om föreningen, "Om oss"</td>
            </tr>
            <tr>
                <td>Phone</td>
                <td><asp:TextBox runat="server" ID="txtPhone" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Email</td>
                <td><asp:TextBox runat="server" ID="txtEmail" /></td>
                <td class="help"></td>
            </tr>
            <!-- <tr>
                <td>Description (not used)</td>
                <td><asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" Rows="10" /></td>
            </tr> -->
            <tr>
                <td>Address</td>
                <td><asp:TextBox runat="server" ID="txtAddress" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Zip</td>
                <td><asp:TextBox runat="server" ID="txtZipcode" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>City</td>
                <td><asp:TextBox runat="server" ID="txtCity" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Homepage</td>
                <td><asp:TextBox runat="server" ID="txtHomepage" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Facebook</td>
                <td><asp:TextBox runat="server" ID="txtFacebook" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Twitter</td>
                <td><asp:TextBox runat="server" ID="txtTwitter" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td class="first">Language</td>
                <td><asp:TextBox runat="server" ID="txtLanguages" /></td>
                <td class="help">"sv" = Svenska, "no" = norska</td>
            </tr>
        </table>
            
        <%-- Position --%>
        <h1>Position</h1>
        <table>
            <tr>
                <td class="first">Longitude</td>
                <td><asp:TextBox runat="server" ID="txtLongitude" /></td>
                <td class="help">Kan hittas på <a href="http://www.hitta.se">http://www.hitta.se</a> - Sök adress och kolla på "Mer" uppe till höger för detta värde.</td>
            </tr>
            <tr>
                <td>Latitude</td>
                <td><asp:TextBox runat="server" ID="txtLatitude" /></td>
                <td class="help">Se ovan</td>
            </tr>
        </table>
            
        <%-- Authentication --%>
        <h1>Authentication</h1>
        <table>
            <tr>
                <td class="first">Username</td>
                <td><asp:TextBox runat="server" ID="txtUsername" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Password</td>
                <td><asp:TextBox runat="server" ID="txtPassword" /></td>
                <td class="help"></td>
            </tr>
        </table>

        <%-- Other --%>
        <h1>Other</h1>
        <table>
            <tr>
                <td class="first">UsernameLabel</td>
                <td><asp:TextBox runat="server" ID="txtUsernameLabel" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>PasswordLabel</td>
                <td><asp:TextBox runat="server" ID="txtPasswordLabel" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>ScheduleLength</td>
                <td><asp:TextBox runat="server" ID="txtScheduleLength" /></td>
                <td class="help"></td>
            </tr>
        </table>

        <%-- Service --%>
        <h1>Service</h1>
        
        <table>
            <tr>
                <td class="first">Välj service: </td>
                <td><asp:DropDownList runat="server" ID="ddlService" AutoPostBack="true" OnSelectedIndexChanged="ddlService_OnSelectedIndexChanged">
                        <asp:ListItem Text="BRP" Value="BRPService"></asp:ListItem>
                        <asp:ListItem Text="PastellData" Value="PastellService"></asp:ListItem>
                        <asp:ListItem Text="Defendo" Value="DefendoService"></asp:ListItem>
                        <asp:ListItem Text="DL Software" Value="DLSoftwareService"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="help"></td>
            </tr>
        </table>

        <div runat="server" id="divBrp">
            <h2>BRP</h2>
            <table>
                <tr>
                    <td class="first">ServiceUrl</td>
                    <td><asp:TextBox runat="server" ID="txtBRPServiceUrl" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ServiceLicense</td>
                    <td><asp:TextBox runat="server" ID="txtBRPServiceLicense" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ScheduleLoginNeeded</td>
                    <td><asp:CheckBox runat="server" ID="chkBRPScheduleLoginNeeded" /></td>
                    <td class="help"></td>
                </tr>
            </table>
        </div>
        
        <div runat="server" id="divPastellData">
            <h2>PastellData</h2>
            <table>
                <tr>
                    <td class="first">ServiceUrl</td>
                    <td><asp:TextBox runat="server" ID="txtPastellDataServiceUrl" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ServiceLicense</td>
                    <td><asp:TextBox runat="server" ID="txtPastellDataServiceLicense" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ServiceRevision</td>
                    <td><asp:TextBox runat="server" ID="txtPastellDataServiceRevision" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ScheduleLoginNeeded</td>
                    <td><asp:CheckBox runat="server" ID="chkPastellDataScheduleLoginNeeded" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>LoginId</td>
                    <td><asp:TextBox runat="server" ID="txtPastellDataLoginId" /></td>
                    <td class="help"></td>
                </tr>
            </table>
        </div>
        
        <div runat="server" id="divDefendo">
            <h2>Defendo</h2>
            <table>
                <tr>
                    <td class="first">ServiceUrl</td>
                    <td><asp:TextBox runat="server" ID="txtDefendoServiceUrl" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ServiceContextId</td>
                    <td><asp:TextBox runat="server" ID="txtDefendoServiceContextId" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ServiceLicense</td>
                    <td><asp:TextBox runat="server" ID="txtDefendoServiceLicense" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ScheduleLoginNeeded</td>
                    <td><asp:CheckBox runat="server" ID="chkDefendoScheduleLoginNeeded" /></td>
                    <td class="help"></td>
                </tr>
            </table>
        </div>
        
        <div runat="server" id="divDLSoftware">
            <h2>DL Software</h2>
            <table>
                <tr>
                    <td class="first">ServiceUrl</td>
                    <td><asp:TextBox runat="server" ID="txtDLServiceUrl" /></td>
                    <td class="help"></td>
                </tr>
                <!-- 
                <tr>
                    <td>ServiceContextId</td>
                    <td><asp:TextBox runat="server" ID="txtDLServiceContextId" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ServiceLicense</td>
                    <td><asp:TextBox runat="server" ID="txtDLServiceLicense" /></td>
                    <td class="help"></td>
                </tr>
                <tr>
                    <td>ScheduleLoginNeeded</td>
                    <td><asp:CheckBox runat="server" ID="chkDLScheduleLoginNeeded" /></td>
                    <td class="help"></td>
                </tr> -->
            </table>
        </div>

        <asp:Button runat="server" ID="btnSave" Text="Spara" OnClick="btnSave_OnClick" />
    </div>
    </form>
</body>
</html>
