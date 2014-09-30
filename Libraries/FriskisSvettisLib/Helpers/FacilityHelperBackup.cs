using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace FriskisSvettisLib.Helpers
{
    public class FacilityHelper
    {

        private const string path = "~/App_Data/facilities/facilities-main.xml";

        public static List<Facility> GetAll()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath(path));
            return XmlToList(xmlDoc);
        }

        public static Facility GetById(Guid LocalId)
        {
            return GetAll().Where(f => f.LocalId.Equals(LocalId)).FirstOrDefault();
        }

        /// <summary>
        /// Adds the facility if no LocalId is found. 
        /// </summary>
        /// <param name="facility"></param>
        /// <returns></returns>
        public static void Save(List<Facility> facilities)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xml = CreateXml(facilities);
            xmlDoc.LoadXml(xml);
            xmlDoc.Save(HttpContext.Current.Server.MapPath(path));
        }

        private static List<Facility> XmlToList(XmlDocument xmlDoc)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("//Facilities/Facility");
            List<Facility> facilities = new List<Facility>();

            foreach (XmlNode node in nodes)
            {
                Facility facility = new Facility();

                // facility 
                facility.Visible = XmlHelper.GetValue<bool>(node, "Visible", false);

                try
                {
                    facility.VisibleGuid = new Guid(XmlHelper.GetValue<string>(node, "VisibleGuid", Guid.Empty.ToString()));
                }
                catch { }

                facility.Id = XmlHelper.GetValue<string>(node, "Id", "");
                facility.LocalId = new Guid(XmlHelper.GetValue<string>(node, "LocalId", Guid.Empty.ToString()));
                facility.Id2 = XmlHelper.GetValue<string>(node, "Id2", "");

                facility.Name = XmlHelper.GetValue<string>(node, "Name", "");

                facility.HtmlInfo = XmlHelper.GetValue<string>(node, "HtmlInfo", "");
                facility.Phone = XmlHelper.GetValue<string>(node, "Phone", "");
                facility.Email = XmlHelper.GetValue<string>(node, "Email", "");
                facility.Description = XmlHelper.GetValue<string>(node, "Description", "");
                facility.Address = XmlHelper.GetValue<string>(node, "Address", "");
                facility.City = XmlHelper.GetValue<string>(node, "City", "");

                facility.Longitude = XmlHelper.GetValue<string>(node, "Longitude", "");
                facility.Latitude = XmlHelper.GetValue<string>(node, "Latitude", "");

                // service
                switch (XmlHelper.GetAttr<string>(node, "Service", "type", ""))
                {
                    case "BRPService":

                        BRPService brpService = new BRPService();

                        brpService.ServiceUrl = XmlHelper.GetValue<string>(node, "Service/ServiceUrl", "");
                        brpService.ScheduleLoginNeeded = XmlHelper.GetValue<bool>(node, "Service/ScheduleLoginNeeded", true);

                        facility.Service = brpService;

                        break;
                    case "PastellService":

                        PastellService pastellService = new PastellService();

                        pastellService.ServiceUrl = XmlHelper.GetValue<string>(node, "Service/ServiceUrl", "");
                        pastellService.ServiceLicense = XmlHelper.GetValue<string>(node, "Service/ServiceLicense", "");
                        pastellService.ServiceRevision = XmlHelper.GetValue<string>(node, "Service/ServiceRevision", "");
                        pastellService.ScheduleLoginNeeded = XmlHelper.GetValue<bool>(node, "Service/ScheduleLoginNeeded", true);
                        pastellService.LoginId = XmlHelper.GetValue<string>(node, "Service/LoginId", "");

                        facility.Service = pastellService;

                        break;

                    case "DefendoService": 

                        DefendoService defendoService = new DefendoService();

                        defendoService.ServiceUrl = XmlHelper.GetValue<string>(node, "Service/ServiceUrl", "");
                        defendoService.ServiceContextId = XmlHelper.GetValue<string>(node, "Service/ServiceContextId", "");
                        defendoService.ServiceLicense = XmlHelper.GetValue<string>(node, "Service/ServiceLicense", "");
                        defendoService.ScheduleLoginNeeded = XmlHelper.GetValue<bool>(node, "Service/ScheduleLoginNeeded", true);

                        facility.Service = defendoService;

                        break;

                    case "DTSoftwareService": break;

                        // DTSoftwareService dtsoftwareService = new DTSoftwareService();

                    // facility.Service = dtsoftwareService;

                }

                // add to list
                facilities.Add(facility);
            }

            return facilities;
        }

        private static string CreateXml(List<Facility> facilities)
        {

            string xMain = "<Facilities>";
            string xService = "";

            foreach (Facility facility in facilities)
            {

                switch (facility.Service.GetType().Name)
                {
                    case "BRPService":

                        BRPService brpService = (BRPService)facility.Service;

                        xService =
                        "<Service type=\"BRPService\">" +
                            "<ServiceUrl>" + brpService.ServiceUrl + "</ServiceUrl>" +
                            "<ScheduleLoginNeeded>" + brpService.ScheduleLoginNeeded.ToString().ToLower() + "</ScheduleLoginNeeded>" +
                        "</Service>";
                        break;

                    case "PastellService":

                        PastellService pastellService = (PastellService)facility.Service;

                        xService =
                        "<Service type=\"PastellService\">" +
                            "<ServiceUrl>" + pastellService.ServiceUrl + "</ServiceUrl>" +
                            "<ServiceLicense>" + pastellService.ServiceLicense + "</ServiceLicense>" +
                            "<ServiceRevision>" + pastellService.ServiceRevision + "</ServiceRevision>" +
                            "<ScheduleLoginNeeded>" + pastellService.ScheduleLoginNeeded.ToString().ToLower() + "</ScheduleLoginNeeded>" +
                            "<LoginId>" + pastellService.LoginId + "</LoginId>" +
                        "</Service>";
                        break;
                    case "DefendoService":

                        DefendoService defendoService = (DefendoService)facility.Service;

                        xService =
                        "<Service type=\"DefendoService\">" +
                            "<ServiceUrl>" + defendoService.ServiceUrl + "</ServiceUrl>" +
                            "<ServiceContextId>" + defendoService.ServiceContextId + "</ServiceContextId>" +
                            "<ServiceLicense>" + defendoService.ServiceLicense + "</ServiceLicense>" +
                            "<ScheduleLoginNeeded>" + defendoService.ScheduleLoginNeeded.ToString().ToLower() + "</ScheduleLoginNeeded>" +
                        "</Service>";
                        break;

                    case "DTSoftwareService":

                        // DefendoService defendoService = (DefendoService)facility.Service;

                        xService =
                        "<Service type=\"DTSoftwareService\">" +
                            // "<ServiceUrl>" + pastellService.ServiceUrl + "</ServiceUrl>" +
                        "</Service>";
                        break;
                }

                xMain += @"
                        <Facility>" + 
                            "<Visible>" + facility.Visible.ToString().ToLower() + "</Visible>" +
                            "<VisibleGuid>" + facility.VisibleGuid.ToString() + "</VisibleGuid>" +

                            "<LocalId>" + facility.LocalId.ToString() + "</LocalId>" +
                            "<Id>" + facility.Id.ToString() + "</Id>" +
                            "<Id2>" + facility.Id2.ToString() + "</Id2>" +

                            "<Name>" + facility.Name.ToString() + "</Name>" +
                            "<HtmlInfo><![CDATA[" + facility.HtmlInfo.ToString() + "]]></HtmlInfo>" +
                            "<Phone>" + facility.Phone.ToString() + "</Phone>" +
                            "<Email><![CDATA[" + facility.Email.ToString() + "]]></Email>" +
                            "<Description><![CDATA[" + facility.Description.ToString() + "]]></Description>" +
                            "<Address><![CDATA[" + facility.Address.ToString() + "]]></Address>" +
                            "<City><![CDATA[" + facility.City.ToString() + "]]></City>" +

                            "<Longitude>" + facility.Longitude.ToString() + "</Longitude>" +
                            "<Latitude>" + facility.Latitude.ToString() + "</Latitude>" +

                            xService + 

                        "</Facility>";
            }

            xMain += "</Facilities>";

            return xMain;
        }
    }
}
