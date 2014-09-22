<%@ WebHandler Language="C#" Class="ActivityHandler" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Resources;
using System.Web.SessionState;
using System.Globalization;
using System.Threading;

using MoMA.Helpers;

public class ActivityHandler : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/xml";

        string s_facilityId = ContextHelper.GetValue<string>("facilityId", "");
        
        // set language
        string localizationLanguage = ContextHelper.GetValue<string>("localizationLanguage", "sv");
        if (string.IsNullOrEmpty(localizationLanguage))
        {
            localizationLanguage = "sv";
        }
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(localizationLanguage);
        
        if (string.IsNullOrEmpty(s_facilityId) && FriskisService.IsLoggedIn)
        {
            s_facilityId = FriskisService.LoggedInMember.Facility.LocalId.ToString();
        }
        
        Guid facilityId;
        
        if (Guid.TryParse(s_facilityId, out facilityId))
        {
            // get activities
            Facility facility = FacilityHelper.GetByLocalId(facilityId);
            List<Activity> activities = new List<Activity>();
            activities.AddRange(facility.Service.CachedActivities);

            // create xml writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;        
            StringBuilder builder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                // print xml
                writer.WriteStartElement("items");

                activities.Insert(0, new Activity()
                {
                    Id = "",
                    Name = Resources.LocalizedText.All
                });
                
                foreach (Activity item in activities)
                {
                    writer.WriteStartElement("item");
                    writer.WriteElementString("id", item.Id);
                    writer.WriteElementString("name", item.Name);

                    if (item.ChildActivites != null && item.ChildActivites.Count > 0)
                    {
                        List<Activity> childActivities = new List<Activity>();
                        childActivities.AddRange(item.ChildActivites);

                        childActivities.Insert(0, new Activity()
                        {
                            Id = "",
                            Name = Resources.LocalizedText.All
                        });

                        foreach (Activity child in childActivities)
                        {
                            writer.WriteStartElement("child");
                            writer.WriteElementString("id", child.Id);
                            writer.WriteElementString("name", child.Name);
                            writer.WriteEndElement();
                        }
                    }
                    
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            HttpContext.Current.Response.Write(builder);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}