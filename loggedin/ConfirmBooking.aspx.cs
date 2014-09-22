using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;

public partial class loggedin_ConfirmBooking : MobilePage
{
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

                    if (string.IsNullOrEmpty(id))
                    {
                        Response.Redirect("/Default.aspx");
                    }
                    else
                    {
                        // main facility (förening)
                        Facility facility = FriskisService.GetCurrentFacility();
                        _item = FriskisService.GetCurrentFriskisService().GetScheduleItem(id, facility, dateTime);
                    }
                }
                return _item;
            }
            else
            {
                return new ScheduleItem();
            }
        }
    }

    private DateTime dateTime
    {
        get
        {
            string sDate = ContextHelper.GetValue<string>("date", DateTime.Now.ToShortDateString());
            DateTime date = DateTime.Now;
            DateTime.TryParse(sDate, out date);
            return date;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // instant book (no need to confirm)
        btnBook_OnClick(null, new EventArgs());
        return; 

        // iphone-fix to keep __doPostback()
        // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
        ClientTarget = "uplevel";

        string mode = ContextHelper.GetValue<string>("mode", "html");
        if (!mode.Equals("css"))
        {
            Title = Resources.LocalizedText.PageTitleConfirmBooking;
            error.InnerHtml = "";

            // get item and check that there are still spots available
            if (!Page.IsPostBack)
            {
                if (Item.Available == 0 && Item.StandbyAvailable == 0)
                {
                    error.Visible = true;
                    error.InnerHtml = Resources.LocalizedText.BookingFullError;
                    btnBook.Visible = false;
                }
            }
        }
    }

    protected void btnBook_OnClick(object sender, EventArgs e)
    {
        if (FriskisService.IsLoggedIn)
        {
            string id = ContextHelper.GetValue<string>("id", "");

            if (!string.IsNullOrEmpty(id))
            {
                Facility facility = FriskisService.GetCurrentFacility();

                Result result = null;

                try
                {
                    switch (ContextHelper.GetValue<string>("m", "book"))
                    {
                        case "book":
                            result = FriskisService.GetCurrentFriskisService().Book(id, facility, dateTime);
                            break;
                        case "standby":
                            result = FriskisService.GetCurrentFriskisService().BookStandby(id, facility, dateTime);
                            break;
                    }
                }
                catch
                {
                    result.Success = false;
                    result.Message = Resources.LocalizedText.UnknownError;
                }

                if (result.Success)
                {
                    FriskisService.Message = Resources.LocalizedText.BookingWasSuccessfull;
                    Response.Redirect("MyBookings.aspx"); //?booking=true&id=" + id + "&date=" + dateTime.ToShortDateString());
                }
                else
                {
                    error.Visible = true;
                    error.InnerHtml = result.Message;
                }
            }
        }
        else
        {
            Response.Redirect("/Login.aspx");
        }
    }

    public string SpotsText()
    {
        switch (ContextHelper.GetValue<string>("m", "book"))
        {
            case "book":
                return Resources.LocalizedText.Spots + ": " + Item.Available + "/" + Item.Total;
            case "standby":
                if (FriskisService.LoggedInMember.Facility.Service.ServiceType == FriskisServiceType.BRP)
                {
                    return ""; // Resources.LocalizedText.InQueue + ": " + Item.StandbyTotal;
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