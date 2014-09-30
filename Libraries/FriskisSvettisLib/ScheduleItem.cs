using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using MoMA.Helpers;

/// <summary>
/// Schedule item
/// </summary>
public class ScheduleItem
{

    #region Properties 

        #region Id

            /// <summary>
            /// Id in service
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Id to use when unbooking (booking id) a booked schedule item
            /// </summary>
            public string BookId { get; set; }

         #endregion

        #region Information

            /// <summary>
            /// The original item passed by service
            /// </summary>
            public object OriginalItem { get; set; }

            /// <summary>
            /// Facility where the schedule item is being held.
            /// </summary>
            public Facility Where { get; set; }

            /// <summary>
            /// Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Room where the schedule item is held
            /// </summary>
            public string Room { get; set; }

            /// <summary>
            /// Instructor(s) of this schedule item
            /// </summary>
            public string Instructor { get; set; }

        #endregion

        #region Book

            /// <summary>
            /// If this scheudule item is bookable
            /// </summary>
            public bool Bookable { get; set; }

            /// <summary>
            /// If this scheudule item is bookable
            /// </summary>
            public DateTime BookableFrom { get; set; }

            /// <summary>
            /// If this scheudule item is bookable
            /// </summary>
            public DateTime BookableTo { get; set; }

            /// <summary>
            /// Total number of bookable vacancies  
            /// </summary>
            public int Total { get; set; }

            /// <summary>
            /// Number of bookable vacancies left
            /// </summary>
            public int Available { get; set; }

            /// <summary>
            /// If this scheudule item is available to view in schedule for the logged in user
            /// </summary>
            public bool AvailableToUser { get; set; }

            /// <summary>
            /// If this scheudule item is cancelled
            /// </summary>
            public bool Cancelled { get; set; }

            /// <summary>
            /// If this scheudule item is bookable from internet
            /// </summary>
            public bool BookableFromInternet { get; set; }

            /// <summary>
            /// If this scheudule item is bookable from internet
            /// </summary>
            public bool Visible { get; set; }

        #endregion

        #region Standby

        /// <summary>
            /// If this scheudule item has standby vacancies
            /// </summary>
            public bool Standby
            {
                get
                {
                    // there are standby vacancies
                    // OR
                    // there are NO available bookable vacancies
                    // OR
                    // there are standby vacancies
                    // OR
                    // standby element exists (BRP)
                    //bool isStandBy = (StandbyTotal > 0 || !string.IsNullOrEmpty(StandbyBookingId)) 
                    //                && Available == 0 || 
                    //                StandbyPosition > 0 || 
                    //                (StandbyElementExists && Available == 0);

                    // TODO: Check if this should be used on Pastell and Defendo too
                    if (FriskisService.GetCurrentFriskisService().ServiceType == FriskisServiceType.BRP)
                    {
                        bool isBooked = !string.IsNullOrEmpty(BookId);

                        // if the item is booked 
                        if (isBooked)
                        {
                            // this is a booked standby if there is a standbyBookingId
                            return !string.IsNullOrEmpty(StandbyBookingId);
                        }

                        // if the item isn't booked
                        return StandbyAvailable > 0 && StandbyTotal > 0 &&     // det finns ett standby platser och totalt mer än en plats
                               Available == 0 &&                               // finns inga vanliga bokningsplatser kvar
                               DateTime.Now.CompareTo(BookableFrom) > 0 &&     // bokning öppnat
                               DateTime.Now.CompareTo(BookableTo) < 0;         // bokning inte stängt
                    }

                    if (FriskisService.GetCurrentFriskisService().ServiceType == FriskisServiceType.DLSoftware)
                    {
                        bool isBooked = !string.IsNullOrEmpty(BookId);

                        // if the item is booked 
                        if (isBooked)
                        {
                            // this is a booked standby if there is a standbyBookingId
                            return StandbyPosition > 0;
                        }

                        // if the item isn't booked
                        return StandbyAvailable > 0 && StandbyTotal > 0 &&     // det finns ett standby platser och totalt mer än en plats
                               Available == 0 &&                               // finns inga vanliga bokningsplatser kvar
                               DateTime.Now.CompareTo(BookableFrom) > 0 &&     // bokning öppnat
                               DateTime.Now.CompareTo(BookableTo) < 0;         // bokning inte stängt
                    }

                    // NOTE: This works with pastell because when you load booked 
                    //       schedule items, the other parameters aren't loading
                    //       ie: standbyavailable is always 0 when loading
                    //           a booked item
                    return  !string.IsNullOrEmpty(StandbyBookingId) ||          // Pastell: Om passet har ett standby-booking-id (används för att se om det är ett reservpass i MyBookings bland annat)
                            (
                                StandbyAvailable > 0  && StandbyTotal > 0 &&    // det finns ett standby platser och totalt mer än en plats
                                Available == 0 &&                               // finns inga vanliga bokningsplatser kvar
                                DateTime.Now.CompareTo(BookableFrom) > 0 &&     // bokning öppnat
                                DateTime.Now.CompareTo(BookableTo) < 0          // bokning inte stängt    // att det finns folk som är bokade på reserv
                            );         


                    //if (isStandBy &&
                    //    DateTime.Now.CompareTo(BookableFrom) > 0 && // now > from 
                    //    DateTime.Now.CompareTo(BookableTo) < 0)  // now < to
                    //{
                    //    return true;
                    //}
                    //else
                    //{
                    //    return false;
                    //}
                }
            }

