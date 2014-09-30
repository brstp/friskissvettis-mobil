using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoMA.Helpers;
using System.Xml;
using FriskisSvettisLib.PastellWebService;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using FriskisSvettisLib.PastellMobileService;
using FriskisSvettisLib.Helpers;
using FriskisSvettisLib;

/// <summary>
/// PastellService
/// 
/// Test
/// ---------------------------------------
/// Id1: 9823
/// Id2: 982311110000000001
/// LoginId: 9823
/// User: marcus
/// Pass: marcus
/// </summary>
public class PastellService : FriskisService, IFriskisService
{
    Logger logger = new Logger();

    #region Cache

        /// <summary>
        /// Updates each time a user logs in
        /// </summary>
        public List<ScheduleItem> CachedBookings
        {
            get
            {
                List<ScheduleItem> scheduleItems = null;

                try
                {
                    string name = "APPLICATION_BOOKINGS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    scheduleItems = (List<ScheduleItem>)HttpContext.Current.Session[name];

                    if (scheduleItems != null)
                    {
                        return scheduleItems;
                    }
                }
                catch { }

                // load from service if not found in application
                scheduleItems = GetBookings(FriskisService.LoggedInMember);

                CachedBookings = scheduleItems;

                return scheduleItems;
            }
            set
            {
                string name = "APPLICATION_BOOKINGS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Session[name] = value;
            }
        }

