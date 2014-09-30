using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MoMA.Mobile.Pages;
using System.Globalization;
using System.Threading;

public partial class ChooseLanguage : MobilePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        // iphone-fix to keep __doPostback()
        // http://stackoverflow.com/questions/7275695/uiwebview-and-iphone-content-does-not-postback-asp-net-browser-capability-issue
        ClientTarget = "uplevel";

        Title = Resources.LocalizedText.PageTitleChooseLanguage;

        btnSwedish.Visible = false;
        btnNorwegian.Visible = false;
 
        switch (FriskisService.GetDomainLanguage())
        {
            case "se":
                btnSwedish.Visible = true;
                break;
            case "no":
                btnNorwegian.Visible = true;
                break;
            default:
                btnSwedish.Visible = true;
                break;
        }
    }

    private void ChangeLanguage(string language)
    {
        CookieHelper.SetCookie("LANGUAGE", language);
        Response.Redirect("/Default.aspx");
    }

    protected void btnSwedish_Click(object sender, EventArgs e)
    {
        ChangeLanguage("sv");
    }

    protected void btnNorwegian_Click(object sender, EventArgs e)
    {
        ChangeLanguage("no");
    }

    protected void btnEnglish_Click(object sender, EventArgs e)
    {
        ChangeLanguage("en");
    }
}