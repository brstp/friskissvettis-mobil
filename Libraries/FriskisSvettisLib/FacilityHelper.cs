using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

public class FacilityHelper
{
    #region Authentication

        private const string SESSION_LOGGEDIN_FACILITY = "SESSION_LOGGEDIN_FACILITY";

        public static FacilityData LoggedInFacility
        {
            get
            {
                if (HttpContext.Current.Session[SESSION_LOGGEDIN_FACILITY] != null)
                {
                    return (FacilityData)HttpContext.Current.Session[SESSION_LOGGEDIN_FACILITY];
                }

                return null;
            }

            set
            {
                HttpContext.Current.Session[SESSION_LOGGEDIN_FACILITY] = value;
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                return LoggedInFacility != null;
            }
        }

        public static bool Login(string username, string password)
        {
            DatabaseEntities entities = new DatabaseEntities();

            LoggedInFacility = entities.FacilityDatas.Where(f => f.Username.Equals(username) && f.Password.Equals(password)).FirstOrDefault();

            return LoggedInFacility != null;
        }

    #endregion

    #region Converters 

        private static Facility FacilityDataToFacility(FacilityData facilityData)
        {
            Facility facility = new Facility();

            // facility 
            facility.Visible = facilityData.Visible.Value;

            try
            {
                facility.VisibleGuid = facilityData.VisibleGuid.Value;
            }
            catch { }

            facility.Id = facilityData.Id;
            facility.LocalId = facilityData.LocalId;
            facility.Id2 = facilityData.Id2;

            facility.Name = facilityData.Name;

            facility.HtmlInfo = facilityData.HtmlInfo;
            facility.News = facilityData.News;
            facility.Offers = facilityData.Offers;
            facility.About = facilityData.About;

            facility.Phone = facilityData.Phone;
            facility.Email = facilityData.Email;
            facility.Description = facilityData.Description;
            facility.Address = facilityData.Address;
            facility.Zipcode = facilityData.Zipcode;
            facility.City = facilityData.City;

            facility.Homepage = facilityData.Homepage;
            facility.Facebook = facilityData.Facebook;
            facility.Twitter = facilityData.Twitter;

            facility.Longitude = facilityData.Longitude;
            facility.Latitude = facilityData.Latitude;

            facility.Languages = facilityData.Languages;

            facility.Username = facilityData.Username;
            facility.Password = facilityData.Password;

            facility.UsernameLabel = facilityData.UsernameLabel;
            facility.PasswordLabel = facilityData.PasswordLabel;
            facility.ScheduleLength = facilityData.ScheduleLength;

            // service
            switch (facilityData.ServiceType)
            {
                case "BRPService":

                    BRPService brpService = new BRPService();

                    brpService.ServiceUrl = facilityData.ServiceUrl;
                    brpService.ServiceLicense = facilityData.ServiceLicense;
                    brpService.ScheduleLoginNeeded = facilityData.ScheduleLoginNeeded.Value;

                    facility.Service = brpService;

                    break;
                case "PastellService":

                    PastellService pastellService = new PastellService();

                    pastellService.ServiceUrl = facilityData.ServiceUrl;
                    pastellService.ServiceLicense = facilityData.ServiceLicense;
                    pastellService.ServiceRevision = facilityData.ServiceRevision;
                    pastellService.ScheduleLoginNeeded = facilityData.ScheduleLoginNeeded.Value;
                    pastellService.LoginId = facilityData.LoginId;

                    facility.Service = pastellService;

                    break;
                case "DefendoService":

                    DefendoService defendoService = new DefendoService();

                    defendoService.ServiceUrl = facilityData.ServiceUrl;
                    defendoService.ServiceContextId = facilityData.ServiceContextId;
                    defendoService.ServiceLicense = facilityData.ServiceLicense;
                    defendoService.ScheduleLoginNeeded = facilityData.ScheduleLoginNeeded.Value;

                    facility.Service = defendoService;

                    break;
                case "DLSoftwareService":
                    DLSoftwareService dlsoftwareService = new DLSoftwareService();
                    dlsoftwareService.ServiceUrl = facilityData.ServiceUrl;
                    facility.Service = dlsoftwareService;
                    break;
            }

            return facility;
        }

        private static List<Facility> FacilityDatasToFacilities(List<FacilityData> facilityDatas)
        {
            List<Facility> facilities = new List<Facility>();

            foreach (FacilityData facilityData in facilityDatas)
            {
                facilities.Add(FacilityDataToFacility(facilityData));
            }

            return facilities;
        }

