using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;
using System.Threading;

public partial class Login : MobilePage
{
    public Facility GetFacilityFromSelect()
    {
        Facility facility = null;
        string facilityId = drpCity.SelectedValue;

        if (!string.IsNullOrEmpty(facilityId))
        {
            try
            {
                facility = FacilityHelper.GetByLocalId(new Guid(facilityId));
            }
            catch { }
        }

        return facility;
    }

    public string UsernameLabel
    {
        get
        {
            string label = "";
            Facility facility = GetFacilityFromSelect();

            if (facility == null || string.IsNullOrWhiteSpace(facility.UsernameLabel))
            {
                label = Resources.LocalizedText.Username;
            }
            else
            {
                label = facility.UsernameLabel;
            }

            return label;
        }
    }

    public string PasswordLabel
    {
        get
        {
            string label = "";
            Facility facility = GetFacilityFromSelect();

            if (facility == null || string.IsNullOrWhiteSpace(facility.PasswordLabel))
            {
                label = Resources.LocalizedText.Password;
            }
            else
            {
                label = facility.PasswordLabel;
            }

            return label;
        }
    }

    public string lang
    {
        get
        {
            string l = Thread.CurrentThread.CurrentUICulture.ToString();
            return string.IsNullOrEmpty(l) ? "sv" : l;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // iphone-fix to keep __doPostback()
        // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
        ClientTarget = "uplevel";

        Title = Resources.LocalizedText.PageTitleLogin;

        // txtUsername.Attributes.Add("placeholder", Resources.LocalizedText.Username);
        // txtPassword.Attributes.Add("placeholder", Resources.LocalizedText.Password);

        if (!Page.IsPostBack)
        {
            // check if login-cookie exists
            HttpCookie cookie = CookieHelper.CheckLoginCookie();

            if (cookie != null)
            {
                Response.Redirect("/loggedin/MyBookings.aspx");
            }

            // Fill cities 
            //List<string> cities = new List<string>
            //{
            //    Resources.LocalizedText.ChooseTown
            //};

            //cities.AddRange(FacilityHelper.GetAllCities(FriskisService.GetDomainLanguage()));

            //foreach (string city in cities)
            //{
            //    drpCity.Items.Add(new ListItem(city, city));
            //}

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

            // Check if facility should be selected from start
            string facilityId = ContextHelper.GetValue<string>("facilityId", "");
            if (!string.IsNullOrEmpty(facilityId))
            {
                Facility facility = null;

                try
                {
                    facility = FacilityHelper.GetByLocalId(new Guid(facilityId));
                }
                catch { }

                if (facility != null)
                {
                    drpCity.SelectedValue = facility.LocalId.ToString();
                    FillFacilities();
                    drpFacility.SelectedValue = facilityId;
                }
            }
            else
            {
                drpFacility.Visible = false;
            }

            // check if there should be a postback on city-select
            //if (!FacilityHelper.HasSubCities(FriskisService.GetDomainLanguage()))
            //{
            //    drpCity.AutoPostBack = false;
            //}
        }

        error.Visible = false;
    }

    //protected void drpCity_OnSelectedIndexChanged(object sender, EventArgs e)
    //{
    //    // Fill facilities
    //    if (!string.IsNullOrEmpty(drpCity.SelectedValue))
    //    {
    //        FillFacilities();
    //    }
    //}

    /// <summary>
    /// Todo: Test all services here? 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        // check if a facility was choosen
        if (drpCity.SelectedIndex < 1)
        {
            error.InnerHtml = Resources.LocalizedText.NoCityChoosen;
            error.Visible = true;
            return;
        }
        else
        {
            // default error is "wrong username or password"
            error.InnerHtml = Resources.LocalizedText.WrongMemberOrPassword;
            error.Visible = false;
        }

        // try to get facility and service
        Facility facility = null;
        IFriskisService service = null;

        try 
        {
            facility = FacilityHelper.GetByLocalId(new Guid(drpCity.SelectedValue));
            service = facility.Service;
        }
        catch {}

        // if service and facility was found and a username and password was entered, then try to login
        if (service != null && facility != null && !string.IsNullOrEmpty(txtUsername.Value) && !string.IsNullOrEmpty(txtPassword.Value))
        {
            Member member = FriskisService.Login(txtUsername.Value, txtPassword.Value, facility);

            if (member != null)
            {
                // set the login-cookie to be able to be logged in the next time the user visits the page 
                CookieHelper.SetLoginCookie(txtUsername.Value, txtPassword.Value, facility.LocalId.ToString());

                // goto users bookings
                Response.Redirect("/loggedin/MyBookings.aspx");
            }
        }

        // todo: service error
        error.Visible = true;
        return;
    }

    public void FillFacilities()
    {
        drpFacility.Items.Clear();

        List<Facility> facilities = FacilityHelper.GetByCity(drpCity.SelectedValue);

        foreach (Facility facility in facilities)
        {
            drpFacility.Items.Add(new ListItem(facility.Name, facility.LocalId.ToString()));
        }

        drpFacility.Visible = facilities.Count > 1;
    }
}