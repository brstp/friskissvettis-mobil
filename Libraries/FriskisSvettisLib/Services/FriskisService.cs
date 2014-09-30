using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using MoMA.Helpers;

/// <summary>
/// Summary description for FriskisService
/// </summary>
public class FriskisService
{

    private const string SESSION_LOGGEDIN_MEMBER = "SESSION_LOGGEDIN_MEMBER";
    private const string SESSION_MESSAGE = "SESSION_MESSAGE";

    // client detection
    private const string SESSION_CLIENTTYPE = "SESSION_CLIENTTYPE";
    private const string COOKIE_CLIENTTYPE = "COOKIE_CLIENTTYPE";
    private const string COOKIE_CLIENTTYPE_NAME = "NAME";
    private const string QUERYSTRING_CLIENTTYPE = "client";

    #region App helpers 

        /// <summary>
        ///  Returns the current message from session
        /// </summary>
        public static string Message
        {
            get
            {
                string message = "";

                // check if message exists
                if (HttpContext.Current.Session[SESSION_MESSAGE] != null)
                {
                    message = HttpContext.Current.Session[SESSION_MESSAGE].ToString();
                }

                return message;
            }
            set
            {
                HttpContext.Current.Session[SESSION_MESSAGE] = value;
            }
        }

        public static bool IsApp
        {
            get
            {
                //return true; // debug

                List<string> clients = new List<string>() { "android", "iphone", "wp" };

                // check if accepted client type
                return clients.Contains(Client.ToLower());
            }
        }

        private static string GetCorrectProtocol()
        {
            switch (Client) 
            {
                case "wp": return "about://";
                default: return "moma://";
            }
        }

        public static string GetAddToCalendarLink(ScheduleItem item) 
        {
            string protocol = GetCorrectProtocol();

            string linkUrl = protocol + "calendar?mode=add&title=" + HttpContext.Current.Server.UrlEncode(item.Name) + "&location=" + HttpContext.Current.Server.UrlEncode(item.Where.Name) + "&start=" + HttpContext.Current.Server.UrlEncode(item.From.ToString("yyyy-MM-dd HH:mm:ss")) + "&end=" + HttpContext.Current.Server.UrlEncode(item.To.ToString("yyyy-MM-dd HH:mm:ss"));
            return linkUrl.Replace("+", "%20");
        }

        public static string GetAppMapLink()
        {
            return GetAppMapLink(Guid.Empty);
        }

        public static string GetAppMapLink(Guid facilityLocalId)
        {
            string protocol = GetCorrectProtocol();
            Uri uri = HttpContext.Current.Request.Url;
            string url = "http://" + uri.Host + (uri.Port != 80 ? ":" + uri.Port : "") + "/handler/MapPositionHandler.ashx?action=GetByBounds";

            // string baseUrl = "moma://map?url=http%3A%2F%2Fwww.psan.se%2Fmap%2Fmap.xml";
            string baseUrl = protocol + "map?url=" + HttpContext.Current.Server.UrlEncode(url); 

            if (facilityLocalId.Equals(Guid.Empty))
            {
                return baseUrl;
            }
            else
            {
                return baseUrl + "&id=" + facilityLocalId.ToString();
            }
        }

        public static string Client
        {
            get
            {
                string client = "";

                // check querystring
                if (HttpContext.Current.Request.QueryString[QUERYSTRING_CLIENTTYPE] != null)
                {
                    client = HttpContext.Current.Request.QueryString[QUERYSTRING_CLIENTTYPE];
                }
                else
                {
                    // check session
                    if (HttpContext.Current.Session[SESSION_CLIENTTYPE] != null)
                    {
                        client = HttpContext.Current.Session[SESSION_CLIENTTYPE].ToString();
                    }

                    // check cookie 
                    else
                    {
                        HttpCookie cookie = CookieHelper.GetCookie(COOKIE_CLIENTTYPE);

                        if (cookie != null)
                        {
                            client = cookie[COOKIE_CLIENTTYPE_NAME].ToString();
                        }
                    }
                }

                // save in session
                HttpContext.Current.Session[SESSION_CLIENTTYPE] = client;

                // save in cookie 
                CookieHelper.SetCookie(COOKIE_CLIENTTYPE, new Dictionary<string, string>()
                {
                    {COOKIE_CLIENTTYPE_NAME, client}
                });

                // return found client (empty if not found)
                return client;
            }
        }