            /// <summary>
            /// If a standby-element exists (BRP)
            /// </summary>
            private bool StandbyElementExists { get; set; }

            /// <summary>
            /// Standby booking id
            /// </summary>
            public string StandbyBookingId { get; set; }

            /// <summary>
            /// If the user has booked a standby item, then this property says what position the user has
            /// </summary>
            public int StandbyPosition { get; set; }

            /// <summary>
            /// If the user has booked a standby item, then this property says what position the user has
            /// </summary>
            public int StandbyBooked { get; set; }

            /// <summary>
            /// Total number of standby vacancies
            /// </summary>
            public int StandbyTotal { get; set; }

            /// <summary>
            /// If this scheudule item has standby vacancies
            /// </summary>
            public int StandbyAvailable { get; set; }

        #endregion

        #region Dropin

            /// <summary>
            /// If this scheudule item has dropin
            /// </summary>
            public bool Dropin { get; set; }

            /// <summary>
            /// number of dropin vacancies left
            /// </summary>
            public int DropinAvailable { get; set; }

        #endregion

        #region Activity type

            /// <summary>
            /// Main activity type
            /// </summary>
            public string ActivityTypeId { get; set; }

            /// <summary>
            /// Child activity type
            /// </summary>
            public string ChildActivityTypeId { get; set; }

        #endregion

        #region Level

            /// <summary>
            /// Difficulty level of schedule item
            /// </summary>
            public WorkoutLevel Level { get; set; }

        #endregion

        #region Dates

        /// <summary>
            /// When schedule item starts
            /// </summary>
            public DateTime From { get; set; }

            /// <summary>
            /// When schedule item ends
            /// </summary>
            public DateTime To { get; set; }

        #endregion

        /// <summary>
        /// Used for Pastell to get what mode is used
        /// </summary>
        public string PastellIsBookable { get; set; }

    #endregion

    #region Helpers 

