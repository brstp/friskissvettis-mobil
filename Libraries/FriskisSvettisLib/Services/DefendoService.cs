using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoMA.Helpers;
using System.Xml;
using FriskisSvettisLib.DefendoWebService;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using FriskisSvettisLib.Helpers;
using System.Diagnostics;
using FriskisSvettisLib;

/// <summary>
/// DefendoService
/// 
/// Test/demo
/// -------------------------------
/// 
/// Id1: 19eef68c-d8d0-11d4-b4ba-0020af85698d
/// User: 9999-000129
/// Password: PIkaAs1R
/// ServiceLicense: 956855897
/// ServiceContextId: 48190819-E74C-48AC-8F50-A02ADF235E4B
/// ScheduleLogin: True
/// 
/// </summary>
public class DefendoService : FriskisService, IFriskisService
{
    Logger logger = new Logger();

    #region Cache

        public List<string> CachedRooms
        {
            get
            {
                string name = "APPLICATION_ROOMS_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                string name = "APPLICATION_ROOMS_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                    // try to find in application 
                    string name = "APPLICATION_FACILITY_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    facility = (Facility)HttpContext.Current.Application[name];

                    if (facility != null)
                    {
                        return facility;
                    }
                }
                catch { }

                // load facilities 
                facility = FriskisService.GetCurrentFacility();
                facility.Facilities = new List<Facility>();
                BookingServiceClient service = GetBookingServiceClient();
                Facility currentFacility = FriskisService.GetCurrentFacility();
                Session session = GetSession(new Guid(currentFacility.Id));

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: CachedFacility, WebService: GetLocations");
                GetLocationsResult getLocationsResult = service.GetLocations(session);

                foreach (FriskisSvettisLib.DefendoWebService.Location defendoLocation in getLocationsResult.Locations)
                {
                    Facility loopFacility = new Facility();
                    loopFacility.originalObject = defendoLocation;
                    loopFacility.Id = defendoLocation.Id.ToString(); // how can this have multiple ids? 
                    loopFacility.Name = defendoLocation.Name;

                    facility.Facilities.Add(loopFacility);
                }

                // cache result 
                CachedFacility = facility;

                // return found facilities
                return facility;
            }
            set
            {
                string name = "APPLICATION_FACILITY_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                List<Activity> activities = null;

                try
                {
                    // try to find in application 
                    string name = "APPLICATION_ACTIVITIES_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    activities = (List<Activity>)HttpContext.Current.Application[name];

                    if (activities != null)
                    {
                        return activities;
                    }
                }
                catch { }

                // load activities 
                activities = new List<Activity>();
                BookingServiceClient service = GetBookingServiceClient();
                Facility facility = FriskisService.GetCurrentFacility();
                Session session = GetSession(new Guid(facility.Id));

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: CachedActivities, WebService: GetActivityFormsAndTypes");
                GetActivityFormsAndTypesResult getActivityFormsAndTypesResult = service.GetActivityFormsAndTypes(session);

                foreach (FriskisSvettisLib.DefendoWebService.ActivityForm defendoActivityForm in getActivityFormsAndTypesResult.ActivityForms)
                {
                    Activity activity = new Activity();
                    activity.Id = defendoActivityForm.Id.ToString(); // how can this have multiple ids? 
                    activity.originalObject = defendoActivityForm;
                    activity.Name = defendoActivityForm.Name;
                    activities.Add(activity);

                    if (defendoActivityForm.ActivityTypes != null && defendoActivityForm.ActivityTypes.Count() > 0)
                    {

                    }
                    activity.ChildActivites = new List<Activity>();

                    foreach (FriskisSvettisLib.DefendoWebService.ActivityType defendoActivityType in defendoActivityForm.ActivityTypes)
                    {
                        Activity activityChild = new Activity();
                        activityChild.Id = defendoActivityType.Id.ToString(); // how can this have multiple ids? 
                        activityChild.originalObject = defendoActivityType;
                        activityChild.Name = defendoActivityType.Name;
                        activity.ChildActivites.Add(activityChild);
                    }
                }

                // cache result 
                CachedActivities = activities;

                // return found instructors
                return activities;
            }
            set
            {
                string name = "APPLICATION_ACTIVITIES_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public List<Instructor> CachedInstructors
        {
            get
            {
                List<Instructor> instructors = null;

                try
                {
                    // try to find in application 
                    string name = "APPLICATION_INSTRUCTORS_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    instructors = (List<Instructor>)HttpContext.Current.Application[name];

                    if (instructors != null)
                    {
                        return instructors;
                    }
                }
                catch { }

                // load instructors 
                instructors = new List<Instructor>();
                BookingServiceClient service = GetBookingServiceClient();
                Facility facility = FriskisService.GetCurrentFacility();
                Session session = GetSession(new Guid(facility.Id));

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: CachedInstructors, WebService: GetInstructors");
                GetInstructorsResult getInstructorsResult = service.GetInstructors(session);

                foreach (FriskisSvettisLib.DefendoWebService.Instructor defendoInstructor in getInstructorsResult.Instructors)
                {
                    Instructor instructor = new Instructor();
                    instructor.Id = defendoInstructor.Id.ToString();
                    instructor.originalObject = defendoInstructor;
                    instructor.Name = defendoInstructor.Name;

                    instructors.Add(instructor);
                }

                // cache result 
                CachedInstructors = instructors;

                // return found instructors
                return instructors;
            }
            set
            {
                string name = "APPLICATION_INSTRUCTORS_DEFENDO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                    {"ServiceUrl", "https://services.jympa.nu/Moma/Jympa.BookingService.BookingService.svc/BasicHttp"},
                    {"ServiceContextId","48190819-E74C-48AC-8F50-A02ADF235E4B"},
                    {"ServiceLicense", "956855897"},
                    {"ScheduleLoginNeeded", false},
                };
            }
        }

