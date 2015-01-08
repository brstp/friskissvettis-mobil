using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;
using System.ServiceModel;
using FriskisSvettisLib.PastellWebService;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;
using MoMA.Mobile.Device;

public partial class _Default : MobilePage
{

    public string lang
    {
        get
        {
            string l = Thread.CurrentThread.CurrentUICulture.ToString();
            return string.IsNullOrEmpty(l) ? "sv" : l;
        }
    }

    public string AppMapLink 
    {
        get 
        {
            return FriskisService.GetAppMapLink(Guid.Empty);
        }
    }

    public string SlideshowCSS
    {
        get
        {
            float width = DeviceInfo.CurrentDevice.DisplayWidth;
            float height = width * 0.55f;

            return @"<style type=""text/css"">
                        .fadein { position:relative; height:{height}px; width:{height}px; }
                        .fadein img { position:absolute; left:0; top:0; }
                     </style>".Replace("{height}", height.ToString()).Replace("{width}", width.ToString());
        }
    }

   

    protected void Page_Load(object sender, EventArgs e)
    {        
        
        // iphone-fix to keep __doPostback()
        // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
        ClientTarget = "uplevel";

        Title = Resources.LocalizedText.PageTitleStartpage;

        if (!Page.IsPostBack)
        {
            HttpCookie cookie = CookieHelper.CheckLoginCookie();

            if (cookie != null)
            {
                Response.Redirect("/loggedin/MyBookings.aspx");
            }

            // Fill cities 
            List<Facility> cities = new List<Facility>
            {
                new Facility() {
                    Id = "",
                    Name = Resources.LocalizedText.ChooseTown
                }
            };

            cities.AddRange(FacilityHelper.GetAllVisibleByLang(FriskisService.GetDomainLanguage()).OrderBy(c => c.Name).ToList());

            foreach (Facility city in cities)
            {
                drpCity.Items.Add(new ListItem(city.Name, city.LocalId.ToString()));
            }

            // check if there should be a postback on city-select
            if (!FacilityHelper.HasSubCities(FriskisService.GetDomainLanguage()))
            {
                drpCity.AutoPostBack = false;
            }
        }

        //if (drpFacility.Items.Count == 0)
        //{
        //    drpFacility.Visible = false;
        //}
        //else
        //{
        //    drpFacility.Visible = true;
        //}
    }

    //protected void drpCity_OnSelectedIndexChanged(object sender, EventArgs e)
    //{
    //    drpFacility.Items.Clear();

    //    List<ListItem> facilityItems = new List<ListItem>()
    //    {
    //        new ListItem(Resources.LocalizedText.ChooseFacility, "")
    //    };

    //    List<Facility> facilities = FacilityHelper.GetByCity(drpCity.SelectedValue);

    //    foreach (Facility facility in facilities)
    //    {
    //        drpFacility.Items.Add(new ListItem(facility.Name, facility.LocalId.ToString()));
    //    }

    //    if (drpFacility.Items.Count <= 1)
    //    {
    //        drpFacility.Visible = false;
    //    }
    //    else
    //    {
    //        drpFacility.Visible = true;
    //    }
    //}

    //protected void btnSelect_Click(object sender, EventArgs e)
    //{
    //    if (!string.IsNullOrEmpty(drpCity.SelectedValue) && !string.IsNullOrEmpty(drpFacility.SelectedValue))
    //    {
    //        Response.Cache.SetCacheability(HttpCacheability.Public);
    //        TimeSpan ts = new TimeSpan(36, 0, 0);
    //        Response.Cache.SetMaxAge(ts);

    //        Response.Redirect("FacilityMain.aspx?facilityId=" + drpFacility.SelectedValue);
    //    }
    //}

    protected override void OnPreRender(EventArgs e)
    {
        // hide toolbar
        ((MasterPage)this.Master).UseToolbar = false;
        this.AutoAddMobileMetaTags = false;

        base.OnPreRender(e);
    }
}