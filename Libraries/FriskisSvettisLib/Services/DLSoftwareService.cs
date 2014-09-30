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
using FriskisSvettisLib.DLService;
using FriskisSvettisLib.Helpers;
using FriskisSvettisLib;

/// <summary>
/// PastellService
/// WSDL: http://testweb2.dlsoftware.com/dlp2wsr_test/dlp2wsr_ext.dll/wsdl/IDlp2wsRR_Ext
/// 
/// T = True
/// F = False
/// Q = Queue
/// 
/// TODO
/// -------------------
/// 
/// - Kolla mot om man får Q som svar 
/// - Då man visar bokningar och schema inte är visat så kommer inte något vara cachat och man får inte fram passet. 
/// 
/// </summary>
public class DLSoftwareService : FriskisService, IFriskisService
{
    Logger logger = new Logger();

    public IDlp2wsRR_Extservice GetService()
    {
        // get service
        IDlp2wsRR_Extservice service = new IDlp2wsRR_Extservice();
        service.Url = ServiceUrl;
        return service;
    }

    #region Cache

        #region Cached ScheduleItems 

            /// <summary>
            /// Should be replaced when GetById is implemented at DL Software.
            /// Should only be used for testing. NOT LIVE!
            /// </summary>
            private List<ScheduleItem> CachedScheduleItems
            {
                get
                {
                    List<ScheduleItem> scheduleItems = null;

                    try
                    {
                        // try to find in application 
                        string name = "APPLICATION_CURRENT_SCHEDULEITEMS_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                        scheduleItems = (List<ScheduleItem>)HttpContext.Current.Application[name];
                    }
                    catch { }

                    if (scheduleItems == null)
                    {
                        scheduleItems = new List<ScheduleItem>();
                    }

                    return scheduleItems;
                }
                set
                {
                    string name = "APPLICATION_CURRENT_SCHEDULEITEMS_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    HttpContext.Current.Application[name] = value;
                }
            }

            /// <summary>
            /// Adds or updates schedule items to the cache
            /// </summary>
            /// <param name="scheduleItems"></param>
            private void UpdateCachedScheduleItems(List<ScheduleItem> scheduleItems) 
            {
                // Get cached items 
                List<ScheduleItem> cachedItems = CachedScheduleItems;

                List<string> removeItems = new List<string>();

                // Find out which items that should be removed
                foreach (ScheduleItem scheduleItem in scheduleItems)
                {
                    bool exists = cachedItems.Exists(i => i.Id.Equals(scheduleItem.Id));
                    if (exists)
                    {
                        removeItems.Add(scheduleItem.Id);
                    }
                }

                // Update cache
                cachedItems = CachedScheduleItems;
                cachedItems.RemoveAll(i => removeItems.Contains(i.Id));
                cachedItems.AddRange(scheduleItems);
                CachedScheduleItems = cachedItems;
            }

            /// <summary>
            /// Adds all items in cache
            /// </summary>
            /// <param name="scheduleItems"></param>
            private void ReCacheScheduleItems()
            {
                DateTime now = DateTime.Parse(DateTime.Now.ToShortDateString());
                List < ScheduleItem > scheduleItems = GetScheduleItems(null, FriskisService.GetCurrentFacility().Facilities, FriskisService.LoggedInMember, null, null, null, now, now.AddDays(7));
            }

        #endregion

