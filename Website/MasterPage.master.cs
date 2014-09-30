using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Globalization;
using MoMA.Helpers;

public partial class MasterPage : System.Web.UI.MasterPage
{
    public bool UseToolbar { get; set; }

    public string lang
    {
        get
        {
            string l =  Thread.CurrentThread.CurrentUICulture.ToString();
            return string.IsNullOrEmpty(l) ? "sv" : l;
        }
    }

    private Facility CurrentFacility
    {
        get
        {
            string facilityId = ContextHelper.GetValue<string>("facilityId", "");
            if (!string.IsNullOrEmpty(facilityId))
            {
                try
                {
                    return FacilityHelper.GetByLocalId(new Guid(facilityId));
                } 
                catch { }
            }
            else if (FriskisService.IsLoggedIn)
            {
                return FriskisService.LoggedInMember.Facility;
            }

            return null;
        }
    }

    /// <summary>
    /// If the visitor is on a facility page or is logged in and not on startpage. 
    /// </summary>
    public bool ChoosenFacility
    {
        get
        {
            // Start page
            //if (Request.Url.ToString().Contains("Default.aspx"))
            //{
            //    return false;
            //}

            // If a facility is watched
            if (Request.QueryString["facilityId"] != null)
            {
                return true;
            }

            // if on logged in page
            if (Request.Url.ToString().Contains("loggedin"))
            {
                return true;
            }

            if (FriskisService.LoggedInMember != null)
            {
                return true;
            }

            return false;
        }
    }

    public string LoginText
    {
        get
        {
            string text = "";
            Member member = FriskisService.LoggedInMember;

            if (member != null)
            {
                if (!string.IsNullOrEmpty(member.Firstname) && !string.IsNullOrEmpty(member.Lastname))
                {
                    text = Resources.LocalizedText.LoggedInAs + ": ";
                }
            }

            return text;
        }
    }

