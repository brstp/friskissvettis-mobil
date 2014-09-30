<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Facility.aspx.cs" Inherits="Admin_Facility" %>

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
            plugins: "safari,paste",
            theme_advanced_toolbar_location: "top",
            /*setup: function (ed) {

                ed.pasteAsPlainText = true;

                ed.onPaste.add(function (ed, e) {
                    console.debug('Pasted plain text');
                    // ed.execCommand('mcePasteText', true);

                    ed.pasteAsPlainText = true;
                    return tinymce.dom.Event.cancel(e);
                });
            }*/
            paste_auto_cleanup_on_paste : true,
            paste_remove_styles: true,
            paste_remove_styles_if_webkit: true,
            paste_strip_class_attributes: true /*,

            paste_preprocess: function (pl, o) {
                // Content string containing the HTML from the clipboard
                alert(o.content);
                o.content = "-: CLEANED :-\n" + o.content;
            },
            paste_postprocess: function (pl, o) {
                // Content DOM node containing the DOM structure of the clipboard
                alert(o.node.innerHTML);
                o.node.innerHTML = o.node.innerHTML + "\n-: CLEANED :-";
            }*/

        });
    </script>

    <style type="text/css">
        
        .container-ok {
            background-color: #a1d5b4;
            margin-right: 30px;
            margin-top: 20px;
            padding: 15px;
            color: #222;
        }
        
        .container-error {
            background-color: #CB3334;
            margin-right: 30px;
            margin-top: 20px;
            padding: 15px;
            color: #eee;
        }
        
    </style>

</head>
<body>

    <div id="facilityForm">
    <form id="form1" runat="server">

        <div runat="server" visible="false" id="saveDone" class="container-ok">
            Saving done.
        </div>

        <div runat="server" visible="false" id="saveError" class="container-error">
            Something went wrong. Please contact support if the problem remains. 
        </div>

        <br />
        <asp:LinkButton runat="server" ID="btnLogout" OnClick="btnLogout_OnClick">Logout</asp:LinkButton>
        <br />

        <h1>Edit facility</h1>

        <%-- Description --%>
        <h2>Description</h2>
        <table>
            <tr>
                <td class="first">Name</td>
                <td><asp:TextBox runat="server" ID="txtName" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td valign="top">Description</td>
                <td><asp:TextBox runat="server" ID="txtHtmlInfo" TextMode="MultiLine" Rows="15" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td valign="top">About</td>
                <td><asp:TextBox runat="server" ID="txtAbout" TextMode="MultiLine" Rows="15" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td valign="top">News</td>
                <td><asp:TextBox runat="server" ID="txtNews" TextMode="MultiLine" Rows="15" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td valign="top">Classes</td>
                <td><asp:TextBox runat="server" ID="txtOffers" TextMode="MultiLine" Rows="15" /></td>
                <td class="help"></td>
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
            <tr>
                <td>Address</td>
                <td><asp:TextBox runat="server" ID="txtAddress" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Zipcode</td>
                <td><asp:TextBox runat="server" ID="txtZipcode" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>City</td>
                <td><asp:TextBox runat="server" ID="txtCity" /></td>
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
            
        <%-- Position 
        <h2>Position</h2>
        <table>
            <tr>
                <td class="first">Longitude</td>
                <td><asp:TextBox runat="server" ID="txtLongitude" /></td>
                <td class="help"></td>
            </tr>
            <tr>
                <td>Latitude</td>
                <td><asp:TextBox runat="server" ID="txtLatitude" /></td>
                <td class="help"></td>
            </tr>
        </table>--%>
        
        <h2>Save</h2>


        <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSave_OnClick" />

        <div class="help">
            Beware that the changes will take effekt immediently after you
            click on "Save".
        </div>
    </form>
    </div>
</body>
</html>