        // used to connect to service api
        public string ServiceUrl { get; set; }
        public string ServiceContextId { get; set; }
        public string ServiceLicense { get; set; }

        public FriskisServiceType ServiceType
        {
            get
            {
                return FriskisServiceType.Defendo;
            }
        }

        public bool ScheduleLoginNeeded { get; set; }

        private BookingServiceClient GetBookingServiceClient()
        {
            BasicHttpBinding basicBinding = new BasicHttpBinding();
            basicBinding.MaxReceivedMessageSize = 5242880;

            if (ServiceUrl.Trim().StartsWith("https"))
            {
                basicBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            CustomBinding binding = new CustomBinding(basicBinding);
            binding.Elements.Find<TextMessageEncodingBindingElement>().ReaderQuotas.MaxStringContentLength = 5242880;
            binding.Elements.Find<HttpTransportBindingElement>().MaxBufferSize = 5242880;

            EndpointAddress address = new EndpointAddress(ServiceUrl);
            BookingServiceClient service = new BookingServiceClient(binding, address);


            return service;
        }

        private Session GetSession(Guid organisationId)
        {
            if (FriskisService.LoggedInMember != null && FriskisService.LoggedInMember.DefendoSession != null)
            {
                return FriskisService.LoggedInMember.DefendoSession;
            }

            BookingServiceClient service = GetBookingServiceClient();

            string language = "se";

            switch (FriskisService.GetDomainLanguage())
            {
                case "sv": language = "se"; break;
                case "no": language = "no"; break;
            }

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetSession, WebService: GetOrganisations");
            GetOrganisationsResult organisations = service.GetOrganisations(language, new Guid(ServiceContextId));

            Organisation organisation = organisations.Organisations.Where(o => o.Id == organisationId).FirstOrDefault();

            //foreach (Organisation org in organisations.Organisations)
            //{
            //    Debug.WriteLine(org.Name + " : " + org.Id.ToString());
            //}

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetSession, WebService: StartSession");
            StartSessionResult startSessionResult = service.StartSession(ServiceLicense, organisation);
            if (organisation == null || startSessionResult.ErrorLevel != ErrorLevel.None)
            {
                // An error occured
                return null;
            }

            Session session = startSessionResult.Session;

            return session;
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
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));

        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: Login, WebService: LoginUser");
        LoginUserResult response = service.LoginUser(session, username, password);