    public string LoginLink
    {
        get
        {
            if (FriskisService.IsLoggedIn)
            {
                return @"<a href=""/Logout.aspx"" alt=""Start""><img id=""icon-login"" alt=""" + Resources.LocalizedText.Logout + @""" class=""icon"" src=""/images/master/icons/logout-" + lang + @".png"" /></a>";
            }
            else
            {
                string facilityId = ContextHelper.GetValue<string>("facilityId", "");
                if (string.IsNullOrEmpty(facilityId))
                {
                    return @"<a href=""/Login.aspx"" alt=""Start""><img id=""icon-login"" alt=""" + Resources.LocalizedText.Login + @""" class=""icon"" src=""/images/master/icons/login-" + lang + @".png"" /></a>";
                }
                else
                {
                    return @"<a href=""/Login.aspx?facilityId=" + facilityId + @""" alt=""Start""><img id=""icon-login"" alt=""" + Resources.LocalizedText.Login + @""" class=""icon"" src=""/images/master/icons/login-" + lang + @".png"" /></a>";
                }
                
            }
        }
    }

    public string HomeAhref
    {
        get
        {
            string link = "/Default.aspx";

            if (ChoosenFacility)
            {
                string facilityId = "";

                if (FriskisService.LoggedInMember != null)
                {
                    facilityId = FriskisService.LoggedInMember.Facility.LocalId.ToString();
                }
                else
                {
                    facilityId = ContextHelper.GetValue<string>("facilityId", "");
                }

                // link = "/FacilityMain.aspx?facilityId=" + facilityId;
                link = FriskisService.GetFacilityMainAddress(facilityId);
            }

            return link;
        }
    }

    public string HomeLink
    {
        get
        {
            if (FriskisService.LoggedInMember != null)
            {
                return @"<a href=""" + HomeAhref + @""" alt=""Start""><img id=""icon-logo"" alt=""Friskis & Svettis"" class=""icon"" src=""/images/master/icons/logo.png"" /></a>";
            }
            else
            {
                return @"<a href=""/"" alt=""Start""><img id=""icon-logo"" alt=""Friskis & Svettis"" class=""icon"" src=""/images/master/icons/logo.png"" /></a>";
            }

            
            // return @"<a href=""/"" alt=""Start""><img id=""icon-logo"" alt=""Friskis & Svettis"" class=""icon"" src=""/images/master/icons/logo.png"" /></a>";
        }
    }

    /// <summary>
    /// AboutUs when not logged in and Bookings when logged in.
    /// </summary>
    public string BookingsAboutLink
    {
        get
        {
            string link = "/AboutUs.aspx";
            string linkImage = "about";
            string alt = Resources.LocalizedText.AboutUs;

            // change image, link and alt if user is logged in
            if (FriskisService.IsLoggedIn)
            {
                link = "/loggedin/MyBookings.aspx";
                linkImage = "bookings";
                alt = Resources.LocalizedText.MyBookings;
            }

            // get current facility
            if (ChoosenFacility)
            {
                string facilityId = "";

                if (FriskisService.LoggedInMember != null)
                {
                    facilityId = FriskisService.LoggedInMember.Facility.LocalId.ToString();
                    link += "?facilityId=" + facilityId;
                }
                else
                {
                    facilityId = ContextHelper.GetValue<string>("facilityId", "");
                    link = "/AboutUs.aspx";
                    link += "?facilityId=" + facilityId;
                }

            }

            return @"<a href=""" + link + @""" alt=""""><img id=""icon-about"" alt=""" + alt + @""" class=""icon"" src=""/images/master/icons/" + linkImage + "-" + lang + @".png"" /></a>";
        }
    }

    public string BackImage
    {
        get
        {
            return @"<img src=""/images/button/back-" + lang + @".png"" />";
        }
    }

    public string HomeImageLink
    {
        get
        {
            return "<a href=\"" + HomeAhref + "\" id=\"btnHome\" class=\"no-decoration\">" +
                        "<img src=\"/images/button/home-" + lang + ".png\" />" +
                   "</a>";
        }
    }

    public string AboutLink
    {
        get
        {
            string link = "/AboutUs.aspx?" + ((FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "");
            string facilityId = ContextHelper.GetValue<string>("facilityId", "");

            if (!string.IsNullOrEmpty(facilityId)) {
                link += "facilityId=" + facilityId;
            }

            return link;
        }
    }

    public string AboutSmallImageLink
    {
        get
        {
            if (FriskisService.IsLoggedIn)
            {
                Facility facility = FriskisService.GetCurrentFacility();

                if (facility != null)
                {
                    string link = "/AboutUs.aspx?" + ((FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "") + "facilityId=" + facility.LocalId;

                    return "<a href=\"" + link + "\" id=\"btnAboutUs\" class=\"no-decoration\">" +
                                "<img src=\"/images/button/about-" + lang + ".png\" />" +
                           "</a>";
                }
            }

            return "";
        }
    }

    public string HomepageLink
    {
        get
        {
            if (CurrentFacility != null && !string.IsNullOrEmpty(CurrentFacility.Homepage))
            {
                return CurrentFacility.Homepage;
            }
            else 
            {
                switch (FriskisService.GetDomainLanguage())
                {
                    case "sv": 
                        return "http://www.web.friskissvettis.se";
                    case "no":
                        return "http://www.friskissvettis.no/";
                    default: 
                        return "http://www.web.friskissvettis.se";
                }
            }
        }
    }

    public string FacebookLink
    {
        get
        {
            string link = "<a target=\"_blank\"  href=\"";

            if (CurrentFacility != null)
            {
                if (string.IsNullOrEmpty(CurrentFacility.Facebook))
                {
                    return "";
                }

                link += CurrentFacility.Facebook;
            }
            else
            {
                link += "http://www.facebook.com/FriskisSvettisRiks";
            }

            return link + "\"><img id=\"footer-icons-facebook\" src=\"/images/master/footer-facebook.png\" /></a>";
        }
    }

    public string TwitterLink
    {
        get
        {
            string link = "<a target=\"_blank\" href=\"";

            if (CurrentFacility != null)
            {
                if (string.IsNullOrEmpty(CurrentFacility.Twitter))
                {
                    return "<div id=\"footer-icons-twitter-empty\">&nbsp;</div>";
                }
                
                link += CurrentFacility.Twitter;
            }
            else
            {
                link += "http://twitter.com/annaiwarsson";
            }

            return link + "\"><img id=\"footer-icons-twitter\" src=\"/images/master/footer-twitter.png\" /></a>";
        }
    }

    private void CheckLoginCookie()
    {
        // only check if login-cookie exists if user is not logged in or is logging out
        if (!FriskisService.IsLoggedIn && !Request.Url.ToString().Contains("Logout.aspx"))
        {
            HttpCookie cookie = CookieHelper.CheckLoginCookie();

            if (cookie != null)
            {
                string AUTH = "AUTH";

                string facilityId = CookieHelper.GetCookie(AUTH).Values["FACILITY"];
                string username = CookieHelper.GetCookie(AUTH).Values["USERNAME"];
                string password = CookieHelper.GetCookie(AUTH).Values["PASSWORD"];

                Facility facility = null;

                try
                {
                    facility = FacilityHelper.GetByLocalId(new Guid(facilityId));
                }
                catch { }

                if (facility != null)
                {
                    Member member = FriskisService.Login(username, password, facility);

                    if (member != null && HttpContext.Current.Request.Url.ToString().ToLower().Contains("default.aspx"))
                    {
                        Response.Redirect("/loggedin/MyBookings.aspx", true);
                    }
                }
            }
        }
    }

    private void SetLanguage()
    {
        string language = "";

        // check if language is stored in cookie first
        if (CookieHelper.GetCookie("LANGUAGE") != null)
        {
            language = CookieHelper.GetCookie("LANGUAGE").Value;
        }

        // get language by domain (".se", ".no" or ".com")
        if (string.IsNullOrEmpty(language))
        {
            language = FriskisService.GetDomainLanguage();
        }

        Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
    }

    protected override void OnInit(EventArgs e)
    {
        // init client everywhere
        string client = FriskisService.Client;

        SetLanguage();
        CheckLoginCookie();
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Header.Title = Resources.LocalizedText.PageTitle + " - " + Page.Header.Title;

        // default
        UseToolbar = true;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
    }
}
