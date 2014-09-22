using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;

public partial class FacilityInfo : MobilePage
{

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
            try
            {
                return FacilityHelper.GetByLocalId(new Guid(FacilityId));
                // return Facility.Facilities.Where(f => f.LocalId == new Guid(FacilityId)).FirstOrDefault();
            }
            catch
            {
                Response.Redirect("/Default.aspx");
            }

            return null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleFacilityInfo;
    }
}