        /// <summary>
        /// Book type
        /// </summary>
        /// <returns>Returns the current book type</returns>
        public BookType GetBookType()
        {
            string facilityId = ContextHelper.GetValue<string>("facilityId", "");
            ScheduleItem item = this;

            // Pastell
            if (!string.IsNullOrEmpty(PastellIsBookable))
            {

                if (!string.IsNullOrEmpty(BookId))
                {
                    return BookType.Booked;
                }   

                // Ej bokbar - Öppnar: 2012-02-16 21:00
                // Boka
                // Reservboka
                // Stängd
                switch (PastellIsBookable.Split(' ').First())
                {

                    // could be dropin - check (always dropin?) 
                    // include dropdin opening?
                    case "Ej":

                        if (DropinAvailable > 0)
                        {
                            return BookType.Dropin;
                        }

                        return BookType.NotBookable;
                    case "Boka":
                        return BookType.Bookable;
                    case "Reservboka":
                        // when full, the service will return "Fullt"
                        return BookType.Standby;
                    case "Fullt":
                        return BookType.Full;
                    case "Inställt":
                        // 2013-02-04
                        return BookType.Cancelled;
                    case "Stängd":

                        if (DropinAvailable > 0)
                        {
                            return BookType.Dropin;
                        }

                        return BookType.NotBookable;
                }
            }

            // BRP | Defendo

            // inställt
            if (item.Cancelled)
            {
                return BookType.NotVisible;
            }

            // Inte inloggad 
            else if (!FriskisService.IsLoggedIn)
            {
                return BookType.NotLoggedIn;
            }

            // Man inte kan boka från internet
            else if (!item.BookableFromInternet)
            {
                return BookType.NotBookable;
            }

            // Passet är bokat av inloggade användaren (det finns ett book-id)
            else if (!string.IsNullOrEmpty(item.BookId))
            {
                return BookType.Booked;
            }

            // Bokning har inte öppnat än och det finns bokningsbara platser
            else if (DateTime.Now.CompareTo(item.BookableFrom) < 0 && Available > 0)
            {
                // can't book yet
                return BookType.NotAvailable;
            }

            // Reservbokning , egenskap gör följande kontroll:
                 // Om passet har ett standby-booking-id (används för att se om det är ett reservpass i MyBookings bland annat)
                 // ELLER
                 // Det finns standby platser och totalt mer än en plats OCH
                 // det finns inga vanliga bokningsplatser kvar OCH
                 // bokning är öppnad OCH
                 // bokning inte stängd 
            else if (item.Standby)
            {
                return BookType.Standby;
            }

            // Användaren inloggad
            // Ingen facilityId i querystring (inloggad på schema)
            // Passet är inte bokat av den inloggade användaren
            // 
            // Bookable kontrollerar följande i BRP:
            //   - Om det finns platser
            //   - Bokning har öppnat 
            //   - Bokning är inte stängd.
            else if (FriskisService.IsLoggedIn && string.IsNullOrEmpty(facilityId) && string.IsNullOrEmpty(item.BookId) && item.Bookable)
            // else if (FriskisService.IsLoggedIn && string.IsNullOrEmpty(item.BookId) && item.Bookable)
            {
                return BookType.Bookable;
            }

            // Det är dropin
            // Dropin kontrollerar (BRP)
            //   - Om det finns dropin-platser
            //   - Om bokningsbara platser == 0
            else if (item.Dropin)
            {
                return BookType.Dropin;
            }

            // Det finns varken bokningsbara platser eller dropin-platser
            else if (!item.Dropin && !item.Bookable)
            {
                // Pass är fullt eftersom det inte finns några bokningsbara platser eller dropin.
                if (!item.AvailableToUser)
                {
                    return BookType.NotAvailable;
                }
                else if (item.Available == 0)
                {
                    return BookType.Full;
                }

                return BookType.NotAvailable;
            }

            return BookType.NotAvailable;
        }

        /// <summary>
        /// Used in MyBookings and Schedule.
        /// </summary>
        /// <returns>Html to display information about this schedule item</returns>
        public string GetDescriptionHtml()
        {
            ScheduleItem item = this;
            string whereName = item.Where.Name; 
            Facility facility = FriskisService.GetCurrentFacility();


            // filter out "All" option when counting facilities
            int facilityCount = facility.Facilities == null ? 0 : facility.Facilities.Where(f => !f.Id.Equals("All")).Count();

            if (facility.Service.ServiceType == FriskisServiceType.BRP && facilityCount == 1) 
            {
                whereName = item.Room;
            }

            return item.From.ToString("dd") + "/" + item.From.ToString("MM yyyy, H:mm") + " - " + item.To.ToString("H:mm") + "<br />" +
                (string.IsNullOrEmpty(item.Instructor) ? "" : item.Instructor + "<br />") +
                whereName + "<br />" +
                item.Where.Address;
        }

    #endregion

    #region Constructors

        public ScheduleItem()
        {
            Where = new Facility()
            {
                Id = "",
                Name = "",
                Description = "",
                Address = "",
                Email = "",
                Phone = "",
                HtmlInfo = "",
            };

            AvailableToUser = true;
            ActivityTypeId = "";

            Level = WorkoutLevel.None;
            Bookable = true;

            From = DateTime.MinValue;
            To = DateTime.MaxValue;

            StandbyElementExists = false;

            // default
            BookableFrom = new DateTime(2012, 1, 1);
            BookableTo = new DateTime(2090, 1, 1);

            BookableFromInternet = true;
            Cancelled = false;
            Visible = true;
        }

