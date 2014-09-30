using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using MoMA.Helpers;

/// <summary>
/// Facility
/// </summary>
public class Facility
{

    #region Properties

        /// <summary>
        /// The original object that was fetched from the service
        /// </summary>
        public object originalObject { get; set; }

        /// <summary>
        /// Id in database
        /// </summary>
        public Guid LocalId { get; set; }

        /// <summary>
        /// Id of facility in service
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Depricated, was used in Pastell before but is 
        /// no longer necissary
        /// </summary>
        public string Id2 { get; set; }

        /// <summary>
        /// Username for member in service
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for member in service
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Name of facility
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Html-adjusted information about the facility
        /// </summary>
        public string HtmlInfo { get; set; }

        /// <summary>
        /// News 
        /// </summary>
        public string News { get; set; }

        /// <summary>
        /// Offers
        /// </summary>
        public string Offers { get; set; }

        /// <summary>
        /// About
        /// </summary>
        public string About { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Zipcode
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Homepage
        /// </summary>
        public string Homepage { get; set; }

        /// <summary>
        /// Twitter
        /// </summary>
        public string Twitter { get; set; }

        /// <summary>
        /// Facebook
        /// </summary>
        public string Facebook { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Label for username textbox when login. Default is stored in resources.
        /// </summary>
        public string UsernameLabel { get; set; }

        /// <summary>
        /// Label for password textbox when login. Default is stored in resources.
        /// </summary>
        public string PasswordLabel { get; set; }

        /// <summary>
        /// How many days you are allowed to show in the schedule. Default is 7.
        /// </summary>
        public int? ScheduleLength { get; set; }

        /// <summary>
        /// If this facility should be shown (Pastell)
        /// </summary>
        public bool ShowOnWeb { get; set; }

        /// <summary>
        /// What main language this facility is using. Available languages are "sv", "no" and "en".
        /// </summary>
        public string Languages { get; set; }

        /// <summary>
        /// If this facility should be visible on the website
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// If the facility is invisible, you can add this guid (example.aspx?guid=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx) to the website call and it will be visible
        /// </summary>
        public Guid VisibleGuid { get; set; }

        /// <summary>
        /// All activities that can be used to filter schedule items for this facility.
        /// </summary>
        public List<Activity> Activities { get; set; }

        /// <summary>
        /// All inner facilities that can be used to filter schedule items for this facility.
        /// </summary>
        public List<Facility> Facilities { get; set; } 

        /// <summary>
        /// Service to use to get data for this facility
        /// </summary>
        public IFriskisService Service { get; set; }

    #endregion

    #region Helpers 

        /// <summary>
        /// Looks for this facility Id/Id2 and its inner Id1/Id2 to find 
        /// the correct facility. This is because som services has multiple 
        /// facilities under one society (called facility in code) (society->facility).
        /// 
        /// If the facility has child facilities, this will be checked first and if nothing is 
        /// found there, the main facility will be checked. 
        /// </summary>
        /// <param name="Id">Id to match</param>
        /// <returns>The matched facility and null if not found</returns>
        public Facility GetInnerFacility(string Id)
        {
            Facility facility = null;

            // get inner facility first if there are many (like in Pastell)
            if (this.Facilities != null && this.Facilities.Count > 0)
            {
                // check if matches any childfacility

                facility = this.Facilities.Where(f => f.Id.Equals(Id) || (f.Id2 != null && f.Id2.Equals(Id))).FirstOrDefault();
            }

            // fallback and look and this facility
            if (facility == null)
            {
                // checked if matched main facility
                if (this.Id.Equals(Id) || (this.Id2 != null && this.Id2.Equals(Id)))
                {
                    facility = this;
                }
            }

            // return result
            return facility;
        }

    #endregion

    #region Constructors

        /// <summary>
        /// Creates a blank facility
        /// </summary>
        public Facility()
        {
            Visible = true;
            VisibleGuid = Guid.Empty;
        }

        /// <summary>
        /// Will parse a facility from xml depending on which service is being used
        /// </summary>
        /// <param name="data"></param>
        /// <param name="service"></param>
	    public Facility(string data, IFriskisService service) : this()
	    {
            switch (service.ServiceType)
            {
                case FriskisServiceType.Demo:
                    break;
                case FriskisServiceType.PastellData:
                    FillFacilityFromPastell(data);
                    break;
                case FriskisServiceType.BRP:
                    FillFacilityFromBRP(data);
                    break;
            }
	    }

    #endregion

    #region Parsers

        /// <summary>
        /// Parses multiple facilities from xml depending on the service being used
        /// </summary>
        /// <param name="data"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static List<Facility> ParseMany(string data, IFriskisService service)
        {
            switch (service.ServiceType)
            {
                case FriskisServiceType.Demo:
                    break;
                case FriskisServiceType.PastellData:
                    return FillFacilitiesFromPastell(data);
                case FriskisServiceType.BRP:
                    return FillFacilitiesFromBRP(data);
            }

            return new List<Facility>();
        }

    #endregion

    #region Pastell Data

        /// <summary>
        /// Creates a list of facilities based on xml that was fetched from a "Pastell Data" service
        /// </summary>
        /// <param name="xml">Raw xml to parse into a facility list</param>
        /// <returns></returns>
        private static List<Facility> FillFacilitiesFromPastell(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            List<Facility> facilities = null;
            try
            {
                xmlDoc.LoadXml(xml);
                facilities = new List<Facility>();
                XmlNodeList facilityNodes = xmlDoc.SelectNodes("//units/unit");

                foreach (XmlNode facilityNode in facilityNodes)
                {
                    facilities.Add(new Facility(facilityNode.OuterXml, new PastellService()));
                }
            }
            catch 
            {
                facilities = new List<Facility>();
            }

            return facilities;
        }

        /// <summary>
        /// Creates a facility from xml that was fetched from a "Pastell Data" service
        /// </summary>
        /// <param name="xml">Raw xml to parse into a facility</param>
        private void FillFacilityFromPastell(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            Id = XmlHelper.GetValue<string>(xmlDoc, "//unit/gid", "");
            Id2 = XmlHelper.GetValue<string>(xmlDoc, "//unit/id", "");
            Name = XmlHelper.GetValue<string>(xmlDoc, "//unit/name", "");
            ShowOnWeb = XmlHelper.GetValue<string>(xmlDoc, "//unit/showonweb", "1").Equals("1");
        }

    #endregion

    #region BRP

        /// <summary>
        /// Creates a list of facilities based on xml that was fetched from a BRP service
        /// </summary>
        /// <param name="xml">Raw xml to parse into a facility list</param>
        /// <returns></returns>
        private static List<Facility> FillFacilitiesFromBRP(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            List<Facility> facilities = new List<Facility>();
            XmlNodeList facilityNodes = xmlDoc.SelectNodes("//businessunitlist/businessunit");

            foreach (XmlNode facilityNode in facilityNodes)
            {
                facilities.Add(new Facility(facilityNode.OuterXml, new BRPService()));
            }

            return facilities;
        }

        /// <summary>
        /// Creates a facility from xml that was fetched from a BRP service
        /// </summary>
        /// <param name="xml">Raw xml to parse into a facility</param>
        private void FillFacilityFromBRP(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            Id = XmlHelper.GetValue<string>(xmlDoc, "//businessunit/id", "");
            Name = XmlHelper.GetValue<string>(xmlDoc, "//businessunit/name", "");
            HtmlInfo = XmlHelper.GetValue<string>(xmlDoc, "//businessunit/appinfo", "");
        }

    #endregion
}