using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Dummy service 
/// </summary>
public class DemoService : FriskisService, IFriskisService
{
    #region Cache

        /// <summary>
        /// Updates each time a user logs in
        /// </summary>
        public Facility CachedFacility
        {
            get
            {
                Facility activities = new Facility();

                try
                {
                    string name = "SESSION_FACILITY_DEMO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    activities = (Facility)HttpContext.Current.Session[name];
                }
                catch { }

                return activities;
            }
            set
            {
                string name = "SESSION_FACILITY_DEMO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Session[name] = value;
            }
        }

        public List<string> CachedRooms
        {
            get
            {
                string name = "APPLICATION_ROOMS_DEMO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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
                string name = "APPLICATION_ROOMS_DEMO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
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

                try
                {
                    string name = "SESSION_ACTIVITIES_DEMO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                    activities = (List<Activity>)HttpContext.Current.Session[name];
                }
                catch { }

                return activities;
            }
            set
            {
                string name = "SESSION_ACTIVITIES_DEMO_" + GetCurrentFacility().LocalId.ToString().Replace("-", "");
                HttpContext.Current.Session[name] = value;
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

    public string ServiceUrl { get; set; }

    public bool ScheduleLoginNeeded { get; set; }

    public static Dictionary<string, object> DefaultValues
    {
        get
        {
            return new Dictionary<string, object>()
            {

            };
        }
    }

    public FriskisServiceType ServiceType 
    {
        get
        {
            return FriskisServiceType.Demo;
        }
    }

    // used in scheduleitems
    private static Facility DemoFacility = new Facility()
    {
        Id = "2",
        Name = "Göteborg",
        Description = "Beskrivning av Göteborg",
        Address = "411 12, Drottninggatan 7"
    };

    #region Authentication

        /// <summary>
        /// Returns a member when the member was validated and 
        /// null when the login was incorrect.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Member Login(string username, string password, Facility facility)
        {
            // session id
            if (username.Equals("friskis") && password.Equals("svettis"))
            {
                Member member = new Member()
                {
                    Id = "1", 
                    Firstname = "Friskis",
                    Lastname = "Svettis"
                };

                return member;
            }

            return null;
        }

        public override bool Logout()
        {
            base.Logout();
            return true;
        }

    #endregion

    public List<Facility> GetAllFacilities(string city)
    {
        return GetAllFacilities();
    }

    public List<Facility> GetAllFacilities()
    {
        return new List<Facility>()
        {
            new Facility()
            {
                Id = "1",
                Name = "Varberg",
                Description = "Beskrivning av Varberg",
                Address = "432 30, Storgatan 11"
            },
            new Facility()
            {
                Id = "2",
                Name = "Göteborg",
                Description = "Beskrivning av Göteborg",
                Address = "411 12, Drottninggatan 7"
            }
        };
    }

    public Facility GetFacility(string id)
    {
        List<Facility> facilities = GetAllFacilities();
        return facilities.Where(f => f.Id.Equals(id)).FirstOrDefault();
    }

    #region Bookings 

        private const string SESSION_BOOKINGS = "SESSION_BOOKINGS";

        public List<ScheduleItem> GetBookings(Member member)
        {
            if (HttpContext.Current.Session[SESSION_BOOKINGS] == null)
            {

                // init demo bookings in session
                HttpContext.Current.Session[SESSION_BOOKINGS] = new List<ScheduleItem>()
                {
                    new ScheduleItem()
                    {
                        Id = "1",
                        Name = "Step up",
                        Where = DemoFacility,
                        Description = "",
                        Total = 15,
                        Available = 6,
                        Level = WorkoutLevel.Easy,
                        From = new DateTime(2011, 12, 6, 19, 00, 00),
                        To = new DateTime(2011, 12, 6, 20, 30, 00)
                    },

                    new ScheduleItem()
                    {
                        Id = "2",
                        Name = "Spinning Lätt",
                        Where = DemoFacility,
                        Description = "",
                        Total = 20,
                        Available = 17,
                        Level = WorkoutLevel.Easy,
                        From = new DateTime(2011, 12, 6, 20, 00, 00),
                        To = new DateTime(2011, 12, 6, 21, 00, 00)
                    }
                };
            }

            return (List<ScheduleItem>)HttpContext.Current.Session[SESSION_BOOKINGS];

        }

        public Result Book(string id, Facility facility, DateTime dateTime)
        {
            ScheduleItem item = GetScheduleItem(id, facility, dateTime);
            List<ScheduleItem> bookings = GetBookings(null);
            bookings.Add(item);
            HttpContext.Current.Session[SESSION_BOOKINGS] = bookings;

            return new Result("", true);
        }

        public Result Unbook(string id, string bookId, Facility facility, DateTime dateTime)
        {
            // find item to remove
            List<ScheduleItem> items = GetBookings(FriskisService.LoggedInMember);
            ScheduleItem removeItem = items.Where(i => i.Id.Equals(id)).FirstOrDefault();

            // return result of removal
            return new Result("", items.Remove(removeItem));
        }

    #endregion

    public ScheduleItem GetBookedItem(string id, string bookId, Facility facility)
    {
        List<ScheduleItem> items = GetBookings(FriskisService.LoggedInMember);
        return items.Where(i => i.Id.Equals(id)).FirstOrDefault();
    }

    public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activityType, WorkoutType workoutType, DateTime From, DateTime To)
    {
        return new List<ScheduleItem>();
    }

    public List<ScheduleItem> GetScheduleItems(Facility mainFacility, List<Facility> facilities, Member member, string activity, string activityType, string instructor, DateTime From, DateTime To)
    {
        return GetScheduleItems(mainFacility, facilities, member, activity, null, From, To);
    }

    public List<ScheduleItem> GetScheduleItems(Facility facility, Member member, string activityType, WorkoutType workoutType, DateTime From, DateTime To)
    {
        return new List<ScheduleItem>()
        {
            new ScheduleItem()
            {
                Id = "3",
                Name = "Step up",
                Where = DemoFacility,
                Description = "",
                Total = 15,
                Available = 6,
                Level = WorkoutLevel.Easy,
                From = new DateTime(2011, 12, 6, 19, 00, 00),
                To = new DateTime(2011, 12, 6, 20, 30, 00)
            },
            new ScheduleItem()
            {
                Id = "4",
                Name = "Aerobics",
                Where = DemoFacility,
                Description = "",
                Total = 25,
                Available = 21,
                Level = WorkoutLevel.Medium,
                From = new DateTime(2011, 12, 6, 18, 00, 00),
                To = new DateTime(2011, 12, 6, 19, 00, 00)
            }/*,
            new ScheduleItem()
            {
                Id = "5",
                Name = "Spinning Lätt",
                Where = DemoFacility,
                Description = "",
                Total = 20,
                Available = 17,
                Level = WorkoutLevel.Easy,
                From = new DateTime(2011, 12, 6, 20, 00, 00),
                To = new DateTime(2011, 12, 6, 21, 00, 00)
            },
            new ScheduleItem()
            {
                Id = "6",
                Name = "Spinning Svår",
                Where = DemoFacility,
                Description = "",
                Total = 20,
                Available = 19,
                Level = WorkoutLevel.Hard,
                From = new DateTime(2011, 12, 6, 21, 00, 00),
                To = new DateTime(2011, 12, 6, 22, 00, 00)
            }*/
        };
    }

    public ScheduleItem GetScheduleItem(string id, Facility facility, DateTime dateTime)
    {
        List<ScheduleItem> items = GetScheduleItems(facility, null, "", null, DateTime.Now, DateTime.Now);
        return items.Where(i => i.Id.Equals(id)).FirstOrDefault();
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