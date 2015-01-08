using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;

public partial class AboutUs : MobilePage
{
    public string AboutUsText
    {
        get
        {
            if (Request.QueryString["facilityId"] != null)
            {
                string facilityId = Request.QueryString["facilityId"].ToString();
                Facility facility = FacilityHelper.GetByLocalId(new Guid(facilityId));
                return facility.About;
            }
            else
            {
                return Resources.LocalizedText.AboutUsBody;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleAboutUs;
        this.AutoAddMobileMetaTags = false;

    }
}