        public ScheduleItem(string data, IFriskisService service, DateTime dateTime)
            : this(data, service, dateTime, ScheduleItemType.Normal)
        {
        }

        public ScheduleItem(string data, IFriskisService service, DateTime dateTime, ScheduleItemType scheduleItemType)
            : this()
	    {
            switch (service.ServiceType)
            {
                case FriskisServiceType.Demo:
                    break;
                case FriskisServiceType.BRP:
                    FillScheduleItemFromBRP(data);
                    break;
                case FriskisServiceType.PastellData:
                    FillScheduleItemFromPastellData(data, dateTime, scheduleItemType);
                    break;
            }
        }

    #endregion

    #region Parsing

        public static List<ScheduleItem> ParseMany(string xml, DateTime dateTime, IFriskisService service)
        {
            return ParseMany(xml, dateTime, service, ScheduleItemType.Normal);
        }

        public static List<ScheduleItem> ParseMany(string xml, DateTime dateTime, IFriskisService service, ScheduleItemType scheduleItemType)
        {
            List<ScheduleItem> items = new List<ScheduleItem>();

            switch (service.ServiceType)
            {
                case FriskisServiceType.Demo:
                    break;
                case FriskisServiceType.BRP:
                    items = ParseManyFromBRP(xml);
                    break;
                case FriskisServiceType.PastellData:
                    items = ParseManyFromPastellData(xml, dateTime, scheduleItemType);
                    break;
            }

            return items;
        }

    #endregion

    #region PastellData

