using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoMA.Helpers;
using System.Xml;
using FriskisSvettisLib.Helpers;
using FriskisSvettisLib;

/// <summary>
/// BRPService
/// 
/// Test/Demo
/// 
/// API-key: b134ac13766540599924f678a004019f
/// user:arvid@momasolutions.com
/// pw: test7798
/// </summary>
public class BRPService : FriskisService, IFriskisService
{
    Logger logger = new Logger();

    #region Demo

        //private const string DEMO_URL = "http://212.91.140.132:8080/appdemo/mesh/";
        //public const string DEMO_APIKEY = "b134ac13766540599924f678a004019f";
        //public const string DEMO_USERNAME = "arvid@momasolutions.com";
        //public const string DEMO_PASSWORD = "test7798";

    #endregion

    #region Cache

        public List<string> CachedRooms
        {
            get
            {
                string name = "APPLICATION_ROOMS_BRP_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                List<string> rooms = null;

                try
                {
                    // get from cache
                    rooms = (List<string>)HttpContext.Current.Application[name];
                }
                catch { }

                // fallback
                if (rooms == null)
                {
                    // get from server
                    rooms = GetAllRooms();
                    CachedRooms = rooms;
                }

                return rooms;
            }
            set
            {
                string name = "APPLICATION_ROOMS_BRP_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        /// <summary>
        /// Updates each time a user logs in
        /// </summary>
        public Facility CachedFacility
        {
            get
            {
                Facility facility = null;

                try
                {
                    // get from cache
                    string name = "APPLICATION_FACILITY_BRP_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    facility = (Facility)HttpContext.Current.Application[name];
                }
                catch { }

                // fallback
                if (facility == null)
                {
                    // get from server
                    facility = new Facility();
                    facility.Facilities = GetAllFacilities();
                    CachedFacility = facility;
                }

                facility.Service = this;
                foreach (Facility childFacility in facility.Facilities)
                {
                    childFacility.Service = this;
                }

                return facility;
            }
            set
            {
                string name = "APPLICATION_FACILITY_BRP_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        /// <summary>
        /// Updates each time a user logs in
        /// </summary>
        public List<Activity> CachedActivities
        {
            get
            {
                string name = "APPLICATION_ACTIVITIES_BRP_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                List<Activity> activities = null;

                try
                {
                    // get from cache
                    activities = (List<Activity>)HttpContext.Current.Application[name];
                }
                catch { }

                // fallback
                if (activities == null)
                {
                    // get from server
                    activities = GetAllActivities();
                    CachedActivities = activities;
                }

                return activities;
            }
            set
            {
                string name = "APPLICATION_ACTIVITIES_BRP_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public List<Instructor> CachedInstructors
        {
            get
            {
                return new List<Instructor>();
            }
            set
            {
            }
        }

    #endregion

    #region Properties

        public bool ScheduleLoginNeeded { get; set; }
        public string ServiceUrl { get; set; }
        public string ServiceLicense { get; set; }

        public static Dictionary<string, object> DefaultValues
        {
            get
            {
                return new Dictionary<string, object>()
                {
                        {"ServiceUrl", ""},
                        {"ServiceLicense", ""},
                        {"ScheduleLoginNeeded", false},
                };
            }
        }

        public string ApiKey
        {
            get
            {
                // return DEMO_APIKEY; // update to real url later
                return ServiceLicense;
            }
        }

        public FriskisServiceType ServiceType
        {
            get
            {
                return FriskisServiceType.BRP;
            }
        }

    #endregion

    /// <summary>
    /// Returns a member when the member was validated and 
    /// null when the login was incorrect.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Member Login(string username, string password, Facility facility)
    {
        // Demouser: arvid@momasolutions.com / test7798


        // Anrop
        // ------------------------------------------------------------
        // APIURL/verifyUser.action?user=(email eller kortnummer)&username={kortnummer i BRP}&email={epostadress}&password={lösenord}
        // user, username eller email används tillsammans med password.

        // Svar
        // ------------------------------------------------------------
        // Vid lyckad verifiering:
        // <authresponse>
        //  <result>success</result>
        //  <user>
        //   <customerid>12345</customerid>
        //   <firstname>Förnamn</firstname>
        //   <surname>Efternamn</surname>
        //   <email>email@example.com</email>
        //   <mobile>123 45 67</mobile>
        //   <address>
        //   <street>Gatuadress 123</street>
        //   <zip>12345</zip>
        //   <city>POSTORT</city>
        //   </address>
        //   <cardnumber>67890</cardnumber>
        //   <businessunit>1</businessunit>
        //   <gender>Man/Kvinna/Odefinierad</gender>
        //   <staffmember>true/false</staffmember>
        //   <image>showImage.action?customerNumber=12345</image>
        //   <birthDate>1977-07-07</birthDate>
        //  </user>
        // </authresponse>
        //
        // <image> är en relativ URL till användarens bild. I exemplet ovan gör man alltså ett anrop till APIURL/
        // showImage.action?customerNumber=12345 för att hämta bilden. Om användaren inte har någon bild
        // returneras felkod 404 vid försök att hämta bilden.

        // Svar
        // ------------------------------------------------------------
        // Vid misslyckad verifiering:
        // 
        // <authresponse>
        //  <result>failed</result>
        // </authresponse>

        string url = ServiceUrl + "verifyUser.action?" +
                        "user=" + username + "&password=" + password;


        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: Login, GET: verifyUser");

        string response = ScrapeHelper.Get(new Uri(url));

        if (!response.Contains("<result>failed</result>"))
        {
            Member member = new Member(response, this);

            if (member != null)
            {
                member.Username = username;
                member.Password = password;

                // facilities
                facility.Facilities = GetAllFacilities();

                foreach (Facility innerFacility in facility.Facilities)
                {
                    innerFacility.Service = facility.Service;
                }

                CachedFacility = facility;

                // activities
                List<Activity> activities = GetAllActivities();

                CachedActivities = activities;
                facility.Activities = activities;

                return member;
            }
        }

        return null;
    }

    private List<Activity> GetAllActivities()
    {
        List<Activity> activities = new List<Activity>();
        string url = ServiceUrl + "loadProductGroupsXML.action?" + "businessUnit=1" + "&apikey=" + ApiKey;


        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: GetAllActivities, GET: loadProductGroupsXML");
        string response = ScrapeHelper.Get(new Uri(url));

        XmlDocument xmlDocResponse = new XmlDocument();
        xmlDocResponse.LoadXml(response);

        XmlNodeList activityNodes = xmlDocResponse.SelectNodes("//productgrouplist/productgroup");
        foreach (XmlNode rootactivityNode in activityNodes)
        {
            Activity rootactivity = new Activity();
            rootactivity.Id = XmlHelper.GetValue<string>(rootactivityNode, "id", "");
            rootactivity.Name = XmlHelper.GetValue<string>(rootactivityNode, "name", "");
            rootactivity.ChildActivites = new List<Activity>();

            activities.Add(rootactivity);
        }

        return activities;
    }

    private List<string> GetAllRooms()
    {
        Facility facility = FriskisService.GetCurrentFacility();
        List<ScheduleItem> items = GetScheduleItems(facility, null, null, null, DateTime.Now, DateTime.Now.AddDays(6));
        List<string> rooms = items.Select(item => item.Room).ToList();
        return rooms.Distinct().ToList();
    }

    public List<Facility> GetAllFacilities()
    {

        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: GetAllFacilities, GET: loadBusinessUnitsXML");
        string response = ScrapeHelper.Get(new Uri(ServiceUrl.Trim() + "loadBusinessUnitsXML.action?apikey=" + ApiKey.Trim()));

        List<Facility> facilities = Facility.ParseMany(response, this);

        foreach (Facility facility in facilities)
        {
            facility.Service = this;
        }

        return facilities;
    }

    public List<ScheduleItem> GetBookings(Member member)
    {
        if (member != null) 
        {
            string url = ServiceUrl + @"showBookedGroupActivitiesXML.action?user=" + member.Username + "&password=" + member.Password + "&apikey=" + ApiKey;

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: GetBookings, GET: showBookedGroupActivitiesXML");
            string response = ScrapeHelper.Get(new Uri(url));

            List<ScheduleItem> items = ScheduleItem.ParseMany(response, DateTime.Now, this);

            foreach (ScheduleItem item in items)
            {
                Facility facility = member.Facility.Facilities.Where(f => f.Id.Equals(item.Where.Id)).FirstOrDefault();

                if (facility != null)
                {
                    item.Where = facility;
                }
            }

            return items;
        }

        return new List<ScheduleItem>();
    }

    public Result Book(string activityId, Facility facility, DateTime dateTime)
    {
        //  <?xml version="1.0" encoding="ISO-8859-1"?>
        //  <reply>
        //    <command>bookGroupActivity</command>
        //    <result>success</result>
        //    <message></message>
        //  </reply>


        Member member = FriskisService.LoggedInMember;
        Result result = new Result();

        if (member != null)
        {
            string url = ServiceUrl + @"bookGroupActivityXML.action?groupActivityId=" + activityId + "&user=" + member.Username +
                            "&password=" + member.Password + @"&locale=sv&channel=1&apikey=" + ApiKey;

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: Book, GET: bookGroupActivityXML");
            string response = ScrapeHelper.Get(new Uri(url));

            XmlDocument xmlDocResponse = new XmlDocument();
            xmlDocResponse.LoadXml(response);

            // result.Success = response.Contains("success");
            result.Success = xmlDocResponse.SelectSingleNode("//reply/result").InnerText.Contains("success");
            result.Message = xmlDocResponse.SelectSingleNode("//reply/message").InnerText;

            // Logg result 
            logger.Log("", LogItemProvider.BRP, facility.Name , LogItemAction.Book, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

            return result;
        }

        // Logg result 
        logger.Log("", LogItemProvider.BRP, facility.Name, LogItemAction.Book , "","User not logged in", LogItemType.Error);

        result.Success = false;
        result.Message = "Ej inloggad.";

        return result;
    }

    public Result Unbook(string id, string bookId, Facility facility, DateTime dateTime)
    {
        //  APIURL/debookGroupActivityXML.action?
        //  bookingId={bokningsid i BRP}
        //  &user={passdeltagarens epostadress/kortnummer}
        //  &password={kundens lösenord}
        //  &locale={sv/en/no}
        //  &apikey={tilldelad säkerhetsnyckel}

        //  <reply>
        //    <command>debookGroupActivity</command>
        //    <result>success eller error</result>
        //    <message>tomt vid success, annars felmeddelande</message>
        //  </reply>

        //  <?xml version="1.0" encoding="ISO-8859-1"?>
        //  <reply>
        //    <command>debookGroupActivity</command>
        //    <result>success</result>
        //    <message></message>
        //  </reply>

        //  Följande meddelanden (message) kan dyka upp, tror de är ganska självförklarande:
        //
        //   - invalid or missing apikey
        //   - En eller flera obligatoriska parametrar saknas
        //   - Felaktigt användarnamn eller lösenord
        //   - Kunde inte avboka
        //
        //  "Kunde inte avboka" innefattar "övriga" icke-hanterade fel, om ni får dessa återkommande så måste vi undersöka vad som går snett, en tydligare förklaring bör finnas i våra loggar i de fallen.

        Member member = FriskisService.LoggedInMember;

        if (member != null)
        {
            //ScheduleItem bookItem = GetBookedItem(id, bookId, facility);

            //// check if this isn't a standby item before trying to unbook (bug in brp api) 
            //if (!bookItem.Standby)
            //{
                Result result = new Result();

                // construct call to brp
                string url = ServiceUrl + @"debookGroupActivityXML.action?bookingId=" + bookId + "&user=" + member.Username +
                                "&password=" + member.Password + @"&locale=sv&channel=1&apikey=" + ApiKey;

                // log call (if logged is on - default off)
                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: Unbook, GET: debookGroupActivityXML");

                // get response from brp 
                string response = ScrapeHelper.Get(new Uri(url));

                // handle response from brp
                XmlDocument xmlDocResponse = new XmlDocument();
                xmlDocResponse.LoadXml(response);

                result.Success = xmlDocResponse.SelectSingleNode("//reply/result").InnerText.Contains("success");
                result.Message = xmlDocResponse.SelectSingleNode("//reply/message").InnerText;

                // Extra check if item was unbooked
                ScheduleItem bookItem = GetBookedItem(id, bookId, facility);

                if (bookItem != null)
                {
                    result.Success = false;
                    result.Message = "Ett fel inträffade. Var god försök försök igen.";
                }

                // Logg result 
                logger.Log("", LogItemProvider.BRP, facility.Name , LogItemAction.Unbook, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

                return result;
            //}
            //else
            //{
            //    return new Result("Ett fel inträffade.", false);
            //}
        }

        // Logg result 
        logger.Log("", LogItemProvider.BRP, facility.Name, LogItemAction.Unbook, "", "User not logged in", LogItemType.Error);

        return new Result("", false);
    }

    public List<ScheduleItem> GetScheduleItems(Facility facility, Member member, string activityType, WorkoutType workoutType, DateTime From, DateTime To)
    {
        return GetScheduleItems(facility, new List<Facility>() { facility }, member, activityType, workoutType, From, To);
    }

    /// <summary>
    /// Get from ALL facilities
    /// </summary>
    /// <returns></returns>
    private List<ScheduleItem> GetFromAllFacilitiesScheduleItems(Facility facility, Member member, string activityType, WorkoutType workoutType, DateTime From, DateTime To)
    {
        return GetScheduleItems(facility, null, member, activityType, workoutType, From, To);
    }

    public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activity, string activityType, string instructor, DateTime From, DateTime To)
    {
        return GetScheduleItems(mainFacility, facilities, member, activity, null, From, To);
    }

    /// <summary>
    /// Get from ALL facilities
    /// </summary>
    /// <returns></returns>
    private List<ScheduleItem> GetScheduleItems(Facility mainFacility, Member member, string activity, string activityType, string instructor, DateTime From, DateTime To)
    {
        return GetScheduleItems(mainFacility, null, member, activity, null, From, To);
    }

    public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activityType, WorkoutType workoutType, DateTime From, DateTime To)
    {
        List<ScheduleItem> items = new List<ScheduleItem>();

        string url = "";

        TimeSpan span = To - From;
        int days = span.Days + 1;
        days = days > 7 ? 7 : days;

        if (facilities == null)
        {
            // if no facilities was sent, add a new one with no id
            facilities = new List<Facility>(){
                new Facility()
                {
                    Id = ""
                }
            };
        }

        foreach (Facility facility in facilities)
        {

            List<ScheduleItem> innerItems = new List<ScheduleItem>();
            for (int i = 0; i < days; i++)
            {
                string businessUnit = "";

                if (!string.IsNullOrEmpty(facility.Id))
                {
                    businessUnit = "&businessUnit=" + facility.Id;
                }

                if (member == null)
                {
                    // url = ServiceUrl + "showGroupActivitiesXML.action?&businessUnit=" + facility.Id + "&date=" + From.AddDays(i).ToShortDateString() + "&todate=" + To.ToShortDateString();
                    url = ServiceUrl + "showGroupActivitiesXML.action?" + businessUnit + "&date=" + From.AddDays(i).ToShortDateString() + "&todate=" + To.ToShortDateString();
                }
                else
                {
                    // url = ServiceUrl + "showGroupActivitiesXML.action?user=" + member.Username + "&password=" + member.Password + "&businessUnit=" + facility.Id + "&date=" + From.AddDays(i).ToShortDateString() + "&todate=" + To.ToShortDateString();
                    url = ServiceUrl + "showGroupActivitiesXML.action?user=" + member.Username + "&password=" + member.Password + businessUnit + "&date=" + From.AddDays(i).ToShortDateString() + "&todate=" + To.ToShortDateString();
                }

                // check so that ?& becomes ?
                url = url.Replace("?&", "?");

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: GetScheduleItems, GET: showGroupActivitiesXML");
                string response = ScrapeHelper.Get(new Uri(url));
                List<ScheduleItem> parsedItems = ScheduleItem.ParseMany(response, From, this);

                foreach (ScheduleItem parsedItem in parsedItems)
                {
                    Activity activity = CachedActivities.Where(c => c.Name.Equals(parsedItem.ActivityTypeId)).FirstOrDefault();
                    if (activity != null)
                    {
                        parsedItem.ActivityTypeId = activity.Id;
                    }
                }

                innerItems.AddRange(parsedItems);
            }

            // get facilities based on facility id and member facility login 
            foreach (ScheduleItem item in innerItems)
            {
                string facilityId = item.Where.Id;
                item.Where = facility.GetInnerFacility(facilityId);

                if (item.Where == null)
                {
                    item.Where = mainFacility.GetInnerFacility(facilityId);
                }
            }

            items.AddRange(innerItems);
        }

        return items;
    }

    public ScheduleItem GetBookedItem(string id, string bookId, Facility facility)
    {
        List<ScheduleItem> items = GetBookings(FriskisService.LoggedInMember);

        if (string.IsNullOrEmpty(id))
        {
            return items.Where(i => i.BookId.Equals(bookId)).FirstOrDefault();
        }
        else
        {
            return items.Where(i => i.Id.Equals(id)).FirstOrDefault();
        }
    }

    /// <summary>
    /// todo: add facility, member, workoutType, from and to here? 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime)
    {
        for (int i = 0; i < 7; i++)
        {
            //foreach (Facility innerFacility in facility.Facilities)
            //{            
                // TimeSpan difference = dateTime.Subtract(DateTime.Now);
                TimeSpan difference = DateTime.Now.Subtract(DateTime.Now);

                // DateTime date = DateTime.Now.AddDays(difference.Days + 1);
                DateTime date = DateTime.Now.AddDays(i);
                List<ScheduleItem> items = GetFromAllFacilitiesScheduleItems(facility, null, "", null, date, date);
                ScheduleItem item = items.Where(it => it.Id.Equals(id)).FirstOrDefault();

                if (item != null)
                {
                    return item;
                }

                //if (item == null)
                //{
                //        date = DateTime.Now.AddDays(i);
                //        items = GetScheduleItems(facility, null, "", null, date, date);
                //        item = items.Where(it => it.Id.Equals(id)).FirstOrDefault();

                //        if (item != null)
                //        {
                //            return item;
                //        }
                //}
                //else
                //{
                //    return item;
                //}
            //}
        }

        return null;
    }

    #region Standby

        public List<ScheduleItem> GetScheduleStandyItems(Member member)
        {
            return new List<ScheduleItem>();
        }

        public Result BookStandby(string activityId, Facility facility, DateTime dateTime)
        {
            Member member = FriskisService.LoggedInMember;
            Result result = new Result();

            if (member != null) 
            {
                string url = ServiceUrl + @"bookWaitingListXML.action?groupActivityId=" + activityId + "&user=" + member.Username +
                                "&password=" + member.Password + @"&locale=sv&channel=1&apikey=" + ApiKey;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: BookStandby, GET: bookWaitingListXML");
                string response = ScrapeHelper.Get(new Uri(url));

                XmlDocument xmlDocResponse = new XmlDocument();
                xmlDocResponse.LoadXml(response);

                result.Success = response.Contains("success");
                result.Message = xmlDocResponse.SelectSingleNode("//reply/message").InnerText;

                // Logg result 
                logger.Log("", LogItemProvider.BRP, facility.Name, LogItemAction.BookStandby, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

                return result;
            }

            // Logg result 
            logger.Log("", LogItemProvider.BRP, facility.Name, LogItemAction.BookStandby, "", "User not logged in", LogItemType.Error);

            result.Success = false;
            result.Message = "Ej inloggad.";

            return result;

        }

        public Result UnbookStandby(string bookId, Facility facility)
        {
            Member member = FriskisService.LoggedInMember;

            if (member != null)
            {

                //ScheduleItem bookItem = GetBookedItem(null, bookId, facility);

                //// check if this is a standby item before trying to unbook (bug in brp api) 
                //if (bookItem.Standby)
                //{
                    string url = ServiceUrl + @"debookWaitingListXML.action?bookingId=" + bookId + "&user=" + member.Username +
                                    "&password=" + member.Password + @"&locale=sv&channel=1&apikey=" + ApiKey;

                    OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "BRP", "C#: UnbookStandby, GET: debookWaitingListXML");
                    string response = ScrapeHelper.Get(new Uri(url));

                    // Extra check if item was unbooked
                    ScheduleItem bookItem = GetBookedItem(null, bookId, facility);

                    Result result = new Result("", response.Contains("success"));

                    if (bookItem != null)
                    {
                        result.Success = false;
                        result.Message = "Ett fel inträffade. Var god försök igen.";
                    }

                    // Logg result 
                    logger.Log("", LogItemProvider.BRP, facility.Name, LogItemAction.UnbookStandby, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

                    return result;
                //}
                //else
                //{
                //    return new Result("Ett fel inträffade", false);
                //}
            }

            // Logg result 
            logger.Log("", LogItemProvider.BRP, facility.Name, LogItemAction.UnbookStandby, "", "User not logged in", LogItemType.Error);

            return new Result("", false);
        }

    #endregion
}