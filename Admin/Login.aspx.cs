using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            lblMessage.Text = "";
        }
    }

    protected void btnLogin_OnClick(object sender, EventArgs e)
    {
        if (FacilityHelper.Login(txtUsername.Text, txtPassword.Text))
        {
            Response.Redirect("Facility.aspx?id=" + FacilityHelper.LoggedInFacility.LocalId.ToString());
        }
        else
        {
            lblMessage.Text = "Error: Wrong username and/or password!";
        }
    }
}