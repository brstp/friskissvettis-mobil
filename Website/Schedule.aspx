<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Schedule.aspx.cs" Inherits="Schedule" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <link href="css/common-schedule-item.css" rel="stylesheet" type="text/css" momasdk="true" />
    <link href="css/schedule.css" rel="stylesheet" type="text/css" momasdk="true" />
    
    <!-- load activity selects from handler via ajax instead of postback -->
    <script type="text/javascript">

        // used to reselect the boxes (since they wasn't filled from .net - viewstate won't take care of this)
        var selectedActivityId = '<%= SelectedActivityId %>';
        var selectedActivityChildId = '<%= SelectedActivityChildId %>';
        var localizationLanguage = '<%= localizationLanguage %>';

    </script>

    <!-- <script src="js/jquery.min.js" type="text/javascript"></script> -->
    <script src="js/select.js" type="text/javascript"></script>
    <script src="js/schedule.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div runat="server" visible="false" id="error" class="container container-error">
        
    </div>

    <div runat="server" id="divContainer" class="container">

        <div runat="server" id="divSuceessMessage" visible="false" class="container container-success">
            <asp:Label runat="server" ID="lblSuccessMessage"></asp:Label>
        </div>

        <h1 class="schedule"><%= Resources.LocalizedText.Schedule %></h1>

        <div class="line" />

        <div class="spacer">&nbsp;</div>
        
        <div class="schedule">

            <div class="spacer">&nbsp;</div>
            
            <label runat="server" id="lblFacilities" visible="false"><%= Resources.LocalizedText.LableScheduleFacility %>: </label>
            <div runat="server" id="divFacilities" visible="false">
                <asp:DropDownList runat="server" DataTextField="Name" DataValueField="Id" ID="drpFacilities"></asp:DropDownList>
                <div class="spacer">&nbsp;</div>
            </div>

            <label runat="server" id="lblRooms" visible="false"><%= Resources.LocalizedText.LableScheduleRoom %>: </label>
            <div runat="server" id="divRooms" visible="false">
                <asp:DropDownList runat="server" DataTextField="Name" DataValueField="Id" ID="drpRooms"></asp:DropDownList>
                <div class="spacer">&nbsp;</div>
            </div>
            
            <label runat="server" id="lblActivity" visible="true"><%= Resources.LocalizedText.LableScheduleActivity %>: </label>
            <div runat="server" id="divActivity" visible="true">
                <asp:DropDownList runat="server" DataTextField="Name" DataValueField="Id" ID="drpActivity" OnSelectedIndexChanged="drpActivity_OnSelectedIndexChanged"></asp:DropDownList>
                <div class="spacer">&nbsp;</div>
            </div>
            
            <label runat="server" id="lblActivityTypes" visible="true"><%= Resources.LocalizedText.LableScheduleActivityType %>: </label>
            <div runat="server" id="divActivityTypes" visible="true">
                <asp:DropDownList runat="server" DataTextField="Name" DataValueField="Id" ID="drpActivityTypes"></asp:DropDownList>
                <div class="spacer">&nbsp;</div>
            </div>
            
            <label runat="server" id="lblInstructors" visible="false"><%= Resources.LocalizedText.LableScheduleInstructor %>: </label>
            <div runat="server" id="divInstructors" visible="false">
                <asp:DropDownList runat="server" DataTextField="Name" DataValueField="Id" ID="drpInstructors"></asp:DropDownList>
                <div class="spacer">&nbsp;</div>
            </div>
            
            <label><%= Resources.LocalizedText.LableScheduleFromDate %>: </label>
            <asp:DropDownList runat="server" ID="drpFrom" AutoPostBack="false" OnSelectedIndexChanged="drpFrom_OnSelectedIndexChanged"></asp:DropDownList>

            <div class="spacer">&nbsp;</div>

            <label><%= Resources.LocalizedText.LableScheduleToDate %>: </label>
            <asp:DropDownList runat="server" ID="drpTo"></asp:DropDownList>
            
            <div class="spacer">&nbsp;</div>
            <div class="line" />
            <div class="spacer">&nbsp;</div>
                
            <asp:LinkButton runat="server" ID="btnShow" OnClick="btnShow_Click" CssClass="no-decoration" >
                <div class="button button-red">
                    <%= Resources.LocalizedText.Show %>
                    <img alt="" src="/images/button/button-red-arrow.png" />
                </div>
            </asp:LinkButton>
        </div>
    </div>
    
    <div runat="server" id="divScheduleItems" class="container container-dark">

        <div runat="server" class="text-normal text-white" id="divNoPass" visible="false">
            <%= Resources.LocalizedText.NoActivitiesCouldBeFound %>
        </div>

        <asp:Repeater ID="rptrSchedule" runat="server" OnItemDataBound="rptrSchedule_ItemDataBound">
            <ItemTemplate>

                <div class="container-schedule container-workout-schedule">
                    <table width="100%">
                        <tr>
                            <td class="description-td" valign="top">
                                <h1><%# DataBinder.Eval(Container.DataItem, "Name").ToString().Replace("/", "/&#8203;") %></h1>
                                <div class="description">
                                    <%# GetDescription((ScheduleItem)Container.DataItem) %>
                                </div>
                            </td>
                            <td valign="top" rowspan="2">
                                <%# GetBook((ScheduleItem)Container.DataItem) %>
                                <%# GetSpots((ScheduleItem)Container.DataItem) %>
                            </td>
                        </tr>
                    </table>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>

