﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;

public partial class FacilityNews : MobilePage
{
    public string FacilityNewsText
    {
        get
        {
            Facility facility = FriskisService.GetCurrentFacility();

            if (facility != null)
            {
                return facility.News;
            }

            return "";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitleNews;
        this.AutoAddMobileMetaTags = false;
    }
}