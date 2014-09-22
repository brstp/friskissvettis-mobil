using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Helpers;
using MoMA.Mobile.Pages;

public partial class loggedin_ConfirmUnbooking : MobilePage
{
    public string BookId
    {
        get
        {
            return ContextHelper.GetValue<string>("bookid", "");
        }
    }

    private DateTime dateTime
    {
        get
        {
            string sDate = ContextHelper.GetValue<string>("date", DateTime.Now.ToShortDateString()).Split(' ').First();
            DateTime date = DateTime.Now;
            DateTime.TryParse(sDate, out date);
            return date;
        }
    }

    private ScheduleItem _item = null;
    public ScheduleItem Item
    {
        get
        {
            string mode = ContextHelper.GetValue<string>("mode", "html");
            if (!mode.Equals("css"))
            {
                if (_item == null)
                {
                    string id = ContextHelper.GetValue<string>("id", "");
                    string bookid = ContextHelper.GetValue<string>("bookid", "");

                    if (string.IsNullOrEmpty(id))
                    {
                        Response.Redirect("/Default.aspx");
                    }
                    else
                    {
                        Facility facility = FriskisService.GetCurrentFacility();

                        if (facility != null)
                        {
                            // booked
                            _item = FriskisService.GetCurrentFacility().Service.GetBookedItem(id, bookid, facility);

                            if (_item == null)
                            {
                                _item = FriskisService.GetCurrentFacility().Service.GetScheduleItem(id, facility, dateTime);
                            }
                        }
                    }
                }

                if (_item == null)
                {
                    Response.Redirect("/Default.aspx");
                }

                return _item;
            }
            else
            {
                return new ScheduleItem();
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // skip confirm 
        btnUnbook_OnClick(null, new EventArgs());
        return;


        // iphone-fix to keep __doPostback()
        // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
        ClientTarget = "uplevel";

        string mode = ContextHelper.GetValue<string>("mode", "html");
        if (!mode.Equals("css"))
        {
            Title = Resources.LocalizedText.PageTitleConfirmUnbooking;
        }
    }

    protected void btnUnbook_OnClick(object sender, EventArgs e)
    {
        if (FriskisService.IsLoggedIn)
        {
            string id = ContextHelper.GetValue<string>("id", "");
            string bookid = ContextHelper.GetValue<string>("bookid", "");

            if (!string.IsNullOrEmpty(id))
            {

                Facility facility = FriskisService.GetCurrentFacility();

                Result result = null;

                switch (ContextHelper.GetValue<string>("m", "unbook"))
                {
                    case "unbook":
                        result = FriskisService.GetCurrentFriskisService().Unbook(id, bookid, facility, dateTime);
                        break;
                    case "standby":
                        result = FriskisService.GetCurrentFriskisService().UnbookStandby(bookid, facility);
                        break;
                }

                if (result.Success)
                {
                    FriskisService.Message = Resources.LocalizedText.UnbookingWasSuccessfull;
                    Response.Redirect("MyBookings.aspx"); // ?id=" + id + "&date=" + dateTime.ToShortDateString());
                }
                else
                {
                    FriskisService.Message = "Error: " + result.Message;
                    Response.Redirect("MyBookings.aspx");
                }
            }
        }
    }

    public string SpotsText()
    {
        return "";

        switch (ContextHelper.GetValue<string>("m", "book"))
        {
            case "book":
                return Resources.LocalizedText.Spots + ": " + Item.Available + "/" + Item.Total;
            case "standby":
                if (FriskisService.LoggedInMember.Facility.Service.ServiceType == FriskisServiceType.BRP)
                {
                    return Resources.LocalizedText.InQueue + ": " + Item.StandbyTotal;
                }
                else
                {
                    return Resources.LocalizedText.Spots + ": " + Item.StandbyAvailable;
                }
            default:
                return "";
        }
    }

    public string GetName(ScheduleItem item)
    {
        if (item.Standby)
        {
            return item.Name + " (" + Resources.LocalizedText.Standby.ToLower() + ")";
        }
        else
        {
            return item.Name;
        }
    }
}