        if (response.ErrorLevel != ErrorLevel.None)
        {
                // Ett fel har inträffat avbryt!
                return null;
        }
        GetPersonInfoResult getPersonInfoResult = service.GetPersonInfo(session, response.Token);

        Member member = new Member();

        member.Firstname = getPersonInfoResult.Person.Firstname;
        member.Lastname = getPersonInfoResult.Person.Lastname;
        member.Email = getPersonInfoResult.Person.PrimaryEmail;
        // member.... // todo: add all properties here

        member.Username = username;
        member.Password = password;

        member.Token = response.Token;
        member.DefendoSession = session;

        return member;
    }

    public List<Facility> GetAllFacilities()
    {
        return new List<Facility>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="members"></param>
    /// <returns></returns>
    public List<ScheduleItem> GetBookings(Member member)
    {
        List<ScheduleItem> bookedItems = new List<ScheduleItem>();

        try
        {
            Facility facility = FriskisService.GetCurrentFacility();
            BookingServiceClient service = GetBookingServiceClient();
            Session session = GetSession(new Guid(facility.Id));

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetBookings, WebService: GetBookingData");
            GetBookingDataResult getBookingData = service.GetBookingData(session, FriskisService.LoggedInMember.Token);
            if (getBookingData.ErrorLevel != ErrorLevel.None)
            {
                // An error occured
                return bookedItems;
            }

            foreach (BookedDatedActivity bookedDatedActivity in getBookingData.BookedActivites)
            {
                // create schedule item
                bookedItems.Add(new ScheduleItem()
                {
                    Id = bookedDatedActivity.Id.ToString(),
                    // BookId = bookedDatedActivity.Id.ToString(),
                    BookId = bookedDatedActivity.BookingId.ToString(),

                    Where = new Facility()
                    {
                        Id = facility.Id
                    },

                    Available = bookedDatedActivity.MaxBookable - bookedDatedActivity.CurrentBooked,
                    BookableFrom = bookedDatedActivity.BookingStarts.HasValue ? bookedDatedActivity.BookingStarts.Value : DateTime.Now.AddDays(-10),
                    BookableTo = bookedDatedActivity.BookingStops.HasValue ? bookedDatedActivity.BookingStops.Value : DateTime.Now.AddDays(10),
                    Dropin = bookedDatedActivity.IsDropIn,
                    OriginalItem = bookedDatedActivity,
                    Name = bookedDatedActivity.Name,
                    Description = bookedDatedActivity.Description.Plain,
                    Room = bookedDatedActivity.Location.Plain,
                    Total = bookedDatedActivity.MaxBookable,
                    Level = WorkoutLevel.None,
                    Instructor = bookedDatedActivity.Instructors.Plain,
                    Cancelled = bookedDatedActivity.IsCancelled, 
                    
                    From = bookedDatedActivity.Starts,
                    To = bookedDatedActivity.Ends
                });
            }
        }
        catch (Exception ex)
        {
            // An error occured
            return bookedItems;
        }

        return bookedItems;
    }

    /// <summary>
    /// TODO: DO!
    /// </summary>
    /// <param name="activityId"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    public Result Book(string activityId, Facility facility, DateTime dateTime)
    {
        Member member = FriskisService.LoggedInMember;
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));
        Result result = new Result()
        {
            Success = false
        };

        BookableDatedActivity bookableDatedActivity = GetBookableItem(activityId, facility);

        if (bookableDatedActivity != null)
        {
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: Book, WebService: Book");
            BookResult bookResult = service.Book(session, member.Token, bookableDatedActivity);

            result.Success = bookResult.ErrorLevel == ErrorLevel.None;

            if (result.Success == false)
            {
                result.Message = bookResult.ErrorMessage;
            }

            //if (bookResult.ErrorLevel != ErrorLevel.None)
            //{
            //    result.Success = true;
            //}
            //else
            //{
            //    result.Message = bookResult.ErrorMessage;
            //}
        }
        else
        {
            result.Success = false;
            result.Message = "Error";
        }

        // Logg result 
        if (string.IsNullOrEmpty(result.Message))
        {
            result.Message = "";
        }

        logger.Log("", LogItemProvider.Defendo, facility.Name, LogItemAction.Book, member.Username, result.Message, result.Success ? LogItemType.Information : LogItemType.Error);

        return result;
    }

    /// <summary>
    /// TODO: DO!
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    public Result Unbook(string id, string bookingId, Facility facility, DateTime dateTime)
    {
        Member member = FriskisService.LoggedInMember;
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));
        Result result = new Result()
        {
            Success = false
        };

        BookedDatedActivity bookableDatedActivity = GetBookedItemRaw(id, bookingId, facility);

        if (bookableDatedActivity != null)
        {
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: Unbook, WebService: CancelBooking");
            CancelBookingResult bookResult = service.CancelBooking(session, member.Token, bookableDatedActivity);

            result.Success = bookResult.ErrorLevel == ErrorLevel.None;

            if (result.Success == false)
            {
                result.Message = bookResult.ErrorMessage;
            }
        }

        // Logg result 
        if (string.IsNullOrEmpty(result.Message))
        {
            result.Message = "";
        }

        // Logg result 
        logger.Log("", LogItemProvider.Defendo, facility.Name, LogItemAction.Unbook, member.Username, result.Message, result.Success ? LogItemType.Information : LogItemType.Error);

        return result;
    }

    public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activityId, string activityTypeId, string instructorId, DateTime From, DateTime To)
    {
        List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

        try
        {
            BookingServiceClient service = GetBookingServiceClient();
            Session session = GetSession(new Guid(mainFacility.Id));

            // get filter items 
            string locationId = facilities.Count() == 1 ? facilities.FirstOrDefault().Id : "";
            Facility loc = CachedFacility.Facilities.Where(i => i.Id.Equals(locationId)).FirstOrDefault();
            Location locationObject = null;
            if (loc != null)
            {
                locationObject = (Location)loc.originalObject;
            }

            Activity form = CachedActivities.Where(i => i.Id.Equals(activityId)).FirstOrDefault();
            ActivityForm activityFormObject = null;
            ActivityType activityTypeObject = null;
            if (form != null) 
            {
                activityFormObject = (FriskisSvettisLib.DefendoWebService.ActivityForm)form.originalObject;

                if (!string.IsNullOrEmpty(activityTypeId))
                {
                    activityTypeObject = activityFormObject.ActivityTypes.Where(i => i.Id.Equals(new Guid(activityTypeId))).FirstOrDefault();
                }
            }

            Instructor inst = CachedInstructors.Where(i => i.Name.Equals(instructorId)).FirstOrDefault();
            FriskisSvettisLib.DefendoWebService.Instructor instructorObject = null;
            if (inst != null) 
            {
                instructorObject = (FriskisSvettisLib.DefendoWebService.Instructor)inst.originalObject;
            }

            // get schedule for current timespan
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetScheduleItems, WebService: GetSchemaWithFilter");
            GetSchemaWithFilterResult getSchemaResult = service.GetSchemaWithFilter(session, From.Date, To.Date, locationObject, activityFormObject, activityTypeObject, instructorObject);
            if (getSchemaResult.ErrorLevel != ErrorLevel.None)
            {
                // An error occured
                return scheduleItems;
            }

            // get all items
            GetBookingDataResult bookItems = GetBookItems(mainFacility);

            foreach (DatedActivity datedActivity in getSchemaResult.DatedActivities)
            {
                // check if item is bookable
                BookableDatedActivity bookableItem = null;
                if (bookItems.BookableActivites != null)
                {
                    bookableItem = bookItems.BookableActivites.Where(b => b.Id.Equals(datedActivity.Id)).FirstOrDefault();
                }

                // check if item is booked
                BookedDatedActivity bookedItem = null;
                if (bookItems.BookedActivites != null)
                {
                    bookedItem = bookItems.BookedActivites.Where(b => b.Id.Equals(datedActivity.Id)).FirstOrDefault();
                }

                ScheduleItem scheduleItem = ConvertToScheduleItem(datedActivity, bookableItem, bookedItem);
                scheduleItems.Add(scheduleItem);
            }
        }
        catch (Exception ex)
        {
            // An error occured
            return scheduleItems;
        }

        return scheduleItems;
    }

    public ScheduleItem ConvertToScheduleItem(DatedActivity datedActivity, BookableDatedActivity bookableItem, BookedDatedActivity bookedItem)
    {
        // default values
        int available = 0;
        string bookId = "";
        bool bookable = false;
        bool found = false;

        if (datedActivity.Name.Contains("Spin bas"))
        {
        }

        // check if item is bookable
        if (bookableItem != null)
        {
            available = datedActivity.MaxBookable - bookableItem.CurrentBooked;
            bookId = "";
            bookable = true;
            found = true;
        }

        // check if item is booked
        if (bookedItem != null)
        {
            available = datedActivity.MaxBookable - datedActivity.CurrentBooked;
            bookId = bookedItem.Id.ToString();
            bookable = false;
            found = true;
        }

        string[] locationArray = datedActivity.Location.Plain.Split("\\\\".ToCharArray());

        string facilityName = locationArray.First();
        string local = "";

        if (locationArray.Count() > 1)
        {
            local = locationArray[1];
        }

        // create sheduleitem 
        ScheduleItem scheduleItem = new ScheduleItem()
        {
            Id = datedActivity.Id.ToString(),
            Where = new Facility()
            {
                Name = facilityName
            },
            OriginalItem = datedActivity,
            Bookable = bookable,
            BookId = bookId,
            Name = datedActivity.Name,
            Description = datedActivity.Description.Plain,
            Room = local,
            Available = available,
            Total = datedActivity.MaxBookable,
            Level = WorkoutLevel.None,
            Cancelled = datedActivity.IsCancelled,
            From = datedActivity.Starts,
            To = datedActivity.Ends,
            Dropin = datedActivity.IsDropIn,
            Instructor = datedActivity.Instructors.Plain,
            AvailableToUser = found // Not available if not found any BookableActivity or BookedActivity was found 
        };

        // since there are no available spots, you can't book it
        // you could still dropin if thats allowed
        scheduleItem.DropinAvailable = datedActivity.MaxParticipants - datedActivity.MaxBookable;
        if (scheduleItem.Available == 0)
        {
            scheduleItem.Bookable = false;
        }

        // Dropin (calculate spots for dropin)
        if (datedActivity.IsDropIn)
        {
            scheduleItem.DropinAvailable = datedActivity.MaxParticipants - datedActivity.MaxBookable;
        }


        // hide if not booked or bookable
        scheduleItem.Visible = (bookableItem != null || bookedItem != null) || !FriskisService.IsLoggedIn;

        return scheduleItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    //public ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime)
    //{
    //    Member member = FriskisService.LoggedInMember;
    //    List<ScheduleItem> items = GetScheduleItems(facility, member, "", null, DateTime.Today, DateTime.Today.AddDays(7));
    //    ScheduleItem item = items.Where(it => it.Id.Equals(id)).FirstOrDefault();

    //    return item;
    //}

    public ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime)
    {
        Member member = FriskisService.LoggedInMember;
        ScheduleItem item = null;
        LoginToken loginToken = null;

        if (member != null)
        {
            loginToken = member.Token;
        }

        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));

        // get schedule for current timespan
        OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetScheduleItem, WebService: GetDatedActivityById");
        GetDatedActivityByIdResult getDatedActivityByIdResult = service.GetDatedActivityById(session, member.Token, new Guid(id));

        return ConvertToScheduleItem(getDatedActivityByIdResult.DatedActivity, getDatedActivityByIdResult.BookableActivity, getDatedActivityByIdResult.BookedActivity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    private GetBookingDataResult GetBookItems(Facility facility)
    {
        Member member = FriskisService.LoggedInMember;
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));

        if (member != null)
        {
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetBookItems, WebService: GetBookingData");
            GetBookingDataResult bookingDataResult = service.GetBookingData(session, member.Token);
            return bookingDataResult;
        }

        return new GetBookingDataResult();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    private BookableDatedActivity GetBookableItem(string id, Facility facility)
    {
        //List<BookableDatedActivity> items = GetBookItems(facility).BookableActivites.ToList();
        //BookableDatedActivity item = items.Where(i => i.Id == new Guid(id)).FirstOrDefault();

        //return item;

        // Nedan verkar inte fungera? 

        Member member = FriskisService.LoggedInMember;
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));
        BookableDatedActivity item = null;

        if (member != null)
        {
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetBookableItem, WebService: GetDatedActivityById");
            GetDatedActivityByIdResult getDatedActivityByIdResult = service.GetDatedActivityById(session, member.Token, new Guid(id));

            if (getDatedActivityByIdResult != null && getDatedActivityByIdResult.BookableActivity != null)
            {
                item = getDatedActivityByIdResult.BookableActivity;
            }
        }

        return item;
    }

    public ScheduleItem GetBookedItem(string id, string bookId, Facility facility)
    {
        Member member = FriskisService.LoggedInMember;
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));

        if (member != null)
        {
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetBookedItem, WebService: GetDatedActivityById");
            GetDatedActivityByIdResult getDatedActivityByIdResult = service.GetDatedActivityById(session, member.Token, new Guid(id));

            if (getDatedActivityByIdResult != null && getDatedActivityByIdResult.BookedActivity != null)
            {
                try
                {
                    getDatedActivityByIdResult.BookedActivity.BookingId = new Guid(bookId);
                }
                catch { /* sometimes the guid is not sent */ }

                return ConvertToScheduleItem(getDatedActivityByIdResult.DatedActivity, getDatedActivityByIdResult.BookableActivity, getDatedActivityByIdResult.BookedActivity);
            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="facility"></param>
    /// <returns></returns>
    private BookedDatedActivity GetBookedItemRaw(string id, string bookId, Facility facility)
    {
        // List<BookedDatedActivity> items = GetBookItems(facility).BookedActivites.ToList();
        // BookedDatedActivity item = items.Where(i => i.Id == new Guid(id)).FirstOrDefault();

        // return item;

        Member member = FriskisService.LoggedInMember;
        BookingServiceClient service = GetBookingServiceClient();
        Session session = GetSession(new Guid(facility.Id));
        BookedDatedActivity item = null;

        if (member != null)
        {
            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "Defendo", "C#: GetBookedItemRaw, WebService: GetDatedActivityById");
            GetDatedActivityByIdResult getDatedActivityByIdResult = service.GetDatedActivityById(session, member.Token, new Guid(id));

            if (getDatedActivityByIdResult != null && getDatedActivityByIdResult.BookedActivity != null)
            {
                item = getDatedActivityByIdResult.BookedActivity;
                item.BookingId = new Guid(bookId);
            }
        }

        return item;
    }

    #region Standby

        public List<ScheduleItem> GetScheduleStandyItems(Member member)
        {
            return new List<ScheduleItem>();
        }

        public Result BookStandby(string activityId, Facility facility, DateTime dateTime)
        {
            return new Result();
        }

        public Result UnbookStandby(string bookId, Facility facility)
        {
            return new Result();
        }

    #endregion

}