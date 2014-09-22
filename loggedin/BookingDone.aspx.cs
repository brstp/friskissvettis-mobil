using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;

public partial class loggedin_BookingDone : MobilePage
{
    //public string UrlEncode(string str)
    //{
    //    return Server.UrlEncode(str).Replace("+", "%20");
    //}

    public string AddToPhoneCalendarText
    {
        get
        {
            // return "moma://calendar?mode=add&title=" + Server.UrlEncode(Item.Name) + "&location=" + Server.UrlEncode(Item.Where.Name) + "&start=" + Server.UrlEncode(Item.From.ToString()) + "&end=" + Server.UrlEncode(Item.To.ToString()) + "";
            return FriskisService.GetAddToCalendarLink(Item);
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
                        Facility facility = FriskisService.GetCurrentFacility();

                        _item = FriskisService.GetCurrentFacility().Service.GetBookedItem(id, "", facility);

                        if (_item == null)
                        {
                            _item = FriskisService.GetCurrentFacility().Service.GetScheduleItem(id, facility, dateTime);
                        }
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

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleBookingDone;
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