        public static string GetDomainLanguage()
        {
            string language = "";

            string host = HttpContext.Current.Request.Url.Host;
            List<string> parts = host.Split('.').ToList();

            string defaultLanguage = "sv";

            try
            {
                defaultLanguage = WebConfigurationManager.AppSettings["DefaultLanguage"];
            }
            catch
            {

            }

            if (host.Contains("localhost"))
            {
                language = defaultLanguage;
            }
            else if (host.Contains("sqlceno"))
            {
                language = "no";
            }
            else
            {
                switch (parts[parts.Count - 1])
                {
                    case "se":
                        language = "sv";
                        break;
                    case "no":
                        language = "no";
                        break;
                    default:
                        language = defaultLanguage;
                        break;
                }
            }

            return language;
        }

        public static string GetFacilityMainAddress(string facilityId)
        {
            string link = "/FacilityMain.aspx?facilityId=" + facilityId;

            // check if app and authenticated
            if (!string.IsNullOrEmpty(Client) && IsLoggedIn)
            {
                link += "&authenticated=true";
            }

            return link;
        }

    #endregion

    public static Member LoggedInMember
    {
        get
        {
            try
            {
                if (HttpContext.Current.Session[SESSION_LOGGEDIN_MEMBER] != null)
                {
                    return (Member)HttpContext.Current.Session[SESSION_LOGGEDIN_MEMBER];
                }
            }
            catch
            {
            }

            return null;
        }

        set
        {
            HttpContext.Current.Session[SESSION_LOGGEDIN_MEMBER] = value;
        }
    }

    public static bool IsLoggedIn
    {
        get
        {
            return LoggedInMember != null;
        }
    }

    /// <summary>
    /// Todo: update to be able to login to multiple services. 
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static Member Login(string username, string password, Facility facility)
    {
        // go through service login 
        Member member = facility.Service.Login(username, password, facility);

        if (member != null)
        {
            // if facility isn't set in service
            if (member.Facility == null)
            {
                member.Facility = facility;
            }
            LoggedInMember = member;
            return LoggedInMember;
        }

        return null;
    }

    public virtual bool Logout()
    {
        LoggedInMember = null;
        return true;
    }

    public static IFriskisService GetCurrentFriskisService()
    {
        // todo: replace with real service fetch
        // return new DemoService();
        // return new PastellService();
        if (IsLoggedIn)
        {
            return LoggedInMember.Facility.Service;
        }

        return null;
    }

    public static Facility GetCurrentFacility()
    {

        string facilityId = "";

        if (IsLoggedIn)
        {
            //loged in
            return LoggedInMember.Facility;
        }
        else
        {
            facilityId = ContextHelper.GetValue<string>("facilityId", "");

            try
            {
                // on page when not logged in
                return FacilityHelper.GetByLocalId(new Guid(facilityId));
            }
            catch
            {
                // when logging in
                string formFacilityName = "ctl00$ContentPlaceHolder1$drpFacility";
                string formCityName = "ctl00$ContentPlaceHolder1$drpCity";

                if (HttpContext.Current.Request.Form.AllKeys.Contains(formFacilityName) && !string.IsNullOrEmpty(HttpContext.Current.Request.Form[formFacilityName]))
                {
                    try
                    {
                        return FacilityHelper.GetByLocalId(new Guid(HttpContext.Current.Request.Form[formFacilityName]));
                    }
                    catch { }
                }
                else if (HttpContext.Current.Request.Form.AllKeys.Contains(formCityName) && !string.IsNullOrEmpty(HttpContext.Current.Request.Form[formCityName]))
                {
                    try
                    {
                        string formCityNameId = HttpContext.Current.Request.Form[formCityName];
                        return FacilityHelper.GetByLocalId(new Guid(formCityNameId));
                    }
                    catch { }
                }
                else
                {
                    // check if cached login exists
                    HttpCookie cookie = CookieHelper.CheckLoginCookie();

                    if (cookie != null)
                    {
                        facilityId = cookie.Values["FACILITY"];
                        return FacilityHelper.GetByLocalId(new Guid(facilityId));

                    }
                }
            }
        }

        return null;
    }
}