    #endregion

    #region Select

        /// <summary>
        /// Should be cleared when updating or deleting. 
        /// </summary>
        public static List<Facility> Cached
        {
            get
            {
                // variable to hold facilities
                List<Facility> facilities = null;

                try
                {
                    // get from cache
                    string name = "APPLICATION_CACHED_FACILITIES";
                    facilities = (List<Facility>)HttpContext.Current.Application[name];

                    if (facilities != null)
                    {
                        return facilities;
                    }
                }
                catch { }

                // get from db
                DatabaseEntities entities = new DatabaseEntities();
                facilities = FacilityDatasToFacilities(entities.FacilityDatas.ToList());

                // cache
                Cached = facilities;

                // return fetched facilities from db
                return facilities;
            }
            set
            {
                string name = "APPLICATION_CACHED_FACILITIES";
                HttpContext.Current.Application[name] = value;
            }
        }

        public static List<Facility> GetAll()
        {
            //List<Facility> facilities = new List<Facility>();
            //DatabaseEntities entities = new DatabaseEntities();

            return Cached;
            // return FacilityDatasToFacilities(entities.FacilityDatas.ToList());
        }

        public static List<Facility> GetAllVisibleByLang(string lang)
        {
            //List<Facility> facilities = new List<Facility>();
            //DatabaseEntities entities = new DatabaseEntities();

            return GetAll().Where(f => f.Visible && f.Languages.Contains(lang)).ToList();
            // return FacilityDatasToFacilities(entities.FacilityDatas.ToList());
        }

        /// <summary>
        /// Returns true if any city has multiple associations
        /// </summary>
        public static bool HasSubCities(string language)
        {
            int distinct = GetAllCities(language).Count;
            int all = GetAll().Where(f => f.Visible).ToList().Count;

            return all > distinct;
        }

        public static List<string> GetAllCities(string language)
        {
            //List<Facility> facilities = new List<Facility>();
            //DatabaseEntities entities = new DatabaseEntities();

            return GetAll().Where(f => f.Visible && f.Languages.Contains(language)).Select(f => f.City).Distinct().ToList<string>().OrderBy(s => s).ToList();
            // return entities.FacilityDatas.Select(f => f.City).Distinct().ToList<string>().OrderBy(s => s).ToList();
        }

        public static List<Facility> GetByCity(string City)
        {
            //List<Facility> facilities = new List<Facility>();
            //DatabaseEntities entities = new DatabaseEntities();

            return GetAll().Where(f => f.City.Trim().Equals(City.Trim())).ToList().OrderBy(f => f.Name).ToList();
            // List<FacilityData> facilitieDatas = entities.FacilityDatas.Where(f => f.City.Trim().Equals(City.Trim())).ToList().OrderBy(f => f.Name).ToList();
            // return FacilityDatasToFacilities(facilitieDatas);
        }

        public static Facility GetByLocalId(Guid LocalId)
        {
            //List<Facility> facilities = new List<Facility>();
            //DatabaseEntities entities = new DatabaseEntities();

            //FacilityData facilityData = entities.FacilityDatas.Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();

            //if (facilityData != null)
            //{
            //    return FacilityDataToFacility(facilityData);
            //}

            //return null;

            return GetAll().Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();
        }

    #endregion

    #region Commented

    /// <summary>
    /// Used in BRP/PastellService.GetBookings to filter out correct facility based on item 
    /// that was fetched in brp/pastell service.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    //public static Facility Get(ScheduleItem item, Member member)
    //{
    //    List<Facility> facilities = new List<Facility>();
    //    DatabaseEntities entities = new DatabaseEntities();

    //    string facilityId = item.Where.Id;
    //    string serviceType = member.Facility.Service.GetType().Name;
    //    string serviceUrl = member.Facility.Service.ServiceUrl;

    //    FacilityData facilityData = entities.FacilityDatas.Where(f => f.Id.Equals(facilityId)                                   // Get by facility id
    //                                                                  && f.ServiceType.Equals(serviceType)    // Also check type of service
    //                                                                  && f.ServiceUrl.Equals(serviceUrl)      // and correct service url
    //                                                            ).FirstOrDefault();
    //    return FacilityDataToFacility(facilityData);
    //}

    //public static Facility GetById(string Id)
    //{
    //    List<Facility> facilities = new List<Facility>();
    //    DatabaseEntities entities = new DatabaseEntities();
    //    FacilityData facilityData = entities.FacilityDatas.Where(f => f.Id.Equals(Id)).FirstOrDefault();

