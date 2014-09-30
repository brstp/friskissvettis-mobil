<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ChooseLanguage.aspx.cs" Inherits="ChooseLanguage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	
    <div class="container">

	   <h1>Choose language</h1>
	   
	   <div class="spacer" />
	   <div class="line" />
	   <div class="spacer" />

	   <asp:LinkButton CssClass="first" runat="server" ID="btnSwedish" OnClick="btnSwedish_Click">
		  <div class="button button-red">
			 Svenska
			 <img src="images/button/button-red-arrow.png" />
		  </div>
		  <div class="spacer" />
	   </asp:LinkButton>
	   <asp:LinkButton CssClass="first" runat="server" ID="btnNorwegian" OnClick="btnNorwegian_Click">
		  <div class="button button-red">
			 Norsk
			 <img src="images/button/button-red-arrow.png" />
		  </div>
		  <div class="spacer" />
	   </asp:LinkButton>
	   <asp:LinkButton runat="server" ID="btnEnglish" OnClick="btnEnglish_Click">
		  <div class="button button-red">
			 English
			 <img src="images/button/button-red-arrow.png" />
		  </div>
	   </asp:LinkButton>

    </div>
    
</asp:Content>

