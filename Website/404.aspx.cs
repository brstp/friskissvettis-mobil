using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;

public partial class _404 : MobilePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Resources.LocalizedText.PageTitle404;
    }
}