    private void FillScheduleItemFromPastellData(string xml, DateTime dateTime, ScheduleItemType scheduleItemType)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);


            string from = "";
            string to = "";

            DateTime dFrom = DateTime.MinValue;
            DateTime dTo = DateTime.MinValue;

            switch (scheduleItemType)
            {
                case ScheduleItemType.Normal:

                    Id = XmlHelper.GetValue<string>(xmlDoc, "//bo/BOID", "");

                    Where = new Facility()
                    {
                        Id = "",
                        Id2 = XmlHelper.GetValue<string>(xmlDoc, "//bo/uid", "")
                    };

                    Name = XmlHelper.GetValue<string>(xmlDoc, "//bo/desc", "");
                    Instructor = XmlHelper.GetValue<string>(xmlDoc, "//bo/l", "");

                    // inspect a special object
                    if (Name.Contains("Spinning - Medel"))
                    {
                        
                    }

                    Description = XmlHelper.GetValue<string>(xmlDoc, "//bo/desc", "");
                    Room = XmlHelper.GetValue<string>(xmlDoc, "//bo/r", "");
                    Total = XmlHelper.GetValue<int>(xmlDoc, "//bo/s", 0);

                    
                    // Ej bokbar - Öppnar: 2012-02-16 21:00
                    // Boka
                    // Reservboka
                    // Stängd
                    PastellIsBookable = XmlHelper.GetValue<string>(xmlDoc, "//bo/isbookable", "");

                    ActivityTypeId = XmlHelper.GetValue<string>(xmlDoc, "//bo/aid", ""); // used to filter by type
                    ChildActivityTypeId = XmlHelper.GetValue<string>(xmlDoc, "//bo/acid", ""); // used to filter by type

                    Level = WorkoutLevel.None;

                    Cancelled = false;

                    // Start/end-dates
                    from = XmlHelper.GetValue<string>(xmlDoc, "//bo/start", "");
                    to = XmlHelper.GetValue<string>(xmlDoc, "//bo/e", "");

                    dFrom = new DateTime();
                    dTo = new DateTime();

                    DateTime.TryParse(from, out dFrom);
                    DateTime.TryParse(to, out dTo);

                    From = dFrom;
                    To = dTo;

                    // 
                    Available = XmlHelper.GetValue<int>(xmlDoc, "//bo/sl", 0);
                    DropinAvailable = XmlHelper.GetValue<int>(xmlDoc, "//bo/dsl", 0);

                    Bookable = Available > 0;
                    Dropin = DropinAvailable > 0 && Available == 0;

                    StandbyTotal = XmlHelper.GetValue<int>(xmlDoc, "//bo/rs", 0);
                    StandbyAvailable = XmlHelper.GetValue<int>(xmlDoc, "//bo/rsl", 0);

                    if (Name.Contains("Spinning - Medel"))
                    {
                    }

                    break;

                case ScheduleItemType.Booked:

                    Id = XmlHelper.GetValue<string>(xmlDoc, "//Booking/bookableobjectid", "");
                    BookId = XmlHelper.GetValue<string>(xmlDoc, "//Booking/BOOKINGID", "");

                    Where = new Facility()
                    {
                        Id = XmlHelper.GetValue<string>(xmlDoc, "//Booking/bookableobjectglobalunitid", "")
                    };

                    Name = XmlHelper.GetValue<string>(xmlDoc, "//Booking/description", "");
                    Description = XmlHelper.GetValue<string>(xmlDoc, "//Booking/description", "");
                    Room = XmlHelper.GetValue<string>(xmlDoc, "//Booking/room", "");
                    Instructor = XmlHelper.GetValue<string>(xmlDoc, "//Booking/leader", "");
                    
                    Bookable = false;
                    Available = 0; // won't show on page because Bookable = false
                    Total = 0; // won't show on page because Bookable = false

                    Level = WorkoutLevel.None;

                    Cancelled = false;

                    // dates
                    from = XmlHelper.GetValue<string>(xmlDoc, "//Booking/date", "");
                    to = XmlHelper.GetValue<string>(xmlDoc, "//Booking/enddate", "");

                    dFrom = new DateTime();
                    dTo = new DateTime();

                    DateTime.TryParse(from, out dFrom);
                    DateTime.TryParse(to, out dTo);

                    From = dFrom;
                    To = dTo;
                    break;

                case ScheduleItemType.BookedStandby:

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

                    Id = XmlHelper.GetValue<string>(xmlDoc, "//Booking/bookableobjectid", "");
                    BookId = XmlHelper.GetValue<string>(xmlDoc, "//Booking/RESERVEBOOKINGID", "");
                    StandbyBookingId = BookId;

                    Where = new Facility()
                    {
                        Id = XmlHelper.GetValue<string>(xmlDoc, "//Booking/bookableobjectglobalunitid", "")
                    };

                    Name = XmlHelper.GetValue<string>(xmlDoc, "//Booking/description", "");
                    Description = XmlHelper.GetValue<string>(xmlDoc, "//Booking/description", "");
                    Room = XmlHelper.GetValue<string>(xmlDoc, "//Booking/room", "");
                    Instructor = XmlHelper.GetValue<string>(xmlDoc, "//Booking/leader", "");

                    StandbyPosition = XmlHelper.GetValue<int>(xmlDoc, "//Booking/POSITION", 1);

                    Bookable = false;
                    Available = 0; // won't show on page because Bookable = false
                    Total = 0; // won't show on page because Bookable = false

                    Level = WorkoutLevel.None;

                    Cancelled = false;

                    // dates
                    from = XmlHelper.GetValue<string>(xmlDoc, "//Booking/date", "");
                    to = XmlHelper.GetValue<string>(xmlDoc, "//Booking/enddate", "");

                    dFrom = new DateTime();
                    dTo = new DateTime();

                    DateTime.TryParse(from, out dFrom);
                    DateTime.TryParse(to, out dTo);

                    From = dFrom;
                    To = dTo;

                    break;

                case ScheduleItemType.WithoutLogin:

                    // NOT AVAILABLE
                    // Id = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/bookableobjectid", "");
                    // BookId = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/BOOKINGID", "");

                    Where = new Facility()
                    {
                        Name = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/unit", "")
                    };

                    Name = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/name", ""); 
                    Description = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/information", ""); 
                    Room = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/room", ""); 
                    Available = XmlHelper.GetValue<int>(xmlDoc, "//bookableobject/slotsleft", 0);
                    Total = XmlHelper.GetValue<int>(xmlDoc, "//bookableobject/slots", 0);
                    Instructor = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/leader", "");

                    ActivityTypeId = Name.Split('-').First().Trim();

                    Bookable = false; // not logged in

                    Level = WorkoutLevel.None;

                    Cancelled = false;

                    // dates
                    string starttime = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/starttime", ""); 

                    dFrom = new DateTime();
                    dTo = new DateTime();

                    DateTime.TryParse(dateTime.ToShortDateString() + " " + starttime.Split('-')[0], out dFrom);
                    DateTime.TryParse(dateTime.ToShortDateString() + " " + starttime.Split('-')[1], out dTo); 

                    From = dFrom;
                    To = dTo;

                    // dropin
                    Dropin = Total == 0;
                    DropinAvailable = XmlHelper.GetValue<int>(xmlDoc, "//bookableobject/dropinslots", 0);

                    // TODO: check status 
                    string status = XmlHelper.GetValue<string>(xmlDoc, "//bookableobject/status", "");

                    switch (status)
                    {
                        case "Drop-in" : // Medför att ingen bokning längre kan ske på webb och då är slotsleft antalet dropin-platsers kvar.
                            Bookable = false;
                            Dropin = true;
                            DropinAvailable = Available;
                            break;
                        case "Platser kvar": // Medför att boknings fortfarande kan ske på webb, nu anges slotsleft antalet webbplatser kvar.
                            Bookable = true;
                            Dropin = false;
                            DropinAvailable = 0;
                            break;
                        case "Inställt": // Passet är inte tillgängligt längre
                            Bookable = false;
                            Dropin = false;
                            DropinAvailable = 0;
                            break;
                        case "Fullt": // Passet är fullbokat
                            Bookable = false;
                            Dropin = false;
                            DropinAvailable = 0;
                            break;
                        case "Stängd": // Passet är stängt för bokning
                            Bookable = false;
                            Dropin = false;
                            DropinAvailable = 0;
                            break;
                        case "Reserv": // Enbart reservplatsbokning kan ske, lägg till när stöd finns
                            Bookable = false;
                            Dropin = false;
                            DropinAvailable = 0;
                            break;
                        case "Dropin/Reserv": // Reservplatsbokning alt chansa på dropinplatser, får kontrollera nogrannare sen
                            Bookable = false;
                            Dropin = true;
                            DropinAvailable = Available;
                            break;
                    }

                    if (Dropin || Available == 0)
                    {
                        Bookable = false;
                    }
                    else
                    {
                        Bookable = true;
                    }

                    break;

            }
        }

        public static List<ScheduleItem> ParseManyFromPastellData(string xml, DateTime dateTime, ScheduleItemType scheduleItemType)
        {
            XmlDocument xmlDoc = new XmlDocument();
            List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

            try
            {
                xmlDoc.LoadXml(xml);

                XmlNodeList scheduleItemNodes = null;

                switch (scheduleItemType)
                {
                    case ScheduleItemType.Normal:
                        scheduleItemNodes = xmlDoc.SelectNodes("//ProfitAndroid/AndroidBookableObjects/bo");
                        break;
                    case ScheduleItemType.Booked:
                        scheduleItemNodes = xmlDoc.SelectNodes("//ProfitAndroid/AndroidBookingObjects/Booking");
                        break;
                    case ScheduleItemType.BookedStandby:
                        scheduleItemNodes = xmlDoc.SelectNodes("//ProfitAndroid/AndroidBookingObjects/Booking");
                        break;
                    case ScheduleItemType.WithoutLogin:
                        scheduleItemNodes = xmlDoc.SelectNodes("//bookableobjectlist/bookableobject");
                        break;
                }

                foreach (XmlNode facilityNode in scheduleItemNodes)
                {
                    scheduleItems.Add(new ScheduleItem(facilityNode.OuterXml, new PastellService(), dateTime, scheduleItemType));
                }
            }
            catch
            {

            }

            return scheduleItems;
        }

    #endregion

    #region BRP

        private void FillScheduleItemFromBRP(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            Id = XmlHelper.GetValue<string>(xmlDoc, "//pass/id", "");
            BookId = XmlHelper.GetValue<string>(xmlDoc, "//pass/bookingid", "");

            Where = new Facility()
            {
                Id = XmlHelper.GetValue<string>(xmlDoc, "//pass/businessunitid", "")
            };

            Name = XmlHelper.GetValue<string>(xmlDoc, "//pass/aktivitet", "");
            Instructor = XmlHelper.GetValue<string>(xmlDoc, "//pass/resurs", "");
            Description = XmlHelper.GetValue<string>(xmlDoc, "//pass/beskrivning", "");
            Room = XmlHelper.GetValue<string>(xmlDoc, "//pass/lokal", "");
            Available = XmlHelper.GetValue<int>(xmlDoc, "//pass/bokningsbara", 0);
            Total = XmlHelper.GetValue<int>(xmlDoc, "//pass/totalt", 0);

            // Bookable from/to
            string bookableFrom = XmlHelper.GetValue<string>(xmlDoc, "//pass/bookableEarliest", "2012-01-01 00:00");
            string bookableTo = XmlHelper.GetValue<string>(xmlDoc, "//pass/bookableLatest", "2090-01-01 00:00");
            bookableFrom = string.IsNullOrEmpty(bookableFrom) ? "2012-01-01 00:00" : bookableFrom;
            bookableTo = string.IsNullOrEmpty(bookableTo) ? "2090-01-01 00:00" : bookableTo;
            DateTime bookableFromDateTime;
            DateTime bookableToDateTime;
            DateTime.TryParse(bookableFrom, out bookableFromDateTime);
            DateTime.TryParse(bookableTo, out bookableToDateTime);
            BookableFrom = bookableFromDateTime;
            BookableTo = bookableToDateTime;

            // Kontrollerar lediga platser
            // Kontrollerar så att inga bokningsbara platser finns (BRP regel)
            int free = XmlHelper.GetValue<int>(xmlDoc, "//pass/lediga", 0);
            if (free > Available && Available == 0)
            {
                Dropin = true;
            }

            // to be able to show how many dropin there will be
            DropinAvailable = free - Available;
            
            Level = WorkoutLevel.None;

            // Kontrollerar ifall passet är inställt
            Cancelled = XmlHelper.GetValue<string>(xmlDoc, "//pass/installt", "").Equals("true");

            // Kontrollerar ifall passet får bokas på nätet
            BookableFromInternet = XmlHelper.GetValue<string>(xmlDoc, "//pass/bookableFromInternet", "true").Equals("true");

            // start / slut
            string date = XmlHelper.GetValue<string>(xmlDoc, "//pass/datum", "");
            string from = XmlHelper.GetValue<string>(xmlDoc, "//pass/tid", "").Substring(0, 5);
            string to = XmlHelper.GetValue<string>(xmlDoc, "//pass/tid", "").Substring(8, 5);
            From = DateTime.Parse(date + " " + from);
            To = DateTime.Parse(date + " " + to);

            // Bookable om det finns platser
            // Bokning har öppnat 
            // Bokning är inte stängd.
            Bookable = Available > 0 && 
                DateTime.Now.CompareTo(BookableFrom) > 0 && // now > from 
                DateTime.Now.CompareTo(BookableTo) < 0; // now < to

            // name, have to be converted later
            ActivityTypeId = XmlHelper.GetValue<string>(xmlDoc, "//pass/group", "");

            //<waitinglist> #Finns bara om köhantering används
            //  <waitinglistsize>3</waitinglistsize>
            //  <waitinglistposition># köplats (börjar på 1)</waitinglistposition>
            //  <waitinglistid>köplatsbokningens id</waitinglistid>
            //</waitinglist>

            StandbyElementExists = xmlDoc.SelectNodes("//pass/waitinglist").Count > 0;
            StandbyPosition = XmlHelper.GetValue<int>(xmlDoc, "//pass/waitinglist/waitinglistposition", 0);
            StandbyBookingId = XmlHelper.GetValue<string>(xmlDoc, "//pass/waitinglist/waitinglistid", "");
            StandbyBooked = XmlHelper.GetValue<int>(xmlDoc, "//pass/waitinglist/waitinglistsize", 0);

            // Det finns "oändliga" standby platser enligt BRP
            if (StandbyElementExists)
            {
                StandbyTotal = 100;
                StandbyAvailable = 100 - StandbyBooked;
            }
            else
            {
                StandbyTotal = 0;
                StandbyAvailable = 0;
            }

            if (!string.IsNullOrEmpty(StandbyBookingId))
            {
                // also used to see if this is a standby item in property StandBy
                BookId = StandbyBookingId;
            }

            // fallback
            StandbyAvailable = StandbyTotal;
        }

        public static List<ScheduleItem> ParseManyFromBRP(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
            XmlNodeList scheduleItemNodes = xmlDoc.SelectNodes("//passlista/pass");

            foreach (XmlNode facilityNode in scheduleItemNodes)
            {
                scheduleItems.Add(new ScheduleItem(facilityNode.OuterXml, new BRPService(), DateTime.Now));
            }

            return scheduleItems;
        }

    #endregion
}