    //    if (facilityData != null)
    //    {
    //        return FacilityDataToFacility(facilityData);
    //    }

    //    return null;
    //}

    //public static FacilityData GetDataByLocalId(Guid LocalId)
    //{
    //    List<Facility> facilities = new List<Facility>();
    //    DatabaseEntities entities = new DatabaseEntities();
    //    FacilityData facilityData = entities.FacilityDatas.Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();

    //    entities.SaveChanges();

    //    return facilityData;
    //}

    #endregion

    #region Insert, Update & Delete

    /// <summary>
    /// Adds the facility if no LocalId is found. 
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    public static void Save(Facility facility)
    {
        List<Facility> facilities = new List<Facility>();
        DatabaseEntities entities = new DatabaseEntities();
        FacilityData facilityData = entities.FacilityDatas.Where(f => f.LocalId.Equals(facility.LocalId)).FirstOrDefault();

        if (facilityData == null)
        {
            facilityData = new FacilityData();
            entities.FacilityDatas.AddObject(facilityData);
        }

        // facility 
        facilityData.Visible = facility.Visible;

        try
        {
            facilityData.VisibleGuid = facility.VisibleGuid;
        }
        catch { }

        facilityData.Id = facility.Id;
        facilityData.LocalId = facility.LocalId;
        facilityData.Id2 = facility.Id2;

        facilityData.Name = facility.Name;

        facilityData.HtmlInfo = facility.HtmlInfo;
        facilityData.News = facility.News;
        facilityData.Offers = facility.Offers;
        facilityData.About = facility.About;

        facilityData.Phone = facility.Phone;
        facilityData.Email = facility.Email;
        facilityData.Description = facility.Description;
        facilityData.Address = facility.Address;
        facilityData.Zipcode = facility.Zipcode;
        facilityData.City = facility.City;

        facilityData.Homepage = facility.Homepage;
        facilityData.Facebook = facility.Facebook;
        facilityData.Twitter = facility.Twitter;

        facilityData.Longitude = facility.Longitude;
        facilityData.Latitude = facility.Latitude;

        facilityData.Languages = facility.Languages;

        facilityData.Username = facility.Username;
        facilityData.Password = facility.Password;

        facilityData.UsernameLabel = facility.UsernameLabel;
        facilityData.PasswordLabel = facility.PasswordLabel;

        facilityData.ScheduleLength = facility.ScheduleLength;

        // service
        facilityData.ServiceType = facility.Service.GetType().Name;

        switch (facilityData.ServiceType)
        {
            case "BRPService":

                BRPService brpService = (BRPService)facility.Service;

                facilityData.ServiceUrl = brpService.ServiceUrl;
                facilityData.ServiceLicense = brpService.ServiceLicense;
                facilityData.ScheduleLoginNeeded = brpService.ScheduleLoginNeeded;

                break;
            case "PastellService":

                PastellService pastellService = (PastellService)facility.Service;

                facilityData.ServiceUrl = pastellService.ServiceUrl;
                facilityData.ServiceLicense = pastellService.ServiceLicense;
                facilityData.ServiceRevision = pastellService.ServiceRevision;
                facilityData.ScheduleLoginNeeded = pastellService.ScheduleLoginNeeded;
                facilityData.LoginId = pastellService.LoginId;

                break;
            case "DefendoService":

                DefendoService defendoService = (DefendoService)facility.Service;

                facilityData.ServiceUrl = defendoService.ServiceUrl;
                facilityData.ServiceContextId = defendoService.ServiceContextId;
                facilityData.ServiceLicense = defendoService.ServiceLicense;
                facilityData.ScheduleLoginNeeded = defendoService.ScheduleLoginNeeded;

                break;
            case "DLSoftwareService":

                DLSoftwareService dlService = (DLSoftwareService)facility.Service;

                facilityData.ServiceUrl = dlService.ServiceUrl;

                break;
        }

        entities.SaveChanges();
        Cached = null;
    }

    /// <summary>
    /// Adds the facility if no LocalId is found. 
    /// </summary>
    /// <param name="facility"></param>
    /// <returns></returns>
    public static void Remove(Guid LocalId)
    {

        List<Facility> facilities = new List<Facility>();
        DatabaseEntities entities = new DatabaseEntities();
        FacilityData facilityData = entities.FacilityDatas.Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();
        
        entities.FacilityDatas.DeleteObject(facilityData);
        entities.SaveChanges();
        Cached = null;
    }

    #endregion
}