        private List<TRRLanguage> CachedLanguages
        {
            get
            {
                List<TRRLanguage> languageList = null;

                try
                {
                    // try to find in application 
                    string name = "APPLICATION_LANGUAGES_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    languageList = (List<TRRLanguage>)HttpContext.Current.Application[name];

                    if (languageList != null)
                    {
                        return languageList;
                    }
                }
                catch { }

                // get service
                IDlp2wsRR_Extservice service = GetService();

                // languages 
                TRRLanguage[] languageArray = null;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: CachedLanguages, WebService: GetLanguages");
                bool getLanguagesResult = service.GetLanguages(ref languageArray);

                if (!getLanguagesResult)
                {
                    return new List<TRRLanguage>(); 
                }

                languageList = languageArray.ToList();

                CachedLanguages = languageList;

                return languageList;
            }
            set
            {
                string name = "APPLICATION_LANGUAGES_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public List<string> CachedRooms
        {
            get
            {
                string name = "APPLICATION_ROOMS_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                string name = "APPLICATION_ROOMS_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public Facility CachedFacility
        {
            get
            {
                Facility facility = null;

                try
                {
                    // try to find in application 
                    string name = "APPLICATION_FACILITY_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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

                // get service
                IDlp2wsRR_Extservice service = GetService();

                int language_number = CachedLanguages.Where(l => l.language.Contains("Svenska")).FirstOrDefault().language_number;

                // facilities
                TLocation[] locationArray = null;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: CachedFacility, WebService: GetLocationList");
                bool getLocationListResultSuccess = service.GetLocationList(language_number, ref locationArray);

                if (!getLocationListResultSuccess)
                {
                    return facility;
                }

                foreach (TLocation dlLocation in locationArray)
                {
                    Facility loopFacility = new Facility();
                    loopFacility.originalObject = dlLocation;
                    loopFacility.Id = dlLocation.Description; // how can this have multiple ids? 
                    loopFacility.Name = dlLocation.Description;

                    facility.Facilities.Add(loopFacility);
                }

                // cache result 
                CachedFacility = facility;

                // return found facilities
                return facility;
            }
            set
            {
                string name = "APPLICATION_FACILITY_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

        public List<Activity> CachedActivities
        {
            get
            {
                List<Activity> activities = null;

                try
                {
                    // try to find in application 
                    string name = "APPLICATION_ACTIVITIES_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    activities = (List<Activity>)HttpContext.Current.Application[name];

                    if (activities != null)
                    {
                        return activities;
                    }
                }
                catch { }

                // load activities 
                activities = new List<Activity>();

                // get service
                IDlp2wsRR_Extservice service = GetService();

                int language_number = CachedLanguages.Where(l => l.language.Contains("Svenska")).FirstOrDefault().language_number;

                // activities
                TRRObjectCLass[] objectClassArray = null;
                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: CachedActivities, WebService: GetObjectClassList");
                bool getObjectClassListSuccess = service.GetObjectClassList("G", language_number, ref objectClassArray);

                if (!getObjectClassListSuccess)
                {
                    return activities;
                }

                foreach (TRRObjectCLass dlActivity in objectClassArray)
                {
                    Activity activity = new Activity();
                    activity.Id = dlActivity.ClassCode; // how can this have multiple ids? 
                    activity.originalObject = dlActivity;
                    activity.Name = dlActivity.Description;
                    activities.Add(activity);
                }

                CachedActivities = activities;
                return activities;
            }
            set
            {
                string name = "APPLICATION_ACTIVITIES_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                    string name = "APPLICATION_INSTRUCTORS_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    instructors = (List<Instructor>)HttpContext.Current.Application[name];

                    if (instructors != null)
                    {
                        return instructors;
                    }
                }
                catch { }

                // load instructors 
                instructors = new List<Instructor>();

                // get service
                IDlp2wsRR_Extservice service = GetService();

                // TObjectClassCode[] objectClassCodeArray = activities.Select(i => new TObjectClassCode() { Code = i.ClassCode }).ToArray<TObjectClassCode>();

                // Instructors 
                TRRInstructor[] instructorArray = null;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: CachedInstructors, WebService: GetInstructors");
                // string s = string.IsNullOrEmpty(SessionId) ? "" : SessionId;

                Member member = FriskisService.LoggedInMember;

                if (member != null)
                {
                    bool getInstructorsResultSuccess = service.GetInstructors(member.DLSession, ref instructorArray);
                }
                else
                {
                    instructorArray = new TRRInstructor[0];
                }

                //TInstructorCode[] instructorCodeArray = instructorArray.Select(i => new TInstructorCode()
                //{
                //    Code = i.Code
                //}).ToArray<TInstructorCode>();

                foreach (TRRInstructor dlInstructor in instructorArray)
                {
                    Instructor instructor = new Instructor();
                    instructor.Id = dlInstructor.Code.ToString();
                    instructor.originalObject = dlInstructor;
                    instructor.Name = dlInstructor.LastName + ", " + dlInstructor.FirstName;

                    instructors.Add(instructor);
                }

                // cache result 
                CachedInstructors = instructors;

                // return found instructors
                return instructors;
            }
            set
            {
                string name = "APPLICATION_INSTRUCTORS_DLSOFTWARE_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Application[name] = value;
            }
        }

    #endregion

    #region Properties

        // used to connect to service api
        public string ServiceRevision { get; set; }
        public string ServiceUrl { get; set; }
        public string ServiceLicense { get; set; }
        public bool ScheduleLoginNeeded { get; set; }
        public string SessionId { get; set; }

        public static Dictionary<string, object> DefaultValues
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    {"ServiceUrl", "http://testweb2.dlsoftware.com/dlp2wsr_test/dlp2wsr_ext.dll/wsdl/IDlp2wsRR_Ext"}
                };
            }
        }

        public FriskisServiceType ServiceType
        {
            get
            {
                return FriskisServiceType.DLSoftware;
            }
        }

    #endregion

    #region Login

        public Member Login(string username, string password, Facility facility)
        {
            IDlp2wsRR_Extservice service = GetService();
            Member member = null;

            string loginResultString = "";
            string sessionId = "";

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: Login, WebService: CustomerLogin");
            bool customerLoginResultSuccess = service.CustomerLogin(username, password, ref sessionId, ref loginResultString);

            if (customerLoginResultSuccess)
            {
                string customerCode = "";
                string name = "";
                string mobile = "";
                string email = "";

                // Customer info
                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: Login, WebService: GetCustomerInfo");
                bool customerInfoResultSuccess = service.GetCustomerInfo(sessionId, ref customerCode, ref name, ref mobile, ref email);

                List<string> nameList = name.Split(' ').ToList();
                string firstname = nameList[0];
                string lastname = nameList.Count > 1 ? nameList[1] : "";;


                member = new Member()
                {
                    Firstname = firstname,
                    Lastname = lastname,
                    Mobile = mobile,
                    Email = email,
                    DLSession = sessionId
                };
            }

            // Cache 
            facility = CachedFacility;

            return member;
        }

        private void UpdateSession()
        {
            Member member = LoggedInMember;

            if (member != null) 
            {
                Login(member.Username, member.Password, member.Facility);
            }
        }

    #endregion

    #region Inner facilities

    public List<Facility> GetAllFacilities()
        {
            return new List<Facility>();
        }

    #endregion

    #region Booking

        public List<ScheduleItem> GetBookings(Member member)
        {
            IDlp2wsRR_Extservice service = GetService();

            TRRCustomerReservation[] customerReservations = null;
            List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: GetBookings, WebService: GetCustomerReservations");

            try
            {
                service.GetCustomerReservations(member.DLSession, ref customerReservations);
            }
            catch (Exception ex)
            {
                // Try to update session and run the function again
                UpdateSession();
                service.GetCustomerReservations(member.DLSession, ref customerReservations);
            }

            foreach (TRRCustomerReservation customerReservation in customerReservations)
            {
                ScheduleItem scheduleItem = GetScheduleItem(customerReservation.classID.ToString(), new Facility(), customerReservation.start_time.Date);
                scheduleItem.BookId = customerReservation.ReservationID.ToString();
                scheduleItem.StandbyPosition = customerReservation.Queue_position;
                scheduleItems.Add(scheduleItem);
            }

            return scheduleItems;
        }

        public Result Book(string activityId, Facility facility, DateTime dateTime)
        {
            Member member = FriskisService.LoggedInMember;
            Result result = new Result();

            if (member != null)
            {
                ScheduleItem scheduleItem = GetScheduleItem(activityId, facility, dateTime);
                IDlp2wsRR_Extservice service = GetService();
                int reservationId = 0;
                int errorStatus = 0;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: Book, WebService: MakeReservation");

                bool makeReservationResultSuccess = false;
                
                try
                {
                    makeReservationResultSuccess = service.MakeReservation(member.DLSession, scheduleItem.Where.Id, int.Parse(activityId), ref reservationId, ref errorStatus);
                }
                catch (Exception ex)
                {
                    // Try to update session and run the function again
                    UpdateSession();
                    makeReservationResultSuccess = service.MakeReservation(member.DLSession, scheduleItem.Where.Id, int.Parse(activityId), ref reservationId, ref errorStatus);
                }

                result = new Result
                {
                    Success = makeReservationResultSuccess,
                    Message = errorStatus.ToString()
                };
            }
            else
            {
                result = new Result
                {
                    Success = false,
                    Message = "Ett fel har inträffat"
                };
            }

            // Logg result 
            logger.Log("", LogItemProvider.DLSoftware, facility.Name, LogItemAction.Book, member.Username, result.Message, result.Success ? LogItemType.Information : LogItemType.Error);

            return result;
        }

        public Result Unbook(string id, string bookId, Facility facility, DateTime dateTime)
        {
            Member member = FriskisService.LoggedInMember;
            Result result = new Result();

            if (member != null)
            {

                ScheduleItem scheduleItem = GetBookings(FriskisService.LoggedInMember).Where(b => b.BookId == bookId).FirstOrDefault();
                IDlp2wsRR_Extservice service = GetService();
                // int reservationId = 0;
                int errorStatus = 0;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: Unbook, WebService: CancelReservation");

                bool makeReservationResultSuccess = false;
                try
                {
                    makeReservationResultSuccess = service.CancelReservation(member.DLSession, scheduleItem.Where.Id, int.Parse(scheduleItem.Id), int.Parse(bookId), ref errorStatus);
                }
                catch (Exception ex)
                {
                    // Try to update session and run the function again
                    UpdateSession();
                    makeReservationResultSuccess = service.CancelReservation(member.DLSession, scheduleItem.Where.Id, int.Parse(scheduleItem.Id), int.Parse(bookId), ref errorStatus);
                }

                result = new Result()
                {
                    Success = makeReservationResultSuccess,
                    Message = errorStatus.ToString()
                };
            }
            else
            {
                result = new Result
                {
                    Success = false,
                    Message = "Ett fel har inträffat"
                };
            }

            // Logg result 
            logger.Log("", LogItemProvider.DLSoftware, facility.Name, LogItemAction.Unbook, member.Username, result.Message, result.Success ? LogItemType.Information : LogItemType.Error);

            return result;
        }

    #endregion

    #region Schedule

        public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activityId, string activityTypeId, string instructorId, DateTime From, DateTime To)
        {
            // Drop-in:  GetClasses funktio returnerar i fälten max_rsv_qty  max antal platser man kan reservera. 
            // Fältet max_web_rsv_qty  innehåller max. antal platser man kan reservera i förväg. 
            // Differansen är ju antar drop-in platser.

            IDlp2wsRR_Extservice service = GetService();

            // Languages
            int language_number = CachedLanguages.Where(l => l.language.Contains("Svenska")).FirstOrDefault().language_number;

            // Locations
            string locationId = facilities.Count() == 1 ? facilities.FirstOrDefault().Id : "";
            Facility location = CachedFacility.Facilities.Where(i => i.Id.Equals(locationId)).FirstOrDefault();
            List<TLocation> locationList = new List<TLocation>();
            if (location != null)
            {
                // Add single
                locationList.Add((TLocation)location.originalObject);
            }
            else
            {
                // Add all
                foreach (Facility facility in facilities)
                {
                    locationList.Add((TLocation)facility.originalObject);
                }
            }

            locationList = locationList.Where(l => l != null).ToList();
            TLocationCode[] locationCodeArray = locationList.ToArray<TLocationCode>();

            // Activites
            Activity act = CachedActivities.Where(i => i.Id.Equals(activityId)).FirstOrDefault();
            List<TRRObjectCLass> activities = new List<TRRObjectCLass>();
            if (act != null)
            {
                // add single
                activities.Add((TRRObjectCLass)act.originalObject);
            }
            else
            {
                // add all
                foreach (Activity activity in CachedActivities)
                {
                    activities.Add((TRRObjectCLass)activity.originalObject);
                }
            }

            TObjectClassCode[] objectClassCodeArray = activities.Select(i => new TObjectClassCode() { Code = i.ClassCode }).ToArray<TObjectClassCode>();


            // Instructors
            Instructor instructor = CachedInstructors.Where(i => i.Name.Equals(instructorId)).FirstOrDefault();
            List<TRRInstructor> instructors = new List<TRRInstructor>();
            if (instructor != null)
            {
                // add single
                instructors.Add((TRRInstructor)instructor.originalObject);
            }
            else
            {
                // add all
                foreach (Instructor _instructor in CachedInstructors)
                {
                    instructors.Add((TRRInstructor)_instructor.originalObject);
                }
            }

            TInstructorCode[] instructorCodeArray = instructors.Select(i => new TInstructorCode()
            {
                Code = i.Code
            }).ToArray<TInstructorCode>();

            // Classes
            DateTime fromDateTime = From.Date;
            DateTime toDateTime = To.Date.AddDays(1);

            TRRClass[] classArray = null;

            OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: GetScheduleItems, WebService: GetClasses");

            string sessionId = member != null ? member.DLSession : "";
            bool getClassesResultSuccess = service.GetClasses(sessionId, locationCodeArray, objectClassCodeArray, instructorCodeArray, fromDateTime, toDateTime, ref classArray);

            List<ScheduleItem> classList = classArray.Select(c => new ScheduleItem()
            {
                Id = c.ClassID.ToString(),
                Name = c.Class_description,
                Instructor = c.Instructor1_name + " " + c.Instructor2_name,
                Available = c.Max_rsv_qty - c.Reservation_qty,
                DropinAvailable = c.Max_rsv_qty - c.Max_web_rsv_qty,
                Dropin = (c.Max_web_rsv_qty - c.Max_rsv_qty) > 0 && (c.Max_rsv_qty - c.Reservation_qty == 0),
                Total = c.Max_rsv_qty,

                // Ändra nedan efter svar från DL
                StandbyAvailable = 100,
                StandbyTotal = 100,

                From = c.Start_time,
                To = c.End_time,
                Where = new Facility()
                {
                    Id = locationList.Where(l => l.Code.Equals(c.Location)).FirstOrDefault().Code,
                    Name = locationList.Where(l => l.Code.Equals(c.Location)).FirstOrDefault().Description
                }
            }).ToList();

            List<ScheduleItem> dropinItems = classList.Where(c => c.DropinAvailable > 0).ToList();

            // Update cached items 
            UpdateCachedScheduleItems(classList);

            return classList;
        }

        public ScheduleItem GetBookedItem(string id, string bookId, Facility facility)
        {
            List<ScheduleItem> items = GetBookings(FriskisService.LoggedInMember);
            return items.Where(i => i.Id.Equals(id)).FirstOrDefault();
        }

        public ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime)
        {

            // TODO: Update list if items isn't found
            List<ScheduleItem> scheduleItems = CachedScheduleItems;
            ScheduleItem item = scheduleItems.Where(it => it.Id.Equals(id)).FirstOrDefault();

            if (item == null)
            {
                ReCacheScheduleItems();

                // new try after recache
                scheduleItems = CachedScheduleItems;
                item = scheduleItems.Where(it => it.Id.Equals(id)).FirstOrDefault();
            }

            return item;
        }

    #endregion

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
                ScheduleItem scheduleItem = GetScheduleItem(activityId, facility, dateTime);
                IDlp2wsRR_Extservice service = GetService();
                int reservationId = 0;
                int errorStatus = 0;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: Book, WebService: MakeReservation");
                bool makeReservationResultSuccess = service.MakeRsvToQueue(member.DLSession, scheduleItem.Where.Id, int.Parse(activityId), ref errorStatus);

                result = new Result()
                {
                    Success = makeReservationResultSuccess,
                    Message = errorStatus.ToString()
                };
            }
            else
            {
                result = new Result
                {
                    Success = false,
                    Message = "Ett fel har inträffat"
                };
            }

            return result;
        }

        public Result UnbookStandby(string bookId, Facility facility)
        {
            Member member = FriskisService.LoggedInMember;
            Result result = new Result();

            if (member != null)
            {
                ScheduleItem scheduleItem = GetBookings(FriskisService.LoggedInMember).Where(b => b.BookId == bookId).FirstOrDefault();
                IDlp2wsRR_Extservice service = GetService();
                // int reservationId = 0;
                int errorStatus = 0;

                OldLogger.LogCall(HttpContext.Current.Request.Url.ToString(), "DLSoftware", "C#: Unbook, WebService: CancelReservation");
                bool makeReservationResultSuccess = service.CancelQueue(member.DLSession, scheduleItem.Where.Id, int.Parse(scheduleItem.Id), ref errorStatus);

                result = new Result()
                {
                    Success = makeReservationResultSuccess,
                    Message = errorStatus.ToString()
                };
            }
            else
            {
                result = new Result
                {
                    Success = false,
                    Message = "Ett fel har inträffat"
                };
            }

            return result;
        }

    #endregion
}