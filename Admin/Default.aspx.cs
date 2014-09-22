using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FriskisSvettisLib;
using FriskisSvettisLib.Helpers;
using MoMA.Helpers;

public partial class Admin_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // authenticate
        if (!AdminAuth.Authenticated)
        {
            // failed
            Response.Redirect("Login.aspx");
        }

        // Load all facilities 
        rptrFacilities.DataSource = FacilityHelper.GetAll();
        rptrFacilities.DataBind();
    }

    protected void btnRemove_OnDataBinding(object sender, EventArgs e)
    {
        LinkButton btnRemove = (LinkButton)sender;
        btnRemove.CommandArgument = Eval("LocalId").ToString();
    }

    protected void btnRemove_OnClick(object sender, EventArgs e)
    {
        LinkButton btnRemove = (LinkButton)sender;
        Guid LocalId = new Guid(btnRemove.CommandArgument);

        FacilityHelper.Remove(LocalId);
        Response.Redirect(Request.Url.ToString());
    }
}