        /// <summary>
        /// Updates each time a user logs in
        /// </summary>
        public List<ScheduleItem> CachedStandbyBookings
        {
            get
            {
                List<ScheduleItem> scheduleItems = null;

                try
                {
                    string name = "APPLICATION_STANDBYBOOKINGS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    scheduleItems = (List<ScheduleItem>)HttpContext.Current.Session[name];

                    if (scheduleItems != null)
                    {
                        return scheduleItems;
                    }
                }
                catch { }

                // load from service if not found in application
                scheduleItems = GetScheduleStandyItems(FriskisService.LoggedInMember);

                CachedStandbyBookings = scheduleItems;

                return scheduleItems;
            }
            set
            {
                string name = "APPLICATION_STANDBYBOOKINGS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Session[name] = value;
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
                    string name = "APPLICATION_FACILITY_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    facility = (Facility)HttpContext.Current.Application[name];

                    if (facility != null)
                    {
                        return facility;
                    }
                }
                catch { }

                // load from service if not found in application
                facility = FriskisService.GetCurrentFacility();

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: CachedFacility, Xml: UsualAuthenticate");

                string response = Process(
                    "<ProfitAndroid command=\"UsualAuthenticate\">" +
                    "  <globalunit>" + LoginId + "</globalunit>" +
                    "  <type>GUEST</type>" +
                    "  <PartyName>moma</PartyName>" +
                    "  <Licenskey>" + ServiceLicense + "</Licenskey>" +
                    "</ProfitAndroid>"
                );

                // Facilities
                List<Facility> facilities = Facility.ParseMany(response, facility.Service);
                facility.Facilities = new List<Facility>();

                foreach (Facility innerFacility in facilities)
                {
                    innerFacility.Service = facility.Service;

                    // only add the visible facilities
                    if (innerFacility.ShowOnWeb)
                    {
                        facility.Facilities.Add(innerFacility);
                    }
                }

                CachedFacility = facility;

                return facility;

                //XmlDocument docResponse = new XmlDocument();
                //docResponse.LoadXml(response);

                //XmlNodeList activityNodes = docResponse.SelectNodes("//ProfitAndroid/activitylist/rootactivity");
                //foreach (XmlNode rootactivityNode in activityNodes)
                //{
                //    Activity rootactivity = new Activity();
                //    rootactivity.Id = XmlHelper.GetAttr<string>(rootactivityNode, "id", "");
                //    rootactivity.Name = XmlHelper.GetAttr<string>(rootactivityNode, "name", "");
                //    rootactivity.ChildActivites = new List<Activity>();

                //    activities.Add(rootactivity);

                //    // children
                //    XmlNodeList childactivityNodes = rootactivityNode.SelectNodes("childactivity");
                //    foreach (XmlNode childactivityNode in childactivityNodes)
                //    {
                //        Activity childactivity = new Activity();
                //        childactivity.Id = XmlHelper.GetAttr<string>(childactivityNode, "id", "");
                //        childactivity.Name = XmlHelper.GetAttr<string>(childactivityNode, "name", "");

                //        rootactivity.ChildActivites.Add(childactivity);
                //    }
                //}

                //// activities.AddRange(activities);

                //// cache data to be used when not logged in
                //CachedActivities = activities;

                //return activities;

                //return facility;
            }
            set
            {
                string name = "APPLICATION_FACILITY_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                List<Activity> activities = new List<Activity>();

                // try to get from cache
                try
                {
                    string name = "APPLICATION_ACTIVITIES_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    activities = (List<Activity>)HttpContext.Current.Application[name];

                    if (activities != null)
                    {
                        return activities;    
                    }
                }
                catch { }

                // load from service if not found in application
                XmlDocument doc = null;
                activities = new List<Activity>();

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: CachedActivities, Xml: UsualAuthenticate");

                string response = Process(
                    "<ProfitAndroid command=\"UsualAuthenticate\">" +
                    "  <globalunit>" + LoginId + "</globalunit>" +
                    "  <type>GUEST</type>" +
                    "  <PartyName>moma</PartyName>" +
                    "  <Licenskey>" + ServiceLicense + "</Licenskey>" +
                    "</ProfitAndroid>"
                );

                XmlDocument docResponse = new XmlDocument();

                try
                {
                    docResponse.LoadXml(response);

                    XmlNodeList activityNodes = docResponse.SelectNodes("//ProfitAndroid/activitylist/rootactivity");
                    foreach (XmlNode rootactivityNode in activityNodes)
                    {
                        Activity rootactivity = new Activity();
                        rootactivity.Id = XmlHelper.GetAttr<string>(rootactivityNode, "id", "");
                        rootactivity.Name = XmlHelper.GetAttr<string>(rootactivityNode, "name", "");
                        rootactivity.ChildActivites = new List<Activity>();

                        bool showOnWeb = XmlHelper.GetAttr<string>(rootactivityNode, "showonweb", "").Equals("1");

                        if (showOnWeb)
                        {
                            activities.Add(rootactivity);

                            // children
                            XmlNodeList childactivityNodes = rootactivityNode.SelectNodes("childactivity");
                            foreach (XmlNode childactivityNode in childactivityNodes)
                            {
                                Activity childactivity = new Activity();
                                childactivity.Id = XmlHelper.GetAttr<string>(childactivityNode, "id", "");
                                childactivity.Name = XmlHelper.GetAttr<string>(childactivityNode, "name", "");

                                rootactivity.ChildActivites.Add(childactivity);
                            }
                        }
                    }

                    // cache data to be used when not logged in
                    CachedActivities = activities;
                }
                catch
                {

                }

                return activities;
            }
            set
            {
                string name = "APPLICATION_ACTIVITIES_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public List<string> CachedRooms
        {
            get
            {
                string name = "APPLICATION_ROOMS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                    rooms = new List<string>(); // GetAllRooms();
                    CachedRooms = rooms;
                }

                return rooms;
            }
            set
            {
                string name = "APPLICATION_ROOMS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public List<Instructor> CachedInstructors
        {
            get
            {               
                
                List<Instructor> instructors = new List<Instructor>();

                try
                {
                    // try to find in application 
                    string name = "APPLICATION_INSTRUCTORS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    instructors = (List<Instructor>)HttpContext.Current.Application[name];

                    if (instructors != null)
                    {
                        return instructors;
                    }
                }
                catch {}

                // load from service if not found in application
                XmlDocument doc = null;
                string guid = GetGuestAuthKey(out doc);

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: CachedInstructors, Xml: FetchLeaders");

                string response = Process(
                    "<ProfitAndroid command=\"FetchLeaders\">" + 
                    "  <GUID>" + guid + "</GUID>" +
                    // "  <GLOBALUNITID>" + LoginId + "</GLOBALUNITID>" + 
                    "  <GLOBALUNITID>-1</GLOBALUNITID>" + // Get all instructors from all facilities
                    "</ProfitAndroid>"
                );

                XmlDocument docResponse = new XmlDocument();

                try
                {
                    docResponse.LoadXml(response);

                    instructors = new List<Instructor>();

                    XmlNodeList xmlNodeList = docResponse.SelectNodes("//ProfitAndroid/Leaders/Leader"); // [0];

                    foreach (XmlNode node in xmlNodeList)
                    {
                        Instructor instructor = new Instructor();
                        instructor.Id = node.SelectSingleNode("leaderid").InnerText;
                        instructor.Name = node.SelectSingleNode("leadername").InnerText;
                        // instructor.Firstname = node.SelectSingleNode("globalunitid").InnerText;

                        instructors.Add(instructor);
                    }

                    // cache result 
                    CachedInstructors = instructors;
                }
                catch
                {

                }

                // return found instructors
                return instructors;
            }
            set
            {
                string name = "APPLICATION_INSTRUCTORS_PASTELL_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

    #endregion

    #region Helpers

        public static Dictionary<string, object> DefaultValues
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {"ServiceUrl", "http://riks.pastell16.pastelldata.se/v3491/MobileServices.asmx"},
                    {"ServiceLicense", "Txx3453HgbPWW132"},
                    {"ServiceRevision", "0.3"},
                    {"ScheduleLoginNeeded", false},
                    {"LoginId", ""}
                };
            }
        }

        // used to connect to service api
        public string ServiceRevision { get; set; }
        public string ServiceUrl { get; set; }
        public string ServiceLicense { get; set; }

        /// <summary>
        /// Used when logging in, for example the id for gbg or malmö
        /// otherwise the facilityid should be used
        /// Göteborg: 1000
        /// Malmö   : 1392
        /// </summary>
        public string LoginId { get; set; }

        public FriskisServiceType ServiceType
        {
            get
            {
                return FriskisServiceType.PastellData;
            }
        }

        public bool ScheduleLoginNeeded { get; set; }

        // used to encrypt data from service
        public static int key = 129;
        public static string EncryptDecryptXOR(string textToEncrypt)
        {
            StringBuilder inSb = new StringBuilder(textToEncrypt);
            StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
            char c;
            for (int i = 0; i < textToEncrypt.Length; i++)
            {
                c = inSb[i];
                c = (char)(c ^ key);
                outSb.Append(c);
            }
            return outSb.ToString();
        }

        private MobileServicesSoapClient GetMobileServicesSoapClient()
        {
            BasicHttpBinding basicBinding = new BasicHttpBinding();
            basicBinding.MaxReceivedMessageSize = 5242880;

            CustomBinding binding = new CustomBinding(basicBinding);
            binding.Elements.Find<TextMessageEncodingBindingElement>().ReaderQuotas.MaxStringContentLength = 5242880;
            binding.Elements.Find<HttpTransportBindingElement>().MaxBufferSize = 5242880;

            // BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress(ServiceUrl);
            MobileServicesSoapClient service = new MobileServicesSoapClient(binding, address);

            return service;
        }

        private string Process(string xml)
        {
            MobileServices servHTTP = new MobileServices();
            servHTTP.Url = ServiceUrl;

            XmlDocument doc = new XmlDocument();
            doc.InnerXml = xml;

            string authenticateResultCoded = "";

            try
            {
                authenticateResultCoded = servHTTP.process(EncryptDecryptXOR(doc.InnerXml));
            }
            catch
            {
                // Todo: Add exception here? 
            }

            string authenticateResultDecoded = EncryptDecryptXOR(authenticateResultCoded);

            return authenticateResultDecoded;
        }

        public string GetGuestAuthKey()
        {
            XmlDocument doc = new XmlDocument();
            return GetGuestAuthKey(out doc);
        }

        public string GetGuestAuthKey(out XmlDocument doc)
        {
            // login as guest 

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: GetGuestAuthKey, Xml: UsualAuthenticate");

            string authXmlResponse = Process(
                "<ProfitAndroid command=\"UsualAuthenticate\">" +
                "  <globalunit>" + LoginId + "</globalunit>" +
                "  <type>GUEST</type>" +
                "  <PartyName></PartyName>" +
                "  <Licenskey></Licenskey>" +
                "</ProfitAndroid>");

            doc = new XmlDocument();

            string key = "";

            try
            {
                doc.LoadXml(authXmlResponse);
                key = doc.SelectSingleNode("//ProfitAndroid/GUID").InnerText;
            }
            catch
            {
               
            }

            return key;
        }

    #endregion

    /// <summary>
    /// Returns a member when the member was validated and 
    /// null when the login was incorrect.
    /// todo:   return error message via a "Result". 
    ///         <ProfitAndroid command="UsualAuthenticateResult" status="Din anläggning saknar denna WebApp modul" />
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Member Login(string username, string password, Facility facility)
    {
        MobileServicesSoapClient service = GetMobileServicesSoapClient();
        // string response = service.login(ServiceLicense, facility.Id, username, password, ServiceRevision);


        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: Login, WebService: login");

        string response = service.login(ServiceLicense, LoginId, username, password, ServiceRevision);

        XmlDocument xmlDocResponse = new XmlDocument();
        xmlDocResponse.LoadXml(response);

        Member member = new Member(response, this);

        if (string.IsNullOrEmpty(member.SessionId))
        {
            string message = xmlDocResponse.DocumentElement.Attributes["status"].Value;

            return null;
        }

        facility = CachedFacility;
        facility.Activities = CachedActivities;
        member.Facility = facility;

        //// Facilities
        //List<Facility> facilities = Facility.ParseMany(response, facility.Service);
        //facility.Facilities = new List<Facility>();

        //foreach (Facility innerFacility in facilities)
        //{
        //    innerFacility.Service = facility.Service;

        //    // only add the visible facilities
        //    if (innerFacility.ShowOnWeb)
        //    {
        //        facility.Facilities.Add(innerFacility);
        //    }
        //}

        //CachedFacility = facility;

        //// Activities
        //List<Activity> activities = new List<Activity>();

        //// roots
        //XmlNodeList activityNodes = xmlDocResponse.SelectNodes("//ProfitAndroid/activitylist/rootactivity");
        //foreach (XmlNode rootactivityNode in activityNodes)
        //{
        //    Activity rootactivity = new Activity();
        //    rootactivity.Id = XmlHelper.GetAttr<string>(rootactivityNode, "id", "");
        //    rootactivity.Name = XmlHelper.GetAttr<string>(rootactivityNode, "name", "");
        //    rootactivity.ChildActivites = new List<Activity>();

        //    activities.Add(rootactivity);

        //    // children
        //    XmlNodeList childactivityNodes = rootactivityNode.SelectNodes("childactivity");
        //    foreach (XmlNode childactivityNode in childactivityNodes)
        //    {
        //        Activity childactivity = new Activity();
        //        childactivity.Id = XmlHelper.GetAttr<string>(childactivityNode, "id", "");
        //        childactivity.Name = XmlHelper.GetAttr<string>(childactivityNode, "name", "");

        //        rootactivity.ChildActivites.Add(childactivity);
        //    }
        //}

        //facility.Activities = new List<Activity>();
        //facility.Activities.AddRange(activities);

        //// cache data to be used when not logged in
        //CachedActivities = facility.Activities;

        member.Username = username;
        member.Password = password;

        // reset bookings and standby in case a new user log in
        CachedBookings = null;
        CachedStandbyBookings = null;

        return member;
    }

    public List<Facility> GetAllFacilities()
    {
        return new List<Facility>();
    }

    /// <summary>
    /// TODO: DO!
    /// </summary>
    /// <param name="members"></param>
    /// <returns></returns>
    public List<ScheduleItem> GetBookings(Member member)
    {
        //<ProfitAndroid command="FetchBookingsResult">
        //  <AndroidBookingObjects>
        //    <Booking>
        //      <BOOKINGID>982311110000000248</BOOKINGID>
        //      <bookableobjectid>982311110000024223</bookableobjectid>
        //      <bookableobjectglobalunitid>9823</bookableobjectglobalunitid>
        //      <description>Spinning - Medel</description>
        //      <leader />
        //      <room />
        //      <date>2011-11-04 18:30:00</date>
        //      <enddate>2011-11-04 20:00:00</enddate>
        //    </Booking>
        //    <Booking>
        //      <BOOKINGID>982311110000000249</BOOKINGID>
        //      <bookableobjectid>982311110000024210</bookableobjectid>
        //      <bookableobjectglobalunitid>9823</bookableobjectglobalunitid>
        //      <description>Spinning - Core</description>
        //      <leader />
        //      <room />
        //      <date>2011-11-04 16:00:00</date>
        //      <enddate>2011-11-04 17:00:00</enddate>
        //    </Booking>
        //  </AndroidBookingObjects>
        //</ProfitAndroid>


        if (member != null)
        {
            MobileServicesSoapClient service = GetMobileServicesSoapClient();

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: GetBookings, WebService: fetchbookings");

            string response = service.fetchbookings(ServiceLicense, member.SessionId, member.Username, member.Password, DateTime.Now.ToShortDateString(), ServiceRevision);

            //string response2 = Process(
            //    "<ProfitAndroid command=\"FetchBookings\">" + 
            //        "<GUID>" + member.SessionId + "</GUID>" + 
            //        "<Date>[DATUM]</Date>" + 
            //        "<User>[Användare]</User>" +
            //        "<Password>[Lösenord]</Password>" + 
            //    "</ProfitAndroid>");

            List<ScheduleItem> items = ScheduleItem.ParseMany(response, DateTime.Now, this, ScheduleItemType.Booked);

            foreach (ScheduleItem item in items)
            {
                // Facility facility = Facility.Facilities.Where(f => f.Id.Equals(item.Where.Id) && member.Facility.Service.ServiceType == f.Service.ServiceType && member.Facility.Service.ServiceUrl.Equals(f.Service.ServiceUrl)).FirstOrDefault();
                // Facility facility = FacilityHelper.Get(item, member);
                Facility facility = member.Facility;
                Facility innerFacility = null;

                // get inner facility if there are many (like in Pastell)
                if (facility.Facilities != null && facility.Facilities.Count > 0)
                {
                    facility = facility.Facilities.Where(f => f.Id.Equals(item.Where.Id)).FirstOrDefault();

                    if (innerFacility == null)
                    {
                        // fallback
                        innerFacility = facility;
                    }
                }
                else
                {
                    innerFacility = facility;
                }

                if (facility != null)
                {
                    item.Where = innerFacility;
                }
            }

            return items;
        }

        return new List<ScheduleItem>();
    }

    /// <summary>
    /// TODO: DO!
    /// </summary>
    /// <param name="activityId"></param>
    /// <param name="facility">ISN'T USED?</param>
    /// <returns></returns>
    public Result Book(string activityId, Facility facility, DateTime dateTime)
    {
        Member member = FriskisService.LoggedInMember;
        Result result = new Result();

        if (member != null)
        {
            MobileServicesSoapClient service = GetMobileServicesSoapClient();

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: Book, WebService: book");

            string response = service.book(ServiceLicense, member.SessionId, member.Username, member.Password, activityId, ServiceRevision);

            XmlDocument xmlDocResponse = new XmlDocument();
            xmlDocResponse.LoadXml(response);

            result.Message = xmlDocResponse.SelectSingleNode("//ProfitAndroid/status").InnerText;
            result.Success = result.Message.Trim().Equals("OK");

            if (result.Success)
            {
                CachedBookings = null;
            }

            // Logg result 
            logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.Book, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

            return result;
        }

        result.Success = false;
        result.Message = "Ej inloggad.";

        logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.Book, "", "Ej inloggad.", result.Success ? LogItemType.Information : LogItemType.Error);

        return result;
    }

