using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;

public partial class FacilityOffers : MobilePage
{
    public string FacilityOffersText
    {
        get
        {
            Facility facility = FriskisService.GetCurrentFacility();

            if (facility != null)
            {
                return facility.Offers;
            }

            return "";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleOffers;
        this.AutoAddMobileMetaTags = false;
    }
}