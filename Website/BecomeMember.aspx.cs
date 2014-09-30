using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using MoMA.Helpers;

public partial class BecomeMember : MobilePage
{

    public Facility Facility
    {
        get
        {
            try
            {
                string facilityId = ContextHelper.GetValue<string>("facilityId", "");
                Facility facility = FacilityHelper.GetByLocalId(new Guid(facilityId));

                if (facility != null)
                {
                    return facility;
                }
            }
            catch {}

            Response.Redirect("/Default.aspx");
            return null;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleBecomeMember;
    }
}