    /// <summary>
    /// TODO: DO!
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="facility">ISN'T USED?</param>
    /// <returns></returns>
    public Result Unbook(string id, string bookId, Facility facility, DateTime dateTime)
    {
        Member member = FriskisService.LoggedInMember;
        Result result = new Result();

        if (member != null)
        {
            MobileServicesSoapClient service = GetMobileServicesSoapClient();

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: Unbook, WebService: debook");

            string response = service.debook(ServiceLicense, member.SessionId, member.Username, member.Password, bookId, ServiceRevision);

            XmlDocument xmlDocResponse = new XmlDocument();
            xmlDocResponse.LoadXml(response);

            result.Message = xmlDocResponse.SelectSingleNode("//ProfitAndroid/status").InnerText;
            result.Success = result.Message.Trim().Equals("Din avbokning är registrerad");

            if (result.Success)
            {
                CachedBookings = null;
            }            
            
            // Logg result 
            logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.Unbook, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);


            return result;
        }

        result.Success = false;
        result.Message = "Ej inloggad.";

        // Logg result 
        logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.Unbook, "", "Ej inloggad.", result.Success ? LogItemType.Information : LogItemType.Error);

        return result;
    }

    public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activity, string activityType, string instructor, DateTime From, DateTime To)
    {
        // filter parameters 
        activity = string.IsNullOrEmpty(activity) ? "-1" : activity;
        activityType = string.IsNullOrEmpty(activityType) ? "-1" : activityType;
        instructor = string.IsNullOrEmpty(instructor) ? "" : instructor;
        string facility = facilities.Count() == 1 ? facilities.FirstOrDefault().Id : "-1";

        // get session id
        string sessionId = "";
        if (member != null)
        {
            sessionId = member.SessionId;
        }
        else
        {
            sessionId = GetGuestAuthKey();
        }

        // get service response

        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: GetScheduleItems, Xml: FetchBookableObjectsFiltered");

        string response = Process(
            "<ProfitAndroid command=\"FetchBookableObjectsFiltered\">" +
            "  <GUID>" + sessionId + "</GUID>" +
            "  <START>" + From + "</START>" +
            "  <END>" + To + "</END>" +
            "  <GLOBALUNITID>" + facility + "</GLOBALUNITID>" +
            "  <ACTIVITYID>" + activity + "</ACTIVITYID>" +
            "  <ACTIVITYCATEGORYID>" + activityType + "</ACTIVITYCATEGORYID>" +
            "<LEADERFREETEXT>" + instructor + "</LEADERFREETEXT>" +
            "</ProfitAndroid>"
            );

        List<ScheduleItem> items = ScheduleItem.ParseManyFromPastellData(response, From, ScheduleItemType.Normal);

        // set correct facility to items based on facility id
        foreach (ScheduleItem item in items)
        {
            item.Where = CachedFacility.GetInnerFacility(item.Where.Id2);
        }

        // set if schedule item is booked by user or not
        if (items.Count > 0)
        {
            List<ScheduleItem> bookedItems = CachedBookings; // GetBookings(member);
            List<ScheduleItem> bookedStandbyItems = CachedStandbyBookings; // GetScheduleStandyItems(member);

            foreach (ScheduleItem item in items)
            {
                ScheduleItem bookedItem = bookedItems.Where(b => b.Id == item.Id).FirstOrDefault();
                ScheduleItem bookedStandbyItem = bookedStandbyItems.Where(b => b.Id == item.Id).FirstOrDefault();

                if (bookedItem != null)
                {
                    item.BookId = bookedItem.BookId;
                }

                if (bookedStandbyItem != null)
                {
                    item.BookId = bookedStandbyItem.BookId;
                }
            }
        }

        return items;
    }

    /// <summary>
    /// Including standby items
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bookId"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    public ScheduleItem GetBookedItem(string id, string bookId, Facility facility)
    {
        List<ScheduleItem> items = GetBookings(FriskisService.LoggedInMember);
        ScheduleItem item = items.Where(i => i.Id.Equals(id)).FirstOrDefault();

        if (item == null)
        {
            items = GetScheduleStandyItems(FriskisService.LoggedInMember);
            item = items.Where(i => i.Id.Equals(id)).FirstOrDefault();
        }

        return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    public ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime)
    {
        Member member = FriskisService.LoggedInMember;

        if (member != null)
        {
            string response = "";
            XmlDocument doc = null;


            //// Doesn't work??? Is this true?

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: GetScheduleItem, Xml: FetchBookableObject");

            response = Process(
                "<ProfitAndroid command=\"FetchBookableObject\">" +
                "  <GUID>" + member.SessionId + "</GUID>" +
                "  <BOID>" + id + "</BOID>" +
                "</ProfitAndroid>");

            ScheduleItem item = ScheduleItem.ParseManyFromPastellData(response, dateTime, ScheduleItemType.Normal).FirstOrDefault();

            return item;
        }

        return null;
    }

    #region Standby

        public List<ScheduleItem> GetScheduleStandyItems(Member member)
        {
            //<ProfitAndroid command=" FetchReserveBookingsResult ">
            //  <AndroidBookingObjects>
            //    <Booking>
            //      <RESERVEBOOKINGID>[ReservBokningsID]</RESERVEBOOKINGID>
            //      <POSITION>Akutell plats i kön</POSITION>
            //      <bookableobjectid>[PassID]</bookableobjectid>
            //      <bookableobjectglobalunitid>[Globalt AnläggningsID]</bookableobjectglobalunitid>
            //      <description>[Beskrivning]</description>
            //      <leader />
            //      <room />
            //      <date>[Starttid]</date>
            //      <enddate>[Sluttid]</enddate>
            //    </Booking>
            //    <Booking>
            //    </Booking>
            //  </AndroidBookingObjects>
            //</ProfitAndroid>

            if (member != null)
            {
                MobileServicesSoapClient service = GetMobileServicesSoapClient();

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: GetScheduleStandyItems, WebService: fetchReserveBookings");

                string response = service.fetchReserveBookings(ServiceLicense, member.SessionId, member.Username, member.Password, DateTime.Now.ToShortDateString(), ServiceRevision);

                List<ScheduleItem> items = ScheduleItem.ParseMany(response, DateTime.Now, this, ScheduleItemType.BookedStandby);

                foreach (ScheduleItem item in items)
                {
                    Facility facility = member.Facility;
                    Facility innerFacility = null;

                    // get inner facility if there are many (like in Pastell)
                    if (facility.Facilities != null && facility.Facilities.Count > 0)
                    {
                        facility = facility.Facilities.Where(f => f.Id.Equals(item.Where.Id)).FirstOrDefault();

                        if (innerFacility == null)
                        {
                            // fallback
                            innerFacility = facility;
                        }
                    }
                    else
                    {
                        innerFacility = facility;
                    }

                    if (facility != null)
                    {
                        item.Where = innerFacility;
                    }
                }

                return items;
            }

            return new List<ScheduleItem>();
        }

        public Result BookStandby(string activityId, Facility facility, DateTime dateTime)
        {
            Member member = FriskisService.LoggedInMember;
            Result result = new Result();

            if (member != null)
            {
                MobileServicesSoapClient service = GetMobileServicesSoapClient();

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: BookStandby, WebService: bookReserve");

                string response = service.bookReserve(ServiceLicense, member.SessionId, member.Username, member.Password, activityId, ServiceRevision);

                XmlDocument xmlDocResponse = new XmlDocument();
                xmlDocResponse.LoadXml(response);

                result.Message = xmlDocResponse.SelectSingleNode("//ProfitAndroid/status").InnerText;
                result.Success = result.Message.Trim().Equals("OK");

                if (result.Success)
                {
                    CachedStandbyBookings = null;
                }

                // Logg result 
                logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.BookStandby, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

                return result;
            }

            result.Success = false;
            result.Message = "Ej inloggad.";

            logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.BookStandby, "", "Ej inloggad.", result.Success ? LogItemType.Information : LogItemType.Error);

            return result;
        }

        /// <summary>
        /// TODO: DO!
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="facility">ISN'T USED?</param>
        /// <returns></returns>
        public Result UnbookStandby(string bookId, Facility facility)
        {
            Member member = FriskisService.LoggedInMember;
            Result result = new Result();

            if (member != null)
            {
                MobileServicesSoapClient service = GetMobileServicesSoapClient();

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Pastell", "C#: UnbookStandby, WebService: debookReserve");

                string response = service.debookReserve(ServiceLicense, member.SessionId, member.Username, member.Password, bookId, ServiceRevision);

                XmlDocument xmlDocResponse = new XmlDocument();
                xmlDocResponse.LoadXml(response);

                result.Message = xmlDocResponse.SelectSingleNode("//ProfitAndroid/status").InnerText;
                result.Success = result.Message.Trim().Equals("OK");

                if (result.Success)
                {
                    CachedStandbyBookings = null;
                }

                // Logg result 
                logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.UnbookStandby, member.Username, response, result.Success ? LogItemType.Information : LogItemType.Error);

                return result;
            }

            result.Success = false;
            result.Message = "Ej inloggad.";

            // Logg result 
            logger.Log("", LogItemProvider.Pastell, facility.Name, LogItemAction.UnbookStandby, "", "Ej inloggad.", result.Success ? LogItemType.Information : LogItemType.Error);

            return result;
        }

    #endregion
}