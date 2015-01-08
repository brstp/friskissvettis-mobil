using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;
using System.Diagnostics;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;

public partial class Schedule : MobilePage
{

    #region Helpers 

        public string SelectedActivityId { get; set; }
        public string SelectedActivityChildId { get; set; }

        public string localizationLanguage 
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.ToString();
            }
        }
        /// <summary>
        /// Gets selected From-date
        /// </summary>
        public DateTime From
        {
            get
            {
                DateTime from = DateTime.Now;
                DateTime.TryParse(drpFrom.SelectedValue, out from);
                return from.CompareTo(DateTime.MinValue) == 0 ? DateTime.Now : from;
            }
        }

        /// <summary>
        /// Gets selected To-date
        /// </summary>
        public DateTime To
        {
            get
            {
                DateTime to = DateTime.Now;
                DateTime.TryParse(drpTo.SelectedValue, out to);
                return to.CompareTo(DateTime.MinValue) == 0 ? DateTime.Now : to;
            }
        }

        /// <summary>
        /// Gets correct list item for From/To-dropdowns
        /// </summary>
        /// <param name="dateTime">DateTime to transform</param>
        /// <returns></returns>
        public ListItem CreateDateTimeDropdownListItem(DateTime dateTime)
        {
            string day = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);

            int daysBetween = Convert.ToInt32(dateTime.Subtract(DateTime.Now).TotalDays);

            switch (daysBetween)
            {
                case 0:
                    day = Resources.LocalizedText.Today;
                    break;
                case 1:
                    day = Resources.LocalizedText.Tomorrow;
                    break;
                default:
                    day = day.UppercaseFirst();
                    break;
            }

            string dateString = day + " " + dateTime.ToString("dd") + "/" + dateTime.ToString("MM");
            ListItem listItem = new ListItem(dateString, dateTime.ToString("yyyy-MM-dd"));

            return listItem;
        }

        /// <summary>
        /// Current mode, could be removed?
        /// </summary>
        public string Mode
        {
            get
            {
                return ContextHelper.GetValue<string>("mode", "html");
            }
        }

        public string GetFormValue(string name)
        {
            foreach (string key in Request.Form.Keys)
            {
                if (key.EndsWith(name))
                {
                    return Request.Form[key];
                }
            }

            return "";
        }

        /// <summary>
        /// Fills schedule items 
        /// </summary>
        public void FillScheduleItem()
        {
            // shorthand to current logged in member
            Member member = FriskisService.LoggedInMember;

            // used to get schedule items and the current server to use
            List<Facility> facilities = null;
            Facility facility = null;

            // clear error message
            error.InnerHtml = string.Empty;

            // get facility (or inner child facility) from member if logged in
            if (member != null)
            {
                facility = member.Facility;

                if (string.IsNullOrEmpty(drpFacilities.SelectedValue))
                {
                    // default facility (defendo)
                    facilities = new List<Facility>() { member.Facility };
                }
                else if (drpFacilities.SelectedValue.Equals("All"))
                {
                    if (member.Facility.Facilities != null)
                    {
                        facilities = new List<Facility>();
                        facilities.AddRange(member.Facility.Facilities);
                    }
                    else
                    {
                        facilities = new List<Facility>() { member.Facility };
                    }
                }
                else
                {
                    // child facility (pastell and brp)
                    facilities = new List<Facility>() { member.Facility.GetInnerFacility(drpFacilities.SelectedValue) };
                }
            }
            else
            {
                string facilityId = ContextHelper.GetValue<string>("facilityId", "");
                facility = FacilityHelper.GetByLocalId(new Guid(facilityId));
                facility = facility.Service.CachedFacility;
                if (facility != null)
                {
                    facilities = facility.Facilities;
                }

                // just use the selected one
                if (!string.IsNullOrEmpty(drpFacilities.SelectedValue) && !drpFacilities.SelectedValue.Equals("All"))
                {
                    IFriskisService service = facility.Service;
                    Facility selectedFacility = facility.GetInnerFacility(drpFacilities.SelectedValue);
                    facilities = new List<Facility>() { selectedFacility };
                }

            }

            if (facilities == null || facility == null)
            {
                facility = FriskisService.GetCurrentFacility();
                facilities = new List<Facility>() { facility };
            }

            // only proceed if a facility with service was found
            if (facilities != null)
            {
                if (facilities.Count > 0 != null && facility.Service != null)
                {
                    // shorthand to current service that should be used when getting schedule items 
                    // always the same sevice on all facilities
                    IFriskisService service = facility.Service;

                    // check if schedule can be shown without being logged in
                    // if the service require login and the user is not logged in then show 
                    // a error message and return from this function
                    if ((service.ScheduleLoginNeeded && !FriskisService.IsLoggedIn))
                    {
                        error.InnerHtml = Resources.LocalizedText.HaveToBeLoggedInToViewSchedule;
                        error.Visible = true;
                        divContainer.Visible = false;
                        divScheduleItems.Visible = false;
                        return;
                    }

                    SelectedActivityId = GetFormValue("drpActivity");
                    SelectedActivityChildId = GetFormValue("drpActivityTypes");

                    string instructor = drpInstructors.SelectedIndex > 0 ? drpInstructors.SelectedItem.Text : "";
                    List<ScheduleItem> scheduleItems = service.GetScheduleItems(facility, facilities, member, SelectedActivityId, SelectedActivityChildId, instructor, From, To);

                    // filter schedule items by choosen activity type
                    if (facility.Service.CachedActivities != null && !string.IsNullOrEmpty(SelectedActivityId)
                        && service.ServiceType != FriskisServiceType.Defendo
                        && service.ServiceType != FriskisServiceType.DLSoftware)
                    {
                        scheduleItems = scheduleItems.Where(s => s.ActivityTypeId.Equals(SelectedActivityId)).ToList(); // && DateTime.Now.CompareTo(s.From) < 0).ToList(); // && s.From.AddDays(-1).CompareTo(DateTime.Now.) >= 0).ToList();
                    }

                    // don't show schedule items that has already begun
                    // todo: add sort function directly in service.GetScheduleItems
                    scheduleItems = scheduleItems.Where(s => s.From.CompareTo(DateTime.Now) > 0).ToList();

                    // sort items by from-date
                    // todo: add sort function directly in service.GetScheduleItems
                    scheduleItems = scheduleItems.OrderBy(s => s.From).ToList();

                    // check if sorting on room should be used 
                    if (divRooms.Visible && drpRooms.SelectedValue != "All")
                    {
                        scheduleItems = scheduleItems.Where(s => s.Room.Equals(drpRooms.SelectedValue)).ToList();
                    }

                    // display items though repeater
                    rptrSchedule.DataSource = scheduleItems;
                    rptrSchedule.DataBind();

                    // only show error if there is a error-message available in the error-control
                    error.Visible = !string.IsNullOrEmpty(error.InnerHtml);

                    // show a text saying that no schedule items was found if 
                    // there are no schedule items with the current filters
                    // turned on
                    divNoPass.Visible = scheduleItems.Count == 0;
                }
            }
        }

    #endregion

    #region Overrides

        /// <summary>
        /// Adds message functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // MESSAGE
            if (string.IsNullOrEmpty(FriskisService.Message))
            {
                divSuceessMessage.Visible = false;
            }
            else
            {
                divSuceessMessage.Visible = true;
                lblSuccessMessage.Text = FriskisService.Message;

                if (lblSuccessMessage.Text.StartsWith("Error: "))
                {
                    lblSuccessMessage.Text = lblSuccessMessage.Text.Replace("Error: ", "");
                    divSuceessMessage.Attributes["class"] = "container container-error";
                }

                // clear message 
                FriskisService.Message = "";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            // MoMA.Mobile.Cache.CachedOutput.ClearAll();
            // UseCache = true;
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // iphone-fix to keep __doPostback()
            // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
            ClientTarget = "uplevel";

            Title = Resources.LocalizedText.PageTitleSchedule;
            this.AutoAddMobileMetaTags = false;

            if (!Page.IsPostBack)
            {
                // Get current facility (anläggning)
                // Currently not used, but will be used when facilities can be get without login
                string facilityId = ContextHelper.GetValue<string>("facilityId", "");

                // if no facility is choosen and user is not logged in, then redirect to the startpage
                if (string.IsNullOrEmpty(facilityId) && !FriskisService.IsLoggedIn)
                {
                    Response.Redirect("/Default.aspx");
                }

                // From/To 
                List<ListItem> items = new List<ListItem>();

                // Defendo should have eight days in the datetime-select. All the others should have seven. 
                int numberOfDaysInDropdown = 7;
                try
                {
                    if (FriskisService.GetCurrentFacility().Service.ServiceType == FriskisServiceType.Defendo)
                    {
                        numberOfDaysInDropdown = 8;
                    }
                }
                catch { }

                if (FriskisService.GetCurrentFacility().ScheduleLength.HasValue != null && FriskisService.GetCurrentFacility().ScheduleLength >= 0)
                {
                    numberOfDaysInDropdown = FriskisService.GetCurrentFacility().ScheduleLength.Value;
                }

                for (int i = 0; i < numberOfDaysInDropdown; i++)
                {
                    DateTime dateTime = DateTime.Now.AddDays(i);
                    items.Add(CreateDateTimeDropdownListItem(dateTime));
                }

                drpFrom.Items.AddRange(items.ToArray());
                drpTo.Items.AddRange(items.ToArray());

                // facilities
                Facility facility = null;
                if (FriskisService.LoggedInMember != null && FriskisService.LoggedInMember.Facility != null)
                {
                    facility = FriskisService.LoggedInMember.Facility;

                    List<Facility> facilities = new List<Facility>();

                    // multiple facilities
                    if (FriskisService.LoggedInMember.Facility.Facilities != null)
                    {
                        facilities.AddRange(FriskisService.LoggedInMember.Facility.Facilities.OrderBy(f => f.Name).ToList());
                    }
                    // single facility
                    else
                    {
                        facilities.Add(FriskisService.LoggedInMember.Facility);
                    }

                    if (facilities.Where(f => f.Id.Equals("All")).Count() == 0)
                    {
                        facilities.Insert(0, new Facility()
                        {
                            Id = "All",
                            Name = Resources.LocalizedText.All
                        });
                    }

                    if (facilities.Count > 2)
                    {
                        lblFacilities.Visible = true;
                        divFacilities.Visible = facilities.Count > 2;
                    }

                    drpFacilities.DataSource = facilities;
                    drpFacilities.DataBind();

                    drpFacilities.SelectedIndex = 0;
                }

                // show facilities when not logged in
                else if (!FriskisService.IsLoggedIn)
                {
                    facility = FacilityHelper.GetByLocalId(new Guid(facilityId));

                    if (facility != null && facility.Service != null && facility.Service.CachedFacility != null)
                    {
                        List<Facility> facilities = new List<Facility>();
                        facilities.AddRange(facility.Service.CachedFacility.Facilities);
                        facilities = facilities.OrderBy(f => f.Name).ToList();

                        if (facilities.Where(f => f.Id.Equals("All")).Count() == 0)
                        {
                            facilities.Insert(0, new Facility()
                            {
                                Id = "All",
                                Name = Resources.LocalizedText.All
                            });
                        }

                        if (facilities.Count > 2)
                        {
                            lblFacilities.Visible = true;
                            divFacilities.Visible = true;
                        }

                        facility.Facilities = facilities;
                        drpFacilities.DataSource = facilities;
                        drpFacilities.DataBind();
                    }
                    else
                    {
                        divFacilities.Visible = false;
                    }
                }

                // instructors 
                List<Instructor> instructors = new List<Instructor>();

                try
                {
                    instructors.AddRange(facility.Service.CachedInstructors.OrderBy(a => a.Name).Where(a => !string.IsNullOrEmpty(a.Name)).ToList());
                }
                catch { }

                if (instructors != null)
                {
                    List<Instructor> instructorsToShow = new List<Instructor>(){
                        new Instructor()
                        {
                            Id = "",
                            Name = Resources.LocalizedText.All
                        }
                    };

                    instructorsToShow.AddRange(instructors);

                    if (instructorsToShow.Count > 2)
                    {
                        lblInstructors.Visible = true;
                        divInstructors.Visible = true;
                    }

                    drpInstructors.DataSource = instructorsToShow;
                    drpInstructors.DataBind();

                    drpInstructors.SelectedIndex = 0;
                }

                // Check if room should be visible 
                if (facility.Service.ServiceType == FriskisServiceType.BRP && drpFacilities.Items.Count <= 2)
                {
                    divFacilities.Visible = false;
                    lblFacilities.Visible = false;

                    divRooms.Visible = true;
                    lblRooms.Visible = true;

                    List<string> rooms = facility.Service.CachedRooms;

                    drpRooms.Items.Clear();
                    drpRooms.Items.Add(new ListItem(Resources.LocalizedText.All,"All"));
                    foreach (string room in rooms)
                    {
                        drpRooms.Items.Add(new ListItem(room));
                    }
                }
            }
        }

    #endregion

    #region Events

        /// <summary>
        /// Determines if the schedule item should be visible or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void rptrSchedule_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ScheduleItem scheduleItem = (ScheduleItem)e.Item.DataItem;
            BookType bookType = scheduleItem.GetBookType();

            Facility facility = FriskisService.GetCurrentFacility();

            if (scheduleItem.Cancelled)
            {
                e.Item.Visible = false;
            }

            if (facility.Service.ServiceType == FriskisServiceType.BRP && DateTime.Now.CompareTo(scheduleItem.BookableFrom) < 0)
            {
                return;
            }

            if (bookType == BookType.NotAvailable)
            {
                // CHANGE 2012-09-18
                //e.Item.Visible = false;
            }

            if (!scheduleItem.Visible || bookType == BookType.NotVisible)
            {
                // CHANGE 2012-09-18
                //e.Item.Visible = false;
            }
        }

        public void drpActivity_OnSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Updates the "To"-date based on the selected "From". 
        /// "To" can't be less that "From". 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void drpFrom_OnSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Update schedule on page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnShow_Click(object sender, EventArgs e)
        {
            FillScheduleItem();
        }

    #endregion

    #region Output

        public string GetDescription(ScheduleItem item)
        {
            return item.GetDescriptionHtml();
        }

        public string GetSpots(ScheduleItem item)
        {
            BookType bookType = item.GetBookType();
            string spots = "<div class=\"spots\">";

            if (FriskisService.IsLoggedIn)
            {
                switch (bookType)
                {
                    case BookType.Bookable:
                        spots += Resources.LocalizedText.Spots + ": " + item.Available + "/" + item.Total;

                        if (item.DropinAvailable > 0)
                        {
                            spots += "<br />(" + Resources.LocalizedText.Dropin + ": " + item.DropinAvailable + ")";
                        }
                        break;

                    case BookType.Dropin:
                        spots += Resources.LocalizedText.Spots + ": " + item.DropinAvailable;
                        break;

                    case BookType.BookableAndDropin:
                        return "";

                    case BookType.Full:
                        if (item.DropinAvailable > 0)
                        {
                            spots += "(" + Resources.LocalizedText.Dropin + ": " + item.DropinAvailable + ")";
                        }
                        else
                        {
                            return "";
                        }
                        break;

                    case BookType.Standby:
                        if (FriskisService.LoggedInMember.Facility.Service.ServiceType == FriskisServiceType.BRP)
                        {
                            spots += Resources.LocalizedText.InQueue + ": " + item.StandbyBooked; 
                            
                            if (item.DropinAvailable > 0)
                            {
                                spots += "<br />(" + Resources.LocalizedText.Spots + ": " + item.DropinAvailable + ")";
                            }
                        }
                        else if (FriskisService.LoggedInMember.Facility.Service.ServiceType == FriskisServiceType.PastellData)
                        {
                            spots += Resources.LocalizedText.Spots + ": 0";
                        }
                        else
                        {
                            spots += Resources.LocalizedText.Spots + ": " + item.StandbyAvailable;
                        }
                        break;

                    //case BookType.NotAvailable:
                    //    return "";

                    default:
                        return "";
                }

                return spots + "</div>";
            }

            // No info shown if not logged in
            return "";
        }

        public string GetBook(ScheduleItem item)
        {
            BookType bookType = item.GetBookType();

            // These modes should only be shown when logged in
            if (FriskisService.IsLoggedIn)
            {
                switch (bookType)
                {
                    case BookType.Bookable:

                        return @"<a class=""no-decoration button-book"" href=""/loggedin/ConfirmBooking.aspx?id=" + item.Id + @"&date=" + item.From.ToShortDateString() + @""">
                            <div class=""button button-red button-small "">
                                <span class=""text"">" + Resources.LocalizedText.Book + @"</span>
                                <img class=""arrow"" alt=""icon"" src=""/images/button/button-red-arrow.png"" />
                                <!-- <img class=""loader"" src=""/images/ajax-loader.gif"" /> -->
                            </div>
                        </a>";

                    case BookType.Booked:

                        return @"<div class=""button button-gray"">
                                    " + Resources.LocalizedText.Booked + 
                                "</div>";

                    case BookType.Standby:

                        return @"<a class=""no-decoration button-book"" href=""/loggedin/ConfirmBooking.aspx?m=standby&id=" + item.Id + @""">
                            <div class=""button button-red button-small "">
                                <span class=""text"">" + Resources.LocalizedText.BookStandby + /* @"
                                <img alt=""icon"" src=""/images/button/button-red-arrow.png"" />" */
                            "</span></div>" + 
                        "</a>";

                    case BookType.BookableAndDropin:

                        return "";

                    case BookType.NotAvailable:

                        return "";
                }
            }

            // Show this even if user is not logged in
            switch (bookType)
            {

                case BookType.Dropin:

                    return @"<div class=""button button-gray"">
                            Drop-in" +
                            "</div>";

                case BookType.Full:

                    return @"<div class=""button button-gray"">" +
                                Resources.LocalizedText.Full +
                            "</div>";

                case BookType.Cancelled:

                    return @"<div class=""button button-gray"">" +
                                Resources.LocalizedText.Cancelled +
                            "</div>";
            }

            // No info shown if not logged in
            return "";
        }

    #endregion
}