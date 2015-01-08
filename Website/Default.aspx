<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/default.css" rel="stylesheet" type="text/css" momasdk="false" />

    <!-- Slideshow -->
    <%= SlideshowCSS %>
    <script type="text/javascript">
        $(function () {
            $('.fadein img:gt(0)').hide();
            setInterval(function () { $('.fadein :first-child').fadeOut().next('img').fadeIn().end().appendTo('.fadein'); }, 3000);
        });

        $(document).ready(function () {
            var $btnSelect = $("[id$='btnSelect']");
            $btnSelect.click(function () {
                var selectedValue = $("[id$='drpCity']").val();

                if (selectedValue != "00000000-0000-0000-0000-000000000000") {
                    document.location = "FacilityMain.aspx?<%= FriskisService.IsLoggedIn ? "authenticated=true&" : "" %>facilityId=" + selectedValue;
                }
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <div class="fadein">
        <img class="slideshow" src="/images/start/slide1-<%= lang %>.jpg">
        <img class="slideshow" src="/images/start/slide2-<%= lang %>.jpg">
        <img class="slideshow" src="/images/start/slide3-<%= lang %>.jpg">
    </div>
</asp:Content>

<asp:Content ID="ContentExtra" ContentPlaceHolderID="ContentPlaceHolderExtra" Runat="Server">

    <div class="start_container">
	    <div class="container">
	
	<%--        <asp:DropDownList runat="server" ID="drpCity" AutoPostBack="true" OnSelectedIndexChanged="drpCity_OnSelectedIndexChanged"></asp:DropDownList>--%>
		   <asp:DropDownList runat="server" ID="drpCity"></asp:DropDownList>
	
		   <div class="spacer">&nbsp;</div>
	
		   <%--<asp:DropDownList runat="server" ID="drpFacility"></asp:DropDownList>--%>
	
		   <!--<div class="spacer">&nbsp;</div>-->
	
	<%--        <asp:LinkButton runat="server" ID="btnSelect" OnClick="btnSelect_Click">--%>
		   <a href="#" id="btnSelect">
			  <div class="button button-red">
				 <%= Resources.LocalizedText.Continue %>
				 <img src="images/button/button-red-arrow.png" />
			  </div>
		   </asp:LinkButton>
	
		   <div class="spacer">&nbsp;</div>
	    
		   <% if (FriskisService.IsApp) { %>
			  <a class="no-decoration" href="<%= AppMapLink %>">
				 <div class="button button-red">
					<%= Resources.LocalizedText.Map%>
					<img src="/images/button/button-red-arrow.png" />
				 </div>
			  </a>
		   <% } else { %>
			  <a class="no-decoration" href="Map.aspx">
				 <div class="button button-red">
					<%= Resources.LocalizedText.Map%>
					<img src="/images/button/button-red-arrow.png" />
				 </div>
			  </a>
		   <% } %>
	
		   <div class="spacer">&nbsp;</div>
	
		   <a href="ChooseLanguage.aspx" class="no-decoration" style="display: block">
			  <div class="button button-gray-half">
				 Choose language.
				 <img src="images/button/button-grey-arrow.png" />
			  </div>
		   </a>
	
		   <div class="clear" />
	    	<script language="javascript">
			//document.write(navigator.userAgent);
		</script>
	    </div>
	</div>

    <!-- <%= HttpContext.Current.Request.Url.ToString() %> -->

</asp:Content>