using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;

public partial class loggedin_MyBookings : MobilePage
{

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

    protected void Page_Load(object sender, EventArgs e)
    {
    	this.AutoAddMobileMetaTags = false;
    	
        // iphone-fix to keep __doPostback()
        // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
        ClientTarget = "uplevel";

        // FriskisService.Message = "Ditt pass är bokat";

        //// check if message should be shown 
        //if (string.IsNullOrEmpty(FriskisService.Message))
        //{
        //    divSuceessMessage.Visible = false;
        //}
        //else 
        //{
        //    divSuceessMessage.Visible = true;
        //    lblSuccessMessage.Text = FriskisService.Message;

        //    if (lblSuccessMessage.Text.StartsWith("Error: "))
        //    {
        //        lblSuccessMessage.Text = lblSuccessMessage.Text.Replace("Error: ", "");
        //        divSuceessMessage.Attributes["class"] = "container container-error";
        //    }
            
        //    // clear message 
        //    FriskisService.Message = "";
        //}

        Title = Resources.LocalizedText.PageTitleMyBookings;

        if (!FriskisService.IsLoggedIn)
        {
            Response.Redirect("/Default.aspx");
        }

        if (!Page.IsPostBack)
        {
            // booked items
            List<ScheduleItem> items = FriskisService.GetCurrentFriskisService().GetBookings(FriskisService.LoggedInMember);

            // standby items
            items.AddRange(FriskisService.GetCurrentFriskisService().GetScheduleStandyItems(FriskisService.LoggedInMember));

            // show items and order by start date
            rptrBookings.DataSource = items.OrderBy(i => i.From).ToList();
            rptrBookings.DataBind();

            if (items.Count == 0)
            {
                divNoBookings.Visible = true;
            }
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

    public string GetAddToPhoneCalendar(ScheduleItem item)
    {
        return FriskisService.GetAddToCalendarLink(item);
    }

    public string GetDescription(ScheduleItem item)
    {
        if (item.Standby)
        {
            return Resources.LocalizedText.StandbyPosition + ": " + item.StandbyPosition + "<br />" + 
                item.GetDescriptionHtml();
        }
        else
        {
            return item.GetDescriptionHtml();
        }
    }

    public string GetUnbookLink(ScheduleItem item)
    {
        string link = "ConfirmUnbooking.aspx?id=" + item.Id + "&bookid=" + item.BookId + "&date=" + item.From.ToShortDateString();

        if (item.Standby)
        {
            return link + "&m=standby";
        }
        else
        {
            return link;
        }
    }
}