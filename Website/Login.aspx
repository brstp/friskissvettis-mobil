<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!--<link href="css/login.css" rel="stylesheet" type="text/css" momasdk="true" />-->

    <script type="text/javascript">

        $(document).ready(function () {
            $("input").keyup(function (event) {
                if (event.keyCode == 13) {
                    //$(".btnLogin").click();
                    // document.forms[0].submit();
                    __doPostBack("ctl00$ContentPlaceHolder1$btnLogin","");
                }
            });
        });


    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <div class="container login_text_container">
        <h1><%= Resources.LocalizedText.Login %></h1>
    </div>

    <div runat="server" id="error" class="container container-error">
       
    </div>

    <div class="container container-dark">
        <%--<asp:DropDownList runat="server" ID="drpCity" AutoPostBack="true" OnSelectedIndexChanged="drpCity_OnSelectedIndexChanged"></asp:DropDownList>--%>

        <asp:DropDownList runat="server" ID="drpCity" AutoPostBack="true"></asp:DropDownList>

        <div class="spacer">&nbsp;</div>

        <!-- <asp:DropDownList runat="server" ID="drpFacility"></asp:DropDownList>

        <div class="spacer">&nbsp;</div> -->
        
        <div class="container-loginform">
            <label><%= UsernameLabel %>: </label>
            <input runat="server" id="txtUsername" type="text" class="text login" /> <br />
            <label><%= PasswordLabel %>:</label>
            <input runat="server" id="txtPassword" type="password" class="text abort" />
        </div>
    </div>

    <div class="container login_buttons">

        <asp:LinkButton runat="server" ID="btnLogin" OnClick="btnLogin_Click" CssClass="no-decoration btnLogin" >
            <div class="button button-red button-login">
                <%= Resources.LocalizedText.Login %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </asp:LinkButton>
    
        <a class="no-decoration" href="Default.aspx">
            <div class="button button-red">
                <%= Resources.LocalizedText.Abort %>
                <img src="/images/button/button-red-arrow.png" />
            </div>
        </a>

    </div>

</asp:Content>

