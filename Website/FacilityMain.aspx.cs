using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;

public partial class FacilityMain : MobilePage
{
    public bool HasService
    {
        get
        {
            // no service if brp and empty service
            return !string.IsNullOrEmpty(Facility.Service.ServiceUrl) || Facility.Service.ServiceType != FriskisServiceType.BRP;
        }
    }

    public string MapAddress
    {
        get
        {
            if (FriskisService.IsApp)
            {
                try
                {
                    return FriskisService.GetAppMapLink(new Guid(FacilityId));
                }
                catch
                {
                }
            }
               
            return "/Map.aspx?id=" + FacilityId;
        }
    }

    /// <summary>
    /// Calculations of the address that should be shown
    /// </summary>
    public string ScheduleAddress
    {
        get
        {
            //// if the client is a app
            //if (FriskisService.IsApp)
            //{
            //    return FriskisService.GetAppMapLink(new Guid(FacilityId));
            //}

            //// if client is webbrowser
            //else
            //{
            string baseUrl = "/Schedule.aspx?" + ((FriskisService.IsLoggedIn && FriskisService.IsApp) ? "authenticated=true&" : "");

                // if user is not logged in a facilityId should be sent
                if (!FriskisService.IsLoggedIn)
                {
                    return baseUrl + "facilityId=" + FacilityId;
                }

                // if user is logged in, but the not at the facility that is requested then the facility id should be shown.
                else if (!FriskisService.LoggedInMember.Facility.LocalId.ToString().Equals(FacilityId))
                {
                    return baseUrl + "facilityId=" + FacilityId;
                }

                // if the user is logged in at the same facility that is requested, then the facility id should not be shown.
                else
                {
                    return baseUrl;
                }
            //}
        }
    }

    public string FacilityId
    {
        get
        {
            return ContextHelper.GetValue<string>("facilityId", "").Trim();
        }
    }

    public Facility Facility
    {
        get
        {
            Guid facilityGuid;
            
            if (Guid.TryParse(FacilityId, out facilityGuid))
            {
                return FacilityHelper.GetByLocalId(new Guid(FacilityId));
            }

            Response.Redirect("/Default.aspx");
            return null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleFacilityMain;
        this.AutoAddMobileMetaTags = false;
    }
}